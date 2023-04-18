using SQLite;

namespace NotesApp.DatabaseContext
{
    public interface IDbContext
    {
        Task<SQLiteAsyncConnection> GetConnection<T>() where T : new();
    }
}