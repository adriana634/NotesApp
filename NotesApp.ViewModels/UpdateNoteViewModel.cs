using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CSharpFunctionalExtensions;
using NotesApp.Messages;
using NotesApp.Models;
using NotesApp.Repositories;
using NotesApp.Services;
using System.Diagnostics;

namespace NotesApp.ViewModels
{
    public class UpdateNoteViewModel : ObservableObject
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
        public AsyncRelayCommand DeleteCommand { get; private set; }

        public UpdateNoteViewModel(Note note, INoteRepository noteRepository, INavigationService navigationService, IMessenger messenger)
        {
            Debug.Assert(note != null);

            this._note = note;
            this._noteRepository = noteRepository;
            this._navigationService = navigationService;
            this._messenger = messenger;

            this.SaveCommand = new AsyncRelayCommand(this.Save);
            this.DeleteCommand = new AsyncRelayCommand(this.Delete);
        }

        private async Task Save()
        {
            this._note.Date = DateTime.Now;

            var updateNoteResult = await this._noteRepository.UpdateNoteAsync(this._note);

            if (updateNoteResult.IsSuccess)
            {
                var getNoteResult = await this._noteRepository.GetNoteByIdAsync(this._note.Id);

                getNoteResult
                    .Tap(updatedNote => this._messenger.Send(new NoteUpdatedMessage(updatedNote), MessengerTokens.Notes))
                    .TapError(() => this._messenger.Send(new ErrorMessage("Error", "An error has ocurred while saving the note."), MessengerTokens.Errors));
            }
            else
            {
                this._messenger.Send(new ErrorMessage("Error", "An error has ocurred while saving the note."), MessengerTokens.Errors);
            }

            await this._navigationService.PopAsync();
        }

        private async Task Delete()
        {
            var deleteNoteResult = await this._noteRepository.DeleteNoteAsync(this._note);

            deleteNoteResult
                .Tap(() => this._messenger.Send(new NoteDeletedMessage(this._note.Id), MessengerTokens.Notes))
                .TapError(() => this._messenger.Send(new ErrorMessage("Error", "An error has ocurred while deleting the note."), MessengerTokens.Errors));

            await this._navigationService.PopAsync();
        }
    }
}
