namespace confuscsharpnator.UI.WinForms
{
    internal class SoupedUpGroupBox : GroupBox
    {
        public SoupedUpGroupBox()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new(Color.FromArgb(170, 255, 255, 255)))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            base.OnPaint(e);
        }
    }
}
