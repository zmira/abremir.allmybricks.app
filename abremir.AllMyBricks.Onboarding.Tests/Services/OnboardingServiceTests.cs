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
        private ISecureStorageService _secureStorageService;
        private IApiKeyService _apiKeyService;
        private IRegistrationService _registrationService;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            _secureStorageService = Substitute.For<ISecureStorageService>();
            _apiKeyService = Substitute.For<IApiKeyService>();
            _registrationService = Substitute.For<IRegistrationService>();
        }

        [TestMethod]
        public void GetBricksetApiKey_ApiKeyExistsStored_ReturnsApiKeyAndSaveBricksetApiKeyNotInvoked()
        {
            _secureStorageService
                .GetBricksetApiKey()
                .Returns("API KEY");

            var onboardingService = CreateTarget();

            var result = onboardingService.GetBricksetApiKey();

            result.Should().NotBeNullOrWhiteSpace();
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

            var onboardingService = CreateTarget();

            var result = onboardingService.GetBricksetApiKey();

            result.Should().NotBeNullOrWhiteSpace();
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

            var onboardingService = CreateTarget();

            var result = onboardingService.GetBricksetApiKey();

            result.Should().NotBeNullOrWhiteSpace();
            _apiKeyService.Received().GetBricksetApiKey(Arg.Any<Core.Models.Identification>());
            _secureStorageService.Received().SaveDeviceIdentification(Arg.Any<Core.Models.Identification>());
        }

        private OnboardingService CreateTarget()
        {
            var deviceInformationService = Substitute.For<IDeviceInformationService>();

            return new OnboardingService(_secureStorageService, _registrationService, _apiKeyService, deviceInformationService);
        }
    }
}