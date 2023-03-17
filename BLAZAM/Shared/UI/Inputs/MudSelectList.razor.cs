using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BLAZAM;
using BLAZAM.Server.Background;
using BLAZAM.Server.Shared;
using BLAZAM.Server.Pages;
using BLAZAM.Server.Shared.Layouts;
using BLAZAM.Server.Shared.Navs;
using BLAZAM.Server.Shared.Email.Base;
using BLAZAM.Server.Shared.Email;
using BLAZAM.Server.Shared.UI;
using BLAZAM.Server.Shared.UI.Users;
using BLAZAM.Server.Shared.UI.Users.Fields;
using BLAZAM.Server.Shared.UI.OU;
using BLAZAM.Server.Shared.UI.Groups;
using BLAZAM.Server.Shared.UI.Settings;
using BLAZAM.Server.Shared.UI.Settings.Templates;
using BLAZAM.Server.Shared.UI.Inputs;
using BLAZAM.Server.Shared.ResourceFiles;
using BLAZAM.Server.Shared.UI.Outputs;
using BLAZAM.Server.Shared.UI.Themes;
using BLAZAM.Server.Data;
using BLAZAM.Server.Data.Services.Update;
using BLAZAM.Server.Data.Services;
using BLAZAM.Server.Pages.Error;
using BLAZAM.Common;
using BLAZAM.Common.Extensions;
using BLAZAM.Common.Helpers;
using BLAZAM.Common.Data;
using BLAZAM.Common.Data.FileSystem;
using BLAZAM.Common.Exceptions;
using BLAZAM.Common.Data.Database;
using BLAZAM.Common.Data.Services;
using BLAZAM.Common.Data.ActiveDirectory;
using BLAZAM.Common.Data.ActiveDirectory.Interfaces;
using BLAZAM.Common.Data.ActiveDirectory.Models;
using BLAZAM.Common.Data.ActiveDirectory.Searchers;
using BLAZAM.Common.Models.Database.Audit;
using BLAZAM.Common.Models.Database;
using BLAZAM.Common.Models;
using BLAZAM.Common.Models.Database.Templates;
using BLAZAM.Common.Models.Database.Permissions;
using MudBlazor;
using Microsoft.Extensions.Localization;
using BLAZAM.Server.Data.Services.Email;

namespace BLAZAM.Server.Shared.UI.Inputs
{
    public partial class MudSelectList<T> : MudSelect<T> where T : Type
    {

        [Parameter]
        public IEnumerable<T> Values { get; set; }
    }
}