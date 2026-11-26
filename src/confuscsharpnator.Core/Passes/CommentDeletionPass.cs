using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;

namespace confuscsharpnator.Core.Passes
{
    internal class CommentDeletionPass : IObfuscatorStep
    {
        public string Name => "CommentDeletion";
        public string DisplayName => "Usunięcie komentarzy";
        public bool IsEnabledByDefault => true;
        public uint ExecutionOrder => 40;

        public Solution Process(Solution solution, ObfuscationContext context)
        {
            // To be done.
            return solution;
        }
    }
}
