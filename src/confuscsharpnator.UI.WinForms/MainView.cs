using confuscsharpnator.Shared;
using System.Drawing.Drawing2D;

namespace confuscsharpnator.UI.WinForms
{
    public partial class MainView : Form, IMainView, IObfuscatorMetadata
    {
        private readonly IEnumerable<IObfuscationFeatureInfo> _features;

        public IEnumerable<string> SelectedStepNames
        {
            get
            {
                return GroupBoxObfuscationOptions.Controls
                    .OfType<CheckBox>()
                    .Where(cb => cb.Checked)
                    .Select(cb => cb.Tag?.ToString())
                    .Where(tag => !string.IsNullOrEmpty(tag))!;
            }
        }
        public string InputSolutionPath { get => TextBoxEntrySolutionPath.Text.Trim() ?? string.Empty; }
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

        public MainView(IEnumerable<IObfuscationFeatureInfo> features)
        {
            _features = features;

            InitializeComponent();
            InitializeObfuscatorCheckboxes();

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
                null, ButtonEntrySolutionPath, [true]);
            typeof(Button).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, ButtonOutputFolderPath, [true]);

            ButtonObfuscate.Click += (s, e) => Obfuscate?.Invoke();
            ButtonObfuscate.Paint += GradientPainting;

            ButtonEntrySolutionPath.Click += ButtonEntryProjectPath_Click;
            ButtonEntrySolutionPath.Paint += GradientPainting;

            ButtonOutputFolderPath.Click += ButtonOutputFolderPath_Click;
            ButtonOutputFolderPath.Paint += GradientPainting;

            TextBoxEntrySolutionPath.TextChanged += ValidatePaths;

            TextBoxOutputFolderPath.TextChanged += ValidatePaths;
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
            TextBoxEntrySolutionPath.Clear();
            TextBoxOutputFolderPath.Clear();
            ClearCheckboxToDefaultState();
        }

        /// <summary>
        /// Prevents user from interacting with the form while sth is in progress.
        /// </summary>
        /// <param name="freeze"></param>
        public void FreezeActions(bool freeze)
        {
            ButtonEntrySolutionPath.Enabled = !freeze;
            ButtonOutputFolderPath.Enabled = !freeze;
            ButtonObfuscate.Enabled = !freeze;
            TextBoxEntrySolutionPath.Enabled = !freeze;
            TextBoxOutputFolderPath.Enabled = !freeze;
        }

        /// <summary>
        /// Sets frame title if given; if not sets neutral title
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string? title)
        {
            this.Text = title ?? "Confuscsharpnator";
        }

        private void InitializeObfuscatorCheckboxes()
        {
            GroupBoxObfuscationOptions.SuspendLayout();
            GroupBoxObfuscationOptions.Controls.Clear();

            int yPos = 20;

            foreach (var feature in _features.OrderBy(f => f.DisplayName))
            {
                var cb = new CheckBox
                {
                    Text = feature.DisplayName,
                    Tag = feature.Name,
                    Checked = feature.IsEnabledByDefault,
                    AutoSize = true,
                    Location = new Point(10, yPos)
                };

                GroupBoxObfuscationOptions.Controls.Add(cb);
                yPos += 25;
            }

            GroupBoxObfuscationOptions.ResumeLayout(true);
        }

        private void ButtonEntryProjectPath_Click(object? sender, EventArgs e)
        {
            using var fileDialog = new OpenFileDialog
            {
                Title = "Wybierz plik rozwiązania",
                Filter = "Pliki rozwiązania (*.sln;*.slnx)|*.sln;*.slnx",
                CheckFileExists = true,
                Multiselect = false
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxEntrySolutionPath.Text = fileDialog.FileName;
                if (string.IsNullOrWhiteSpace(TextBoxOutputFolderPath.Text))
                {
                    string? projectDir = Path.GetDirectoryName(fileDialog.FileName);
                    if (!string.IsNullOrEmpty(projectDir))
                    {
                        string randomSuffix = Random.Shared.GetString("1234567890qwertyuiopasdfghjklzxcvbnm", 10);
                        TextBoxOutputFolderPath.Text = Path.Combine(projectDir, $"obfuscated_{randomSuffix}");
                    }
                }
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
            foreach (var cb in GroupBoxObfuscationOptions.Controls.OfType<CheckBox>())
            {
                var name = cb.Tag?.ToString();
                var originalFeature = _features.FirstOrDefault(f => f.Name == name);

                if (originalFeature != null)
                {
                    cb.Checked = originalFeature.IsEnabledByDefault;
                }
            }
        }

        /// <summary>
        /// Chekcs if paths are equal, if so informs about possible demage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidatePaths(object? sender, EventArgs e)
        {
            bool willOverwrite = IsOutputSameAsProjectDirectory(TextBoxEntrySolutionPath.Text, TextBoxOutputFolderPath.Text);

            if (willOverwrite)
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

        private static bool IsOutputSameAsProjectDirectory(string projectPath, string outputFolderPath)
        {
            if (string.IsNullOrWhiteSpace(projectPath) || string.IsNullOrWhiteSpace(outputFolderPath))
            {
                return false;
            }

            try
            {
                string fullProjectPath = Path.GetFullPath(projectPath);

                string? projectDirectory = File.Exists(fullProjectPath) ||
                                           Path.GetExtension(fullProjectPath).Equals(".sln", StringComparison.OrdinalIgnoreCase) ||
                                           Path.GetExtension(fullProjectPath).Equals(".slnx", StringComparison.OrdinalIgnoreCase)
                    ? Path.GetDirectoryName(fullProjectPath)
                    : fullProjectPath;

                if (string.IsNullOrWhiteSpace(projectDirectory))
                    return false;

                string normalizedProjectDir = projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string normalizedOutputDir = Path.GetFullPath(outputFolderPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                return normalizedProjectDir.Equals(normalizedOutputDir, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
