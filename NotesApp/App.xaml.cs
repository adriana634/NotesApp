using NotesApp.Repositories;

namespace NotesApp;

public partial class App : Application
{
	public static NoteRepository NoteRepository { get; set; }

	public App(NoteRepository noteRepository)
	{
		InitializeComponent();

		MainPage = new AppShell();

		App.NoteRepository = noteRepository;
	}
}
