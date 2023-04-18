using NotesApp.ViewModels;
using NotesApp.ViewModels.Interfaces;

namespace NotesApp.Views;

public partial class NewNotePage : ContentPage
{
    public NewNotePage(NewNoteViewModel newNoteViewModel)
    {
        this.InitializeComponent();

        base.BindingContext = newNoteViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (base.BindingContext is IPageLifecycleAware viewModel)
        {
            viewModel.OnAppearing();
        }
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