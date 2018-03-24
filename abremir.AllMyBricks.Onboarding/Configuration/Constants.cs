namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class Constants
    {
        private const string AllMyBricksOnboardingUrl = "http://localhost/api/";

        public static readonly string AllMyBricksOnboardingApiKeyService = $"{AllMyBricksOnboardingUrl}ApiKey";
        public const string AllMyBricksOnboardingApiKeyServiceBricksetMethod = "brickset";

        public static readonly string AllMyBricksOnboardingRegistrationService = $"{AllMyBricksOnboardingUrl}allMyBricks";
        public const string AllMyBricksOnboardingRegistrationServiceRegisterMethod = "register";
        public const string AllMyBricksOnboardingRegistrationServiceUnregisterMethod = "unregister";
    }
}