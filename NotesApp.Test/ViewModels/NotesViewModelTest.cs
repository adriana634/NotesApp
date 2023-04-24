using Bogus;
using CommunityToolkit.Mvvm.Messaging;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using NotesApp.Messages;
using NotesApp.Models;
using NotesApp.Navigation;
using NotesApp.Repositories;
using NotesApp.Services;
using NotesApp.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NotesApp.Test.ViewModels
{
    public class NotesViewModelTest
    {
        [Fact]
        public async Task LoadNotesAsync_ShouldLoadAllNotes()
        {
            // Arrange
            var noteFaker = new Faker<Note>()
                .RuleFor(note => note.Id, rule => rule.IndexFaker)
                .RuleFor(note => note.Text, rule => rule.Lorem.Text())
                .RuleFor(note => note.Date, rule => rule.Date.Recent());
            var expectedNotes = noteFaker.Generate(10);

            var noteRepositoryMock = new Mock<INoteRepository>();
            noteRepositoryMock.Setup(noteRepository => noteRepository.GetAllNotesAsync())
                          .ReturnsAsync(Result.Success<List<Note>>(expectedNotes));

            var navigationServiceMock = new Mock<INavigationService>();
            var noteSelectionServiceMock = new Mock<INoteSelectionService>();
            var messengerMock = new Mock<IMessenger>();

            var viewModel = new NotesViewModel(noteRepositoryMock.Object,
                                               navigationServiceMock.Object,
                                               noteSelectionServiceMock.Object,
                                               messengerMock.Object);

            // Act
            var loadNotesResult = await viewModel.LoadNotesAsync();

            // Assert
            loadNotesResult.Should().BeOfType<Result>();
            loadNotesResult.IsSuccess.Should().BeTrue();

            viewModel.AllNotes.Should().BeOfType<ObservableCollection<NoteViewModel>>();

            var expectedViewModelNotes = new ObservableCollection<NoteViewModel>
            (
                expectedNotes.Select(note => new NoteViewModel(note))
            );
            viewModel.AllNotes.Should().BeEquivalentTo(expectedViewModelNotes);
        }

        [Fact]
        public void NewNoteCommand_ShouldSendNewNoteMessageAndNavigateToNewNotePage()
        {
            // Arrange
            var noteRepositoryMock = new Mock<INoteRepository>();
            var navigationServiceMock = new Mock<INavigationService>();
            var noteSelectionServiceMock = new Mock<INoteSelectionService>();
            var messengerMock = new Mock<IMessenger>();

            var viewModel = new NotesViewModel(noteRepositoryMock.Object,
                                               navigationServiceMock.Object,
                                               noteSelectionServiceMock.Object,
                                               messengerMock.Object);

            // Act
            viewModel.NewNoteCommand.Execute(null);

            // Assert
            messengerMock.Verify(messenger => messenger.Send(It.Is<NewNoteMessage>(message => message != null),
                                                             It.IsAny<string>()), 
                                                             Times.Once());
            navigationServiceMock.Verify(navigation => navigation.NavigateToAsync(Routes.NewNotePage), Times.Once());
        }

        [Fact]
        public async Task SelectNoteCommand_ShouldChangeSelectedNoteInSelectionServiceAndNavigateToUpdateNotePage()
        {
            // Arrange
            var noteFaker = new Faker<Note>()
                .RuleFor(note => note.Id, rule => rule.IndexFaker)
                .RuleFor(note => note.Text, rule => rule.Lorem.Text())
                .RuleFor(note => note.Date, rule => rule.Date.Recent());
            var expectedNotes = noteFaker.Generate(10);

            var noteRepositoryMock = new Mock<INoteRepository>();
            noteRepositoryMock.Setup(noteRepository => noteRepository.GetAllNotesAsync())
                              .ReturnsAsync(Result.Success<List<Note>>(expectedNotes));

            var navigationServiceMock = new Mock<INavigationService>();
            var noteSelectionService = new NoteSelectionService();
            var messengerMock = new Mock<IMessenger>();

            var notesViewModel = new NotesViewModel(noteRepositoryMock.Object,
                                               navigationServiceMock.Object,
                                               noteSelectionService,
                                               messengerMock.Object);

            var loadNotesResult = await notesViewModel.LoadNotesAsync();

            loadNotesResult.Should().BeOfType<Result>();
            loadNotesResult.IsSuccess.Should().BeTrue();

            var randomIndex = Random.Shared.Next(0, expectedNotes.Count);
            Debug.Assert(randomIndex >= 0 && randomIndex < expectedNotes.Count);
            var selectedNote = expectedNotes[randomIndex];

            // Act
            var selectedNoteViewModel = new NoteViewModel(selectedNote);
            var canExecuteSelectNoteCommand = notesViewModel.SelectNoteCommand.CanExecute(selectedNoteViewModel);
            await notesViewModel.SelectNoteCommand.ExecuteAsync(selectedNoteViewModel);

            // Assert
            canExecuteSelectNoteCommand.Should().BeTrue();
            noteSelectionService.SelectedNote.Should().Be(selectedNote);
            navigationServiceMock.Verify(navigation => navigation.NavigateToAsync(Routes.UpdateNotePage), Times.Once());
        }

        [Fact]
        public void OnAppearing_ShouldRegisterAllMessages()
        {
            // Arrange
            var noteRepositoryMock = new Mock<INoteRepository>();
            var navigationServiceMock = new Mock<INavigationService>();
            var noteSelectionServiceMock = new Mock<INoteSelectionService>();
            var messengerMock = new Mock<IMessenger>();

            var viewModel = new NotesViewModel(noteRepositoryMock.Object,
                                               navigationServiceMock.Object,
                                               noteSelectionServiceMock.Object,
                                               messengerMock.Object);

            // Act
            viewModel.OnAppearing();

            // Assert
            messengerMock.Verify(messenger => messenger.Register(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<MessageHandler<object, NoteCreatedMessage>>()));
            messengerMock.Verify(messenger => messenger.Register(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<MessageHandler<object, NoteUpdatedMessage>>()));
            messengerMock.Verify(messenger => messenger.Register(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<MessageHandler<object, NoteDeletedMessage>>()));
        }

        [Fact]
        public void Dispose_ShouldUnregisterAllMessages()
        {
            // Arrange
            var noteRepositoryMock = new Mock<INoteRepository>();
            var navigationServiceMock = new Mock<INavigationService>();
            var noteSelectionServiceMock = new Mock<INoteSelectionService>();
            var messengerMock = new Mock<IMessenger>();

            var viewModel = new NotesViewModel(noteRepositoryMock.Object,
                                               navigationServiceMock.Object,
                                               noteSelectionServiceMock.Object,
                                               messengerMock.Object);

            // Act
            viewModel.Dispose();

            // Assert
            messengerMock.Verify(messenger => messenger.Unregister<NoteCreatedMessage, string>(It.IsAny<object>(), It.IsAny<string>()));
            messengerMock.Verify(messenger => messenger.Unregister<NoteUpdatedMessage, string>(It.IsAny<object>(), It.IsAny<string>()));
            messengerMock.Verify(messenger => messenger.Unregister<NoteDeletedMessage, string>(It.IsAny<object>(), It.IsAny<string>()));
        }
    }
}
