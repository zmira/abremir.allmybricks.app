using abremir.AllMyBricks.Platform.Configuration;
using abremir.AllMyBricks.Platform.Interfaces;
using abremir.AllMyBricks.Platform.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstituteAutoMocker.Standard;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Platform.Tests.Services
{
    [TestClass]
    public class FileSystemServiceTest
    {
        private NSubstituteAutoMocker<FileSystemService> _fileSystemService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fileSystemService = new NSubstituteAutoMocker<FileSystemService>();

            _fileSystemService.Get<IFileSystem>()
                .AppDataDirectory
                .Returns("./");
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
            var localPathToFile = _fileSystemService.ClassUnderTest.GetLocalPathToFile(filename, folder);

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
            var thumbnailFolder = _fileSystemService.ClassUnderTest.GetThumbnailFolder(theme, subtheme);

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
        public async Task SaveThumbnailToCache_InvokesWriteAllBytes(string filename, byte[] thumbnail, bool invokesWriteAllBytes)
        {
            await _fileSystemService.ClassUnderTest.SaveThumbnailToCache(string.Empty, string.Empty, filename, thumbnail);

            if (invokesWriteAllBytes)
            {
                await _fileSystemService.Get<IFile>().Received().WriteAllBytes(Arg.Any<string>(), Arg.Any<byte[]>());
            }
            else
            {
                await _fileSystemService.Get<IFile>().DidNotReceive().WriteAllBytes(Arg.Any<string>(), Arg.Any<byte[]>());
            }
        }
    }
}
