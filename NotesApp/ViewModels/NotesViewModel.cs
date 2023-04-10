using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NotesViewModel : IQueryAttributable
    {
        private bool _isDataLoaded;

        public ObservableCollection<NoteViewModel> AllNotes { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectNoteCommand { get; }

        public NotesViewModel()
        {
            this.AllNotes = new ObservableCollection<NoteViewModel>();
            this.NewCommand = new AsyncRelayCommand(NewNoteAsync);
            this.SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(SelectNoteAsync);
        }

        public void LoadNotes()
        {
            if (this._isDataLoaded == true)
                return;

            this._isDataLoaded = true;

            var notes = App.NoteRepository.GetAllNotes()
                                          .Select(note => new NoteViewModel(note));

            foreach (var note in notes)
            {
                this.AllNotes.Add(note);
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
                await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}?load={note.Identifier}");
            }
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("saved"))
            {
                var noteId = int.Parse(query["saved"].ToString());
                var matchedNote = this.AllNotes.Where(note => note.Identifier == noteId).FirstOrDefault();

                // If note is found, update it
                if (matchedNote != null)
                {
                    matchedNote.Reload();
                    this.AllNotes.Move(this.AllNotes.IndexOf(matchedNote), 0);
                }
                else
                {
                    var dbNote = App.NoteRepository.GetNoteById(noteId);
                    this.AllNotes.Insert(0, new NoteViewModel(dbNote));
                }
            }
            else if (query.ContainsKey("deleted"))
            {
                var noteId = int.Parse(query["deleted"].ToString());
                var matchedNote = this.AllNotes.Where(note => note.Identifier == noteId).FirstOrDefault();

                // If note is found, update it
                this.AllNotes?.Remove(matchedNote);
            }
        }
    }
}
