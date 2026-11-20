using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    internal class VariableRenamingPass : IObfuscatorStep
    {
        public string Name => "VariableRenaming";
        public string DisplayName => "Zmiana nazw zmiennych";
        public bool IsEnabledByDefault => true;
        public uint ExecutionOrder => 40;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
