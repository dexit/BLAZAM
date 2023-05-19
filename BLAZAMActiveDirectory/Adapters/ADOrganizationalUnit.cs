﻿using BLAZAM.ActiveDirectory.Interfaces;
using BLAZAM.Database.Models.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.DirectoryServices;
using System.Reflection.PortableExecutable;

namespace BLAZAM.ActiveDirectory.Adapters
{
    public class ADOrganizationalUnit : DirectoryEntryAdapter, IADOrganizationalUnit
    {
        private IEnumerable<IADOrganizationalUnit>? childrenCache;
        private IQueryable<IADUser>? childUserCache;
        private IQueryable<IADComputer>? childComputerCache;
        private IQueryable<IADGroup>? childGroupCache;

        public override bool HasChildren=> SubOUs.Any();

        
        public async Task<bool> HasChildrenAsync()
        {
            return await Task.Run(() =>
            {
                return HasChildren;
            });
        }
        public async Task<IEnumerable<IADOrganizationalUnit>> GetChildrenAsync()
        {
            return await Task.Run(() =>
            {
                return SubOUs;
            });
        }

        public HashSet<IADOrganizationalUnit> CachedTreeViewSubOUs { get; private set; } = new();

        public HashSet<IADOrganizationalUnit> TreeViewSubOUs
        {
            get
            {
                CachedTreeViewSubOUs = SubOUs.ToHashSet();
                return CachedTreeViewSubOUs;
            }
        }
        public override  IEnumerable<IDirectoryEntryAdapter> Children { get {
                List<IDirectoryEntryAdapter>directoryEntries = new List<IDirectoryEntryAdapter>();
                var children = DirectoryEntry.Children;
                DirectoryEntryAdapter? thisObject=null;
                foreach (System.DirectoryServices.DirectoryEntry child in children)
                {
                        
                    if (child.Properties["objectClass"].Contains("top"))
                    {
                        if (child.Properties["objectClass"].Contains("computer"))
                        {
                            thisObject = new ADComputer();
                            thisObject.Parse(child, ActiveDirectoryContext.Instance);
                        }
                        else if (child.Properties["objectClass"].Contains("user"))
                        {
                            thisObject = new ADUser();
                            thisObject.Parse(child, ActiveDirectoryContext.Instance);
                        }
                        else if (child.Properties["objectClass"].Contains("organizationalUnit"))
                        {
                            thisObject = new ADOrganizationalUnit();
                            thisObject.Parse(child, ActiveDirectoryContext.Instance);
                        }
                        else if (child.Properties["objectClass"].Contains("group"))
                        {
                            thisObject = new ADGroup();
                            thisObject.Parse(child, ActiveDirectoryContext.Instance);
                        }
                        else if (child.Properties["objectClass"].Contains("printQueue"))
                        {
                            thisObject = new ADPrinter();
                            thisObject.Parse(child, ActiveDirectoryContext.Instance);
                        }
                        if (thisObject != null)
                            directoryEntries.Add(thisObject);

                    }
                    thisObject = null;

                }
                CachedChildren = directoryEntries;
                return CachedChildren;
            } }
        public IEnumerable<IADOrganizationalUnit> SubOUs
        {
            get
            {
                if (childrenCache == null)
                    childrenCache = Directory.OUs.FindSubOusByDN(DN).OrderBy(ou => ou.CanonicalName).AsQueryable();


                return childrenCache;
            }
        }
        public IQueryable<IADUser> ChildUsers
        {
            get
            {
                if (childUserCache == null)

                    childUserCache = Directory.OUs.FindSubUsersByDN(DN).OrderBy(ou => ou.CanonicalName).AsQueryable();
                return childUserCache;

            }
        }
        public IQueryable<IADComputer> ChildComputers
        {
            get
            {
                if (childComputerCache == null)

                    childComputerCache = Directory.OUs.FindSubComputerByDN(DN).OrderBy(ou => ou.CanonicalName).AsQueryable();
                return childComputerCache;

            }
        }

        public IQueryable<IADGroup> ChildGroups
        {
            get
            {
                if (childGroupCache == null)

                    childGroupCache = Directory.OUs.FindSubGroupsByDN(DN).OrderBy(ou => ou.CanonicalName).AsQueryable();
                return childGroupCache;

            }
        }
        public override string? CanonicalName
        {
            get => Name;
            set => Name = value;
        }
        public string Name
        {
            get
            {
                return GetStringProperty("name");
            }
            set
            {
                SetProperty("name", value);
            }
        }
        public List<PermissionMapping> InheritedPermissionMappings
        {
            get
            {
                return AppliedPermissionMappings.Where(m => !m.OU.Equals(DN)).ToList();
            }
        }
        public List<PermissionMapping> DirectPermissionMappings
        {
            get
            {
                return AppliedPermissionMappings.Where(m => m.OU.Equals(DN)).ToList();
            }
        }


        public IQueryable<PermissionMapping> AppliedPermissionMappings
        {
            get
            {


                return DbFactory.CreateDbContext().PermissionMap.Include(m => m.PermissionDelegates).Where(m => DN.Contains(m.OU)).OrderByDescending(m => m.OU.Length);
            }
        }

        public IQueryable<PermissionMapping> OffspringPermissionMappings
        {
            get
            {


                return DbFactory.CreateDbContext().PermissionMap.Include(m => m.PermissionDelegates).Where(m => m.OU.Contains(DN) && m.OU != DN).OrderByDescending(m => m.OU.Length);
            }
        }




        /// <summary>
        /// Creates a new OU under this OU. Note that the returned Directory object
        /// must execute CommitChanges() to actually create the object in Active
        /// Directory.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns>An uncommited organizational unit</returns>
        public IADOrganizationalUnit CreateOU(string containerName)
        {
            EnsureDirectoryEntry();
            IADOrganizationalUnit newOU = new ADOrganizationalUnit();

            newOU.Parse(DirectoryEntry.Children.Add("OU=" + containerName.Trim(), "OrganizationalUnit"), Directory);
            newOU.NewEntry = true;
            return newOU;
        }
        /// <summary>
        /// Creates a new user under this OU. Note that the returned Directory object
        /// must execute CommitChanges() to actually create the object in Active
        /// Directory.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns>An uncommited user</returns>
        public IADUser CreateUser(string containerName)
        {

            IADUser newUser = new ADUser();
            if (DirectoryEntry == null)
                DirectoryEntry = searchResult?.GetDirectoryEntry();
            newUser.Parse(DirectoryEntry.Children.Add("CN=" + containerName.Trim().Replace(",", "\\,"), "user"), Directory);
            newUser.NewEntry = true;
            return newUser;

        }

        /// <summary>
        /// Creates a new group under this OU. Note that the returned Directory object
        /// must execute CommitChanges() to actually create the object in Active
        /// Directory.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns>An uncommited group</returns>
        public IADGroup CreateGroup(string containerName)
        {

            IADGroup newGroup = new ADGroup();
            if (DirectoryEntry == null)
                DirectoryEntry = searchResult?.GetDirectoryEntry();
            newGroup.Parse(DirectoryEntry.Children.Add("CN=" + containerName.Trim(), "group"), Directory);
            newGroup.NewEntry = true;
            return newGroup;

        }
        public override int GetHashCode()
        {
            return DN.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj is IADOrganizationalUnit otherOU)
            {
                if (otherOU.DN == DN)
                {
                    return true;
                }
            }
            return false;
        }
    }

}