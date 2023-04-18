namespace NotesApp.Services
{
    public class LauncherService : ILauncherService
    {
        public async Task<bool> OpenAsync(string uri)
        {
            return await Launcher.Default.OpenAsync(uri);
        }
    }
}
