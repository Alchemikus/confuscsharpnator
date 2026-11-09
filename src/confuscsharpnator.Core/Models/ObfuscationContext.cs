using System.Collections.Concurrent;

namespace confuscsharpnator.Core.Models
{
    /// <summary>
    /// Holds the shared state and metadata during a single obfuscation run.
    /// Passed along to each obfuscation step to allow data sharing (e.g., mapping old names to new names).
    /// </summary>
    public class ObfuscationContext
    {
        /// <summary>
        /// A thread-safe dictionary that maps original symbol names to their newly generated obfuscated names.
        /// Useful for subsequent passes that need to know how a symbol was renamed.
        /// </summary>
        public ConcurrentDictionary<string, string> RenamedSymbols { get; } = new();

        // This class can be easily extended in the future to include additional shared state.
    }
}