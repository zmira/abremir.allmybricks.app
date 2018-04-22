using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class PackagingType : IReferenceData
    {
        [BsonId(false)]
        public string Value { get; set; }
    }
}