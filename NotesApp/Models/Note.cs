namespace NotesApp.Models
{
    public class Note
    {
        public string Filename { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public Note()
        {
            Filename = $"{Path.GetRandomFileName()}.notes.txt";
            Date = DateTime.Now;
            Text = "";
        }

        public void Save()
        {
            File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, Filename), Text);
        }

        public void Delete()
        {
            File.Delete(Path.Combine(FileSystem.AppDataDirectory, Filename));
        }

        public static Note Load(string filename)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, filename);

            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException("Unable to find the file.", filePath);
            }

            return
                new Note()
                {
                    Filename = Path.GetFileName(filePath),
                    Text = File.ReadAllText(filePath),
                    Date = File.GetLastWriteTime(filePath)
                };
        }

        public static IEnumerable<Note> LoadAll()
        {
            // Get the folder where the notes are stored
            string appDataPath = FileSystem.AppDataDirectory;

            // Use LINQ extensions to load the *.notes.txt files.
            return Directory

                // Select the file names from the directory
                .EnumerateFiles(appDataPath, "*.notes.txt")

                // Each file name is used to create a new Note
                .Select(filename => Note.Load(Path.GetFileName(filename)))

                // With the final collection of notes, order them by date
                .OrderByDescending(note => note.Date);
        }
    }
}
