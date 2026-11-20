using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    internal class AntiDebugPass : IObfuscatorStep
    {
        public string Name => "AntiDebug";
        public string DisplayName => "Anti-Debug";
        public bool IsEnabledByDefault => true;
        public uint ExecutionOrder => 0;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
