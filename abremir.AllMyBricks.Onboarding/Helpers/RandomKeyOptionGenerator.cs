using abremir.AllMyBricks.Core.Enumerations;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("abremir.AllMyBricks.Onboarding.Tests")]

namespace abremir.AllMyBricks.Onboarding.Helpers
{
    internal static class RandomKeyOptionGenerator
    {
        internal static AlgorithmTypeEnum GetRandomKeyOption()
        {
            const int min = (int)AlgorithmTypeEnum.Type1;
            const int max = (int)AlgorithmTypeEnum.Type3 + 1;
            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[4];
                rng.GetBytes(data);

                int generatedValue = Math.Abs(BitConverter.ToInt32(data, startIndex: 0));

                int diff = max - min;
                int mod = generatedValue % diff;
                return (AlgorithmTypeEnum)(min + mod);
            }
        }
    }
}