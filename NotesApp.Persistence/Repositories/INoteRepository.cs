using CSharpFunctionalExtensions;
using NotesApp.Models;

namespace NotesApp.Repositories
{
    public interface INoteRepository
    {
        Task<Result<List<Note>>> GetAllNotesAsync();
        Task<Result<Note>> GetNoteByIdAsync(int id);
        Task<Result> AddNoteAsync(Note note);
        Task<Result> UpdateNoteAsync(Note note);
        Task<Result> DeleteNoteAsync(Note note);   
    }
}