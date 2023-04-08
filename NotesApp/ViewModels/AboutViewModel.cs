﻿using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class AboutViewModel
    {
        public string Title => AppInfo.Name;
        public string Version => AppInfo.VersionString;
        public string MoreInfoUrl => "https://aka.ms/maui";
        public string Message => "This app is written in XAML and C# with .NET MAUI.";
        public ICommand ShowMoreInfoCommand { get; }

        public AboutViewModel()
        {
            this.ShowMoreInfoCommand = new AsyncRelayCommand(this.ShowMoreInfo);
        }

        private async Task ShowMoreInfo()
        {
            await Launcher.Default.OpenAsync(this.MoreInfoUrl);
        }
    }
}
