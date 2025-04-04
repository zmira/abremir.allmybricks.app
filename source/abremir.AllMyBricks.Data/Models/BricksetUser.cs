using System;
using System.Collections.Generic;
using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;
using LiteDB;

namespace abremir.AllMyBricks.Data.Models
{
    public class BricksetUser
    {
        [BsonId(true)]
        public int Id { get; set; }

        public string BricksetUsername { get; set; }
        public BricksetUserType UserType { get; set; }

        private DateTimeOffset? _userSynchronizationTimestamp;
        public DateTimeOffset? UserSynchronizationTimestamp
        {
            get { return _userSynchronizationTimestamp; }
            set { _userSynchronizationTimestamp = value?.ToHundredthOfSecond(); }
        }

        public IList<BricksetUserSet> Sets { get; set; } = [];
    }
}
