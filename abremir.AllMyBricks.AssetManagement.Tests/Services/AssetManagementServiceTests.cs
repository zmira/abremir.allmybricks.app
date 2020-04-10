using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.AssetManagement.Services;
using abremir.AllMyBricks.Platform.Interfaces;
using Flurl.Http.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using NSubstitute;
using NSubstituteAutoMocker.Standard;
using System.IO;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.AssetManagement.Tests.Services
{
    [TestClass]
    public class AssetManagementServiceTests
    {
        private NSubstituteAutoMocker<AssetManagementService> _assetManagementService;
        private HttpTest _httpTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpTest = new HttpTest();
            _assetManagementService = new NSubstituteAutoMocker<AssetManagementService>();
        }

        [DataTestMethod]
        [DataRow("", "C:\\", true)]
        [DataRow(null, "C:\\", true)]
        [DataRow("http", "C:\\", true)]
        [DataRow("http://www.google", "C:\\", true)]
        [DataRow("http://www.google.com", "C:\\", true)]
        [DataRow("http://www.google.com/test.lz", "C:\\test.file", false)]
        public async Task InstallAllMyBricksSeedDatabase_InvalidParameters_ResultIsFalse(string databaseSeedUrl, string targetFolderPath, bool directoryExists)
        {
            _assetManagementService.Get<IDirectory>()
                .Exists(Arg.Any<string>())
                .Returns(directoryExists);

            var result = await _assetManagementService.ClassUnderTest.InstallAllMyBricksSeedDatabase(databaseSeedUrl, targetFolderPath);

            Check.That(result).IsFalse();
            _httpTest.ShouldNotHaveMadeACall();
            _assetManagementService.Get<IAssetUncompression>()
                .DidNotReceiveWithAnyArgs()
                .UncompressAsset(Arg.Any<Stream>(), Arg.Any<string>());
        }

        [TestMethod]
        public async Task InstallAllMyBricksSeedDatabase_ValidParameters_ResultIsTrue()
        {
            _assetManagementService.Get<IAssetUncompression>()
                .UncompressAsset(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<bool>())
                .Returns(true);
            _assetManagementService.Get<IDirectory>()
                .Exists(Arg.Any<string>())
                .Returns(true);

            var result = await _assetManagementService.ClassUnderTest.InstallAllMyBricksSeedDatabase("http://www.google.com/test.lz", "C:\\");

            Check.That(result).IsTrue();
            _httpTest.ShouldHaveMadeACall();
            _assetManagementService.Get<IAssetUncompression>()
                .ReceivedWithAnyArgs()
                .UncompressAsset(Arg.Any<Stream>(), Arg.Any<string>());
        }
    }
}
