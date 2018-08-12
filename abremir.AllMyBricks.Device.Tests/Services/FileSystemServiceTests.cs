using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Device.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Tests.Services
{
    [TestClass]
    public class FileSystemServiceTest
    {
        private static IFileSystemService _fileSystemService;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.AppDataDirectory.Returns("./");

            _fileSystemService = new FileSystemService(fileSystem);
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow(" ", null)]
        [DataRow(null, "")]
        [DataRow(null, " ")]
        [DataRow("", "")]
        [DataRow(" ", " ")]
        [DataRow("FILENAME", null)]
        [DataRow("FILENAME", "")]
        [DataRow("FILENAME", " ")]
        [DataRow(null, "FOLDERNAME")]
        [DataRow("", "FOLDERNAME")]
        [DataRow(" ", "FOLDERNAME")]
        [DataRow("FILENAME", "FOLDERNAME")]
        public void GetLocalPathToFile_ReturnsValidPathWithFilename(string filename, string folder)
        {
            var result = _fileSystemService.GetLocalPathToFile(filename, folder);

            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Contain(Constants.AllMyBricksDataFolder);

            if (!string.IsNullOrWhiteSpace(filename))
            {
                result.Should().EndWith(filename);
            }

            if (!string.IsNullOrWhiteSpace(folder))
            {
                result.Should().Contain(folder);
            }
        }
    }
}