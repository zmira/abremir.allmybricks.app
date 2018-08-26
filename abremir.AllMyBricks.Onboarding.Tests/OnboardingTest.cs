using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Device.Services;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
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
            _registrationService = new RegistrationService();
            _apiKeyService = new ApiKeyService();

            var deviceInfo = Substitute.For<IDeviceInfo>();
            deviceInfo.Manufacturer.Returns("BRAND");
            deviceInfo.Model.Returns("MODEL");
            deviceInfo.VersionString.Returns("VERSION YEAR");
            deviceInfo.Platform.Returns("WINDOWS");
            deviceInfo.Idiom.Returns("PC");

            _deviceInformationService = new DeviceInformationService(deviceInfo);
        }

        [TestMethod, Ignore("Only to be used to validate communication between app and onboarding endpoints")]
        public void EndToEndTest()
        {
            var identification = new Identification
            {
                DeviceIdentification = _deviceInformationService.GenerateNewDeviceIdentification()
            };

            identification = _registrationService.Register(identification);

            var apiKey = _apiKeyService.GetBricksetApiKey(identification);

			apiKey.Should().NotBeNullOrEmpty();

            _registrationService.Unregister(identification);
        }
    }
}