using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CSharpFunctionalExtensions;
using NotesApp.Messages;
using NotesApp.Models;
using NotesApp.Repositories;
using NotesApp.Services;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NewNoteViewModel : ObservableObject
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

        public NewNoteViewModel(INoteRepository noteRepository, INavigationService navigationService)
        {
            this._note = new Note();
            this._noteRepository = noteRepository;
            this._navigationService = navigationService;
            
            this.SaveCommand = new AsyncRelayCommand(this.Save);
        }

        private async Task Save()
        {
            this._note.Date = DateTime.Now;

            var addNoteResult = await this._noteRepository.AddNoteAsync(this._note);

            if (addNoteResult.IsSuccess)
            {
                var getNoteResult = await this._noteRepository.GetNoteByIdAsync(this._note.Id);

                getNoteResult
                    .Tap(newNote => WeakReferenceMessenger.Default.Send(new NoteCreatedMessage(newNote)))
                    .TapError(() => WeakReferenceMessenger.Default.Send(new ErrorMessage("Error", "An error has ocurred while creating the note.")));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new ErrorMessage("Error", "An error has ocurred while creating the note."));
            }

            await this._navigationService.PopAsync();
        }
    }
}
