using CommunityToolkit.Mvvm.ComponentModel;
using NotesApp.Models;

namespace NotesApp.ViewModels
{
    public class NoteViewModel : ObservableObject
    {
        private readonly Note _note;

        public int Identifier => this._note.Id;
        public DateTime Date => this._note.Date;
        public string Text => this._note.Text;

        public NoteViewModel(Note note)
        {
            this._note = note;
        }

        public void Refresh()
        {
            this.OnPropertyChanged(nameof(this.Text));
            this.OnPropertyChanged(nameof(this.Date));
        }
    }
}
