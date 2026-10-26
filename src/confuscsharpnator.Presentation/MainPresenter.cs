using confuscsharpnator.UI.WinForms;

namespace confuscsharpnator.Presentation
{
    internal class MainPresenter : IDisposable
    {
        public Action? CloseApplication;
        private bool _disposed = false;
        private readonly IMainView _mainView;
        private readonly IObfuscatorMetadata _obfuscatorMetadata;

        public MainPresenter(IMainView mainView, IObfuscatorMetadata obfuscatorMetadata)
        {
            _mainView = mainView;
            _obfuscatorMetadata = obfuscatorMetadata;

            // Every action that we subscribe has to be unsubscribed in Dispose method to avoid memory leaks.
            _mainView.ViewClosing += OnMainViewClosing;
            _mainView.Obfuscate += OnObfuscate;
        }

        public void Run()
        {
            _mainView.Show();
        }

        private void OnMainViewClosing() => CloseApplication?.Invoke();

        private void OnObfuscate() => _mainView.ShowMessage("Obfuscation started.");

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            //unsibscribing from events
            _mainView.ViewClosing -= OnMainViewClosing;
            _mainView.Obfuscate -= OnObfuscate;

            //disposing elements
            _mainView.Dispose();

            _disposed = true;
        }
    }
}
