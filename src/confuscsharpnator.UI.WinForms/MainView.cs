using System.Drawing.Drawing2D;

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
        // prevents from screen flickering
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        public MainView()
        {
            InitializeComponent();
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            this.UpdateStyles();

            typeof(Button).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, ButtonObfuscate, [true]);
            typeof(Button).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, ButtonEntryFolder, [true]);
            typeof(Button).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, ButtonOutputFolderPath, [true]);

            ButtonObfuscate.Click += (s, e) => Obfuscate?.Invoke();
            ButtonObfuscate.Paint += GradientPainting;

            ButtonEntryFolder.Click += ButtonEntryFolder_Click;
            ButtonEntryFolder.Paint += GradientPainting;

            ButtonOutputFolderPath.Click += ButtonOutputFolderPath_Click;
            ButtonOutputFolderPath.Paint += GradientPainting;

            TextBoxEntryFolderPath.TextChanged += ValidateFolderPaths;

            TextBoxOutputFolderPath.TextChanged += ValidateFolderPaths;
            this.FormClosing += (s, e) => ViewClosing?.Invoke();
        }

        /// <summary>
        /// Shows message in separate window.
        /// By default we treat it as information.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="icon"></param>
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

        /// <summary>
        /// Prevents user from interacting with the form while sth is in progress.
        /// </summary>
        /// <param name="freeze"></param>
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

        // one of substract responsible for preventing flickering of the form
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackgroundImage != null)
            {
                e.Graphics.Clear(this.BackColor);

                float scale = Math.Max((float)this.Width / this.BackgroundImage.Width,
                                       (float)this.Height / this.BackgroundImage.Height);

                int drawWidth = (int)(this.BackgroundImage.Width * scale);
                int drawHeight = (int)(this.BackgroundImage.Height * scale);

                int x = (this.Width - drawWidth) / 2;
                int y = (this.Height - drawHeight) / 2;

                e.Graphics.DrawImage(this.BackgroundImage, x, y, drawWidth, drawHeight);

                using SolidBrush overlayBrush = new(Color.FromArgb(170, 255, 255, 255));
                e.Graphics.FillRectangle(overlayBrush, this.ClientRectangle);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        // gradient painting
        private void GradientPainting(object? sender, PaintEventArgs e)
        {
            if (sender is not Button)
                return;

            Button btn = (Button)sender;

            using LinearGradientBrush brush = new (btn.ClientRectangle, Color.Empty, Color.Empty, 0f);
            ColorBlend colorBlend = new()
            {
                Colors = [
                    Color.FromArgb(85, 115, 200),  // Blue
                    Color.FromArgb(100, 220, 245), // Light Blue
                    Color.FromArgb(185, 140, 235)  // Violet/Pink
                ],
                Positions = [0.0f, 0.5f, 1.0f] // Define the positions of the colors in the gradient
            };
            brush.InterpolationColors = colorBlend;

            e.Graphics.FillRectangle(brush, btn.ClientRectangle);

            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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

        /// <summary>
        /// Chekcs if paths are equal, if so informs about possible demage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
