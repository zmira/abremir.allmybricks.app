using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace abremir.AllMyBricks.App.Tests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void Get_AllMyBricksOnboardingUrl_ReturnsValidData()
        {
            var result = Settings.AllMyBricksOnboardingUrl;

            Check.That(result).IsNotEmpty();
        }
    }
}
