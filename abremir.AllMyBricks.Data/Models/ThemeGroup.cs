using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class ThemeGroup : IReferenceData
    {
        [BsonId(false)]
        public string Value { get; set; }
    }
}