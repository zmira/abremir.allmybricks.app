﻿using abremir.AllMyBricks.Data.Enumerations;

namespace abremir.AllMyBricks.Data.Models
{
    public class Barcode
    {
        public BarcodeType Type { get; set; }
        public string Value { get; set; }
    }
}
