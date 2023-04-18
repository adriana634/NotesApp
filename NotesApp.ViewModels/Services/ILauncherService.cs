namespace NotesApp.Services
{
    public interface ILauncherService
    {
        Task<bool> OpenAsync(string uri);
    }
}
