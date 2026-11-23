using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Numerics;
using System.Runtime.ConstrainedExecution;

namespace confuscsharpnator.Core.Passes
{
    /// <summary>
    /// Rewriter przekształca stałe liczbowe oraz operacje arytmetyczne
    /// w równoważne wyrażenia bitowo-arytmetyczne.
    /// </summary>
    public sealed class ArithmeticObfuscationRewriter: CSharpSyntaxRewriter
    {
        private readonly ObfuscationContext _context;
        private readonly SemanticModel _semanticModel;
        private readonly Random _random;

        public ArithmeticObfuscationRewriter(ObfuscationContext context, SemanticModel semanticModel, int? seed = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }


        /// <summary>
        /// Przechwytuje wyrażenia dwuargumentowe (np. a + b, x - 5, score * 2, a == b).
        /// </summary>
        public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            // Pomiń const, case, enum, parametry domyślne, atrybuty.
            // Kompilator C# nie pozwala tam na wywołania metod (np. BitConverter) 
            if (IsInCompileTimeConstantContext(node))
            {
                return base.VisitBinaryExpression(node);
            }

            //Pobierz informacje o typach z SemanticModel na węzłach AST
            ITypeSymbol? resultType = _semanticModel.GetTypeInfo(node).Type;
            ITypeSymbol? leftType = _semanticModel.GetTypeInfo(node.Left).Type;
            ITypeSymbol? rightType = _semanticModel.GetTypeInfo(node.Right).Type;


            //Jeśli roslyn nie był w stanie ustalić typu 
            if(resultType == null || leftType == null || rightType == null)
            {
                return base.VisitBinaryExpression(node);
            }



            //Ochrona przed powielaniem efektów ubocznych (np. i++ + 5,  GetVal() + 2)
            //Wzory MBA duplikują operandy (np w wzorze (a ^ b) + ((a & b) << 1) zmienne a i b występują dwukrotnie).
            //Gdyby po którejś stronie stało i++ lub wywołanie metody, wykonałoby się dwa razy.
            //Zamiast tego zejdź głębiej (base.VisitBinaryExpression), żeby obfuskować literały wewnątrz.
            if (HasSideEffects(node.Left) || HasSideEffects(node.Right))
            {
                return base.VisitBinaryExpression(node);
            }

            //Rekurencyjnie odwiedź lewy i prawy node
            //Przez to np. w wyrażeniu "a + 5" najpierw liczba "5" zamieni się na "(encoded ^ key)"
            //Potem dopiero całość będzie owinięta w MBA dla operatora "+"
            var visitedLeft = (ExpressionSyntax?)Visit(node.Left) ?? node.Left;
            var visitedRight = (ExpressionSyntax?)Visit(node.Right) ?? node.Right;


            //Usuń spacje i komentarze - aby nie zepsuć formatowania wewnątrz nawiasów
            string leftCode = visitedLeft.WithoutTrivia().ToFullString();
            string rightCode = visitedRight.WithoutTrivia().ToFullString();


            string? obfuscatedExpression = null;



            //Operacje arytmetyczne (+, -, *) na typach int (Int32) oraz long (Int64).
            //Mniejsze typy(byte, short, ushort) — w C# operatory bitowe (~, ^, &)
            //niejawnie są konwertowane do int, co wywołałoby błąd kompilacji przy przypisaniu z powrotem do byte/short.
            if(IsInt32OrInt64(resultType) && IsInt32OrInt64(rightType) && IsInt32OrInt64(leftType))
            {
                obfuscatedExpression = ObfuscateIntegerBinary(node, leftCode, rightCode);
            }


            //Porównania równości (a == b, a != b) na int/long -> zamiana na sprawdzenie zerowania XOR
            else if(resultType.SpecialType == SpecialType.System_Boolean && IsInt32OrInt64(rightType) && IsInt32OrInt64(leftType))
            {
                obfuscatedExpression = ObfuscateIntegerComparison(node.Kind(), leftCode, rightCode);
            }



