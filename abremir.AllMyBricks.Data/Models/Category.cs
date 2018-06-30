using abremir.AllMyBricks.Data.Interfaces;
using Realms;

namespace abremir.AllMyBricks.Data.Models
{
    public class Category : RealmObject, IReferenceData
    {
        [PrimaryKey]
        public string Value { get; set; }
    }
}