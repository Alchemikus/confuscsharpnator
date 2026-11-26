using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    internal class MathObfuscationPass : IObfuscatorStep
    {
        public string Name => "Math";
        public string DisplayName => "Zaciemnienie matematyki";
        public bool IsEnabledByDefault => false;
        public uint ExecutionOrder => 20;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
