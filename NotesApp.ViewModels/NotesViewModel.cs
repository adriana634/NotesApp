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

namespace NotesApp.ViewModels
{
    public class NotesViewModel : IPageLifecycleAware, IDisposable
    {
        private readonly INoteRepository _noteRepository;
        private readonly INavigationService _navigationService;
        private readonly INoteSelectionService _noteSelectionService;
        private readonly IMessenger _messenger;
        private bool _isDataLoaded;
        private Dictionary<int, Note> _loadedNotes;
        
        public ObservableCollection<NoteViewModel> AllNotes { get; }
        public AsyncRelayCommand NewNoteCommand { get; }
        public AsyncRelayCommand<NoteViewModel> SelectNoteCommand { get; }

        public NotesViewModel(INoteRepository noteRepository,
                              INavigationService navigationService,
                              INoteSelectionService noteSelectionService,
                              IMessenger messenger)
        {
            this._noteRepository = noteRepository;
            this._navigationService = navigationService;
            this._noteSelectionService = noteSelectionService;
            this._messenger = messenger;

            this._isDataLoaded = false;
            this._loadedNotes = new Dictionary<int, Note>();

            this.AllNotes = new ObservableCollection<NoteViewModel>();
            this.NewNoteCommand = new AsyncRelayCommand(this.NewNoteAsync);
            this.SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(this.SelectNoteAsync, this.CanExecuteSelectNote);
        }

        public void OnAppearing()
        {
            if (this._messenger.IsRegistered<NoteCreatedMessage, string>(this, MessengerTokens.Notes) == false)
            {
                this._messenger.Register<NoteCreatedMessage, string>(this, MessengerTokens.Notes, this.OnNoteCreated);
            }

            if (this._messenger.IsRegistered<NoteUpdatedMessage, string>(this, MessengerTokens.Notes) == false)
            {
                this._messenger.Register<NoteUpdatedMessage, string>(this, MessengerTokens.Notes, this.OnNoteUpdated);
            }

            if (this._messenger.IsRegistered<NoteDeletedMessage, string>(this, MessengerTokens.Notes) == false)
            {
                this._messenger.Register<NoteDeletedMessage, string>(this, MessengerTokens.Notes, this.OnNoteDeleted);
            }
        }

        public async Task<Result> LoadNotesAsync()
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
            this._messenger.Send(new NewNoteMessage(), MessengerTokens.Notes);
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
            this._messenger.Unregister<NoteCreatedMessage, string>(this, MessengerTokens.Notes);
            this._messenger.Unregister<NoteUpdatedMessage, string>(this, MessengerTokens.Notes);
            this._messenger.Unregister<NoteDeletedMessage, string>(this, MessengerTokens.Notes);
        }
    }
}
