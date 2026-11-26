using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    internal class ControlFlowObfuscationPass : IObfuscatorStep
    {
        public string Name => "ControlFlow";
        public string DisplayName => "Zaciemnienie przepływu sterowania";
        public bool IsEnabledByDefault => true;
        public uint ExecutionOrder => 30;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
