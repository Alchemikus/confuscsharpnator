using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.UI.WinForms;
using System.Windows.Forms;

namespace confuscsharpnator.Presentation
{
    public class MainPresenter : IDisposable
    {
        public Action? CloseApplication;
        private bool _disposed = false;
        private readonly IMainView _mainView;
        private readonly IObfuscatorMetadata _obfuscatorMetadata;
        private readonly IObfuscationService _obfuscationService;

        public MainPresenter(IMainView mainView, IObfuscatorMetadata obfuscatorMetadata, IObfuscationService obfuscationService)
        {
            _mainView = mainView;
            _obfuscatorMetadata = obfuscatorMetadata;
            _obfuscationService = obfuscationService;

            // Every action that we subscribe has to be unsubscribed in Dispose(bool) method to avoid memory leaks.
            _mainView.ViewClosing += OnMainViewClosing;
            _mainView.Obfuscate += OnObfuscate;
        }

        /// <summary>
        /// Entry point for the presenter.
        /// It shows view.
        /// </summary>
        public void Run()
        {
            _mainView.Show();
        }

        private void OnMainViewClosing() => CloseApplication?.Invoke();

        private async void OnObfuscate()
        {
            var inputPath = _obfuscatorMetadata.InputSolutionPath;
            var outputPath = _obfuscatorMetadata.OutputFolderPath;
            var selectedSteps = _obfuscatorMetadata.SelectedStepNames.ToList();

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                _mainView.ShowMessage("Wybierz plik rozwiązania (.sln lub .slnx).", "Błąd walidacji", MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                _mainView.ShowMessage("Wybierz folder wyjściowy dla zaciemnionego kodu.", "Błąd walidacji", MessageBoxIcon.Error);
                return;
            }

            if (selectedSteps.Count == 0)
            {
                _mainView.ShowMessage("Zaznacz co najmniej jedną opcję obfuskacji.", "Błąd walidacji", MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(inputPath))
            {
                _mainView.ShowMessage("Wskazany plik rozwiązania nie istnieje.", "Błąd walidacji", MessageBoxIcon.Error);
                return;
            }

            var extension = Path.GetExtension(inputPath);
            if (!extension.Equals(".sln", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".slnx", StringComparison.OrdinalIgnoreCase))
            {
                _mainView.ShowMessage("Wskazany plik musi być plikiem rozwiązania (.sln lub .slnx).", "Błąd walidacji", MessageBoxIcon.Error);
                return;
            }

            try
            {
                _mainView.FreezeActions(true);
                _mainView.SetTitle("Trwa zaciemnianie...");

                await Task.Run(async () =>
                {
                    await _obfuscationService.InitializeAsync(inputPath);
                    _obfuscationService.Obfuscate(selectedSteps);
                    await _obfuscationService.SaveAsync(outputPath);
                });

                _mainView.ShowMessage("Zaciemnianie zakończyło się pomyślnie!", "Sukces", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _mainView.ShowMessage($"Wystąpił błąd podczas obfuskacji:\n\n{ex.Message}", "Błąd krytyczny", MessageBoxIcon.Error);
            }
            finally
            {
                _mainView.FreezeActions(false);
                _mainView.SetTitle(null);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            // unsubscribing from events
            _mainView.ViewClosing -= OnMainViewClosing;
            _mainView.Obfuscate -= OnObfuscate;

            // disposing elements
            _mainView.Dispose();

            _disposed = true;
        }
    }
}
