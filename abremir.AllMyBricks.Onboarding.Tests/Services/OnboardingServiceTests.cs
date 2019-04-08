using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstituteAutoMocker.Standard;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Onboarding.Tests.Services
{
    [TestClass]
    public class OnboardingServiceTests
    {
        private NSubstituteAutoMocker<OnboardingService> _onboardingService;

        [TestInitialize]
        public void TestInitialize()
        {
            _onboardingService = new NSubstituteAutoMocker<OnboardingService>();
        }

        [TestMethod]
        public async Task GetBricksetApiKey_ApiKeyExistsStored_ReturnsApiKeyAndSaveBricksetApiKeyNotInvoked()
        {
            _onboardingService.Get<ISecureStorageService>()
                .GetBricksetApiKey()
                .Returns("API KEY");

            var apiKey = await _onboardingService.ClassUnderTest.GetBricksetApiKey();

            apiKey.Should().NotBeNullOrWhiteSpace();
            await _onboardingService.Get<ISecureStorageService>().DidNotReceive().SaveBricksetApiKey(Arg.Any<string>());
        }

        [TestMethod]
        public async Task GetBricksetApiKey_ApiKeyNotStoredButIdentificationStored_ReturnsApiKeyAndSaveDeviceIdentificationNotInvoked()
        {
            _onboardingService.Get<ISecureStorageService>()
                .GetBricksetApiKey()
                .Returns(string.Empty);
            _onboardingService.Get<ISecureStorageService>()
                .IsDeviceIdentificationCreated()
                .Returns(true);
            _onboardingService.Get<ISecureStorageService>()
                .GetDeviceIdentification()
                .Returns(new Core.Models.Identification());
            _onboardingService.Get<IApiKeyService>()
                .GetBricksetApiKey(Arg.Any<Core.Models.Identification>())
                .Returns("API KEY");

            var apiKey = await _onboardingService.ClassUnderTest.GetBricksetApiKey();

            apiKey.Should().NotBeNullOrWhiteSpace();
            await _onboardingService.Get<IApiKeyService>().Received().GetBricksetApiKey(Arg.Any<Core.Models.Identification>());
            await _onboardingService.Get<ISecureStorageService>().DidNotReceive().SaveDeviceIdentification(Arg.Any<Core.Models.Identification>());
        }

        [TestMethod]
        public async Task GetBricksetApiKey_ApiKeyAndIdentificationNotStored_ReturnsApiKeyAndSaveDeviceIdentificationInvoked()
        {
            _onboardingService.Get<ISecureStorageService>()
                .GetBricksetApiKey()
                .Returns(string.Empty);
            _onboardingService.Get<ISecureStorageService>()
                .IsDeviceIdentificationCreated()
                .Returns(false);
            _onboardingService.Get<IRegistrationService>()
                .Register(Arg.Any<Core.Models.Identification>())
                .Returns(new Core.Models.Identification());
            _onboardingService.Get<IApiKeyService>()
                .GetBricksetApiKey(Arg.Any<Core.Models.Identification>())
                .Returns("API KEY");

            var apiKey = await _onboardingService.ClassUnderTest.GetBricksetApiKey();

            apiKey.Should().NotBeNullOrWhiteSpace();
            await _onboardingService.Get<IApiKeyService>().Received().GetBricksetApiKey(Arg.Any<Core.Models.Identification>());
            await _onboardingService.Get<ISecureStorageService>().Received().SaveDeviceIdentification(Arg.Any<Core.Models.Identification>());
        }
    }
}