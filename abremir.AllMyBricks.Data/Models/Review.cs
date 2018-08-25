using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace abremir.AllMyBricks.Data.Models
{
    [DebuggerStepThrough]
    public class Review
    {
        public string Author { get; set; }
        public DateTimeOffset DatePosted { get; set; }
        public string Title { get; set; }
        public string ReviewContent { get; set; }
        public bool Html { get; set; }

        public IList<RatingItem> RatingComponents { get; set; } = new List<RatingItem>();
    }
}