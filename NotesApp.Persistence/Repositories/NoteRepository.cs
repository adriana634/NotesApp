using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using NotesApp.DatabaseContext;
using NotesApp.Models;
using SQLite;
using System.Diagnostics;

namespace NotesApp.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private IDbContext _dbContext;
        private readonly ILogger<NoteRepository> _logger;

        public NoteRepository(IDbContext dbContext, ILogger<NoteRepository> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<Result<List<Note>>> GetAllNotesAsync()
        {
            try
            {
                var connection = await this._dbContext.GetAsyncConnection<Note>();

                var notes = await connection.Table<Note>().ToListAsync();
                return Result.Success<List<Note>>(notes);
            }
            catch (SQLiteException ex)
            {
                this._logger.LogError("Failed to get all notes. Exception: {exception}", ex);
                return Result.Failure<List<Note>>($"Failed to get all notes: {ex.Message}");
            }
        }

        public async Task<Result<Note>> GetNoteByIdAsync(int id)
        {
            try
            {
                var connection = await this._dbContext.GetAsyncConnection<Note>();

                var note = await connection.GetAsync<Note>(id);
                return Result.Success<Note>(note);
            }
            catch (SQLiteException ex)
            {
                this._logger.LogError("Failed to get note by id. Exception: {exception}", ex);
                return Result.Failure<Note>($"Failed to get note by id: {ex.Message}");
            }
        }

        public async Task<Result> AddNoteAsync(Note note)
        {
            Debug.Assert(note != null);

            try
            {
                var connection = await this._dbContext.GetAsyncConnection<Note>();

                await connection.InsertAsync(note);

                return Result.Success();
            }
            catch (SQLiteException ex)
            {
                this._logger.LogError("Failed to add note. Exception: {exception}", ex);
                return Result.Failure($"Failed to add note: {ex.Message}");
            }
        }

        public async Task<Result> UpdateNoteAsync(Note note)
        {
            Debug.Assert(note != null);

            try
            {
                var connection = await this._dbContext.GetAsyncConnection<Note>();

                int rowsAffected = await connection.UpdateAsync(note);

                if (rowsAffected == 1)
                {
                    return Result.Success();
                }
                else
                {
                    this._logger.LogError("Failed to update note: no rows were affected.");
                    return Result.Failure("Failed to update note: no rows were affected.");
                }
            }
            catch (SQLiteException ex)
            {
                this._logger.LogError("Failed to add note. Exception: {exception}", ex);
                return Result.Failure($"Failed to update note: {ex.Message}");
            }
        }

        public async Task<Result> DeleteNoteAsync(Note note)
        {
            Debug.Assert(note != null);

            try
            {
                var connection = await this._dbContext.GetAsyncConnection<Note>();

                int rowsAffected = await connection.DeleteAsync(note);

                if (rowsAffected == 1)
                {
                    return Result.Success();
                }
                else
                {
                    this._logger.LogError("Failed to delete note: no rows were affected.");
                    return Result.Failure("Failed to delete note: no rows were affected.");
                }
            }
            catch (SQLiteException ex)
            {
                this._logger.LogError("Failed to delete note. Exception: {exception}", ex);
                return Result.Failure($"Failed to delete note: {ex.Message}");
            }
        }
    }
}
