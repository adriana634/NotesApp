using NotesApp.Models;

namespace NotesApp.Services
{
    public class NoteSelectionService : INoteSelectionService
    {
        private Note? _selectedNote;

        public Note SelectedNote
        {
            get
            {
                if (this._selectedNote == null)
                {
                    throw new InvalidOperationException("Selected note is null");
                }

                return this._selectedNote;
            }
            set
            {
                this._selectedNote = value;
            }
        }
    }
}
