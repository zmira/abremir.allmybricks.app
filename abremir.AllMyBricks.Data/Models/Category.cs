using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class Category : IReferenceData
    {
        [BsonId(false)]
        public string Value { get; set; }
    }
}
