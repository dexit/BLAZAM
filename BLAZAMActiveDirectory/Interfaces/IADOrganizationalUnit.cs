﻿
using BLAZAM.ActiveDirectory.Adapters;
using BLAZAM.Database.Models.Permissions;

namespace BLAZAM.ActiveDirectory.Interfaces
{
    public interface IADOrganizationalUnit : IDirectoryEntryAdapter

    {

        string? Name { get; set; }
        List<PermissionMapping> InheritedPermissionMappings { get; }
        IQueryable<PermissionMapping> AppliedPermissionMappings { get; }
        List<PermissionMapping> DirectPermissionMappings { get; }
        IQueryable<PermissionMapping> OffspringPermissionMappings { get; }
        IEnumerable<IDirectoryEntryAdapter> SubOUs { get; }

        HashSet<IDirectoryEntryAdapter> CachedTreeViewSubOUs { get;}
        HashSet<IDirectoryEntryAdapter> TreeViewSubOUs { get; }
        bool CanReadInSubOus { get; }
        bool CanCreateUser { get; }
        bool CanReadUsers { get; }

        IADGroup CreateGroup(string containerName);
        IADUser CreateUser(string containerName);
        IADOrganizationalUnit CreateOU(string containerName);
        Task<IEnumerable<IDirectoryEntryAdapter>> GetChildrenAsync();
        Task<bool> HasChildrenAsync();
        IADPrinter CreatePrinter(string containerName, string uncPath, string shortServerName);
        IADPrinter CreatePrinter(SharedPrinter sharedPrinter);
    }
}