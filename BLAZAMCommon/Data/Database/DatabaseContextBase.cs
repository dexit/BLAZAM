﻿using BLAZAM.Common.Data.ActiveDirectory;
using BLAZAM.Common.Helpers;
using BLAZAM.Common.Models.Database;
using BLAZAM.Common.Models.Database.Audit;
using BLAZAM.Common.Models.Database.Permissions;
using BLAZAM.Common.Models.Database.Templates;
using BLAZAM.Common.Models.Database.User;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace BLAZAM.Common.Data.Database
{
    public class DatabaseContextBase : DbContext, IDatabaseContext
    {

        public static AppEvent OnMigrationApplied { get; set; }
        public static AppEvent OnMigrationFailed { get; set; }


        /// <summary>
        /// The connection string as set in the ASP Net Core appsettings.json
        /// <para>This should be set before any attempts to connect.</para>
        /// <para>Usually in the Program.Main method before injecting the service.</para>
        /// </summary>
        public DatabaseConnectionString? ConnectionString { get; set; }

        /// <summary>
        /// Checks the realtime pingabillity and connectivity to the database right now
        /// </summary>
        public virtual ServiceConnectionState Status
        {
            get
            {
                return TestConnection();
            }
        }



        static IEnumerable<string> _pendingMigrations;
        public virtual IEnumerable<string> PendingMigrations
        {
            get
            {
                _pendingMigrations ??= Database.GetPendingMigrations();
                return _pendingMigrations;
            }
        }
        static IEnumerable<string> _appliedMigrations;
        private string _dbType;


        public virtual IEnumerable<string> AppliedMigrations
        {
            get
            {
                _appliedMigrations ??= Database.GetAppliedMigrations();
                return _appliedMigrations;
            }
        }

        public enum DatabaseStatus
        {
            OK, ServerUnreachable,
            TablesMissing,
            DatabaseConnectionIssue,
            IncompleteConfiguration
        }
        /// <summary>
        /// Constructor for building migrations
        /// </summary>
        public DatabaseContextBase()
        {
            ConnectionString = new("");
        }


        public DatabaseContextBase(DatabaseConnectionString databaseConnectionString) : base()
        {
            ConnectionString = databaseConnectionString;
            _dbType = "SQL";
        }






        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="options"><inheritdoc/></param>
        public DatabaseContextBase(DbContextOptions options) : base(options)
        {
        }

        //App Settings
        public virtual DbSet<AppSettings> AppSettings { get; set; }
        public virtual DbSet<ADSettings> ActiveDirectorySettings { get; set; }
        public virtual DbSet<AuthenticationSettings> AuthenticationSettings { get; set; }
        public virtual DbSet<EmailSettings> EmailSettings { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

        //User Tables
        public virtual DbSet<AppUser> UserSettings { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<NotificationMessage> NotificationMessages { get; set; }


        //Audit Logs
        public virtual DbSet<SystemAuditLog> SystemAuditLog { get; set; }
        public virtual DbSet<LogonAuditLog> LogonAuditLog { get; set; }
        public virtual DbSet<UserAuditLog> UserAuditLog { get; set; }
        public virtual DbSet<GroupAuditLog> GroupAuditLog { get; set; }
        public virtual DbSet<ComputerAuditLog> ComputerAuditLog { get; set; }
        public virtual DbSet<OUAuditLog> OUAuditLog { get; set; }
        public virtual DbSet<RequestAuditLog> RequestAuditLog { get; set; }
        public virtual DbSet<PermissionsAuditLog> PermissionsAuditLog { get; set; }
        public virtual DbSet<SettingsAuditLog> SettingsAuditLog { get; set; }



        //Permissions
        public virtual DbSet<ActiveDirectoryField> ActiveDirectoryFields { get; set; }
        public virtual DbSet<ActiveDirectoryFieldObjectType> ActiveDirectoryFieldObjectMappings { get; set; }
        public virtual DbSet<AccessLevel> AccessLevels { get; set; }
        public virtual DbSet<ObjectAccessMapping> AccessLevelObjectMapping { get; set; }
        public virtual DbSet<FieldAccessMapping> AccessLevelFieldMapping { get; set; }
        public virtual DbSet<FieldAccessLevel> FieldAccessLevel { get; set; }
        public virtual DbSet<ObjectAccessLevel> ObjectAccessLevel { get; set; }
        public virtual DbSet<ActionAccessFlag> ObjectActionFlag { get; set; }

        public virtual DbSet<PermissionDelegate> PermissionDelegate { get; set; }
        public virtual DbSet<PermissionMapping> PermissionMap { get; set; }


        //Templates
        public virtual DbSet<DirectoryTemplate> DirectoryTemplates { get; set; }
        public virtual DbSet<DirectoryTemplateFieldValue> DirectoryTemplateFieldValues { get; set; }
        public virtual DbSet<DirectoryTemplateGroup> DirectoryTemplateGroups { get; set; }



        public static ConfigurationManager Configuration { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            throw new NotImplementedException("DatabaseContext of type " + this.GetType().FullName + " has not implemented OnConfiguring");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<ActiveDirectoryField>().HasData(


                new ActiveDirectoryField
                {
                    Id = 100001,
                    FieldName = "sn",
                    DisplayName = "Last Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100002,
                    FieldName = "givenname",
                    DisplayName = "First Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100003,
                    FieldName = "physicalDeliveryOfficeName",
                    DisplayName = "Office"
                },

                new ActiveDirectoryField
                {
                    Id = 100004,
                    FieldName = "employeeId",
                    DisplayName = "Employee ID"
                },

                new ActiveDirectoryField
                {
                    Id = 100005,
                    FieldName = "homeDirectory",
                    DisplayName = "Home Directory"
                },

                new ActiveDirectoryField
                {
                    Id = 100006,
                    FieldName = "scriptPath",
                    DisplayName = "Logon Script Path"
                },

                new ActiveDirectoryField
                {
                    Id = 100007,
                    FieldName = "profilePath",
                    DisplayName = "Profile Path"
                },

                new ActiveDirectoryField
                {
                    Id = 100008,
                    FieldName = "homePhone",
                    DisplayName = "Home Phone Number"
                },

                new ActiveDirectoryField
                {
                    Id = 100009,
                    FieldName = "streetAddress",
                    DisplayName = "Street Address"
                },

                new ActiveDirectoryField
                {
                    Id = 100010,
                    FieldName = "city",
                    DisplayName = "City"
                },

                new ActiveDirectoryField
                {
                    Id = 100011,
                    FieldName = "st",
                    DisplayName = "State"
                },

                new ActiveDirectoryField
                {
                    Id = 100012,
                    FieldName = "postalCode",
                    DisplayName = "Zip Code"
                },

                new ActiveDirectoryField
                {
                    Id = 100013,
                    FieldName = "site",
                    DisplayName = "Site"
                },

                new ActiveDirectoryField
                {
                    Id = 100014,
                    FieldName = "name",
                    DisplayName = "Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100015,
                    FieldName = "samaccountname",
                    DisplayName = "Username"
                },

                new ActiveDirectoryField
                {
                    Id = 100016,
                    FieldName = "objectSID",
                    DisplayName = "SID"
                },

                new ActiveDirectoryField
                {
                    Id = 100017,
                    FieldName = "mail",
                    DisplayName = "E-Mail Address"
                },

                new ActiveDirectoryField
                {
                    Id = 100018,
                    FieldName = "description",
                    DisplayName = "Description"
                },

                new ActiveDirectoryField
                {
                    Id = 100019,
                    FieldName = "displayName",
                    DisplayName = "Display Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100020,
                    FieldName = "distinguishedName",
                    DisplayName = "Distinguished Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100021,
                    FieldName = "memberOf",
                    DisplayName = "Member Of"
                },

                new ActiveDirectoryField
                {
                    Id = 100022,
                    FieldName = "company",
                    DisplayName = "Company"
                },


                new ActiveDirectoryField
                {
                    Id = 100023,
                    FieldName = "title",
                    DisplayName = "Title"
                },

                new ActiveDirectoryField
                {
                    Id = 100024,
                    FieldName = "userPrincipalName",
                    DisplayName = "User Principal Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100025,
                    FieldName = "telephoneNumber",
                    DisplayName = "Telephone Number"
                },

                new ActiveDirectoryField
                {
                    Id = 100026,
                    FieldName = "postOfficeBox",
                    DisplayName = "PO Box"
                },

                new ActiveDirectoryField
                {
                    Id = 100027,
                    FieldName = "cn",
                    DisplayName = "Canonical Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100028,
                    FieldName = "homeDrive",
                    DisplayName = "Home Drive"
                },

                new ActiveDirectoryField
                {
                    Id = 100029,
                    FieldName = "department",
                    DisplayName = "Department"
                },

                new ActiveDirectoryField
                {
                    Id = 100030,
                    FieldName = "middleName",
                    DisplayName = "Middle Name"
                },

                new ActiveDirectoryField
                {
                    Id = 100031,
                    FieldName = "pager",
                    DisplayName = "Pager"
                },

                new ActiveDirectoryField
                {
                    Id = 100032,
                    FieldName = "operatingSystemVersion",
                    DisplayName = "OS"
                },

                new ActiveDirectoryField
                {
                    Id = 100033,
                    FieldName = "accountExpires",
                    DisplayName = "Account Expiration"
                },
                    new ActiveDirectoryField
                    {
                        Id = 100034,
                        FieldName = "manager",
                        DisplayName = "Manager"
                    }


            );


            modelBuilder.Entity<ActiveDirectoryField>()
         .HasMany(x => x.ObjectTypes);
            modelBuilder.Entity<ActiveDirectoryField>()
         .Navigation(x => x.ObjectTypes).AutoInclude();


            modelBuilder.Entity<ActiveDirectoryFieldObjectType>().HasData(


               new ActiveDirectoryFieldObjectType
               {
                   Id = 100001,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100001
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100002,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100002
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100003,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100003
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100004,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100004
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100005,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100005
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100006,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100006
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100007,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100007
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100008,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100008
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100009,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100009
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100010,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100010
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100011,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100011
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100012,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100012
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100013,
                   ObjectType = ActiveDirectoryObjectType.Group,
                   ActiveDirectoryFieldId = 100013

               },
               new ActiveDirectoryFieldObjectType
               {
                   Id = 100014,
                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100013


               },
               new ActiveDirectoryFieldObjectType
               {
                   Id = 100015,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100013


               },
               new ActiveDirectoryFieldObjectType
               {
                   Id = 100016,
                   ObjectType = ActiveDirectoryObjectType.OU,
                   ActiveDirectoryFieldId = 100013

               },


               new ActiveDirectoryFieldObjectType
               {
                   Id = 100017,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100014
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100018,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100015

               },
                 new ActiveDirectoryFieldObjectType
                 {
                     Id = 100019,
                     ObjectType = ActiveDirectoryObjectType.Group,
                     ActiveDirectoryFieldId = 100015

                 },
                   new ActiveDirectoryFieldObjectType
                   {
                       Id = 100020,
                       ObjectType = ActiveDirectoryObjectType.Computer,
                       ActiveDirectoryFieldId = 100015

                   },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100021,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100016

               },

                new ActiveDirectoryFieldObjectType
                {
                    Id = 100022,
                    ObjectType = ActiveDirectoryObjectType.Group,
                    ActiveDirectoryFieldId = 100016

                },

                 new ActiveDirectoryFieldObjectType
                 {
                     Id = 100023,
                     ObjectType = ActiveDirectoryObjectType.Computer,
                     ActiveDirectoryFieldId = 100016

                 },

                  new ActiveDirectoryFieldObjectType
                  {
                      Id = 100024,
                      ObjectType = ActiveDirectoryObjectType.OU,
                      ActiveDirectoryFieldId = 100016

                  },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100025,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100017
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100026,
                   ObjectType = ActiveDirectoryObjectType.Group,
                   ActiveDirectoryFieldId = 100017

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100027,

                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100018

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100028,

                   ObjectType = ActiveDirectoryObjectType.Group,
                   ActiveDirectoryFieldId = 100018

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100029,

                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100018

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100030,

                   ObjectType = ActiveDirectoryObjectType.OU,
                   ActiveDirectoryFieldId = 100018

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100031,

                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100019

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100032,

                   ObjectType = ActiveDirectoryObjectType.Group,
                   ActiveDirectoryFieldId = 100019

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100033,

                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100019

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100034,

                   ObjectType = ActiveDirectoryObjectType.OU,
                   ActiveDirectoryFieldId = 100019

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100035,

                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100020

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100036,

                   ObjectType = ActiveDirectoryObjectType.Group,
                   ActiveDirectoryFieldId = 100020

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100037,

                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100020

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100038,

                   ObjectType = ActiveDirectoryObjectType.OU,
                   ActiveDirectoryFieldId = 100020

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100039,

                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100021


               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100040,

                   ObjectType = ActiveDirectoryObjectType.Group,
                   ActiveDirectoryFieldId = 100021

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100041,

                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100021

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100042,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100022
               },


               new ActiveDirectoryFieldObjectType
               {
                   Id = 100043,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100023
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100044,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100024
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100045,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100025
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100046,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100026
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100047,

                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100027


               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100048,

                   ObjectType = ActiveDirectoryObjectType.Group,
                   ActiveDirectoryFieldId = 100027

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100049,

                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100027

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100050,

                   ObjectType = ActiveDirectoryObjectType.OU,
                   ActiveDirectoryFieldId = 100027

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100051,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100028
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100052,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100029
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100053,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100030
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100054,
                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100031
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100055,
                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100032
               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100056,

                   ObjectType = ActiveDirectoryObjectType.User,
                   ActiveDirectoryFieldId = 100033

               },

               new ActiveDirectoryFieldObjectType
               {
                   Id = 100057,
                   ObjectType = ActiveDirectoryObjectType.Computer,
                   ActiveDirectoryFieldId = 100033

               },
                new ActiveDirectoryFieldObjectType
                {
                    Id = 100058,
                    ObjectType = ActiveDirectoryObjectType.User,
                    ActiveDirectoryFieldId = 100034
                }


           );


            modelBuilder.Entity<AccessLevel>(entity =>
            {
                entity.HasData(
                        new AccessLevel { Id = 1, Name = "Deny All" }
                );
                entity.Navigation(e => e.ObjectMap).AutoInclude();
                entity.Navigation(e => e.FieldMap).AutoInclude();
                entity.Navigation(e => e.ActionMap).AutoInclude();
            });

            modelBuilder.Entity<FieldAccessLevel>().HasData(
                new FieldAccessLevel() { Id = 1, Name = "Deny", Level = 10 },
                new FieldAccessLevel() { Id = 2, Name = "Read", Level = 100 },
                new FieldAccessLevel() { Id = 3, Name = "Edit", Level = 1000 }
            );
            modelBuilder.Entity<FieldAccessMapping>(entity =>
            {
                entity.Navigation(e => e.Field).AutoInclude();
                entity.Navigation(e => e.FieldAccessLevel).AutoInclude();
            });
            modelBuilder.Entity<ObjectAccessLevel>(entity =>
            {
                entity.HasData(
                new ObjectAccessLevel() { Id = 1, Name = "Deny", Level = 10 },
                new ObjectAccessLevel() { Id = 2, Name = "Read", Level = 1000 });

                entity.Navigation(e => e.ObjectAccessMappings).AutoInclude();
            }

            );
            modelBuilder.Entity<ObjectAccessMapping>(entity =>
            {
                entity.Navigation(e => e.ObjectAccessLevel).AutoInclude();
            });
            modelBuilder.Entity<DirectoryTemplate>(entity =>
            {
                entity.Navigation(e => e.FieldValues).AutoInclude();
            });
            modelBuilder.Entity<DirectoryTemplateFieldValue>(entity =>
            {
                entity.Navigation(e => e.Field).AutoInclude();
            });
            modelBuilder.Entity<ActionAccessFlag>().HasData(
                  new ActionAccessFlag() { Id = 1, Name = "Assign",Action = ActiveDirectoryObjectAction.Assign },
                  new ActionAccessFlag() { Id = 2, Name = "UnAssign", Action = ActiveDirectoryObjectAction.Unassign },
                  new ActionAccessFlag() { Id = 3, Name = "Unlock" , Action = ActiveDirectoryObjectAction.Unlock },
                  new ActionAccessFlag() { Id = 4, Name = "Enable", Action = ActiveDirectoryObjectAction.Enable },
                  new ActionAccessFlag() { Id = 5, Name = "Disable", Action = ActiveDirectoryObjectAction.Disable },
                  new ActionAccessFlag() { Id = 6, Name = "Rename", Action = ActiveDirectoryObjectAction.Rename },
                  new ActionAccessFlag() { Id = 7, Name = "Move", Action = ActiveDirectoryObjectAction.Move },
                  new ActionAccessFlag() { Id = 8, Name = "Create", Action = ActiveDirectoryObjectAction.Create },
                  new ActionAccessFlag() { Id = 9, Name = "Delete", Action = ActiveDirectoryObjectAction.Delete }

            );
            modelBuilder.Entity<ActionAccessMapping>(entity =>
            {
                entity.Navigation(e => e.ObjectAction).AutoInclude();
            });


            modelBuilder.Entity<PermissionMapping>(entity =>
            {
                entity.Navigation(e => e.AccessLevels).AutoInclude();

            });


            modelBuilder.Entity<DirectoryTemplate>(entity =>
            {
                entity.Navigation(e => e.AssignedGroupSids).AutoInclude();

            });

            modelBuilder.Entity<AppSettings>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                if (Database.IsMySql())
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "Id = 1"));
                else
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "[Id] = 1"));
            });

            modelBuilder.Entity<ADSettings>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                if (Database.IsMySql())
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "Id = 1"));
                else
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "[Id] = 1"));

            });
            modelBuilder.Entity<AuthenticationSettings>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                if (Database.IsMySql())
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "Id = 1"));

                else
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "[Id] = 1"));
                entity.HasData(new AuthenticationSettings
                {
                    Id = 1,
                    AdminPassword = "password"
                });
            });

            modelBuilder.Entity<EmailSettings>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                if (Database.IsMySql())
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "Id = 1"));

                else
                    entity.ToTable(t => t.HasCheckConstraint("CK_Table_Column", "[Id] = 1"));

            });


            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(e => e.UserGUID).IsUnique();
                entity.Navigation(e => e.Messages).AutoInclude();
            });
            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.Navigation(e => e.Notification).AutoInclude();
            });
            modelBuilder.Entity<PermissionDelegate>(entity =>
          {
              entity.HasIndex(e => e.DelegateSid).IsUnique();
          });

        }






        public static DatabaseException DownReason { get; set; }




        /// <summary>
        /// This should be private
        /// </summary>
        /// <returns></returns>
        private ServiceConnectionState TestConnection()
        {
            if (ConnectionString != null)
            {
                //Check for db connection
                try
                {
                    //Handle SQLite
                    if (Database.IsSqlite())
                    {
                        if (ConnectionString.File.Writable)
                        {
                            ConnectionString.File.EnsureCreated();
                            return ServiceConnectionState.Up;
                        }
                        else
                        {
                            DownReason = new("The Sqlite database folder is not writable by the current server user.");
                        }
                        return ServiceConnectionState.Down;
                    }


                    if (NetworkTools.IsPortOpen(ConnectionString.ServerAddress, ConnectionString.ServerPort))
                    {



                        Database.OpenConnection();
                        //Check for tables

                        if (IsSeeded())
                        {
                            //Installation has been completed

                            Database.CloseConnection();
                        }
                        else
                        {
                            Database.CloseConnection();
                            // return DatabaseStatus.TablesMissing;
                        }
                        return ServiceConnectionState.Up;

                    }
                    else
                    {
                        DownReason = new("The database port is not open or is not reachable.");

                        Database.CloseConnection();
                        // return DatabaseStatus.TablesMissing;
                    }

                }
                catch (SqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 53:
                            //Server unreachable
                            DownReason = new("The database port is open but connecting as an Sql server failed.");
                            break;


                        case 208:
                            //Tables Missing
                            DownReason = new("The database is missing a table. It may be in a corrupt state.");
                            break;
                        case 18456:
                            //Database may be missing or permission issue
                            DownReason = new("The database server is reachable, but the database could not be found or the" +
                                " credentials provided do not have permission to the database.");
                            break;

                    }

                }


                catch (RetryLimitExceededException)
                {
                    //Couldn't connect to DB
                    DownReason = new("The retry limit exceeded trying to connect to the database.");


                }
                catch (DatabaseConnectionStringException ex)
                {
                    DownReason = new("The database connection string is malformed. " + ex.Message);

                }
                catch (ApplicationException ex)
                {

                    DownReason = new("The database experienced a general error. " + ex.Message);

                }
                catch (Exception ex)
                {
                    Loggers.DatabaseLogger.Error(ex.Message, ex);
                    DownReason = new("The database experienced an unexpected error. " + ex.Message);

                }



            }
            return ServiceConnectionState.Down;

        }


        /// <summary>
        ///     Applies any pending migrations for the context to the database. Will create the database
        ///     if it does not already exist.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///        If the migration fails, the <see cref="Status"/> will be set to <see cref="ServiceConnectionState.Down"/>
        ///        </para>
        ///     <para>
        ///         See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see> for more information and examples.
        ///     </para>
        /// </remarks>
        /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> for the context.</param>

        public virtual bool Migrate()
        {
            try
            {
                Database.Migrate();
                return true;
            }
            catch (Exception ex)
            {
                Loggers.DatabaseLogger.Error("Database Auto-Update Failed!!!!", ex);
                DownReason = new DatabaseException(ex.Message, ex);
                OnMigrationFailed?.Invoke();
                return false;
            }
        }

        /// <summary>
        /// Checks if the database seed migration hase been applied
        /// </summary>
        /// <remarks>If the database cannot connect this method returns true</remarks>
        /// <returns>Returns true if the seed migration has been applied, or the database can't be reached, otherwise
        /// returns false.</returns>
        public virtual bool IsSeeded()
        {


            if (AppliedMigrations.Count() > 0) return true;
            try
            {
                if (AuthenticationSettings.FirstOrDefault() == null)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Checks if there is an applied and pending seed, indicating the migration
        /// chain has been reseeded and the database needs to be wiped and reinstalled
        /// </summary>
        /// <remarks>If the database cannot connect this returns false</remarks>
        /// <returns>Returns true if the seed migration has been applied, otherwise
        /// returns false.</returns>
        public virtual bool SeedMismatch
        {
            get
            {
                if (!IsSeeded()) return false;
                var seedMismatch = false;
                PendingMigrations.ForEach(am =>
                {
                    if (am.Contains("seed", StringComparison.OrdinalIgnoreCase))
                    {
                        seedMismatch = true;
                    }
                });
                return seedMismatch;
            }
        }

    }
}
