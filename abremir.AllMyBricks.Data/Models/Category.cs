using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class Category : IReferenceData
    {
        [BsonId(true)]
        public int Id { get; set; }

        public string Value { get; set; }
    }
}
