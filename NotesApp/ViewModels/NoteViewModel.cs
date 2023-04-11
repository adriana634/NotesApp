using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NotesApp.Messages;
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
                await App.NoteRepository.AddNoteAsync(this._note);

                var newNote = await App.NoteRepository.GetNoteByIdAsync(this._note.Id);
                WeakReferenceMessenger.Default.Send(new NoteCreatedMessage(newNote));
            }
            else
            {
                await App.NoteRepository.UpdateNoteAsync(this._note);

                var updatedNote = await App.NoteRepository.GetNoteByIdAsync(this._note.Id);
                WeakReferenceMessenger.Default.Send(new NoteSavedMessage(updatedNote));
            }

            await Shell.Current.GoToAsync("..");
        }

        private async Task Delete()
        {
            await App.NoteRepository.DeleteNoteAsync(this._note);

            WeakReferenceMessenger.Default.Send(new NoteDeletedMessage(this._note.Id));

            await Shell.Current.GoToAsync("..");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("Note"))
            {
                this._isNewNote = false;

                var noteModel = query["Note"] as Models.Note;
                this._note = noteModel;
                this.Refresh();
            }
        }

        public void Refresh()
        {
            this.OnPropertyChanged(nameof(this.Text));
            this.OnPropertyChanged(nameof(this.Date));
        }
    }
}
