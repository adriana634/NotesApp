using Microsoft.Extensions.Logging;
using Moq;
using NotesApp.DatabaseContext;
using NotesApp.Models;
using NotesApp.Repositories;
using SQLite;

namespace NotesApp.Test.Persistence
{
    public class NoteRepositoryFixture
    {
        public NoteRepository GetRepository(SQLiteAsyncConnection dbConnection)
        {
            var dbContextMock = new Mock<IDbContext>();
            dbContextMock.Setup(db => db.GetAsyncConnection<Note>()).ReturnsAsync(dbConnection);

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<NoteRepository>();

            return new NoteRepository(dbContextMock.Object, logger);
        }
    }
}
