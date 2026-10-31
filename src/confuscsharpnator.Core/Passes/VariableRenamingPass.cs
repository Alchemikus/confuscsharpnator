using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace confuscsharpnator.Core.Passes
{


    // implementacja modułu obfuskacji odpowiadająca za zmianę nazw zmiennych.
    public sealed class VariableRenamingPass : IObfuscationPass
    {


        public string Name => "Variable Renaming Pass";


        // Metoda przetwarzająca kod C# przekazany w postaci tekstu
        public string Process(string sourceCode, ObfuscationContext context)
        {

            
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                return sourceCode;
            }


            ArgumentNullException.ThrowIfNull(context);

            //Konwertuj kod C# na drzewo AST
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);

            //Znajdź root drzewa AST - start dla przeszukiwania
            SyntaxNode root = tree.GetRoot();


            //Przejdź po węzłach, zwróć zmodyfikowane AST
            var rewriter = new VariableRenamingRewriter(context);

            SyntaxNode transformedRoot = rewriter.Visit(root);


            //Wróć do postaci kodu C# - gotowy na kolejne etapy obfuskacji
            return transformedRoot.ToFullString();
        }
    }
}
