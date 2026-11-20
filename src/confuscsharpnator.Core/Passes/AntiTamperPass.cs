using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    internal class AntiTamperPass : IObfuscatorStep
    {
        public string Name => "AntiTamper";
        public string DisplayName => "Anti-Tamper";
        public bool IsEnabledByDefault => true;
        public uint ExecutionOrder => 0;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
