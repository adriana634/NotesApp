using CommunityToolkit.Mvvm.Input;
using NotesApp.Services;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class AboutViewModel
    {
        private readonly IAppInfoService _appInfoService;
        private readonly ILauncherService _launcherService;

        public string Title => this._appInfoService.Name;
        public string Version => this._appInfoService.VersionString;
        public string MoreInfoUrl => "https://aka.ms/maui";
        public string Message => "This app is written in XAML and C# with .NET MAUI.";
        public ICommand ShowMoreInfoCommand { get; }

        public AboutViewModel(IAppInfoService appInfoService, ILauncherService launcherService)
        {
            this._appInfoService = appInfoService;
            this._launcherService = launcherService;

            this.ShowMoreInfoCommand = new AsyncRelayCommand(this.ShowMoreInfo);
        }

        private async Task ShowMoreInfo()
        {
            await this._launcherService.OpenAsync(this.MoreInfoUrl);
        }
    }
}
