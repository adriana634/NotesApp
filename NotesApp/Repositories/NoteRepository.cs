using NotesApp.Models;
using SQLite;

namespace NotesApp.Repositories
{
    public class NoteRepository
    {
        private readonly string _dbPath;

        private SQLiteConnection _connection;

        public NoteRepository(string dbPath)
        {
            this._dbPath = dbPath;
        }

        private void Init()
        {
            if (this._connection != null)
                return;

            this._connection = new SQLiteConnection(this._dbPath);
            this._connection.CreateTable<Note>();
        }

        public List<Note> GetAllNotes()
        {
            this.Init();

            return this._connection.Table<Note>().ToList();
        }

        public Note GetNoteById(int id)
        {
            this.Init();

            return this._connection.Get<Note>(id);
        }

        public void AddNote(Note note)
        {
            this.Init();

            this._connection.Insert(note);
        }

        public void UpdateNote(Note note)
        {
            this.Init();

            this._connection.Update(note);
        }

        public void DeleteNote(Note note)
        {
            this.Init();

            this._connection.Delete(note);
        }
    }
}
