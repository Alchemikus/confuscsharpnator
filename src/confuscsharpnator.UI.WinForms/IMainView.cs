namespace confuscsharpnator.UI.WinForms
{
    public interface IMainView : IDisposable
    {
        public void ShowMessage(string message, string? caption = null, MessageBoxIcon? icon = null);
        public void ClearFields();
        public void FreezeActions(bool freeze);
        public void Show();
        public event Action Obfuscate;
        public event Action ViewClosing;
    }
}
