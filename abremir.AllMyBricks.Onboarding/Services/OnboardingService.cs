using abremir.AllMyBricks.Core.Models;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using System.Threading.Tasks;

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

        public async Task<string> GetBricksetApiKey()
        {
            var bricksetApiKey = _secureStorageService.GetBricksetApiKey();

            if (string.IsNullOrWhiteSpace(bricksetApiKey))
            {
                bricksetApiKey = await GetApiKeyFromOnboardingServiceEndpoint();

                _secureStorageService.SaveBricksetApiKey(bricksetApiKey);
            }

            return bricksetApiKey;
        }

        private async Task<string> GetApiKeyFromOnboardingServiceEndpoint()
        {
            Identification identification = null;

            if (!_secureStorageService.DeviceIdentificationCreated)
            {
                identification = await _registrationService.Register(new Identification
                {
                    DeviceIdentification = _deviceInformationService.GenerateNewDeviceIdentification()
                });

                _secureStorageService.SaveDeviceIdentification(identification);
            }
            else
            {
                identification = _secureStorageService.GetDeviceIdentification();
            }

            return await _apiKeyService.GetBricksetApiKey(identification);
        }
    }
}