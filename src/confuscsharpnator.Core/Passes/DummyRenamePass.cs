using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using confuscsharpnator.Shared.Constants;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    /// <summary>
    /// A dummy implementation of the variables renamer step.
    /// Used currently to test the obfuscation pipeline, UI binding, and Dependency Injection setup.
    /// </summary>
    internal class DummyRenamePass : IObfuscatorStep
    {
        public string Name => ObfuscationStepNames.VariablesRenamer;

        public Solution Execute(Solution solution, ObfuscationContext context)
        {
            // TODO: Implement actual Roslyn syntax tree mutation here.

            return solution;
        }
    }
}
