using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace confuscsharpnator.Core.Passes
{
    // Moduł obfuskacji odpowiedzialny za ukrywanie ciągów znaków (String Encryption Pass)
    public sealed class StringEncryptionPass : IObfuscationPass
    {
        public string Name => "String Encryption Pass";

        public string Process(string sourceCode, ObfuscationContext context)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                return sourceCode;
            }

            ArgumentNullException.ThrowIfNull(context);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
            SyntaxNode root = tree.GetRoot();


            //Przejdź po węzłach, zwróć zmodyfikowane AST
            var rewriter = new StringEncryptionRewriter(context);

            SyntaxNode transformedRoot = rewriter.Visit(root);


            //Wróć do postaci kodu C# - gotowy na kolejne etapy obfuskacji
            return transformedRoot.ToString();
        }
    }
}
