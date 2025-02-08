using System;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class Insights
    {
        [BsonId(false)]
        public byte Id { get; set; }

        private DateTimeOffset? _dataSynchronizationTimestamp;
        public DateTimeOffset? DataSynchronizationTimestamp
        {
            get { return _dataSynchronizationTimestamp; }
            set { _dataSynchronizationTimestamp = value?.ToHundredthOfSecond(); }
        }
    }
}
