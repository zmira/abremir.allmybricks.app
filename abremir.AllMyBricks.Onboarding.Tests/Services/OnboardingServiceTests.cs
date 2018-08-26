using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace abremir.AllMyBricks.Onboarding.Tests.Services
{
    [TestClass]
    public class OnboardingServiceTests
    {
        private OnboardingService _onboardingService;
        private ISecureStorageService _secureStorageService;
        private IApiKeyService _apiKeyService;
        private IRegistrationService _registrationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _secureStorageService = Substitute.For<ISecureStorageService>();
            _apiKeyService = Substitute.For<IApiKeyService>();
            _registrationService = Substitute.For<IRegistrationService>();

            var deviceInformationService = Substitute.For<IDeviceInformationService>();

            _onboardingService = new OnboardingService(_secureStorageService, _registrationService, _apiKeyService, deviceInformationService);
        }

        [TestMethod]
        public void GetBricksetApiKey_ApiKeyExistsStored_ReturnsApiKeyAndSaveBricksetApiKeyNotInvoked()
        {
            _secureStorageService
                .GetBricksetApiKey()
                .Returns("API KEY");

            var apiKey = _onboardingService.GetBricksetApiKey();

            apiKey.Should().NotBeNullOrWhiteSpace();
            _secureStorageService.DidNotReceive().SaveBricksetApiKey(Arg.Any<string>());
        }

        [TestMethod]
        public void GetBricksetApiKey_ApiKeyNotStoredButIdentificationStored_ReturnsApiKeyAndSaveDeviceIdentificationNotInvoked()
        {
            _secureStorageService
                .GetBricksetApiKey()
                .Returns(string.Empty);
            _secureStorageService
                .DeviceIdentificationCreated
                .Returns(true);
            _secureStorageService
                .GetDeviceIdentification()
                .Returns(new Core.Models.Identification());
            _apiKeyService
                .GetBricksetApiKey(Arg.Any<Core.Models.Identification>())
                .Returns("API KEY");

            var apiKey = _onboardingService.GetBricksetApiKey();

            apiKey.Should().NotBeNullOrWhiteSpace();
            _apiKeyService.Received().GetBricksetApiKey(Arg.Any<Core.Models.Identification>());
            _secureStorageService.DidNotReceive().SaveDeviceIdentification(Arg.Any<Core.Models.Identification>());
        }

        [TestMethod]
        public void GetBricksetApiKey_ApiKeyAndIdentificationNotStored_ReturnsApiKeyAndSaveDeviceIdentificationInvoked()
        {
            _secureStorageService
                .GetBricksetApiKey()
                .Returns(string.Empty);
            _secureStorageService
                .DeviceIdentificationCreated
                .Returns(false);
            _registrationService
                .Register(Arg.Any<Core.Models.Identification>())
                .Returns(new Core.Models.Identification());
            _apiKeyService
                .GetBricksetApiKey(Arg.Any<Core.Models.Identification>())
                .Returns("API KEY");

            var apiKey = _onboardingService.GetBricksetApiKey();

            apiKey.Should().NotBeNullOrWhiteSpace();
            _apiKeyService.Received().GetBricksetApiKey(Arg.Any<Core.Models.Identification>());
            _secureStorageService.Received().SaveDeviceIdentification(Arg.Any<Core.Models.Identification>());
        }
    }
}