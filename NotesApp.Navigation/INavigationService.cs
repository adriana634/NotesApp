namespace NotesApp.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync(string route);
        Task PopAsync();
    }
}
