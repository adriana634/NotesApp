using Microsoft.Extensions.Logging;
using SQLite;

namespace NotesApp.Test.Persistence
{
    public class SQLiteFixture : IDisposable
    {
        private readonly List<string> _dbFilePaths;
        private readonly List<SQLiteAsyncConnection> _activeConnections;
        private readonly ILogger<SQLiteFixture> _logger;

        public SQLiteFixture()
        {
            this._dbFilePaths = new List<string>();
            this._activeConnections = new List<SQLiteAsyncConnection>();
            this._logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SQLiteFixture>();
        }

        public async Task<SQLiteAsyncConnection> GetAsyncConnection<T>() where T : new()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"notes_test_{Guid.NewGuid()}.db3");
            this._dbFilePaths.Add(dbPath);

            var connection = new SQLiteAsyncConnection(dbPath);
            this._activeConnections.Add(connection);

            await connection.CreateTableAsync<T>();

            return connection;
        }

        public void Dispose()
        {
            foreach (var connection in this._activeConnections)
            {
                connection.CloseAsync().Wait();
            }

            this._activeConnections.Clear();

            foreach (var dbPath in this._dbFilePaths)
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    this._logger.LogInformation("Database file {dbPath} deleted.", dbPath);
                }
            }

            this._dbFilePaths.Clear();
        }
    }
}
