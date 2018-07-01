using abremir.AllMyBricks.Core.Models;
using ExpressMapper;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class Mappings
    {
        public static void Configure()
        {
            Mapper.Register<Identification, ApiKeyRequest>();
        }
    }
}