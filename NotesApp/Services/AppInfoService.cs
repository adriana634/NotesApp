namespace NotesApp.Services
{
    public class AppInfoService : IAppInfoService
    {
        public string Name => AppInfo.Name;
        public string VersionString => AppInfo.VersionString;
    }
}
