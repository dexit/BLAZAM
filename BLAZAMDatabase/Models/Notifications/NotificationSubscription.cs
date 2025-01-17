﻿using BLAZAM.Database.Models.User;

namespace BLAZAM.Database.Models.Notifications
{
    public class NotificationSubscription:RecoverableAppDbSetBase
    {
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public List<SubscriptionNotificationType> NotificationTypes { get; set; } = new();
        public string OU { get; set; }
        public bool InApp { get; set; }
        public bool ByEmail { get; set; }
        public bool Block { get; set; } = false;
    }
}
