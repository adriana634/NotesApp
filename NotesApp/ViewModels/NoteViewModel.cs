using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NoteViewModel : ObservableObject, IQueryAttributable
    {
        private bool _isNewNote;
        private Models.Note _note;

        public int Identifier => this._note.Id;

        public DateTime Date => this._note.Date;

        public string Text
        {
            get => this._note.Text;

            set
            {
                if (this._note.Text != value)
                {
                    this._note.Text = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public NoteViewModel()
        {
            this._isNewNote = true;
            this._note = new Models.Note();
            this.SaveCommand = new AsyncRelayCommand(this.Save);
            this.DeleteCommand = new AsyncRelayCommand(this.Delete);
        }

        public NoteViewModel(Models.Note note)
        {
            this._isNewNote = false;
            this._note = note;
            this.SaveCommand = new AsyncRelayCommand(this.Save);
            this.DeleteCommand = new AsyncRelayCommand(this.Delete);
        }

        private async Task Save()
        {
            this._note.Date = DateTime.Now;

            if (this._isNewNote == true)
            {
                App.NoteRepository.AddNote(this._note);
            }
            else
            {
                App.NoteRepository.UpdateNote(this._note);
            }
            
            await Shell.Current.GoToAsync($"..?saved={this._note.Id}");
        }

        private async Task Delete()
        {
            App.NoteRepository.DeleteNote(this._note);
            await Shell.Current.GoToAsync($"..?deleted={this._note.Id}");
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                this._isNewNote = false;

                var noteId = int.Parse(query["load"].ToString());
                this._note = App.NoteRepository.GetNoteById(noteId);
                this.RefreshProperties();
            }
        }

        public void Reload()
        {
            this._note = App.NoteRepository.GetNoteById(this._note.Id);
            this.RefreshProperties();
        }

        private void RefreshProperties()
        {
            this.OnPropertyChanged(nameof(this.Text));
            this.OnPropertyChanged(nameof(this.Date));
        }
    }
}
