using abremir.AllMyBricks.Core.Enumerations;
using abremir.AllMyBricks.Onboarding.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace abremir.AllMyBricks.Onboarding.Tests.Helpers
{
    [TestClass]
    public class RandomKeyOptionGeneratorTests
    {
        [TestMethod]
        public void GetRandomKeyOption()
        {
            const int count = 1000;
            var algorithmTypeEnumList = new List<AlgorithmTypeEnum>();

            for (int i = 0; i < count; i++)
            {
                var next = RandomKeyOptionGenerator.GetRandomKeyOption();
                algorithmTypeEnumList.Add(next);
            }

            algorithmTypeEnumList.Should().HaveCount(count);
            algorithmTypeEnumList.Should().OnlyContain(algoType => algoType.GetType() == typeof(AlgorithmTypeEnum));
        }
    }
}