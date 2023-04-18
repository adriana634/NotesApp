using NotesApp.Models;

namespace NotesApp.ViewModels
{
    public interface IUpdateNoteViewModelFactory
    {
        UpdateNoteViewModel Create(Note note);
    }
}