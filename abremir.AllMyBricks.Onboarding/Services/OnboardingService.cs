using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly ISecureStorageService _secureStorageService;
        private readonly IRegistrationService _registrationService;
        private readonly IApiKeyService _apiKeyService;
        private readonly IDeviceInformationService _deviceInformationService;

        public OnboardingService(
            ISecureStorageService secureStorageService,
            IRegistrationService registrationService,
            IApiKeyService apiKeyService,
            IDeviceInformationService deviceInformationService)
        {
            _secureStorageService = secureStorageService;
            _registrationService = registrationService;
            _apiKeyService = apiKeyService;
            _deviceInformationService = deviceInformationService;
        }

        public string GetBricksetApiKey()
        {
            var bricksetApiKey = _secureStorageService.GetBricksetApiKey();

            if (string.IsNullOrWhiteSpace(bricksetApiKey))
            {
                bricksetApiKey = GetApiKeyFromOnboardingServiceEndpoint();

                _secureStorageService.SaveBricksetApiKey(bricksetApiKey);
            }

            return bricksetApiKey;
        }

        private string GetApiKeyFromOnboardingServiceEndpoint()
        {
            Identification identification = null;

            if (!_secureStorageService.DeviceIdentificationCreated)
            {
                identification = _registrationService.Register(new Identification
                {
                    DeviceIdentification = _deviceInformationService.GenerateNewDeviceIdentification()
                });

                _secureStorageService.SaveDeviceIdentification(identification);
            }
            else
            {
                identification = _secureStorageService.GetDeviceIdentification();
            }

            return _apiKeyService.GetBricksetApiKey(identification);
        }
    }
}