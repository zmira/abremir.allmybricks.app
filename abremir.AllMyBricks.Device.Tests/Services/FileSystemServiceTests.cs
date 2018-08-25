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
        private static IFileSystemService _fileSystemService;
        private static IFile _file;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
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
            var result = _fileSystemService.GetThumbnailFolder(theme, subtheme);

            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Contain(Constants.AllMyBricksDataFolder);
            result.Should().Contain(Constants.ThumbnailCacheFolder);

            Regex.Matches(result, Constants.FallbackFolderName).Count.Should().Be(countOfFallbackFolderName);
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