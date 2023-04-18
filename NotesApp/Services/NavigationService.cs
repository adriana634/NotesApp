namespace NotesApp.Services
{
    public class NavigationService : INavigationService
    {
        public Task NavigateToAsync(string route)
        {
            return Shell.Current.GoToAsync(route);
        }

        public Task PopAsync()
        {
            return Shell.Current.GoToAsync("..");
        }
    }
}
