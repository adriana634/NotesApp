using NotesApp.Models;

namespace NotesApp.Services
{
    public interface INoteSelectionService
    {
        Note SelectedNote { get; set; }
    }
}