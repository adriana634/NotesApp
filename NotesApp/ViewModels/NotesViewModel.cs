using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NotesViewModel : IQueryAttributable
    {
        public ObservableCollection<NoteViewModel> AllNotes { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectNoteCommand { get; }

        public NotesViewModel()
        {
            var notes = Models.Note.LoadAll()
                                   .Select(note => new NoteViewModel(note));

            this.AllNotes = new ObservableCollection<NoteViewModel>(notes);
            this.NewCommand = new AsyncRelayCommand(NewNoteAsync);
            this.SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(SelectNoteAsync);
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
                var noteId = query["saved"].ToString();
                var matchedNote = this.AllNotes.Where(note => note.Identifier == noteId).FirstOrDefault();

                // If note is found, update it
                if (matchedNote != null)
                {
                    matchedNote.Reload();
                    this.AllNotes.Move(this.AllNotes.IndexOf(matchedNote), 0);
                }
                else
                {
                    this.AllNotes.Insert(0, new NoteViewModel(Models.Note.Load(noteId)));
                }
            }
            else if (query.ContainsKey("deleted"))
            {
                var noteId = query["deleted"].ToString();
                var matchedNote = this.AllNotes.Where(note => note.Identifier == noteId).FirstOrDefault();

                // If note is found, update it
                this.AllNotes?.Remove(matchedNote);
            }
        }
    }
}
