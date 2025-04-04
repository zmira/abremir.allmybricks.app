using LiteDB;

namespace abremir.AllMyBricks.Data.Configuration
{
    public static class LiteDbConfiguration
    {
        public static void Configure()
        {
            BsonMapper.Global.TrimWhitespace = false;
            BsonMapper.Global.EmptyStringToNull = false;
        }
    }
}
