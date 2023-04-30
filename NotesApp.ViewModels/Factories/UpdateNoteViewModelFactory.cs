using CommunityToolkit.Mvvm.Messaging;
using NotesApp.Models;
using NotesApp.Repositories;
using NotesApp.Services;

namespace NotesApp.ViewModels
{
    public class UpdateNoteViewModelFactory : IUpdateNoteViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateNoteViewModelFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public UpdateNoteViewModel Create(Note note)
        {
            var noteRepository = this._serviceProvider.GetService(typeof(INoteRepository)) as INoteRepository;

            if (noteRepository == null)
            {
                throw new InvalidOperationException($"The {nameof(INoteRepository)} service could not be found");
            }

            var navigationService = this._serviceProvider.GetService(typeof(INavigationService)) as INavigationService;

            if (navigationService == null)
            {
                throw new InvalidOperationException($"The {nameof(INavigationService)} service could not be found");
            }

            var messenger = this._serviceProvider.GetService(typeof(IMessenger)) as IMessenger;

            if (messenger == null)
            {
                throw new InvalidOperationException($"The {nameof(IMessenger)} service could not be found");
            }

            return new UpdateNoteViewModel(note, noteRepository, navigationService, messenger);
        }
    }
}
