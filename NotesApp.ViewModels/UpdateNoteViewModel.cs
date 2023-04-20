using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CSharpFunctionalExtensions;
using NotesApp.Messages;
using NotesApp.Models;
using NotesApp.Repositories;
using NotesApp.Services;
using System.Diagnostics;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class UpdateNoteViewModel : ObservableObject
    {
        private readonly INoteRepository _noteRepository;
        private readonly INavigationService _navigationService;

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

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public UpdateNoteViewModel(Note note, INoteRepository noteRepository, INavigationService navigationService)
        {
            Debug.Assert(note != null);

            this._note = note;
            this._noteRepository = noteRepository;
            this._navigationService = navigationService;

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
                    .Tap(updatedNote => WeakReferenceMessenger.Default.Send(new NoteUpdatedMessage(updatedNote)))
                    .TapError(() => WeakReferenceMessenger.Default.Send(new ErrorMessage("Error", "An error has ocurred while saving the note.")));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new ErrorMessage("Error", "An error has ocurred while saving the note."));
            }

            await this._navigationService.PopAsync();
        }

        private async Task Delete()
        {
            var deleteNoteResult = await this._noteRepository.DeleteNoteAsync(this._note);

            deleteNoteResult
                .Tap(() => WeakReferenceMessenger.Default.Send(new NoteDeletedMessage(this._note.Id)))
                .TapError(() => WeakReferenceMessenger.Default.Send(new NoteDeletedMessage(this._note.Id)));

            await this._navigationService.PopAsync();
        }
    }
}
