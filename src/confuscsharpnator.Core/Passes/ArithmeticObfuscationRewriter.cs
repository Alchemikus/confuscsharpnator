using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace confuscsharpnator.Core.Passes
{
    /// <summary>
    /// Rewriter przekształca stałe liczbowe oraz operacje arytmetyczne
    /// w równoważne wyrażenia bitowo-arytmetyczne.
    /// </summary>
    public sealed class ArithmeticObfuscationRewriter: CSharpSyntaxRewriter
    {
        private readonly ObfuscationContext _context;
        private readonly SemanticModel _semanticModel;
        private readonly Random _random;

        public ArithmeticObfuscationRewriter(ObfuscationContext context, SemanticModel semanticModel, int? seed = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }
    }
}
