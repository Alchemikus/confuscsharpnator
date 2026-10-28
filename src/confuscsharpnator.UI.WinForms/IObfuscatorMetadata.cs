using System;
using System.Collections.Generic;
using System.Text;

namespace confuscsharpnator.UI.WinForms
{
    public interface IObfuscatorMetadata
    {
        // obfuscation parameters
        bool VariableRenaming { get; }
        bool CalculationsObfuscator { get; }
        bool ControlFlowDisruption { get; }
        bool StringEncryption { get; }
        bool CallsHiding { get; }
        bool AntiDebug { get; }
        bool AntiTamper { get; }

        // folder paths
        string InputFolderPath { get; }
        string OutputFolderPath { get; }
    }
}
