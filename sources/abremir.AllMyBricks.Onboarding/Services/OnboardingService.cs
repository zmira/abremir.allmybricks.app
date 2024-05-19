using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.Onboarding.Shared.Models;
using abremir.AllMyBricks.Platform.Interfaces;

namespace abremir.AllMyBricks.Onboarding.Services
{
    public class OnboardingService(
        ISecureStorageService secureStorageService,
        IRegistrationService registrationService,
        IApiKeyService apiKeyService,
        IDeviceInformationService deviceInformationService)
        : IOnboardingService
    {
        public async Task<string> GetBricksetApiKey()
        {
            var bricksetApiKey = await secureStorageService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(bricksetApiKey))
            {
                bricksetApiKey = await GetApiKeyFromOnboardingServiceEndpoint().ConfigureAwait(false);

                await secureStorageService.SaveBricksetApiKey(bricksetApiKey).ConfigureAwait(false);
            }

            return bricksetApiKey;
        }

        private async Task<string> GetApiKeyFromOnboardingServiceEndpoint()
        {
            Identification identification;

            if (!await secureStorageService.IsDeviceIdentificationCreated().ConfigureAwait(false))
            {
                identification = await registrationService.Register(new Identification
                {
                    DeviceIdentification = deviceInformationService.GenerateNewDeviceIdentification()
                }).ConfigureAwait(false);

                await secureStorageService.SaveDeviceIdentification(identification).ConfigureAwait(false);
            }
            else
            {
                identification = await secureStorageService.GetDeviceIdentification().ConfigureAwait(false);
            }

            return await apiKeyService.GetBricksetApiKey(identification).ConfigureAwait(false);
        }
    }
}
