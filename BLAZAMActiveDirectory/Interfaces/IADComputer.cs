﻿using System.Net;

namespace BLAZAM.ActiveDirectory.Interfaces
{
    public interface IADComputer : IAccountDirectoryAdapter
    {
        string? OperatingSystem { get; set; }
        bool? Online { get; }
        IPHostEntry? IPHostEntry { get; set; }
        AppEvent<bool> OnOnlineChanged { get; set; }

        List<IADComputerDrive> GetDrives();
        Task<List<IADComputerDrive>> GetDrivesAsync();
        Task<List<IRemoteSession>> GetRemoteSessionsAsync();
    }
}