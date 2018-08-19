using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace abremir.AllMyBricks.Onboarding.Tests.Services
{
    [TestClass]
    public class OnboardingServiceTests
    {
        private static IRegistrationService _registrationService;
        private static IApiKeyService _apiKeyService;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _registrationService = new RegistrationService();
            _apiKeyService = new ApiKeyService();
        }

        [TestMethod, Ignore("Only to be used to validate communication between app and onboarding endpoints")]
        public void EndToEndTest()
        {
            var identification = new Identification
            {
                DeviceIdentification = new Device
                {
                    AppId = Guid.NewGuid().ToString(),
                    DeviceHash = Guid.NewGuid().ToString(),
                    DeviceHashDate = DateTimeOffset.Now,
                    Model = "MODEL",
                    Platform = "PLATFORM",
                    Version = "VERSION"
                }
            };

            identification = _registrationService.Register(identification);

            var apiKey = _apiKeyService.GetBricksetApiKey(identification);

			apiKey.Should().NotBeNullOrEmpty();

            _registrationService.Unregister(identification);
        }
    }
}