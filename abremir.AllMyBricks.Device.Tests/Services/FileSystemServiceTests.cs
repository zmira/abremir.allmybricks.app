using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Device.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Text.RegularExpressions;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Tests.Services
{
    [TestClass]
    public class FileSystemServiceTest
    {
        private IFileSystemService _fileSystemService;
        private IFile _file;

        [TestInitialize]
        public void TestInitialize()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.AppDataDirectory.Returns("./");

            _file = Substitute.For<IFile>();

            _fileSystemService = new FileSystemService(fileSystem, _file);
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
            var localPathToFile = _fileSystemService.GetLocalPathToFile(filename, folder);

            localPathToFile.Should().NotBeNullOrWhiteSpace();
            localPathToFile.Should().Contain(Constants.AllMyBricksDataFolder);

            if (!string.IsNullOrWhiteSpace(filename))
            {
                localPathToFile.Should().EndWith(filename);
            }

            if (!string.IsNullOrWhiteSpace(folder))
            {
                localPathToFile.Should().Contain(folder);
            }
        }

        [DataTestMethod]
        [DataRow(null, null, 2)]
        [DataRow("", null, 2)]
        [DataRow(null, "", 2)]
        [DataRow("", "", 2)]
        [DataRow("THEME", null, 1)]
        [DataRow("", "SUBTHEME", 1)]
        [DataRow("THEME", "SUBTHEME", 0)]
        public void GetThumbnailFolder_ReturnsValidPath(string theme, string subtheme, int countOfFallbackFolderName)
        {
            var thumbnailFolder = _fileSystemService.GetThumbnailFolder(theme, subtheme);

            thumbnailFolder.Should().NotBeNullOrWhiteSpace();
            thumbnailFolder.Should().Contain(Constants.AllMyBricksDataFolder);
            thumbnailFolder.Should().Contain(Constants.ThumbnailCacheFolder);

            Regex.Matches(thumbnailFolder, Constants.FallbackFolderName).Count.Should().Be(countOfFallbackFolderName);
        }

        [DataTestMethod]
        [DataRow(null, null, false)]
        [DataRow("", null, false)]
        [DataRow(null, new byte[] { }, false)]
        [DataRow("", new byte[] { }, false)]
        [DataRow("FILENAME", null, false)]
        [DataRow("FILENAME", new byte[] { }, false)]
        [DataRow(null, new byte[] { 0 }, false)]
        [DataRow("", new byte[] { 0 }, false)]
        [DataRow("FILENAME", new byte[] { 0 }, true)]
        public void SaveThumbnailToCache_InvokesWriteAllBytes(string filename, byte[] thumbnail, bool invokesWriteAllBytes)
        {
            _fileSystemService.SaveThumbnailToCache(string.Empty, string.Empty, filename, thumbnail);

            if (invokesWriteAllBytes)
            {
                _file.Received().WriteAllBytes(Arg.Any<string>(), Arg.Any<byte[]>());
            }
            else
            {
                _file.DidNotReceive().WriteAllBytes(Arg.Any<string>(), Arg.Any<byte[]>());
            }
        }
    }
}