using SQLite;

namespace NotesApp.DatabaseContext
{
    public interface IDbContext
    {
        Task<SQLiteAsyncConnection> GetAsyncConnection<T>() where T : new();
    }
}