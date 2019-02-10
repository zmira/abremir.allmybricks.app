using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.AssetManagement.Services;
using abremir.AllMyBricks.Device.Interfaces;
using FluentAssertions;
using Flurl.Http.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.AssetManagement.Tests.Services
{
    [TestClass]
    public class AssetManagementServiceTests
    {
        private IAssetManagementService _assetManagementService;
        private IAssetUncompression _assetCompression;
        private IDirectory _directory;
        private HttpTest _httpTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpTest = new HttpTest();
            _assetCompression = Substitute.For<IAssetUncompression>();
            _directory = Substitute.For<IDirectory>();

            _assetManagementService = new AssetManagementService(_assetCompression, _directory);
        }

        [DataTestMethod]
        [DataRow("", "C:\\", true)]
        [DataRow(null, "C:\\", true)]
        [DataRow("http", "C:\\", true)]
        [DataRow("http://www.google", "C:\\", true)]
        [DataRow("http://www.google.com", "C:\\", true)]
        [DataRow("http://www.google.com/test.lz", "C:\\test.file", false)]
        public async Task InstallAllMyBricksSeedDatabase_InvalidParameters_ResultIsFalse(string databaseSeedUrl, string destinationFolderPath, bool directoryExists)
        {
            _directory
                .Exists(Arg.Any<string>())
                .Returns(directoryExists);

            var result = await _assetManagementService.InstallAllMyBricksSeedDatabase(databaseSeedUrl, destinationFolderPath);

            result.Should().BeFalse();
            _httpTest.ShouldNotHaveMadeACall();
            _assetCompression.DidNotReceiveWithAnyArgs().UncompressAsset(Arg.Any<Stream>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task InstallAllMyBricksSeedDatabase_ValidParameters_ResultIsTrue()
        {
            _assetCompression
                .UncompressAsset(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<bool>())
                .Returns(true);
            _directory
                .Exists(Arg.Any<string>())
                .Returns(true);

            var result = await _assetManagementService.InstallAllMyBricksSeedDatabase("http://www.google.com/test.lz", "C:\\");

            result.Should().BeTrue();
            _httpTest.ShouldHaveMadeACall();
            _assetCompression.ReceivedWithAnyArgs().UncompressAsset(Arg.Any<Stream>(), Arg.Any<string>());
        }
    }
}
