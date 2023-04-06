using NotesApp.Models;

namespace NotesApp.Views;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public partial class NotePage : ContentPage
{
    public string ItemId
    {
        set
        {
            LoadNote(value);
        }
    }

    public NotePage()
	{
		InitializeComponent();

        string appDataPath = FileSystem.AppDataDirectory;
        string randomFileName = $"{Path.GetRandomFileName()}.notes.txt";

        LoadNote(Path.Combine(appDataPath, randomFileName));
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
		if (BindingContext is Note note)
		{
            // Save the file.
            File.WriteAllText(note.Filename, TextEditor.Text);
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
		if (BindingContext is Note note)
		{
            // Delete the file.
            if (File.Exists(note.Filename))
            {
                File.Delete(note.Filename);
            }
        }

		await Shell.Current.GoToAsync("..");
    }

    private void LoadNote(string filePath)
    {
        Note note = new()
        {
            Filename = filePath
        };

        if (File.Exists(filePath))
        {
            note.Date = File.GetCreationTime(filePath);
            note.Text = File.ReadAllText(filePath);
        }

        BindingContext = note;
    }
}