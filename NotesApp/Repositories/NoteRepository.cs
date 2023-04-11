using NotesApp.Models;
using SQLite;

namespace NotesApp.Repositories
{
    public class NoteRepository
    {
        private readonly string _dbPath;

        private SQLiteAsyncConnection _connection;

        public NoteRepository(string dbPath)
        {
            this._dbPath = dbPath;
        }

        private async Task InitAsync()
        {
            if (this._connection != null)
                return;

            this._connection = new SQLiteAsyncConnection(this._dbPath);

            await this._connection.CreateTableAsync<Note>();
        }

        public async Task<List<Note>> GetAllNotesAsync()
        {
            await this.InitAsync();

            return await this._connection.Table<Note>().ToListAsync();
        }

        public async Task<Note> GetNoteByIdAsync(int id)
        {
            await this.InitAsync();

            return await this._connection.GetAsync<Note>(id);
        }

        public async Task AddNoteAsync(Note note)
        {
            await this.InitAsync();

            await this._connection.InsertAsync(note);
        }

        public async Task UpdateNoteAsync(Note note)
        {
            await this.InitAsync();

            await this._connection.UpdateAsync(note);
        }

        public async Task DeleteNoteAsync(Note note)
        {
            await this.InitAsync();

            await this._connection.DeleteAsync(note);
        }
    }
}
