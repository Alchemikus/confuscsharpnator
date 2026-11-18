using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Models;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Moq;

namespace confuscsharpnator.Core.Tests
{
    /// <summary>
    /// Unit tests for the ObfuscationService class, focusing on state management, 
    /// validation guard clauses, and proper resource disposal.
    /// </summary>
    public class ObfuscationServiceTests
    {
        private readonly Mock<IFileWriter> _fileWriterMock;
        private readonly Mock<IObfuscatorStep> _dummyStepMock;
        private readonly List<IObfuscatorStep> _injectedSteps;

        static ObfuscationServiceTests()
        {
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }
        }

        public ObfuscationServiceTests()
        {
            // Arrange: Setup standard mocks used across tests
            _fileWriterMock = new Mock<IFileWriter>();
            _dummyStepMock = new Mock<IObfuscatorStep>();

            // Mocking the step to act as real
            _dummyStepMock.Setup(s => s.Name).Returns("Rename");

            _dummyStepMock
                .Setup(s => s.Execute(It.IsAny<Solution>(), It.IsAny<ObfuscationContext>()))
                .Returns((Solution sol, ObfuscationContext ctx) => sol);

            _injectedSteps = [_dummyStepMock.Object];
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task InitializeAsync_WithNullOrEmptyPath_ThrowsArgumentException(string? invalidPath)
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.InitializeAsync(invalidPath!));
            Assert.Contains("Invalid solution file path", exception.Message);
        }

        [Fact]
        public async Task InitializeAsync_WhenFileDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            // Create a guaranteed non-existent path
            string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "missing.sln");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.InitializeAsync(nonExistentPath));
        }

        [Fact]
        public void Obfuscate_WhenNotInitialized_ThrowsInvalidOperationException()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            var selectedSteps = new List<string> { "Rename" };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => service.Obfuscate(selectedSteps));
            Assert.Contains("Service is not initialized", exception.Message);
        }

        [Fact]
        public async Task SaveAsync_WhenNotInitialized_ThrowsInvalidOperationException()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);
            string targetPath = Path.Combine(Path.GetTempPath(), "output");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.SaveAsync(targetPath));
            Assert.Contains("No solution to save", exception.Message);
        }

        [Fact]
        public void Dispose_WhenCalledMultipleTimes_DoesNotThrowException()
        {
            // Arrange
            var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);

            // Act
            service.Dispose();

            // Record.Exception captures any exception thrown by the action, or returns null if none.
            var exception = Record.Exception(() => service.Dispose());

            // Assert
            Assert.Null(exception); // Proves that _disposed flag logic works perfectly
        }

        [Fact]
        public async Task ObfuscationPipeline_HappyPath_ExecutesStepsAndSavesFiles()
        {
            // Arrange
            using var service = new ObfuscationService(_fileWriterMock.Object, _injectedSteps);

            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            string slnPath = Path.Combine(tempDir, "DummySolution.sln");

            // Minimalistic solution file content to satisfy MSBuild
            await File.WriteAllTextAsync(slnPath, "Microsoft Visual Studio Solution File, Format Version 12.00", TestContext.Current.CancellationToken);

            string outputDir = Path.Combine(tempDir, "Output");

            try
            {
                // Act
                await service.InitializeAsync(slnPath);
                service.Obfuscate(["Rename"]);
                await service.SaveAsync(outputDir);

                // Assert

                _dummyStepMock.Verify(s => s.Execute(
                    It.IsAny<Solution>(),
                    It.IsAny<ObfuscationContext>()),
                    Times.Once);

                _fileWriterMock.Verify(fw => fw.WriteAllFilesAsync(
                    outputDir,
                    It.IsAny<Dictionary<string, string>>()),
                    Times.Once);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
        }
    }
}