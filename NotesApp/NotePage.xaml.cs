namespace NotesApp;

public partial class NotePage : ContentPage
{
	private readonly string notesFilePath = Path.Combine(FileSystem.AppDataDirectory, "notes.txt");

	public NotePage()
	{
		InitializeComponent();

		if (File.Exists(notesFilePath))
		{
			TextEditor.Text = File.ReadAllText(notesFilePath);
		}
	}

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
		// Save the file.
		File.WriteAllText(notesFilePath, TextEditor.Text);
    }

    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
		// Delete the file.
		if (File.Exists(notesFilePath))
		{
			File.Delete(notesFilePath);
		}

		TextEditor.Text = string.Empty;
    }
}