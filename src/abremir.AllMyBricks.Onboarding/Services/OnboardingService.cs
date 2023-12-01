using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Platform.Interfaces;

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
            var bricksetApiKey = await _secureStorageService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(bricksetApiKey))
            {
                bricksetApiKey = await GetApiKeyFromOnboardingServiceEndpoint().ConfigureAwait(false);

                await _secureStorageService.SaveBricksetApiKey(bricksetApiKey).ConfigureAwait(false);
            }

            return bricksetApiKey;
        }

        private async Task<string> GetApiKeyFromOnboardingServiceEndpoint()
        {
            Identification identification;

            if (!await _secureStorageService.IsDeviceIdentificationCreated().ConfigureAwait(false))
            {
                identification = await _registrationService.Register(new Identification
                {
                    DeviceIdentification = _deviceInformationService.GenerateNewDeviceIdentification()
                }).ConfigureAwait(false);

                await _secureStorageService.SaveDeviceIdentification(identification).ConfigureAwait(false);
            }
            else
            {
                identification = await _secureStorageService.GetDeviceIdentification().ConfigureAwait(false);
            }

            return await _apiKeyService.GetBricksetApiKey(identification).ConfigureAwait(false);
        }
    }
}
