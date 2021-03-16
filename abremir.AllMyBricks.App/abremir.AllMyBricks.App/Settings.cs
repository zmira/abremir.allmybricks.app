using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace abremir.AllMyBricks.App
{
    public sealed class Settings
    {
        private static readonly Lazy<Dictionary<string, string>> _store = new(LoadEmbeddedSettings);
        private static Dictionary<string, string> Store => _store.Value;

        public static string AllMyBricksOnboardingUrl => Store[nameof(AllMyBricksOnboardingUrl)];

        private static Dictionary<string, string> LoadEmbeddedSettings()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                ?.FirstOrDefault(resource => resource.EndsWith(".settings.json", StringComparison.OrdinalIgnoreCase));
            using var file = assembly.GetManifestResourceStream(resourceName);
            using var streamReader = new StreamReader(file);
            var json = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
    }
}
