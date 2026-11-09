using Microsoft.CodeAnalysis;
using confuscsharpnator.Core.Models;

namespace confuscsharpnator.Core.Abstractions
{
    public interface IObfuscatorStep
    {
        string Name { get; }
        Solution Execute(Solution solution, ObfuscationContext context);
    }
}
