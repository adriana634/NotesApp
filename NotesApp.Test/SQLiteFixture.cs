using Microsoft.Extensions.Logging;
using NotesApp.DatabaseContext;
using SQLite;

namespace NotesApp.Test
{
    public class SQLiteFixture : IDisposable
    {
        private readonly string _dbPath;
        private readonly DbContext _dbContext;
        private readonly List<SQLiteAsyncConnection> _activeConnections;
        private readonly ILogger _logger;

        public SQLiteFixture()
        {
            this._dbPath = Path.Combine(Path.GetTempPath(), $"notes_test_{Guid.NewGuid()}.db3");
            this._dbContext = new DbContext(this._dbPath);
            this._activeConnections = new List<SQLiteAsyncConnection>();
            this._logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SQLiteFixture>();
        }

        public async Task<SQLiteAsyncConnection> GetConnection<T>() where T : new()
        {
            var connection = await this._dbContext.GetConnection<T>();
            this._activeConnections.Add(connection);

            return connection;
        }

        public void Dispose()
        {
            foreach (var connection in this._activeConnections)
            {
                connection.CloseAsync().Wait();
            }

            this._activeConnections.Clear();

            if (File.Exists(this._dbPath))
            {
                File.Delete(this._dbPath);
                this._logger.LogInformation("Database file {dbPath} deleted.", this._dbPath);
            }
        }
    }
}