            //Operacje (+, -) na liczbach double.
            //Wykorzystano bezstratne tożsamości algebraiczne (zmiana znaku), aby nie naruszyć precyzji.
            else if(resultType.SpecialType == SpecialType.System_Double && IsNumericType(leftType) && IsNumericType(rightType))
            {
                obfuscatedExpression = ObfuscateDoubleBinary(node.Kind(), leftCode, rightCode);
            }


            //Jeśli udało się dobrać transformację, parsuj kod do węzła AST.
            if (obfuscatedExpression != null)
            {
                return SyntaxFactory.ParseExpression(obfuscatedExpression)
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithTrailingTrivia(node.GetTrailingTrivia());
            }



            //Dla nieobsługiwanych operacji (np. string + string) zwróć oryginalny operator
            //z zaktualizowanymi dziećmi (np. jeśli wewnątrz wywołania metody był literał).
            return node.WithLeft(visitedLeft).WithRight(visitedRight);

        }



        /// <summary>
        /// Dobiera wzór Mixed Boolean-Arithmetic (MBA) lub przesunięcie bitowe dla operacji na int/long.
        /// </summary>
        private string? ObfuscateIntegerBinary(BinaryExpressionSyntax originalNode, string left, string right)
        {
            switch (originalNode.Kind())
            {
                case SyntaxKind.AddExpression:
                    return _random.Next(3) switch
                    {
                        //(XOR) + podwojone przeniesienia (AND << 1)
                        0 => $"(({left} ^ {right}) + (({left} & {right}) << 1))",

                        //(OR) + (AND)
                        1 => $"(({left} | {right}) + ({left} & {right}))",

                        //Podwojona alternatywa bitowa - (XOR)
                        _ => $"((({left} | {right}) << 1) - ({left} ^ {right}))"
                    };


                case SyntaxKind.SubtractExpression:
                    return _random.Next(3) switch
                    {
                        //Różnica bez pożyczek (XOR) - przesunięte pożyczki ((~a & b) << 1)
                        0 => $"(({left} ^ {right}) - (((~({left})) & {right}) << 1))",

                        //odejmowanie po wyzerowaniu wspólnych bitów obu liczb
                        1 => $"(({left} & ~({right})) - (~({left}) & {right}))",

                        //a - b == a + (~b) + 1
                        _ => $"({left} + (~({right})) + 1)"
                    };


                //Mnożenie przez potęgę dwójki (np. x * 2, x * 8) zmień na przesunięcie bitowe (x << 1).
                //Trzeba sprawdzić originalNode.Right, ponieważ visitedRight zostało już wcześniej
                //zamienione przez VisitLiteralExpression na wyrażenie "(encoded ^ key)"
                case SyntaxKind.MultiplyExpression:

                    if(originalNode.Right is LiteralExpressionSyntax rightLit &&
                        rightLit.Token.Value is int multiplier &&
                        IsPowerOfTwo(multiplier))
                    {

                        //Liczba zer na końcu zapisu binarnego potęgi dwójki to wykładnik przesunięcia(np. 8 = 0b1000->shift = 3)
                        int shift = BitOperations.TrailingZeroCount(multiplier);
                        return $"({left} << {shift})";
                    }

                    return null;

                default:
                    return null;
            }
        }




        /// <summary>
        /// Zamienia porównania liczb całkowitych (a == b, a != b) na sprawdzenie zerowania XOR.
        /// XOR dwóch identycznych liczb zawsze daje 0, a różnych — wartość !=0.
        /// </summary>

        private static string? ObfuscateIntegerComparison(SyntaxKind kind, string left, string right)
        {
            switch (kind)
            {
                case SyntaxKind.EqualsExpression:
                    return $"(({left} ^ {right}) == 0)";

                case SyntaxKind.NotEqualsExpression:
                    return $"(({left} ^ {right}) != 0)";

                default:
                    return null;
            }
        }



        /// <summary>
        /// Bezstratne tożsamości algebraiczne dla liczb double.
        /// Podwójny nwaias (-({right})) chroni przed sklejeniem dwóch minusów w operator dekrementacji '--'
        /// (np. gdyby right zaczynało się od znaku '-').
        /// </summary>
        
