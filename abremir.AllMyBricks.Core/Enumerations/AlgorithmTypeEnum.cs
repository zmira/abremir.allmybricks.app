using Jose;

namespace abremir.AllMyBricks.Core.Enumerations
{
    public enum AlgorithmTypeEnum
    {
        Type1 = JwsAlgorithm.HS256,
        Type2 = JwsAlgorithm.HS384,
        Type3 = JwsAlgorithm.HS512
    }
}