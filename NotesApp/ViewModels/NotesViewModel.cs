using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NotesApp.Messages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NotesViewModel
    {
        private bool _isDataLoaded;
        private Dictionary<int, Models.Note> _loadedNotes;

        public ObservableCollection<NoteViewModel> AllNotes { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectNoteCommand { get; }

        public NotesViewModel()
        {
            this.AllNotes = new ObservableCollection<NoteViewModel>();
            this.NewCommand = new AsyncRelayCommand(this.NewNoteAsync);
            this.SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(this.SelectNoteAsync);

            WeakReferenceMessenger.Default.Register<NoteCreatedMessage>(this, OnNoteCreated);
            WeakReferenceMessenger.Default.Register<NoteSavedMessage>(this, OnNoteSaved);
            WeakReferenceMessenger.Default.Register<NoteDeletedMessage>(this, OnNoteDeleted);
        }

        public async Task LoadNotes()
        {
            if (this._isDataLoaded == true)
                return;

            this._isDataLoaded = true;

            var notes = await App.NoteRepository.GetAllNotesAsync();

            this._loadedNotes = new Dictionary<int, Models.Note>(notes.Count);

            foreach (var note in notes)
            {
                this.AllNotes.Add(new NoteViewModel(note));
                this._loadedNotes.Add(note.Id, note);
            }
        }

        private async Task NewNoteAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.NotePage));
        }

        private async Task SelectNoteAsync(NoteViewModel note)
        {
            if (note != null)
            {
                var noteModel = this._loadedNotes[note.Identifier];

                var navigationParameter = new Dictionary<string, object>
                {
                    { "Note", noteModel }
                };
                await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}", navigationParameter);
            }
        }

        private void OnNoteCreated(object recipient, NoteCreatedMessage message)
        {
            Models.Note newNote = message.Value;
            this.AllNotes.Insert(0, new NoteViewModel(newNote));
        }

        private void OnNoteSaved(object recipient, NoteSavedMessage message)
        {
            Models.Note updatedNote = message.Value;
            NoteViewModel matchedNote = this.AllNotes.Where(note => note.Identifier == updatedNote.Id).FirstOrDefault();

            // If note is found, update it
            if (matchedNote != null)
            {
                this.AllNotes.Move(this.AllNotes.IndexOf(matchedNote), 0);
                matchedNote.Refresh();
            }
        }

        private void OnNoteDeleted(object recipient, NoteDeletedMessage message)
        {
            int deletedNoteId = message.Value;
            NoteViewModel matchedNote = this.AllNotes.Where(note => note.Identifier == deletedNoteId).FirstOrDefault();

            // If note is found, update it
            if (matchedNote != null)
            {
                this.AllNotes.Remove(matchedNote);
            }
        }
    }
}
