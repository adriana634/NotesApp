using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CSharpFunctionalExtensions;
using NotesApp.Messages;
using NotesApp.Models;
using NotesApp.Navigation;
using NotesApp.Repositories;
using NotesApp.Services;
using NotesApp.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NotesViewModel : IPageLifecycleAware, IDisposable
    {
        private readonly INoteRepository _noteRepository;
        private readonly INavigationService _navigationService;
        private readonly INoteSelectionService _noteSelectionService;

        private bool _isDataLoaded;
        private Dictionary<int, Note> _loadedNotes;
        
        public ObservableCollection<NoteViewModel> AllNotes { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectNoteCommand { get; }

        public NotesViewModel(INoteRepository noteRepository,
                              INavigationService navigationService,
                              INoteSelectionService noteSelectionService)
        {
            this._noteRepository = noteRepository;
            this._navigationService = navigationService;
            this._noteSelectionService = noteSelectionService;

            this._isDataLoaded = false;
            this._loadedNotes = new Dictionary<int, Note>();

            this.AllNotes = new ObservableCollection<NoteViewModel>();
            this.NewCommand = new AsyncRelayCommand(this.NewNoteAsync);
            this.SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(this.SelectNoteAsync, this.CanExecuteSelectNote);
        }

        public void OnAppearing()
        {
            if (WeakReferenceMessenger.Default.IsRegistered<NoteCreatedMessage>(this) == false)
            {
                WeakReferenceMessenger.Default.Register<NoteCreatedMessage>(this, this.OnNoteCreated);
            }

            if (WeakReferenceMessenger.Default.IsRegistered<NoteUpdatedMessage>(this) == false)
            {
                WeakReferenceMessenger.Default.Register<NoteUpdatedMessage>(this, this.OnNoteUpdated);
            }

            if (WeakReferenceMessenger.Default.IsRegistered<NoteDeletedMessage>(this) == false)
            {
                WeakReferenceMessenger.Default.Register<NoteDeletedMessage>(this, this.OnNoteDeleted);
            }
        }

        public async Task<Result> LoadNotes()
        {
            if (this._isDataLoaded == true)
                return Result.Success();

            this._isDataLoaded = true;

            var notesResult = await this._noteRepository.GetAllNotesAsync();

            if (notesResult.IsSuccess)
            {
                var notes = notesResult.Value;

                foreach (var note in notes)
                {
                    this.AllNotes.Add(new NoteViewModel(note));
                    this._loadedNotes.Add(note.Id, note);
                }

                return Result.Success();
            }
            else
            {
                this._isDataLoaded = false;
                return Result.Failure("Failed to get all notes.");
            }
        }

        private async Task NewNoteAsync()
        {
            WeakReferenceMessenger.Default.Send(new NewNoteMessage());
            await this._navigationService.NavigateToAsync(Routes.NewNotePage);
        }

        private bool CanExecuteSelectNote(NoteViewModel? note)
        {
            if (note == null)
            {
                return false;
            }

            var containsKey = this._loadedNotes.ContainsKey(note.Identifier);
            if (containsKey == false)
            {
                return false;
            }

            return true;
        }

        private async Task SelectNoteAsync(NoteViewModel? note)
        {
            Debug.Assert(note != null);

            var containsKey = this._loadedNotes.ContainsKey(note.Identifier);
            Debug.Assert(containsKey);

            var noteModel = this._loadedNotes[note.Identifier];

            this._noteSelectionService.SelectedNote = noteModel;
            await this._navigationService.NavigateToAsync(Routes.UpdateNotePage);
        }

        private void OnNoteCreated(object recipient, NoteCreatedMessage message)
        {
            Debug.Assert(message != null);
            Debug.Assert(message.Value != null);

            Note newNote = message.Value;
            this.AllNotes.Insert(0, new NoteViewModel(newNote));
            this._loadedNotes.Add(newNote.Id, newNote);
        }

        private void OnNoteUpdated(object recipient, NoteUpdatedMessage message)
        {
            Debug.Assert(message != null);
            Debug.Assert(message.Value != null);

            Note updatedNote = message.Value;
            NoteViewModel? matchedNote = this.AllNotes.Where(note => note.Identifier == updatedNote.Id).FirstOrDefault();

            // If note is found, update it
            if (matchedNote != null)
            {
                this.AllNotes.Move(this.AllNotes.IndexOf(matchedNote), 0);
                matchedNote.Refresh();
            }
        }

        private void OnNoteDeleted(object recipient, NoteDeletedMessage message)
        {
            Debug.Assert(message != null);

            int deletedNoteId = message.Value;
            NoteViewModel? matchedNote = this.AllNotes.Where(note => note.Identifier == deletedNoteId).FirstOrDefault();

            // If note is found, remove it
            if (matchedNote != null)
            {
                this.AllNotes.Remove(matchedNote);
            }
        }

        public void OnDisappearing()
        {
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default.Unregister<NoteCreatedMessage>(this);
            WeakReferenceMessenger.Default.Unregister<NoteUpdatedMessage>(this);
            WeakReferenceMessenger.Default.Unregister<NoteDeletedMessage>(this);
        }
    }
}
