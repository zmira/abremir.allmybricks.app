using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Device.Services;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Onboarding.Tests
{
    [TestClass]
    public class OnboardingTest
    {
        private IRegistrationService _registrationService;
        private IApiKeyService _apiKeyService;
        private IDeviceInformationService _deviceInformationService;

        [TestInitialize]
        public void TestInitialize()
        {
            const string allMyBricksOnboardingUrl = "http://localhost/";

            _registrationService = new RegistrationService(allMyBricksOnboardingUrl);
            _apiKeyService = new ApiKeyService(allMyBricksOnboardingUrl);

            var deviceInfo = Substitute.For<IDeviceInfo>();
            deviceInfo.Manufacturer.Returns("BRAND");
            deviceInfo.Model.Returns("MODEL");
            deviceInfo.VersionString.Returns("VERSION YEAR");
            deviceInfo.Platform.Returns(DevicePlatform.UWP);
            deviceInfo.Idiom.Returns(DeviceIdiom.Desktop);

            _deviceInformationService = new DeviceInformationService(deviceInfo);
        }

        [TestMethod, Ignore("Only to be used to validate communication between app and onboarding endpoints")]
        public async Task EndToEndTest()
        {
            var identification = new Identification
            {
                DeviceIdentification = _deviceInformationService.GenerateNewDeviceIdentification()
            };

            identification = await _registrationService.Register(identification);

            var apiKey = await _apiKeyService.GetBricksetApiKey(identification);

			apiKey.Should().NotBeNullOrEmpty();

            await _registrationService.Unregister(identification);
        }
    }
}