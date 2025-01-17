﻿
using BLAZAM.Common.Data;
using BLAZAM.Common.Data.Database;
using BLAZAM.Helpers;
using BLAZAM.Database.Exceptions;
using BLAZAM.Database.Models;
using BLAZAM.Database.Models.Audit;
using BLAZAM.Database.Models.Permissions;
using BLAZAM.Database.Models.Templates;
using BLAZAM.Database.Models.User;
using BLAZAM.Logger;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using BLAZAM.Database.Models.Chat;
using BLAZAM.Server.Data;
using System.Data;
using BLAZAM.FileSystem;
using BLAZAM.Database.Models.Notifications;

namespace BLAZAM.Database.Context
{
    public class DatabaseContextBase : DbContext, IDatabaseContext
    {





        public DatabaseConnectionString? ConnectionString { get; set; }


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

        }







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
        public virtual DbSet<ReadNewsItem> ReadNewsItems { get; set; }
        public virtual DbSet<UserFavoriteEntry> UserFavoriteEntries { get; set; }
        public virtual DbSet<UserDashboardWidget> UserDashboardWidgets { get; set; }
        public virtual DbSet<NotificationMessage> NotificationMessages { get; set; }
        public virtual DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }


        //Audit Logs
        public virtual DbSet<SystemAuditLog> SystemAuditLog { get; set; }
        public virtual DbSet<LogonAuditLog> LogonAuditLog { get; set; }
        public virtual DbSet<DirectoryEntryAuditLog> DirectoryEntryAuditLogs { get; set; }
        //public virtual DbSet<UserAuditLog> UserAuditLog { get; set; }
        //public virtual DbSet<GroupAuditLog> GroupAuditLog { get; set; }
        //public virtual DbSet<ComputerAuditLog> ComputerAuditLog { get; set; }
        //public virtual DbSet<OUAuditLog> OUAuditLog { get; set; }
        public virtual DbSet<RequestAuditLog> RequestAuditLog { get; set; }
        public virtual DbSet<PermissionsAuditLog> PermissionsAuditLog { get; set; }
        public virtual DbSet<SettingsAuditLog> SettingsAuditLog { get; set; }



        //Permissions
        public virtual DbSet<ActiveDirectoryField> ActiveDirectoryFields { get; set; }
        public virtual DbSet<CustomActiveDirectoryField> CustomActiveDirectoryFields { get; set; }
        public virtual DbSet<ActiveDirectoryFieldObjectType> ActiveDirectoryFieldObjectMappings { get; set; }
        public virtual DbSet<AccessLevel> AccessLevels { get; set; }
        public virtual DbSet<ObjectAccessMapping> AccessLevelObjectMapping { get; set; }
        public virtual DbSet<FieldAccessMapping> AccessLevelFieldMapping { get; set; }
        public virtual DbSet<FieldAccessLevel> FieldAccessLevel { get; set; }
        public virtual DbSet<ObjectAccessLevel> ObjectAccessLevel { get; set; }
        public virtual DbSet<ObjectAction> ObjectActionFlag { get; set; }

        public virtual DbSet<PermissionDelegate> PermissionDelegate { get; set; }
        public virtual DbSet<PermissionMapping> PermissionMap { get; set; }

        public virtual DbSet<ChatRoom> ChatRooms { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<UnreadChatMessage> UnreadChatMessages { get; set; }


        //Templates
        public virtual DbSet<DirectoryTemplate> DirectoryTemplates { get; set; }
        public virtual DbSet<DirectoryTemplateFieldValue> DirectoryTemplateFieldValues { get; set; }
        public virtual DbSet<DirectoryTemplateGroup> DirectoryTemplateGroups { get; set; }



        public static ConfigurationManager Configuration { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            throw new NotImplementedException("DatabaseContext of type " + GetType().FullName + " has not implemented OnConfiguring");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<ActiveDirectoryField>().HasData(


                new ActiveDirectoryField
                {
                    Id = 1,
                    FieldName = "sn",
                    DisplayName = "Last Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 2,
                    FieldName = "givenname",
                    DisplayName = "First Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 3,
                    FieldName = "physicalDeliveryOfficeName",
                    DisplayName = "Office",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 4,
                    FieldName = "employeeId",
                    DisplayName = "Employee ID",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 5,
                    FieldName = "homeDirectory",
                    DisplayName = "Home Directory",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 6,
                    FieldName = "scriptPath",
                    DisplayName = "Logon Script Path",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 7,
                    FieldName = "profilePath",
                    DisplayName = "Profile Path",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 8,
                    FieldName = "homePhone",
                    DisplayName = "Home Phone Number",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 9,
                    FieldName = "streetAddress",
                    DisplayName = "Street Address",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 10,
                    FieldName = "l",
                    DisplayName = "City",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 11,
                    FieldName = "st",
                    DisplayName = "State",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 12,
                    FieldName = "postalCode",
                    DisplayName = "Zip Code",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 13,
                    FieldName = "site",
                    DisplayName = "Site",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 14,
                    FieldName = "name",
                    DisplayName = "Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 15,
                    FieldName = "samaccountname",
                    DisplayName = "Username",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 16,
                    FieldName = "objectSID",
                    DisplayName = "SID",
                    FieldType = ActiveDirectoryFieldType.RawData
                },

                new ActiveDirectoryField
                {
                    Id = 17,
                    FieldName = "mail",
                    DisplayName = "E-Mail Address",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 18,
                    FieldName = "description",
                    DisplayName = "Description",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 19,
                    FieldName = "displayName",
                    DisplayName = "Display Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 20,
                    FieldName = "distinguishedName",
                    DisplayName = "Distinguished Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 21,
                    FieldName = "memberOf",
                    DisplayName = "Member Of",
                    FieldType = ActiveDirectoryFieldType.List
                },

                new ActiveDirectoryField
                {
                    Id = 22,
                    FieldName = "company",
                    DisplayName = "Company",
                    FieldType = ActiveDirectoryFieldType.Text
                },


                new ActiveDirectoryField
                {
                    Id = 23,
                    FieldName = "title",
                    DisplayName = "Title",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 24,
                    FieldName = "userPrincipalName",
                    DisplayName = "User Principal Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 25,
                    FieldName = "telephoneNumber",
                    DisplayName = "Telephone Number",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 26,
                    FieldName = "postOfficeBox",
                    DisplayName = "PO Box",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 27,
                    FieldName = "cn",
                    DisplayName = "Canonical Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 28,
                    FieldName = "homeDrive",
                    DisplayName = "Home Drive",
                    FieldType = ActiveDirectoryFieldType.DriveLetter
                },

                new ActiveDirectoryField
                {
                    Id = 29,
                    FieldName = "department",
                    DisplayName = "Department",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 30,
                    FieldName = "middleName",
                    DisplayName = "Middle Name",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 31,
                    FieldName = "pager",
                    DisplayName = "Pager",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 32,
                    FieldName = "operatingSystemVersion",
                    DisplayName = "OS",
                    FieldType = ActiveDirectoryFieldType.Text
                },

                new ActiveDirectoryField
                {
                    Id = 33,
                    FieldName = "accountExpires",
                    DisplayName = "Account Expiration",
                    FieldType = ActiveDirectoryFieldType.Date
                },
                new ActiveDirectoryField
                {
                    Id = 34,
                    FieldName = "manager",
                    DisplayName = "Manager",
                    FieldType = ActiveDirectoryFieldType.Text
                },
                new ActiveDirectoryField
                {
                    Id = 35,
                    FieldName = "thumbnail",
                    DisplayName = "Photo",
                    FieldType = ActiveDirectoryFieldType.RawData
                },
                new ActiveDirectoryField
                {
                    Id = 36,
                    FieldName = "userWorkstations",
                    DisplayName = "Log On To",
                    FieldType = ActiveDirectoryFieldType.Text
                },
                new ActiveDirectoryField
                {
                    Id = 37,
                    FieldName = "logonHours",
                    DisplayName = "Logon Hours",
                    FieldType = ActiveDirectoryFieldType.RawData
                }


            );


            modelBuilder.Entity<CustomActiveDirectoryField>()
         .HasMany(x => x.ObjectTypes);
            modelBuilder.Entity<CustomActiveDirectoryField>()
         .Navigation(x => x.ObjectTypes).AutoInclude();




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
                entity.Navigation(e => e.CustomField).AutoInclude();
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
                entity.Navigation(e => e.AssignedGroupSids).AutoInclude();

            });
            modelBuilder.Entity<DirectoryTemplateFieldValue>(entity =>
            {
                entity.Navigation(e => e.Field).AutoInclude();
                entity.Navigation(e => e.CustomField).AutoInclude();
            });
        
            modelBuilder.Entity<ObjectAction>().HasData(
                  new ObjectAction() { Id = 1, Name = "Assign", Action = ActiveDirectoryObjectAction.Assign },
                  new ObjectAction() { Id = 2, Name = "UnAssign", Action = ActiveDirectoryObjectAction.Unassign },
                  new ObjectAction() { Id = 3, Name = "Unlock", Action = ActiveDirectoryObjectAction.Unlock },
                  new ObjectAction() { Id = 4, Name = "Enable", Action = ActiveDirectoryObjectAction.Enable },
                  new ObjectAction() { Id = 5, Name = "Disable", Action = ActiveDirectoryObjectAction.Disable },
                  new ObjectAction() { Id = 6, Name = "Rename", Action = ActiveDirectoryObjectAction.Rename },
                  new ObjectAction() { Id = 7, Name = "Move", Action = ActiveDirectoryObjectAction.Move },
                  new ObjectAction() { Id = 8, Name = "Create", Action = ActiveDirectoryObjectAction.Create },
                  new ObjectAction() { Id = 9, Name = "Delete", Action = ActiveDirectoryObjectAction.Delete }

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
                entity.Navigation(e => e.ReadNewsItems).AutoInclude();
                entity.Navigation(e => e.FavoriteEntries).AutoInclude();
                entity.Navigation(e => e.DashboardWidgets).AutoInclude();
               // entity.Navigation(e => e.NotificationSubscriptions).AutoInclude();

                //entity.Navigation(e => e.UnreadChatMessages).AutoInclude();

            });
            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.Navigation(e => e.Notification).AutoInclude();
            });



            modelBuilder.Entity<PermissionDelegate>(entity =>
          {
              entity.HasIndex(e => e.DelegateSid).IsUnique();
          });

            modelBuilder.Entity<ChatRoom>(entity =>
            {
                entity.HasMany(e => e.Members).WithMany();
                entity.Navigation(e => e.Messages).AutoInclude();
                entity.Navigation(e => e.Members).AutoInclude();
            });
            modelBuilder.Entity<ChatMessage>(entity =>
            {

                entity.Navigation(e => e.User).AutoInclude();

            });
            modelBuilder.Entity<NotificationSubscription>(entity =>
            {

                entity.Navigation(e => e.NotificationTypes).AutoInclude();

            });
            modelBuilder.Entity<UnreadChatMessage>(entity =>
         {
             entity.Navigation(e => e.ChatMessage).AutoInclude();

         });



        }




        public bool EntityIsTracked<TEntry>(TEntry entry)
        {
            if (entry == null) return false;
            return base.Entry(entry).State != EntityState.Detached;
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
                            if (ConnectionString.File.Exists)
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
                    Loggers.DatabaseLogger.Error(ex.Message + " {@Error}", ex);
                    DownReason = new("The database experienced an unexpected error. " + ex.Message);

                }



            }
            return ServiceConnectionState.Down;

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


        public void Export(string directory)
        {
            // Get all the DbSet properties of the context
            var dbSets = this.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            // Loop through each DbSet property
            foreach (var dbSet in dbSets)
            {
                // Get the entity type of the DbSet
                var entityType = dbSet.PropertyType.GetGenericArguments()[0];

                // Get the table name of the entity type
                var tableName = Model.FindEntityType(entityType).GetTableName();

                DataTable table = SelectAllDataFromTable(tableName);

                // Create a CSV file name for the table
                var fileName = Path.Combine(directory, tableName + ".csv");
                var file = new SystemFile(fileName);
                file.EnsureCreated();
                // Write the data table to the CSV file
                using (var writer = new StreamWriter(fileName))
                {
                    // Write the column names
                    var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
                    writer.WriteLine(string.Join(",", columnNames));

                    // Write the rows
                    foreach (DataRow row in table.Rows)
                    {
                        var fields = row.ItemArray.Select(f => f?.ToString());
                        List<string> lines = new();
                        foreach(var field in fields)
                        {
                            lines.Add( '"' + field + '"');
                        }
                        writer.WriteLine(string.Join(",", lines));
                    }
                }
            }
        }

        protected virtual DataTable SelectAllDataFromTable(string? tableName)
        {
            throw new NotImplementedException("The SelectAllDataFromTable method has not been implemented");
           
        }

    }
}
