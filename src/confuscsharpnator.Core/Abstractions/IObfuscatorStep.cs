using Microsoft.CodeAnalysis;
using confuscsharpnator.Core.Models;
using confuscsharpnator.Shared;

namespace confuscsharpnator.Core.Abstractions
{
    public interface IObfuscatorStep : IObfuscationFeatureInfo
    {
        // Steps should be executed in ascending order based on this property. Lower values indicate earlier execution.
        // Whats more if two steps have the same ExecutionOrder, they should be executed by the Name property in ascending order.
        uint ExecutionOrder { get; }
        Solution Process(Solution solution, ObfuscationContext context);
    }
}
