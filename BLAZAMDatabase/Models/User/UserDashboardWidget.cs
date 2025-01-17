﻿

using System.ComponentModel.DataAnnotations.Schema;

namespace BLAZAM.Database.Models.User
{
    public enum DashboardWidgetType
    {
        NewUsers,
        LockedOutUsers,
        PasswordsChanged,
        NewOus,
        NewGroups,
        NewComputers,
        NewPrinters,
        FavoriteEntries,
        DeletedEntries,
        ChangedEntries
    }
    public class UserDashboardWidget : AppDbSetBase
    {
        public DashboardWidgetType WidgetType { get; set; }
        public string Slot { get; set; }
        public int Order { get; set; }

        public AppUser User { get; set; }
        public int UserId { get; set; }

        public override int GetHashCode()
        {
            return WidgetType.GetHashCode();
        }
    }
}
