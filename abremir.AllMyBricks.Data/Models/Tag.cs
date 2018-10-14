using abremir.AllMyBricks.Data.Interfaces;
using System;

namespace abremir.AllMyBricks.Data.Models
{
    public class Tag: IReferenceData
    {
        public string Value { get; set; }

        public bool IsName => Value.EndsWith("|n", StringComparison.InvariantCultureIgnoreCase);

        public string TagValue => Value.Replace("|n", "");
    }
}