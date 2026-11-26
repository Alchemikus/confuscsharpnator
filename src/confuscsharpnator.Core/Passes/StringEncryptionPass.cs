using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    public class StringEncryptionPass : IObfuscatorStep
    {
        public string Name => "StringEncryption";
        public string DisplayName => "Szyfrowanie ciągów znakowych";
        public bool IsEnabledByDefault => true;
        public uint ExecutionOrder => 10;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
