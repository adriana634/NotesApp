using NotesApp.ViewModels;
using NotesApp.ViewModels.Interfaces;

namespace NotesApp.Views;

public partial class AllNotesPage : ContentPage
{
    public AllNotesPage(NotesViewModel notesViewModel)
    {
        this.InitializeComponent();

        base.BindingContext = notesViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (this.BindingContext is NotesViewModel notesViewModel)
        {
            var loadNotesResult = await notesViewModel.LoadNotes();

            if (loadNotesResult.IsFailure)
            {
                await base.DisplayAlert("Error", "An error has occurred while loading notes.", "OK");
            }
        }

        if (base.BindingContext is IPageLifecycleAware viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        this.notesCollection.SelectedItem = null;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (base.BindingContext is IPageLifecycleAware viewModel)
        {
            viewModel.OnDisappearing();
        }
    }
}