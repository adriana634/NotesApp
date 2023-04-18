namespace NotesApp.ViewModels.Interfaces
{
    public interface IPageLifecycleAware
    {
        void OnAppearing();
        void OnDisappearing();
    }
}
