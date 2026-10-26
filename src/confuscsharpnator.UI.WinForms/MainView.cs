namespace confuscsharpnator.UI.WinForms
{
    public partial class MainView : Form, IMainView, IObfuscatorMetadata
    {
        public bool VariableRenaming => CheckBoxVariableRenaming.Checked;
        public bool CalculationsObfuscator => CheckBoxCalculationsObfuscator.Checked;
        public bool ControlFlowDisruption => CheckBoxControlFlowDisruption.Checked;
        public bool StringEncryption => CheckBoxStringEncryption.Checked;
        public bool CallsHiding => CheckBoxCallsHiding.Checked;
        public bool AntiDebug => CheckBoxAntiDebug.Checked;
        public bool AntiTamper => CheckBoxAntiTamper.Checked;
        public string InputFolderPath { get => TextBoxEntryFolderPath.Text.Trim() ?? string.Empty; }
        public string OutputFolderPath { get => TextBoxOutputFolderPath.Text.Trim() ?? string.Empty; }
        public event Action? Obfuscate;
        public event Action? ViewClosing;
        private Label? LabelAttention;
        public MainView()
        {
            InitializeComponent();
            ButtonObfuscate.Click += (s, e) => Obfuscate?.Invoke();
            ButtonEntryFolder.Click += ButtonEntryFolder_Click;
            ButtonOutputFolderPath.Click += ButtonOutputFolderPath_Click;
            TextBoxEntryFolderPath.TextChanged += ValidateFolderPaths;
            TextBoxOutputFolderPath.TextChanged += ValidateFolderPaths;
            this.FormClosing += (s, e) => ViewClosing?.Invoke();
        }
        public void ShowMessage(string message, string? caption = null, MessageBoxIcon? icon = null)
        {
            MessageBox.Show(message, caption ?? "Confuscsharpnator", MessageBoxButtons.OK, icon ?? MessageBoxIcon.Information);
        }

        public void ClearFields()
        {
            TextBoxEntryFolderPath.Clear();
            TextBoxOutputFolderPath.Clear();
            ClearCheckboxToDefaultState();
        }

        public void FreezeActions(bool freeze)
        {
            ButtonEntryFolder.Enabled = !freeze;
            ButtonOutputFolderPath.Enabled = !freeze;
            ButtonObfuscate.Enabled = !freeze;
            TextBoxEntryFolderPath.Enabled = !freeze;
            TextBoxOutputFolderPath.Enabled = !freeze;
        }

        private void ButtonEntryFolder_Click(object? sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxEntryFolderPath.Text = folderDialog.SelectedPath;
                if (string.IsNullOrWhiteSpace(TextBoxOutputFolderPath.Text))
                    TextBoxOutputFolderPath.Text = Path.GetFullPath(folderDialog.SelectedPath) +
                        Path.DirectorySeparatorChar + "obfuscated_" + new Random().GetString("1234567890qwertyuiopasdfghjklzxcvbnm", 10);
            }
        }

        private void ButtonOutputFolderPath_Click(object? sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxOutputFolderPath.Text = folderDialog.SelectedPath;
            }
        }

        private void ClearCheckboxToDefaultState()
        {
            CheckBoxVariableRenaming.Checked = true;
            CheckBoxCalculationsObfuscator.Checked = true;
            CheckBoxControlFlowDisruption.Checked = true;
            CheckBoxStringEncryption.Checked = true;
            CheckBoxCallsHiding.Checked = true;
            CheckBoxAntiDebug.Checked = false;
            CheckBoxAntiTamper.Checked = false;
        }

        private void ValidateFolderPaths(object? sender, EventArgs e)
        {
            bool arePathsEqual = !string.IsNullOrWhiteSpace(TextBoxOutputFolderPath.Text) &&
                                 TextBoxEntryFolderPath.Text.Trim().Equals(TextBoxOutputFolderPath.Text.Trim(), StringComparison.OrdinalIgnoreCase);

            if (arePathsEqual)
            {
                if (LabelAttention == null)
                {
                    LabelAttention = new Label
                    {
                        Text = "UWAGA: kod zostanie nadpisany!!!",
                        BackColor = Color.FromArgb(192, 20, 0),
                        ForeColor = Color.Black,
                        Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    TableLayoutPanelInternal.SuspendLayout();

                    TableLayoutPanelInternal.Controls.Add(LabelAttention, 0, 5);
                    TableLayoutPanelInternal.SetColumnSpan(LabelAttention, 2);

                    TableLayoutPanelInternal.ResumeLayout(true);
                    TableLayoutPanelInternal.PerformLayout();
                }
            }
            else
            {
                if (LabelAttention != null)
                {
                    TableLayoutPanelInternal.Controls.Remove(LabelAttention);
                    LabelAttention.Dispose();
                    LabelAttention = null;
                }
            }
        }
    }
}
