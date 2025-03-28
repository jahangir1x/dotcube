﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeltaShare.Model;
using DeltaShare.Service;
using DeltaShare.Util;
using DeltaShare.View;

namespace DeltaShare.ViewModel
{
    public partial class SharePoolViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string qrCodeData = String.Empty;
        public ObservableCollection<User> PoolUsers => StateManager.PoolUsers;

        public SharePoolViewModel(PoolCreatorClientService clientService)
        {
            QrCodeData = PoolCodeHandler.GenerateQrCodeData(NetworkHandler.GetLocalIps());
        }

        [RelayCommand]
        private async Task ClickViewSharedFilesBtn()
        {
            await Shell.Current.GoToAsync(nameof(DownloadFileView));
        }
    }
}
