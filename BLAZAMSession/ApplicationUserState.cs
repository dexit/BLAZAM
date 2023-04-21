﻿using BLAZAM.Common.Data;
using BLAZAM.Common.Data.Services;
using BLAZAM.Database.Context;
using BLAZAM.Database.Models.Permissions;
using BLAZAM.Database.Models.User;
using BLAZAM.Notifications.Services;
using BLAZAM.Session.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BLAZAM.Server.Data.Services
{
    /// <summary>
    /// An application user state as managed by the <see cref="ApplicationUserStateService"/>
    /// </summary>
    /// <value>
    /// <para>Holds the <see cref="ClaimsPrincipal"/> of the user defined from logging
    /// in, and an impersonated claim if applied.</para>
    /// 
    /// <para>It also holds the <see cref="IADUser"/>
    /// if set, and through that, permissions can be applied.</para>
    /// </value>
    public class ApplicationUserState : IApplicationUserState
    {

        public AppEvent<AppUser> OnSettingsChange { get; set; }

        public ClaimsPrincipal User { get; set; }

        public ClaimsPrincipal? Impersonator { get; set; }

        public List<PermissionDelegate> PermissionDelegates { get; set; } = new();


        public List<PermissionMapping> PermissionMappings { get; set; } = new();


        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;




        public IList<UserNotification> Messages
        {
            get
            {
                if (!User.Identity.IsAuthenticated) return default;
                if ((DateTime.Now - lastDataRefresh).TotalSeconds > 1)
                    GetUserSettingFromDB();
                return userSettings?.Messages.Where(m => !m.IsRead).ToList();

            }
        }


        public IApplicationUserSessionCache Cache { get; set; } = new ApplicationUserSessionCache();

        public AuthenticationTicket? Ticket { get; set; }

        public DateTime lastDataRefresh;
        public AppUser? userSettings { get; set; }

        private readonly INotificationPublisher _notificationPublisher;
        private readonly IAppDatabaseFactory _dbFactory;

        public ApplicationUserState(IAppDatabaseFactory factory, INotificationPublisher notificationPublisher)
        {

            _notificationPublisher = notificationPublisher;
            _dbFactory = factory;
            _notificationPublisher.OnNotificationPublished += (notifications) =>
            {
                if (notifications.Select(n => n.User).Contains(Preferences))
                    GetUserSettingFromDB();
            };
        }



        public AppUser? Preferences
        {
            get
            {
                if (!User.Identity.IsAuthenticated) return null;
                if (userSettings == null)
                {

                    GetUserSettingFromDB();
                }
                return userSettings;
            }
        }


        public async Task<bool> MarkRead(UserNotification notification)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var message = context.UserNotifications.Where(un => un.Id == notification.Id).FirstOrDefault(); ;
            if (message != null)
            {
                message.IsRead = true;
                var result = await context.SaveChangesAsync();

                if (result == 1)
                {
                    GetUserSettingFromDB();
                    OnSettingsChange?.Invoke(userSettings);

                    return true;
                }
            }
            return false;
        }
        private void GetUserSettingFromDB()
        {
            try
            {
                if (User == null) return;
                using var context = _dbFactory.CreateDbContext();

                userSettings = context.UserSettings.Where(us => us.UserGUID == User.FindFirstValue(ClaimTypes.Sid)).FirstOrDefault();
                if (userSettings == null)
                {
                    userSettings = new AppUser();
                    userSettings.UserGUID = User.FindFirstValue(ClaimTypes.Sid);
                    userSettings.Username = User.Identity?.Name;
                    context.UserSettings.Add(userSettings);
                    context.SaveChanges();

                }

                lastDataRefresh = DateTime.Now;
            }
            catch
            {

            }
        }


        public async Task<bool> SaveUserSettings()
        {
            try
            {
                using var context = await _dbFactory.CreateDbContextAsync();
                var dbUserSettings = await context.UserSettings.Where(us => us.UserGUID == User.FindFirstValue(ClaimTypes.Sid)).FirstOrDefaultAsync();
                if (dbUserSettings != null)
                {
                    dbUserSettings.Theme = this.Preferences?.Theme;
                    dbUserSettings.DarkMode = this.Preferences?.DarkMode == true;
                    dbUserSettings.ProfilePicture = this.Preferences?.ProfilePicture;
                    dbUserSettings.SearchDisabledUsers = this.Preferences.SearchDisabledUsers;
                    dbUserSettings.SearchDisabledComputers = this.Preferences.SearchDisabledComputers;
                    SaveDashboardWidgets(dbUserSettings);
                    OnSettingsChange?.Invoke(dbUserSettings);

                    return (await context.SaveChangesAsync()) > 0;
                }



            }
            catch
            {

            }
            return false;
        }

        private void SaveDashboardWidgets(AppUser? dbUserSettings)
        {
            foreach (var widget in this.Preferences.DashboardWidgets)
            {
                var matchingWidgt = dbUserSettings.DashboardWidgets.FirstOrDefault(w => w.WidgetType == widget.WidgetType);
                if (matchingWidgt != null)
                {
                    matchingWidgt.Slot = widget.Slot;
                    matchingWidgt.Order = widget.Order;
                }
                else
                {
                    dbUserSettings.DashboardWidgets.Add(widget);
                }
            }
            var widgetsInDB = new List<UserDashboardWidget>(dbUserSettings.DashboardWidgets);
            foreach (var widget in widgetsInDB)
            {
                if (!this.Preferences.DashboardWidgets.Any(w => w.WidgetType == widget.WidgetType))
                {
                    dbUserSettings.DashboardWidgets.Remove(widget);
                }

            }
        }

        public bool IsSuperAdmin
        {
            get
            {
                if (User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == UserRoles.SuperAdmin)) return true;
                if (PermissionDelegates != null)
                    return PermissionDelegates.Any(p => p.IsSuperAdmin);
                return false;
            }
        }

        public string? Username
        {
            get
            {
                string? auditUsername = User.Identity?.Name;

                return auditUsername;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                try
                {
                    return User.Identity.IsAuthenticated;
                }
                catch
                {
                    return false;
                }
            }
        }


        public string? AuditUsername
        {
            get
            {
                string? auditUsername = Username;
                if (Impersonator != null)
                {
                    auditUsername += " impersonated by " + Impersonator?.Identity?.Name;
                }
                return auditUsername;
            }
        }

        public string LastUri { get; set; }

        public override int GetHashCode()
        {
            return User.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj is ApplicationUserState otherState)
            {

                if (otherState.User.FindFirstValue(ClaimTypes.Sid) == this.User.FindFirstValue(ClaimTypes.Sid)
                    && otherState.User.FindFirstValue(ClaimTypes.Actor) == User.FindFirstValue(ClaimTypes.Actor))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasRole(string userRole)
        {
            return User.HasClaim(ClaimTypes.Role, userRole);
        }


        public bool HasUserPrivilege => HasObjectReadPermissions(ActiveDirectoryObjectType.User);
        public bool HasCreateUserPrivilege => HasObjectCreatePermissions(ActiveDirectoryObjectType.User);
        public bool HasGroupPrivilege => HasObjectReadPermissions(ActiveDirectoryObjectType.Group);
        public bool HasCreateGroupPrivilege => HasObjectCreatePermissions(ActiveDirectoryObjectType.Group);
        public bool HasOUPrivilege => HasObjectReadPermissions(ActiveDirectoryObjectType.OU);
        public bool HasCreateOUPrivilege => HasObjectCreatePermissions(ActiveDirectoryObjectType.OU);
        public bool HasComputerPrivilege => HasObjectReadPermissions(ActiveDirectoryObjectType.Computer);

        public bool CanUnlockUsers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        /// <summary>
        /// Checks that the user has some kind of read access for disabled objects of
        /// this type in any place on the OU tree.
        /// </summary>
        public bool CanSearchDisabled(ActiveDirectoryObjectType objectType)
        {

            if (IsSuperAdmin == true) return true;

            return PermissionMappings.Any(pm => pm.AccessLevels.Any(al => al.ObjectMap.Any(om => om.ObjectType == objectType && om.AllowDisabled))) == true;


        }


        private bool HasObjectReadPermissions(ActiveDirectoryObjectType objectType)
        {
            if (IsSuperAdmin == true) return true;
            return PermissionMappings.Any(
                       m => m.AccessLevels.Any(
                           a => a.ObjectMap.Any(
                               o => o.ObjectType == objectType && o.ObjectAccessLevel.Level > ObjectAccessLevels.Deny.Level)
                           )
                       );
        }
        private bool HasObjectCreatePermissions(ActiveDirectoryObjectType objectType)
        {
            if (IsSuperAdmin == true) return true;

            return PermissionMappings.Any(
                        m => m.AccessLevels.Any(
                            a => a.ObjectMap.Any(
                                o => o.ObjectType == objectType && o.ObjectAccessLevel.Level > ObjectAccessLevels.Deny.Level) &&
                                a.ActionMap.Any(am => am.ObjectType == objectType &&
                                am.ObjectAction.Id == ObjectActions.Create.Id)
                            )
                        );
        }

    }
}