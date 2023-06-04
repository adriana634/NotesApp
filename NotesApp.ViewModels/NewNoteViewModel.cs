using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CSharpFunctionalExtensions;
using NotesApp.Messages;
using NotesApp.Models;
using NotesApp.Repositories;
using NotesApp.Services;

namespace NotesApp.ViewModels
{
    public class NewNoteViewModel
    {
        private readonly INoteRepository _noteRepository;
        private readonly INavigationService _navigationService;
        private readonly IMessenger _messenger;

        private readonly Note _note;

        public string Text
        {
            get => this._note.Text;

            set
            {
                if (this._note.Text != value)
                {
                    this._note.Text = value;
                }
            }
        }

        public AsyncRelayCommand SaveCommand { get; private set; }

        public NewNoteViewModel(INoteRepository noteRepository, INavigationService navigationService, IMessenger messenger)
        {
            this._noteRepository = noteRepository;
            this._navigationService = navigationService;
            this._messenger = messenger;

            this._note = new Note();

            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
        }

        private async Task SaveAsync()
        {
            this._note.Date = DateTime.Now;

            var addNoteResult = await this._noteRepository.AddNoteAsync(this._note);

            if (addNoteResult.IsSuccess)
            {
                var getNoteResult = await this._noteRepository.GetNoteByIdAsync(this._note.Id);

                getNoteResult
                    .Tap(newNote => this._messenger.Send(new NoteCreatedMessage(newNote), MessengerTokens.Notes))
                    .TapError(() => this._messenger.Send(new ErrorMessage("Error", "An error has ocurred while creating the note."), MessengerTokens.Errors));
            }
            else
            {
                this._messenger.Send(new ErrorMessage("Error", "An error has ocurred while creating the note."), MessengerTokens.Errors);
            }

            await this._navigationService.PopAsync();
        }
    }
}
