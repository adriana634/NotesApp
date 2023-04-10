using NotesApp.ViewModels;

namespace NotesApp.Views;

public partial class AllNotesPage : ContentPage
{
	public AllNotesPage()
	{
		InitializeComponent();
	}

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
		this.notesCollection.SelectedItem = null;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (this.BindingContext is NotesViewModel notesViewModel)
        {
            notesViewModel.LoadNotes();
        }
    }
}