namespace confuscsharpnator.UI.WinForms
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            LabelEntryFolder = new Label();
            LabelOutputFolder = new Label();
            GroupBoxObfuscationOptions = new SoupedUpGroupBox();
            CheckBoxAntiTamper = new CheckBox();
            CheckBoxAntiDebug = new CheckBox();
            CheckBoxCallsHiding = new CheckBox();
            CheckBoxStringEncryption = new CheckBox();
            CheckBoxControlFlowDisruption = new CheckBox();
            CheckBoxCalculationsObfuscator = new CheckBox();
            CheckBoxVariableRenaming = new CheckBox();
            ButtonEntryFolder = new Button();
            TableLayoutPanelMain = new TableLayoutPanel();
            TableLayoutPanelInternal = new TableLayoutPanel();
            ButtonOutputFolderPath = new Button();
            ButtonObfuscate = new Button();
            TextBoxOutputFolderPath = new TextBox();
            TextBoxEntryFolderPath = new TextBox();
            GroupBoxObfuscationOptions.SuspendLayout();
            TableLayoutPanelMain.SuspendLayout();
            TableLayoutPanelInternal.SuspendLayout();
            SuspendLayout();
            // 
            // LabelEntryFolder
            // 
            LabelEntryFolder.Anchor = AnchorStyles.Left;
            LabelEntryFolder.AutoSize = true;
            LabelEntryFolder.BackColor = Color.FromArgb(200, 255, 255, 255);
            LabelEntryFolder.Location = new Point(3, 88);
            LabelEntryFolder.Name = "LabelEntryFolder";
            LabelEntryFolder.Size = new Size(125, 20);
            LabelEntryFolder.TabIndex = 0;
            LabelEntryFolder.Text = "Folder wejściowy:";
            // 
            // LabelOutputFolder
            // 
            LabelOutputFolder.Anchor = AnchorStyles.Left;
            LabelOutputFolder.AutoSize = true;
            LabelOutputFolder.BackColor = Color.FromArgb(200, 255, 255, 255);
            LabelOutputFolder.Font = new Font("Segoe UI", 9F);
            LabelOutputFolder.Location = new Point(3, 211);
            LabelOutputFolder.Name = "LabelOutputFolder";
            LabelOutputFolder.Size = new Size(124, 20);
            LabelOutputFolder.TabIndex = 1;
            LabelOutputFolder.Text = "Folder wyjściowy:";
            // 
            // GroupBoxObfuscationOptions
            // 
            GroupBoxObfuscationOptions.Anchor = AnchorStyles.Right;
            GroupBoxObfuscationOptions.BackColor = Color.Transparent;
            GroupBoxObfuscationOptions.Controls.Add(CheckBoxAntiTamper);
            GroupBoxObfuscationOptions.Controls.Add(CheckBoxAntiDebug);
            GroupBoxObfuscationOptions.Controls.Add(CheckBoxCallsHiding);
            GroupBoxObfuscationOptions.Controls.Add(CheckBoxStringEncryption);
            GroupBoxObfuscationOptions.Controls.Add(CheckBoxControlFlowDisruption);
            GroupBoxObfuscationOptions.Controls.Add(CheckBoxCalculationsObfuscator);
            GroupBoxObfuscationOptions.Controls.Add(CheckBoxVariableRenaming);
            GroupBoxObfuscationOptions.Location = new Point(501, 110);
            GroupBoxObfuscationOptions.Name = "GroupBoxObfuscationOptions";
            GroupBoxObfuscationOptions.Size = new Size(278, 233);
            GroupBoxObfuscationOptions.TabIndex = 2;
            GroupBoxObfuscationOptions.TabStop = false;
            GroupBoxObfuscationOptions.Text = "Opcje obfuskacji";
            // 
            // CheckBoxAntiTamper
            // 
            CheckBoxAntiTamper.AutoSize = true;
            CheckBoxAntiTamper.BackColor = Color.Transparent;
            CheckBoxAntiTamper.Location = new Point(15, 206);
            CheckBoxAntiTamper.Name = "CheckBoxAntiTamper";
            CheckBoxAntiTamper.Size = new Size(107, 24);
            CheckBoxAntiTamper.TabIndex = 6;
            CheckBoxAntiTamper.Text = "AntiTamper";
            CheckBoxAntiTamper.UseVisualStyleBackColor = false;
            // 
            // CheckBoxAntiDebug
            // 
            CheckBoxAntiDebug.AutoSize = true;
            CheckBoxAntiDebug.BackColor = Color.Transparent;
            CheckBoxAntiDebug.Location = new Point(15, 176);
            CheckBoxAntiDebug.Name = "CheckBoxAntiDebug";
            CheckBoxAntiDebug.Size = new Size(103, 24);
            CheckBoxAntiDebug.TabIndex = 5;
            CheckBoxAntiDebug.Text = "AntiDebug";
            CheckBoxAntiDebug.UseVisualStyleBackColor = false;
            // 
            // CheckBoxCallsHiding
            // 
            CheckBoxCallsHiding.AutoSize = true;
            CheckBoxCallsHiding.BackColor = Color.Transparent;
            CheckBoxCallsHiding.Checked = true;
            CheckBoxCallsHiding.CheckState = CheckState.Checked;
            CheckBoxCallsHiding.Location = new Point(15, 146);
            CheckBoxCallsHiding.Name = "CheckBoxCallsHiding";
            CheckBoxCallsHiding.Size = new Size(159, 24);
            CheckBoxCallsHiding.TabIndex = 4;
            CheckBoxCallsHiding.Text = "ukrywanie wywołań";
            CheckBoxCallsHiding.UseVisualStyleBackColor = false;
            // 
            // CheckBoxStringEncryption
            // 
            CheckBoxStringEncryption.AutoSize = true;
            CheckBoxStringEncryption.BackColor = Color.Transparent;
            CheckBoxStringEncryption.Checked = true;
            CheckBoxStringEncryption.CheckState = CheckState.Checked;
            CheckBoxStringEncryption.Location = new Point(15, 116);
            CheckBoxStringEncryption.Name = "CheckBoxStringEncryption";
            CheckBoxStringEncryption.Size = new Size(237, 24);
            CheckBoxStringEncryption.TabIndex = 3;
            CheckBoxStringEncryption.Text = "szyfrowanie ciągów znakowych";
            CheckBoxStringEncryption.UseVisualStyleBackColor = false;
            // 
            // CheckBoxControlFlowDisruption
            // 
            CheckBoxControlFlowDisruption.AutoSize = true;
            CheckBoxControlFlowDisruption.BackColor = Color.Transparent;
            CheckBoxControlFlowDisruption.Checked = true;
            CheckBoxControlFlowDisruption.CheckState = CheckState.Checked;
            CheckBoxControlFlowDisruption.Location = new Point(15, 86);
            CheckBoxControlFlowDisruption.Name = "CheckBoxControlFlowDisruption";
            CheckBoxControlFlowDisruption.Size = new Size(251, 24);
            CheckBoxControlFlowDisruption.TabIndex = 2;
            CheckBoxControlFlowDisruption.Text = "zaburzenie przepływu sterowania";
            CheckBoxControlFlowDisruption.UseVisualStyleBackColor = false;
            // 
            // CheckBoxCalculationsObfuscator
            // 
            CheckBoxCalculationsObfuscator.AutoSize = true;
            CheckBoxCalculationsObfuscator.BackColor = Color.Transparent;
            CheckBoxCalculationsObfuscator.Checked = true;
            CheckBoxCalculationsObfuscator.CheckState = CheckState.Checked;
            CheckBoxCalculationsObfuscator.Location = new Point(15, 56);
            CheckBoxCalculationsObfuscator.Name = "CheckBoxCalculationsObfuscator";
            CheckBoxCalculationsObfuscator.Size = new Size(178, 24);
            CheckBoxCalculationsObfuscator.TabIndex = 1;
            CheckBoxCalculationsObfuscator.Text = "zaciemnienie obliczeń";
            CheckBoxCalculationsObfuscator.UseVisualStyleBackColor = false;
            // 
            // CheckBoxVariableRenaming
            // 
            CheckBoxVariableRenaming.AutoSize = true;
            CheckBoxVariableRenaming.BackColor = Color.Transparent;
            CheckBoxVariableRenaming.Checked = true;
            CheckBoxVariableRenaming.CheckState = CheckState.Checked;
            CheckBoxVariableRenaming.Location = new Point(15, 26);
            CheckBoxVariableRenaming.Name = "CheckBoxVariableRenaming";
            CheckBoxVariableRenaming.Size = new Size(199, 24);
            CheckBoxVariableRenaming.TabIndex = 0;
            CheckBoxVariableRenaming.Text = "zmianna nazw zmiennych";
            CheckBoxVariableRenaming.UseVisualStyleBackColor = false;
            // 
            // ButtonEntryFolder
            // 
            ButtonEntryFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            ButtonEntryFolder.BackColor = Color.White;
            TableLayoutPanelInternal.SetColumnSpan(ButtonEntryFolder, 2);
            ButtonEntryFolder.Location = new Point(3, 118);
            ButtonEntryFolder.Name = "ButtonEntryFolder";
            ButtonEntryFolder.Size = new Size(486, 30);
            ButtonEntryFolder.TabIndex = 5;
            ButtonEntryFolder.Text = "Wybierz kod do obfuskacji";
            ButtonEntryFolder.UseVisualStyleBackColor = false;
            // 
            // TableLayoutPanelMain
            // 
            TableLayoutPanelMain.BackColor = Color.Transparent;
            TableLayoutPanelMain.ColumnCount = 2;
            TableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanelMain.Controls.Add(TableLayoutPanelInternal, 0, 0);
            TableLayoutPanelMain.Controls.Add(GroupBoxObfuscationOptions, 1, 0);
            TableLayoutPanelMain.Dock = DockStyle.Fill;
            TableLayoutPanelMain.Location = new Point(0, 0);
            TableLayoutPanelMain.Name = "TableLayoutPanelMain";
            TableLayoutPanelMain.RowCount = 1;
            TableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TableLayoutPanelMain.Size = new Size(782, 453);
            TableLayoutPanelMain.TabIndex = 9;
            // 
            // TableLayoutPanelInternal
            // 
            TableLayoutPanelInternal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanelInternal.AutoSize = true;
            TableLayoutPanelInternal.BackColor = Color.Transparent;
            TableLayoutPanelInternal.ColumnCount = 2;
            TableLayoutPanelInternal.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanelInternal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TableLayoutPanelInternal.Controls.Add(ButtonEntryFolder, 0, 2);
            TableLayoutPanelInternal.Controls.Add(LabelEntryFolder, 0, 1);
            TableLayoutPanelInternal.Controls.Add(LabelOutputFolder, 0, 4);
            TableLayoutPanelInternal.Controls.Add(ButtonOutputFolderPath, 0, 6);
            TableLayoutPanelInternal.Controls.Add(ButtonObfuscate, 0, 8);
            TableLayoutPanelInternal.Controls.Add(TextBoxOutputFolderPath, 1, 4);
            TableLayoutPanelInternal.Controls.Add(TextBoxEntryFolderPath, 1, 1);
            TableLayoutPanelInternal.Location = new Point(3, 3);
            TableLayoutPanelInternal.Name = "TableLayoutPanelInternal";
            TableLayoutPanelInternal.RowCount = 10;
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle());
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle());
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle());
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle());
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle());
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle());
            TableLayoutPanelInternal.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            TableLayoutPanelInternal.Size = new Size(492, 447);
            TableLayoutPanelInternal.TabIndex = 3;
            // 
            // ButtonOutputFolderPath
            // 
            ButtonOutputFolderPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanelInternal.SetColumnSpan(ButtonOutputFolderPath, 2);
            ButtonOutputFolderPath.Location = new Point(3, 241);
            ButtonOutputFolderPath.Name = "ButtonOutputFolderPath";
            ButtonOutputFolderPath.Size = new Size(486, 30);
            ButtonOutputFolderPath.TabIndex = 9;
            ButtonOutputFolderPath.Text = "Wybierz folder do zapisu zaciemnienego kodu";
            ButtonOutputFolderPath.UseVisualStyleBackColor = true;
            // 
            // ButtonObfuscate
            // 
            ButtonObfuscate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanelInternal.SetColumnSpan(ButtonObfuscate, 2);
            ButtonObfuscate.Location = new Point(3, 331);
            ButtonObfuscate.Name = "ButtonObfuscate";
            ButtonObfuscate.Size = new Size(486, 29);
            ButtonObfuscate.TabIndex = 10;
            ButtonObfuscate.Text = "Zaciemnij";
            ButtonObfuscate.UseVisualStyleBackColor = true;
            // 
            // TextBoxOutputFolderPath
            // 
            TextBoxOutputFolderPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TextBoxOutputFolderPath.Location = new Point(134, 208);
            TextBoxOutputFolderPath.Name = "TextBoxOutputFolderPath";
            TextBoxOutputFolderPath.Size = new Size(355, 27);
            TextBoxOutputFolderPath.TabIndex = 11;
            // 
            // TextBoxEntryFolderPath
            // 
            TextBoxEntryFolderPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TextBoxEntryFolderPath.Location = new Point(134, 85);
            TextBoxEntryFolderPath.Name = "TextBoxEntryFolderPath";
            TextBoxEntryFolderPath.Size = new Size(355, 27);
            TextBoxEntryFolderPath.TabIndex = 0;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(782, 453);
            Controls.Add(TableLayoutPanelMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(640, 285);
            Name = "MainView";
            Text = "Confuscsharpnator";
            GroupBoxObfuscationOptions.ResumeLayout(false);
            GroupBoxObfuscationOptions.PerformLayout();
            TableLayoutPanelMain.ResumeLayout(false);
            TableLayoutPanelMain.PerformLayout();
            TableLayoutPanelInternal.ResumeLayout(false);
            TableLayoutPanelInternal.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label LabelEntryFolder;
        private Label LabelOutputFolder;
        private SoupedUpGroupBox GroupBoxObfuscationOptions;
        private CheckBox CheckBoxStringEncryption;
        private CheckBox CheckBoxControlFlowDisruption;
        private CheckBox CheckBoxCalculationsObfuscator;
        private CheckBox CheckBoxVariableRenaming;
        private CheckBox CheckBoxAntiDebug;
        private CheckBox CheckBoxCallsHiding;
        private CheckBox CheckBoxAntiTamper;
        private Button ButtonEntryFolder;
        private TableLayoutPanel TableLayoutPanelMain;
        private TableLayoutPanel TableLayoutPanelInternal;
        private Button ButtonOutputFolderPath;
        private Button ButtonObfuscate;
        private TextBox TextBoxOutputFolderPath;
        private TextBox TextBoxEntryFolderPath;
    }
}