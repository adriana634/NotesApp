using System.Diagnostics;

namespace NotesApp.Services
{
    public class NavigationService : INavigationService
    {
        public Task NavigateToAsync(string route)
        {
            Debug.Assert(string.IsNullOrEmpty(route) == false);
            return Shell.Current.GoToAsync(route);
        }

        public Task PopAsync()
        {
            return Shell.Current.GoToAsync("..");
        }
    }
}
