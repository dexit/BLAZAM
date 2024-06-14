﻿using BLAZAM.ActiveDirectory.Adapters;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = MudBlazor.Color;

namespace BLAZAM.Gui.UI.Inputs.TreeViews
{
    public class OUTreeViewBase : AppComponentBase
    {

        protected ADOrganizationalUnit TopLevel;
        IADOrganizationalUnit? _startingSelectedNode;


        IDirectoryEntryAdapter? _selectedEntry;

        [Parameter]
        public bool StartRootExpanded { get; set; } = true;

        [Parameter]
        public string? Label { get; set; }
        /// <summary>
        /// The root ou of this TreeView
        /// </summary>
        /// <remarks>
        /// Defaults to the App Base root
        /// </remarks>
        [Parameter]
        public HashSet<IDirectoryEntryAdapter> RootOU { get; set; } = new HashSet<IDirectoryEntryAdapter>();
        [Parameter]
        public IADOrganizationalUnit? StartingSelectedOU
        {
            get => _startingSelectedNode; set
            {
                if (value == _startingSelectedNode) return;
                _startingSelectedNode = value;
                _selectedEntry = value;


                StartingSelectedOUChanged.InvokeAsync(value);
            }

        }

        [Parameter]
        public EventCallback<IADOrganizationalUnit> StartingSelectedOUChanged
        {
            get; set;
        }

        [Parameter]
        public IDirectoryEntryAdapter? SelectedEntry
        {
            get => _selectedEntry; set
            {
                if (value == _selectedEntry) return;
                if (value != null)
                {
                    var cache = _selectedEntry;

                    _selectedEntry = value;
                    if (cache == null && RootOU.Count>0 && value == RootOU.First()) return;


                    SelectedEntryChanged.InvokeAsync(value);


                    //if (TopLevel == null)
                    //    OnInitializedAsync();
                    
                    if(RootOU.Count > 0)
                    OpenToSelected();

                }
            }

        }

        [Parameter]
        public EventCallback<IDirectoryEntryAdapter> SelectedEntryChanged { get; set; }
        /// <summary>
        /// Text to show at the end of the TreeView item
        /// </summary>
        [Parameter]
        public Func<IDirectoryEntryAdapter, string>? EndText { get; set; }
        
        protected Color GetItemColor(IDirectoryEntryAdapter item)
        {
            if (item is IAccountDirectoryAdapter account)
            {
                if (account.Disabled) return Color.Error;
                if (account.LockedOut) return Color.Warning;
                if (account.Created > DateTime.Now.AddDays(-14)) return Color.Success;
            }
            return Color.Default;
        }
       protected async Task InitializeTreeView()
        {
            //Wait a few milliseconds to trip the async into acutally awaiting
            await Task.Delay(500);

            //ApplicationBaseOUs = Directory.OUs.FindSubOusByDN(null);


            if (RootOU is null || RootOU.Count < 1)
            {
                TopLevel = new ADOrganizationalUnit();
                TopLevel.Parse(directory: Directory, directoryEntry: Directory.GetDirectoryEntry());
                _ = TopLevel.SubOUs;
                RootOU = new HashSet<IDirectoryEntryAdapter>() { TopLevel as IDirectoryEntryAdapter };
            }
            if (StartingSelectedOU == null)
            {
                StartingSelectedOU = TopLevel;

            }

            OpenToSelected();
            LoadingData = false;
            await InvokeAsync(StateHasChanged);
        }












        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await InitializeTreeView();

        }

        protected void OpenToSelected()
        {

            if (StartRootExpanded)
            {
                if (!SelectedEntry.Equals(RootOU))
                {
                    IDirectoryEntryAdapter? firstThing = RootOU.First();
                    if (firstThing is IADOrganizationalUnit openThis)
                    {
                        openThis.IsExpanded = true;
                        while (openThis != null)
                        {
                            var child = openThis.SubOUs.Where(c => SelectedEntry.DN.Contains(c.DN) && !SelectedEntry.DN.Equals(c.DN)).FirstOrDefault();
                            if (child != null)
                            {
                                child.IsExpanded = true;
                                
                                _ = (child as IADOrganizationalUnit)?.TreeViewSubOUs;
                                openThis = child as IADOrganizationalUnit;
                            }
                            else
                            {
                                var matchingOU = openThis.SubOUs.Where(c => SelectedEntry.DN.Equals(c.DN)).FirstOrDefault();
                                if (matchingOU != null)
                                    matchingOU.IsSelected = true;
                                break;
                            }


                        }
                    }
                }
            }
            //InvokeAsync(StateHasChanged);


        }
        /// <summary>
        /// Defines a function to determine whether an Active Directory object should be
        /// displayed in the tree view or not
        /// </summary>
        [Parameter]
        public Func<IDirectoryEntryAdapter,bool>? AdditionalVisibilityFilters { get; set; }


        protected bool ShouldShowOU(IDirectoryEntryAdapter entry)
        {
            if (entry is IADOrganizationalUnit ou)
            {
                if (ou.CanRead)
                    return true;
                if (AdditionalVisibilityFilters != null)
                {
                    if(AdditionalVisibilityFilters(entry)) return true;
                }
            }
            return false;
        }
    }
}