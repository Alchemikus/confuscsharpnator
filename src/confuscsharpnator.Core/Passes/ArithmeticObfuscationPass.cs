using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace confuscsharpnator.Core.Passes
{

    /// <summary>
    /// Pass odpowiedzialny za obfuskację obliczeń arytmetycznych i stałych liczbowych.
    /// Wykorzystuje Mixed Boolean-Arithmetic (MBA), rozbijanie stałych (Constant Splitting)
    /// oraz bezstratne tożsamości algebraiczne dla liczb zmiennoprzecinkowych.
    /// </summary>
    public sealed class ArithmeticObfuscationPass : IObfuscationPass
    {
        public string Name => "Arithmetic Obfuscation Pass";
        private readonly int? _seed;


        /// <summary>
        /// Pozwala opcjonalnie przekazać ziarno generatora liczb losowych (przydatne w testach),
        /// domyślnie korzysta z losowych kluczy przy każdym uruchomieniu.
        /// </summary>
        /// 
        public ArithmeticObfuscationPass(int? seed = null)
        {
            _seed = seed;
        }


        

        public string Process(string sourceCode, ObfuscationContext context)
        {
            //Pusty kod zwracamy bez zmian
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                return sourceCode;
            }

            ArgumentNullException.ThrowIfNull(context);

            //Parsowanie kodu źródłowego do drzewa składniowego (AST)
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
            SyntaxNode root = tree.GetRoot();

            // Utworzenie kompilacji w pamięci w celu uzyskania SemanticModel.
            // Wykorzystałem podstawową bibliotekę .NET (System.Private.CoreLib przez typeof(object)),
            // aby Roslyn potrafił rozpoznać typy wbudowane: int, long, double oraz string.
            var coreLibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(
                assemblyName: "ArithmeticObfuscationAnalysis",
                syntaxTrees: new[] { tree },
                references: new[] { coreLibReference },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            SemanticModel semanticModel = compilation.GetSemanticModel(tree);

            //Przejście po AST, transformacja wyrażeń matematycznych oraz literałów
            var rewriter = new ArithmeticObfuscationRewriter(context, semanticModel, _seed);
            SyntaxNode transformedRoot = rewriter.Visit(root);

            //Serializacja do kodu c#
            return transformedRoot.ToFullString();
        }
    }
}
