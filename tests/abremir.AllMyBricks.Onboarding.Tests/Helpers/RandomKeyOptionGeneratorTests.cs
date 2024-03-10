using System.Collections.Generic;
using abremir.AllMyBricks.Onboarding.Helpers;
using abremir.AllMyBricks.Onboarding.Shared.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace abremir.AllMyBricks.Onboarding.Tests.Helpers
{
    [TestClass]
    public class RandomKeyOptionGeneratorTests
    {
        [TestMethod]
        public void GetRandomKeyOption()
        {
            const int count = 1000;
            List<AlgorithmType> algorithmTypeEnumList = [];

            for (int i = 0; i < count; i++)
            {
                var next = RandomKeyOptionGenerator.GetRandomKeyOption();
                algorithmTypeEnumList.Add(next);
            }

            Check.That(algorithmTypeEnumList).CountIs(count).And.ContainsOnlyElementsThatMatch(algoType => algoType != 0);
        }
    }
}
