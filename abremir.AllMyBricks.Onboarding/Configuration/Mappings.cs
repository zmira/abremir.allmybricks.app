using abremir.AllMyBricks.Core.Models;
using Nelibur.ObjectMapper;

namespace abremir.AllMyBricks.Onboarding.Configuration
{
    public static class Mappings
    {
        public static void Configure()
        {
            TinyMapper.Bind<Identification, ApiKeyRequest>();
        }
    }
}