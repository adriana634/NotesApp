using NotesApp.ViewModels;

namespace NotesApp.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage(AboutViewModel aboutViewModel)
	{
		this.InitializeComponent();

		base.BindingContext = aboutViewModel;
	}
}