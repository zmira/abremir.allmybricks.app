using Realms;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    internal class Instruction : RealmObject
    {
        public string Url { get; set; }
        public string Description { get; set; }
    }
}