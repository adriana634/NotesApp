using Bogus;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NotesApp.Models;

namespace NotesApp.Test.Persistence
{
    public class NoteRepositoryTest : IClassFixture<SQLiteFixture>, IClassFixture<NoteRepositoryFixture>
    {
        private readonly SQLiteFixture _sqliteFixture;
        private readonly NoteRepositoryFixture _noteRepositoryFixture;

        public NoteRepositoryTest(SQLiteFixture sqliteFixture, NoteRepositoryFixture noteRepository)
        {
            this._sqliteFixture = sqliteFixture;
            this._noteRepositoryFixture = noteRepository;
        }

        [Fact]
        public async Task GetAllNotesAsync_ShouldReturnAllNotes()
        {
            // Arrange
            var dbConnection = await this._sqliteFixture.GetAsyncConnection<Note>();

            var noteFaker = new Faker<Note>()
                .RuleFor(note => note.Text, rule => rule.Lorem.Text())
                .RuleFor(note => note.Date, rule => rule.Date.Recent());
            var expectedNotes = noteFaker.Generate(10);

            await dbConnection.InsertAllAsync(expectedNotes);

            var repository = this._noteRepositoryFixture.GetRepository(dbConnection);

            // Act
            var actualNotesResult = await repository.GetAllNotesAsync();

            // Assert
            actualNotesResult.Should().BeOfType<Result<List<Note>>>();
            actualNotesResult.IsSuccess.Should().BeTrue();

            actualNotesResult.Value.Should().BeOfType<List<Note>>();
            actualNotesResult.Value.Should().BeEquivalentTo(expectedNotes);
        }

        [Fact]
        public async Task GetNoteByIdAsync_ShouldReturnCorrectNote()
        {
            // Arrange
            var dbConnection = await this._sqliteFixture.GetAsyncConnection<Note>();

            var noteFaker = new Faker<Note>()
                .RuleFor(note => note.Text, rule => rule.Lorem.Text())
                .RuleFor(note => note.Date, rule => rule.Date.Recent());
            var expectedNote = noteFaker.Generate();

            await dbConnection.InsertAsync(expectedNote);

            var repository = this._noteRepositoryFixture.GetRepository(dbConnection);

            // Act
            var actualNoteResult = await repository.GetNoteByIdAsync(expectedNote.Id);

            // Assert
            actualNoteResult.Should().BeOfType<Result<Note>>();
            actualNoteResult.IsSuccess.Should().BeTrue();

            actualNoteResult.Value.Should().BeOfType<Note>();
            actualNoteResult.Value.Should().BeEquivalentTo(expectedNote);
        }

        [Fact]
        public async Task AddNoteAsync_ShouldAddNewNote()
        {
            // Arrange
            var dbConnection = await this._sqliteFixture.GetAsyncConnection<Note>();

            var noteFaker = new Faker<Note>()
                .RuleFor(note => note.Text, rule => rule.Lorem.Text())
                .RuleFor(note => note.Date, rule => rule.Date.Recent());
            var expectedNote = noteFaker.Generate();

            var repository = this._noteRepositoryFixture.GetRepository(dbConnection);

            // Act
            var addNoteResult = await repository.AddNoteAsync(expectedNote);
            
            // Assert
            addNoteResult.Should().BeOfType<Result>();
            addNoteResult.IsSuccess.Should().BeTrue();

            var actualNotes = await dbConnection.Table<Note>().ToListAsync();
            actualNotes.Should().BeOfType<List<Note>>();
            actualNotes.Should().ContainSingle();
            actualNotes.Should().ContainEquivalentOf(expectedNote);
        }

        [Fact]
        public async Task UpdateNoteAsync_ShouldUpdateNoteInDatabase()
        {
            // Arrange
            var dbConnection = await this._sqliteFixture.GetAsyncConnection<Note>();

            var addNoteFaker = new Faker<Note>()
                .RuleFor(note => note.Text, rule => rule.Lorem.Text())
                .RuleFor(note => note.Date, rule => rule.Date.Recent());
            var updatedNote = addNoteFaker.Generate();

            await dbConnection.InsertAsync(updatedNote);

            var repository = this._noteRepositoryFixture.GetRepository(dbConnection);

            var updateNoteFaker = new Faker<Note>()
                .RuleFor(note => note.Text, rule => rule.Lorem.Text());
            updateNoteFaker.Populate(updatedNote);
            updatedNote.Date = DateTime.Now;

            // Act
            var updateNoteResult = await repository.UpdateNoteAsync(updatedNote);

            // Assert
            updateNoteResult.Should().BeOfType<Result>();
            updateNoteResult.IsSuccess.Should().BeTrue();

            var actualNotes = await dbConnection.Table<Note>().ToListAsync();
            actualNotes.Should().BeOfType<List<Note>>();
            actualNotes.Should().ContainSingle();
            actualNotes.Should().ContainEquivalentOf(updatedNote);
        }

        [Fact]
        public async Task DeleteNoteAsync_ShouldDeleteNoteInDatabase()
        {
            // Arrange
            var dbConnection = await this._sqliteFixture.GetAsyncConnection<Note>();

            var noteFaker = new Faker<Note>()
                .RuleFor(note => note.Text, rule => rule.Lorem.Text())
                .RuleFor(note => note.Date, rule => rule.Date.Recent());
            var expectedNote = noteFaker.Generate();

            await dbConnection.InsertAsync(expectedNote);

            var repository = this._noteRepositoryFixture.GetRepository(dbConnection);

            // Act
            var deleteNoteResult = await repository.DeleteNoteAsync(expectedNote);

            // Assert
            deleteNoteResult.Should().BeOfType<Result>();
            deleteNoteResult.IsSuccess.Should().BeTrue();

            var actualNotes = await dbConnection.Table<Note>().ToListAsync();
            actualNotes.Should().BeOfType<List<Note>>();
            actualNotes.Should().NotContainEquivalentOf(expectedNote);
            actualNotes.Should().BeEmpty();
        }
    }
}
