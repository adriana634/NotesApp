using System.Diagnostics;

namespace NotesApp.Services
{
    public class LauncherService : ILauncherService
    {
        public async Task<bool> OpenAsync(string uri)
        {
            Debug.Assert(string.IsNullOrEmpty(uri) == false);
            return await Launcher.Default.OpenAsync(uri);
        }
    }
}
