using SQLite;
using System.Diagnostics;

namespace NotesApp.DatabaseContext
{
    public class DbContext : IDbContext
    {
        private readonly Lazy<SQLiteAsyncConnection> _lazyConnection;

        public DbContext(string dbPath)
        {
            Debug.Assert(string.IsNullOrEmpty(dbPath) == false);
            this._lazyConnection = new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(dbPath));
        }

        public async Task<SQLiteAsyncConnection> GetAsyncConnection<T>() where T : new()
        {
            var connection = this._lazyConnection.Value;

            if (connection.TableMappings.Any(tableMapping => tableMapping.MappedType == typeof(T)) == false)
            {
                await connection.CreateTableAsync<T>();
            }

            return connection;
        }
    }
}
