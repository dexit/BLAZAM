﻿using BLAZAM.Common.Data.Database;
using BLAZAM.Server.Background;
using BLAZAM.Server.Pages.Error;
using Microsoft.EntityFrameworkCore;

namespace BLAZAM.Server.Middleware
{
    internal class ApplicationStatusRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConnMonitor _monitor;
        private readonly List<string> _uriIgnoreList = new List<string> { "/static","/css", "/_content","/_blazor","/BLAZAM.styles.css","/_framework" };
        private string intendedUri;

        public ApplicationStatusRedirectMiddleware(
           RequestDelegate next,
           ConnMonitor monitor)
        {
            _next = next;
            _monitor = monitor;
        }

        public async Task InvokeAsync(HttpContext context, IDbContextFactory<DatabaseContext> factory)
        {
            intendedUri = context.Request.Path.ToUriComponent();
            if (!InIgnoreList(intendedUri))
            {
                try
                {
                    switch (_monitor.AppReady)
                    {
                        case ConnectionState.Connecting:
                            SendTo(context, "/");
                            break;
                        case ConnectionState.Up:
                            var appliedSeedMigration = factory.CreateDbContext().AppliedMigrations.Where(m => m.Contains("seed", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            var stagedSeedMigration = factory.CreateDbContext().AppliedMigrations.Where(m => m.Contains("seed", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if(appliedSeedMigration!=null && stagedSeedMigration !=null)
                            {
                                Oops.ErrorMessage = "The application database is incompatible with this version of the application";
                                Oops.DetailsMessage = "The database seed is different from the current version of the application";
                                Oops.HelpMessage = "Either install an older version of the application. Or create a new database to use with the new version.";
                                SendTo(context, "/oops");

                            }
                            if (!Program.InstallationCompleted)
                            {
                                SendTo(context,"/install");
                            }
                            break;
                        case ConnectionState.Down:
                            SendTo(context, "/oops");

                            break;
                    }
                

                }
                catch
                {
                    SendTo(context, "/oops");

                }
            }
            await _next(context);
        }

        //Sets the response header to redirect to
        private void SendTo(HttpContext context, string uri)
        {
            if (intendedUri != uri)
                context.Response.Redirect(uri);    
        }

        private bool InIgnoreList(string intendedUri)
        {
            foreach(var uri in _uriIgnoreList) { 
                if (intendedUri.StartsWith(uri)) return true;
            }
            return false;
        }
    }
}