using Realms;

namespace abremir.AllMyBricks.Data.Models.Realm
{
    public class Instruction : RealmObject
    {
        public string Url { get; set; }
        public string Description { get; set; }
    }
}