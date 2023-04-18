using NotesApp.Services;
using NotesApp.ViewModels;
using NotesApp.ViewModels.Interfaces;

namespace NotesApp.Views;

public partial class UpdateNotePage : ContentPage
{
    private readonly INoteSelectionService _noteSelectionService;
    private readonly IUpdateNoteViewModelFactory _updateNoteViewModelFactory;

    public UpdateNotePage(INoteSelectionService noteSelectionService, IUpdateNoteViewModelFactory updateNoteViewModelFactory)
    {
        this.InitializeComponent();

        this._noteSelectionService = noteSelectionService;
        this._updateNoteViewModelFactory = updateNoteViewModelFactory;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        base.BindingContext = this._updateNoteViewModelFactory.Create(this._noteSelectionService.SelectedNote);

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