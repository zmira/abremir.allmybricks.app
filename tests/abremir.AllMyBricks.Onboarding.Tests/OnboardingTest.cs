﻿using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Configuration;
using abremir.AllMyBricks.Onboarding.Services;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Onboarding.Shared.Security;
using abremir.AllMyBricks.Platform.Services;
using Flurl.Http.Configuration;
using Microsoft.Maui.Devices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using NSubstitute;

namespace abremir.AllMyBricks.Onboarding.Tests
{
    [TestClass]
    public class OnboardingTest
    {
        private RegistrationService _registrationService;
        private ApiKeyService _apiKeyService;
        private DeviceInformationService _deviceInformationService;

        [TestInitialize]
        public void TestInitialize()
        {
            const string allMyBricksOnboardingUrl = "http://localhost/";

            var clientCache = new FlurlClientCache()
                .Add(Constants.AllMyBricksOnboardingUrlFlurlClientCacheName, allMyBricksOnboardingUrl, (builder) => builder.Settings.JsonSerializer = OnboardingJsonSerializer.JsonSerializer)
                .Add(Constants.AllMyBricksOnboardingHmacUrlFlurlClientCacheName, allMyBricksOnboardingUrl, (builder) =>
                {
                    builder.Settings.JsonSerializer = OnboardingJsonSerializer.JsonSerializer;
                    builder.AddMiddleware(() => new HmacDelegatingHandler());
                });

            _registrationService = new RegistrationService(clientCache);
            _apiKeyService = new ApiKeyService(clientCache);

            var deviceInfo = Substitute.For<IDeviceInfo>();
            deviceInfo.Manufacturer.Returns("BRAND");
            deviceInfo.Model.Returns("MODEL");
            deviceInfo.VersionString.Returns("VERSION YEAR");
            deviceInfo.Platform.Returns(DevicePlatform.WinUI);
            deviceInfo.Idiom.Returns(DeviceIdiom.Desktop);

            _deviceInformationService = new DeviceInformationService(deviceInfo);
        }

        [TestMethod, Ignore("Only to be used to validate communication between app and onboarding endpoints")]
#pragma warning disable MSTEST0015 // Test method should not be ignored
        public async Task EndToEndTest()
#pragma warning restore MSTEST0015 // Test method should not be ignored
        {
            var identification = new Identification
            {
                DeviceIdentification = _deviceInformationService.GenerateNewDeviceIdentification()
            };

            identification = await _registrationService.Register(identification);

            var apiKey = await _apiKeyService.GetBricksetApiKey(identification);

            Check.That(apiKey).Not.IsNullOrEmpty();

            await _registrationService.Unregister(identification);
        }
    }
}
