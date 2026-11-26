using System.Windows.Forms;
using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.UI.WinForms;
using Moq;

namespace confuscsharpnator.Presentation.Tests
{
    public class MainPresenterTests : IDisposable
    {
        private readonly Mock<IMainView> _viewMock;
        private readonly Mock<IObfuscatorMetadata> _metadataMock;
        private readonly Mock<IObfuscationService> _serviceMock;
        private readonly MainPresenter _presenter;

        private readonly List<string> _tempDirectories = [];
        private readonly List<string> _tempFiles = [];

        public MainPresenterTests()
        {
            _viewMock = new Mock<IMainView>();
            _metadataMock = new Mock<IObfuscatorMetadata>();
            _serviceMock = new Mock<IObfuscationService>();

            _serviceMock.Setup(s => s.InitializeAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _serviceMock.Setup(s => s.SaveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            _presenter = new MainPresenter(_viewMock.Object, _metadataMock.Object, _serviceMock.Object);
        }

        [Fact]
        public void Run_ShouldCallShowOnView()
        {
            // Act
            _presenter.Run();

            // Assert
            _viewMock.Verify(v => v.Show(), Times.Once);
        }

        [Fact]
        public void ViewClosing_ShouldInvokeCloseApplicationDelegate()
        {
            // Arrange
            bool delegateInvoked = false;
            _presenter.CloseApplication = () => delegateInvoked = true;

            // Act
            _viewMock.Raise(v => v.ViewClosing += null);

            // Assert
            Assert.True(delegateInvoked);
        }

        [Theory]
        [InlineData("", "C:\\out", "Rename", "Wybierz plik rozwiązania (.sln lub .slnx).")]
        [InlineData("C:\\in\\app.sln", "", "Rename", "Wybierz folder wyjściowy dla zaciemnionego kodu.")]
        [InlineData("C:\\in\\app.sln", "C:\\out", "", "Zaznacz co najmniej jedną opcję obfuskacji.")]
        public void OnObfuscate_ValidationFails_ShouldShowErrorMessageAndAbort(
            string inputPath, string outputPath, string selectedStep, string expectedMessage)
        {
            // Arrange
            _metadataMock.Setup(m => m.InputSolutionPath).Returns(inputPath);
            _metadataMock.Setup(m => m.OutputFolderPath).Returns(outputPath);

            var steps = string.IsNullOrEmpty(selectedStep) ? [] : new[] { selectedStep };
            _metadataMock.Setup(m => m.SelectedStepNames).Returns(steps);

            // Act (walidacja kończy się synchronicznie przed Task.Run)
            _viewMock.Raise(v => v.Obfuscate += null);

            // Assert
            _viewMock.Verify(v => v.ShowMessage(expectedMessage, "Błąd walidacji", MessageBoxIcon.Error), Times.Once);
            _serviceMock.Verify(s => s.InitializeAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void OnObfuscate_NonExistentFile_ShouldShowValidationError()
        {
            // Arrange
            string missingFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.sln");

            _metadataMock.Setup(m => m.InputSolutionPath).Returns(missingFilePath);
            _metadataMock.Setup(m => m.OutputFolderPath).Returns("C:\\Output");
            _metadataMock.Setup(m => m.SelectedStepNames).Returns(["Rename"]);

            // Act
            _viewMock.Raise(v => v.Obfuscate += null);

            // Assert
            _viewMock.Verify(v => v.ShowMessage("Wskazany plik rozwiązania nie istnieje.", "Błąd walidacji", MessageBoxIcon.Error), Times.Once);
            _serviceMock.Verify(s => s.InitializeAsync(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("TestProject.csproj")]
        [InlineData("notes.txt")]
        public void OnObfuscate_InvalidFileExtension_ShouldShowValidationError(string fileName)
        {
            // Arrange
            string tempDir = CreateTempDirectory();
            string invalidFilePath = CreateTempFile(tempDir, fileName);

            _metadataMock.Setup(m => m.InputSolutionPath).Returns(invalidFilePath);
            _metadataMock.Setup(m => m.OutputFolderPath).Returns("C:\\Output");
            _metadataMock.Setup(m => m.SelectedStepNames).Returns(["Rename"]);

            // Act
            _viewMock.Raise(v => v.Obfuscate += null);

            // Assert
            _viewMock.Verify(v => v.ShowMessage("Wskazany plik musi być plikiem rozwiązania (.sln lub .slnx).", "Błąd walidacji", MessageBoxIcon.Error), Times.Once);
            _serviceMock.Verify(s => s.InitializeAsync(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("TestProject.sln")]
        [InlineData("TestProject.slnx")]
        public async Task OnObfuscate_ValidSolutionFile_ShouldExecuteFullSuccessFlow(string fileName)
        {
            // Arrange
            string tempDir = CreateTempDirectory();
            string solutionPath = CreateTempFile(tempDir, fileName);

            _metadataMock.Setup(m => m.InputSolutionPath).Returns(solutionPath);
            _metadataMock.Setup(m => m.OutputFolderPath).Returns("C:\\ValidOutput");
            _metadataMock.Setup(m => m.SelectedStepNames).Returns(["StringEncryption"]);

            var completed = WaitForObfuscationCompletion();

            // Act
            _viewMock.Raise(v => v.Obfuscate += null);
            await completed.WaitAsync(TimeSpan.FromSeconds(5));

            // Assert
            _viewMock.Verify(v => v.FreezeActions(true), Times.Once);
            _viewMock.Verify(v => v.SetTitle("Trwa zaciemnianie..."), Times.Once);

            _serviceMock.Verify(s => s.InitializeAsync(solutionPath), Times.Once);
            _serviceMock.Verify(s => s.Obfuscate(It.Is<IEnumerable<string>>(steps => steps.Contains("StringEncryption"))), Times.Once);
            _serviceMock.Verify(s => s.SaveAsync("C:\\ValidOutput"), Times.Once);

            _viewMock.Verify(v => v.ShowMessage("Zaciemnianie zakończyło się pomyślnie!", "Sukces", MessageBoxIcon.Information), Times.Once);

            _viewMock.Verify(v => v.FreezeActions(false), Times.Once);
            _viewMock.Verify(v => v.SetTitle(null), Times.Once);
        }

        [Fact]
        public async Task OnObfuscate_ServiceThrowsException_ShouldCatchAndShowErrorAndUnfreeze()
        {
            // Arrange
            string tempDir = CreateTempDirectory();
            string solutionPath = CreateTempFile(tempDir, "TestProject.slnx");

            _metadataMock.Setup(m => m.InputSolutionPath).Returns(solutionPath);
            _metadataMock.Setup(m => m.OutputFolderPath).Returns("C:\\Output");
            _metadataMock.Setup(m => m.SelectedStepNames).Returns(["Rename"]);

            _serviceMock.Setup(s => s.InitializeAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Brak dostępu do pliku."));

            var completed = WaitForObfuscationCompletion();

            // Act
            _viewMock.Raise(v => v.Obfuscate += null);
            await completed.WaitAsync(TimeSpan.FromSeconds(5));

            // Assert
            _viewMock.Verify(v => v.ShowMessage("Wystąpił błąd podczas obfuskacji:\n\nBrak dostępu do pliku.", "Błąd krytyczny", MessageBoxIcon.Error), Times.Once);
            _viewMock.Verify(v => v.FreezeActions(false), Times.Once);
            _viewMock.Verify(v => v.SetTitle(null), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldUnsubscribeEventsAndDisposeView()
        {
            // Arrange
            bool delegateInvoked = false;
            _presenter.CloseApplication = () => delegateInvoked = true;

            // Act
            _presenter.Dispose();
            _viewMock.Raise(v => v.ViewClosing += null);
            _viewMock.Raise(v => v.Obfuscate += null);

            // Assert
            Assert.False(delegateInvoked);
            _viewMock.Verify(v => v.ShowMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxIcon?>()), Times.Never);
            _viewMock.Verify(v => v.Dispose(), Times.Once);
        }

        // --- Helper methods ---

        private Task WaitForObfuscationCompletion()
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _viewMock.Setup(v => v.SetTitle(null)).Callback(() => { tcs.TrySetResult(); });
            return tcs.Task;
        }

        private string CreateTempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            _tempDirectories.Add(path);
            return path;
        }

        private string CreateTempFile(string directoryPath, string fileName)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(filePath, "dummy content");
            _tempFiles.Add(filePath);
            return filePath;
        }

        public void Dispose()
        {
            _presenter.Dispose();

            foreach (var file in _tempFiles)
            {
                if (File.Exists(file)) File.Delete(file);
            }

            foreach (var dir in _tempDirectories)
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }

            GC.SuppressFinalize(this);
        }
    }
}