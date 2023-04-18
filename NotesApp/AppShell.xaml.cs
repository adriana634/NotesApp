using NotesApp.Navigation;
using NotesApp.Views;

namespace NotesApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
        this.InitializeComponent();

        Routing.RegisterRoute(Routes.AllNotesPage, typeof(AllNotesPage));
        Routing.RegisterRoute(Routes.NewNotePage, typeof(NewNotePage));
        Routing.RegisterRoute(Routes.UpdateNotePage, typeof(UpdateNotePage));
        Routing.RegisterRoute(Routes.AboutPage, typeof(AboutPage));
    }
}