        private string? ObfuscateDoubleBinary(SyntaxKind kind, string left, string right)
        {
            switch (kind)
            {
                case SyntaxKind.AddExpression:
                    return _random.Next(3) switch
                    {
                        //a + b == a - (-b)
                        0 => $"({left} - (-({right})))",

                        //a + b == -((-a) - b)
                        1 => $"(-((-({left})) - {right}))",

                        //przemienność dodawania połączona ze zmianą znaku: b - (-a)
                        _ => $"({right} - (-({left})))"
                    };

                case SyntaxKind.SubtractExpression:
                    return _random.Next(2) switch
                    {
                        //a - b == a + (-b)
                        0 => $"({left} + (-({right})))",

                        // a - b == -((-a) + b)
                        _ => $"(-((-({left})) + {right}))"
                    };

                case SyntaxKind.MultiplyExpression:
                    //a * b == (-a) * (-b)
                    return $"((-({left})) * (-({right})))";

                default:
                    return null;
            }
        }





        /// <summary>
        /// Sprawdź, czy liczba jest potęgą dwójki większą od 1 (2, 4, 8, 16...).
        /// Potęga dwójki ma w zapisie binarnym dokładnie jeden bit '1', więc (value & (value - 1)) zawsze daje 0.
        /// </summary>
        private bool IsPowerOfTwo(int value)
        {
            return value > 1 && (value & (value - 1)) == 0;
        }


        private static bool IsInt32OrInt64(ITypeSymbol type)
        {
            return type.SpecialType is SpecialType.System_Int32 or SpecialType.System_Int64;
        }


        private static bool IsNumericType(ITypeSymbol type)
        {
            return type.SpecialType is
                SpecialType.System_Int32 or
                SpecialType.System_Int64 or
                SpecialType.System_Single or
                SpecialType.System_Double;
        }


        /// <summary>
        /// Sprawdza, czy węzeł znajduje się w miejscu wymagającym stałej znanej w czasie kompilacji.
        /// Przeszukuje wszystkich przodków węzła w górę drzewa AST.
        /// </summary>
        private static bool IsInCompileTimeConstantContext(SyntaxNode node)
        {
            foreach(var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    //stałe (const) zadeklarowane bezpośrednio w ciele klasy lub struktury
                    case FieldDeclarationSyntax field when field.Modifiers.Any(SyntaxKind.ConstKeyword):

                    //stałe (const) zadeklarowane wewnątrz metody lub konstruktora
                    case LocalDeclarationStatementSyntax local when local.Modifiers.Any(SyntaxKind.ConstKeyword):

                    //Etykiety case 10: w switch
                    case CaseSwitchLabelSyntax:

                    //pattern matching: is 10 lub switch { 10 => ... }
                    case ConstantPatternSyntax:

                    //Wartości pól w enumach: MyEnum { A = 1 }
                    case EnumMemberDeclarationSyntax:

                    //Argumenty atrybutów: [MaxLength(50)]
                    case AttributeArgumentSyntax:         
                        return true;

                    //Wartość domyślna dla parametrów metod, np. void Foo(int x = 10)
                    case EqualsValueClauseSyntax equalsClause when equalsClause.Parent is ParameterSyntax:
                        return true;
                }


            }

            return false;
        }




        private static bool HasSideEffects(ExpressionSyntax expression)
        {
            return expression.DescendantNodesAndSelf().Any(node =>

                //Wywołania metod, np. GetNextId()
                node is InvocationExpressionSyntax ||

                //Tworzenie obiektów: new Foo()
                node is ObjectCreationExpressionSyntax ||

                //Tworzenie obiektów: new()
                node is ImplicitObjectCreationExpressionSyntax ||

                //Przypisania wewnątrz wyrażeń: (a = 5) + 2
                node is AssignmentExpressionSyntax ||

                //Postfix i++ oraz i-- 
                node is PostfixUnaryExpressionSyntax ||

                // Prefiksowe ++i oraz --i (pomiń operatory jak -x, +x, !x, ~x, które nie mają efektów ubocznych)
                node is PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.PreIncrementExpression or (int)SyntaxKind.PreDecrementExpression } ||

                //Wyrażenie await
                node is AwaitExpressionSyntax
            );
        }
    }
}
