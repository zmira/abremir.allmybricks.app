using abremir.AllMyBricks.Data.Interfaces;
using Realms;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("abremir.AllMyBricks.Data.Tests")]

namespace abremir.AllMyBricks.Data.Models.Realm
{
    internal class Category : RealmObject, IReferenceData
    {
        [PrimaryKey]
        public string Value { get; set; }
    }
}