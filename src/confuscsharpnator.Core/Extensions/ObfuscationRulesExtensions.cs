using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Extensions
{
    internal static class ObfuscationRulesExtensions
    {
        /// <summary>
        /// Evaluates whether a specific obfuscation feature should be excluded for a given symbol, 
        /// based on the <c>[Obfuscation]</c> attributes applied to it or its parent structures.
        /// </summary>
        /// <param name="symbol">The Roslyn ISymbol (method, class, property, etc.) to evaluate.</param>
        /// <param name="featureName">The name of the obfuscation feature (e.g., "Rename", "StringEncryption").</param>
        /// <returns>
        /// <c>true</c> if the feature is explicitly excluded by the developer in code; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFeatureExcluded(this ISymbol? symbol, string featureName)
        {
            // Fallback/stub for the architectural phase
            return false;
        }
    }
}