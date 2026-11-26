using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    internal class CallsHidingPass : IObfuscatorStep
    {
        public string Name => "CallsHiding";
        public string DisplayName => "Ukrywanie wywołań";
        public bool IsEnabledByDefault => true;
        public uint ExecutionOrder => 20;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
