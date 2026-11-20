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
            LabelEntrySolutionPath = new Label();
            LabelOutputFolder = new Label();
            GroupBoxObfuscationOptions = new SoupedUpGroupBox();
            ButtonEntrySolutionPath = new Button();
            TableLayoutPanelMain = new TableLayoutPanel();
            TableLayoutPanelInternal = new TableLayoutPanel();
            ButtonOutputFolderPath = new Button();
            ButtonObfuscate = new Button();
            TextBoxOutputFolderPath = new TextBox();
            TextBoxEntrySolutionPath = new TextBox();
            TableLayoutPanelMain.SuspendLayout();
            TableLayoutPanelInternal.SuspendLayout();
            SuspendLayout();
            // 
            // LabelEntrySolutionPath
            // 
            LabelEntrySolutionPath.Anchor = AnchorStyles.Left;
            LabelEntrySolutionPath.AutoSize = true;
            LabelEntrySolutionPath.BackColor = Color.FromArgb(200, 255, 255, 255);
            LabelEntrySolutionPath.Location = new Point(3, 88);
            LabelEntrySolutionPath.Name = "LabelEntrySolutionPath";
            LabelEntrySolutionPath.Size = new Size(129, 20);
            LabelEntrySolutionPath.TabIndex = 0;
            LabelEntrySolutionPath.Text = "Projekt wejściowy:";
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
            GroupBoxObfuscationOptions.Anchor = AnchorStyles.None;
            GroupBoxObfuscationOptions.BackColor = Color.Transparent;
            GroupBoxObfuscationOptions.Location = new Point(501, 110);
            GroupBoxObfuscationOptions.Name = "GroupBoxObfuscationOptions";
            GroupBoxObfuscationOptions.Size = new Size(278, 233);
            GroupBoxObfuscationOptions.TabIndex = 2;
            GroupBoxObfuscationOptions.TabStop = false;
            GroupBoxObfuscationOptions.Text = "Opcje obfuskacji";
            // 
            // ButtonEntrySolutionPath
            // 
            ButtonEntrySolutionPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            ButtonEntrySolutionPath.BackColor = Color.White;
            TableLayoutPanelInternal.SetColumnSpan(ButtonEntrySolutionPath, 2);
            ButtonEntrySolutionPath.Location = new Point(3, 118);
            ButtonEntrySolutionPath.Name = "ButtonEntrySolutionPath";
            ButtonEntrySolutionPath.Size = new Size(486, 30);
            ButtonEntrySolutionPath.TabIndex = 5;
            ButtonEntrySolutionPath.Text = "Wybierz kod do obfuskacji";
            ButtonEntrySolutionPath.UseVisualStyleBackColor = false;
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
            TableLayoutPanelInternal.Controls.Add(ButtonEntrySolutionPath, 0, 2);
            TableLayoutPanelInternal.Controls.Add(LabelEntrySolutionPath, 0, 1);
            TableLayoutPanelInternal.Controls.Add(LabelOutputFolder, 0, 4);
            TableLayoutPanelInternal.Controls.Add(ButtonOutputFolderPath, 0, 6);
            TableLayoutPanelInternal.Controls.Add(ButtonObfuscate, 0, 8);
            TableLayoutPanelInternal.Controls.Add(TextBoxOutputFolderPath, 1, 4);
            TableLayoutPanelInternal.Controls.Add(TextBoxEntrySolutionPath, 1, 1);
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
            TextBoxOutputFolderPath.Location = new Point(138, 208);
            TextBoxOutputFolderPath.Name = "TextBoxOutputFolderPath";
            TextBoxOutputFolderPath.Size = new Size(351, 27);
            TextBoxOutputFolderPath.TabIndex = 11;
            // 
            // TextBoxEntrySolutionPath
            // 
            TextBoxEntrySolutionPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TextBoxEntrySolutionPath.Location = new Point(138, 85);
            TextBoxEntrySolutionPath.Name = "TextBoxEntrySolutionPath";
            TextBoxEntrySolutionPath.Size = new Size(351, 27);
            TextBoxEntrySolutionPath.TabIndex = 0;
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
            TableLayoutPanelMain.ResumeLayout(false);
            TableLayoutPanelMain.PerformLayout();
            TableLayoutPanelInternal.ResumeLayout(false);
            TableLayoutPanelInternal.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label LabelEntrySolutionPath;
        private Label LabelOutputFolder;
        private SoupedUpGroupBox GroupBoxObfuscationOptions;
        private Button ButtonEntrySolutionPath;
        private TableLayoutPanel TableLayoutPanelMain;
        private TableLayoutPanel TableLayoutPanelInternal;
        private Button ButtonOutputFolderPath;
        private Button ButtonObfuscate;
        private TextBox TextBoxOutputFolderPath;
        private TextBox TextBoxEntrySolutionPath;
    }
}