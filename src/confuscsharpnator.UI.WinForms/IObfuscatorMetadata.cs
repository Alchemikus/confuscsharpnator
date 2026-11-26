using System;
using System.Collections.Generic;
using System.Text;

namespace confuscsharpnator.UI.WinForms
{
    public interface IObfuscatorMetadata
    {
        // obfuscation parameters
        IEnumerable<string> SelectedStepNames { get; }

        // folder paths
        string InputSolutionPath { get; }
        string OutputFolderPath { get; }
    }
}
