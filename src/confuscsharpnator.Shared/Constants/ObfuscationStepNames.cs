namespace confuscsharpnator.Shared.Constants
{
    /// <summary>
    /// Global registry of available obfuscation step names.
    /// Used by modules to identify themselves, by the UI to list available options, 
    /// and by the target code attributes for feature toggling.
    /// </summary>
    public static class ObfuscationStepNames
    {
        // 1. Const strings strictly required for [Obfuscation(Feature = "...")] compatibility
        public const string VariablesRenamer = "Rename";
        public const string CommentsDeletion = "Comment";
        public const string MathObfuscation = "Math";
        public const string ControlFlowObfuscation = "ControlFlow";
        public const string StringEncryption = "StringEncryption";
        public const string CallsHiding = "CallsHiding";
        public const string AntiDebug = "AntiDebug";
        public const string AntiTamper = "AntiTamper";

        // 2. Metadata mapping for UI
        public static IReadOnlyDictionary<string, string> StepDisplayNames { get; } = new Dictionary<string, string>
        {
            { VariablesRenamer, "Rename Symbols" },
            { CommentsDeletion, "Remove Comments" },
            { MathObfuscation, "Math Obfuscation" },
            { ControlFlowObfuscation, "Control Flow Flattening" },
            { StringEncryption, "Encrypt Strings" },
            { CallsHiding, "Hide Method Calls" },
            { AntiDebug, "Anti-Debug Injection" },
            { AntiTamper, "Anti-Tamper Protection" }
        };

        // Convenience property if you only need the technical keys
        public static IReadOnlyCollection<string> AllSteps => [.. StepDisplayNames.Keys.ToList()];
    }
}