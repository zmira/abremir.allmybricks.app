using System;
using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class Tag : IReferenceData
    {
        [BsonId(true)]
        public int Id { get; set; }

        public string Value { get; set; }

        [BsonIgnore]
        public bool IsName => Value.EndsWith("|n", StringComparison.InvariantCultureIgnoreCase);

        [BsonIgnore]
        public string TagValue => Value.Replace("|n", "");
    }
}
