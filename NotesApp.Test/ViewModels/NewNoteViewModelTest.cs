using CommunityToolkit.Mvvm.Messaging;
using CSharpFunctionalExtensions;
using Moq;
using NotesApp.Messages;
using NotesApp.Models;
using NotesApp.Repositories;
using NotesApp.Services;
using NotesApp.ViewModels;

namespace NotesApp.Test.ViewModels
{
    public class NewNoteViewModelTest
    {
        [Fact]
        public async Task SaveCommand_ShouldSendNoteCreatedMessageWhenCreatedSuccessfuly()
        {
            // Arrange
            var noteRepositoryMock = new Mock<INoteRepository>();
            
            noteRepositoryMock.Setup(noteRepository => noteRepository.AddNoteAsync(It.IsAny<Note>()))
                              .ReturnsAsync(Result.Success);

            var noteStub = new Note
            {
                Id = 1,
                Date = DateTime.Now,
                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
            };

            noteRepositoryMock.Setup(noteRepository => noteRepository.GetNoteByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(Result.Success(noteStub));

            var navigationServiceMock = new Mock<INavigationService>();
            var messengerMock = new Mock<IMessenger>();

            var viewModel = new NewNoteViewModel(noteRepositoryMock.Object,
                                                 navigationServiceMock.Object,
                                                 messengerMock.Object);

            // Act
            await viewModel.SaveCommand.ExecuteAsync(null);

            // Assert
            messengerMock.Verify(messenger => messenger.Send(It.Is<NoteCreatedMessage>(message => message != null),
                                                             It.IsAny<string>()),
                                                             Times.Once());
            navigationServiceMock.Verify(navigation => navigation.PopAsync(), Times.Once());
        }

        [Fact]
        public async Task SaveCommand_ShouldSendErrorMessageWhenAddNoteFails()
        {
            // Arrange
            var noteRepositoryMock = new Mock<INoteRepository>();

            var errorMessageStub = "Failed to add note";
            noteRepositoryMock.Setup(noteRepository => noteRepository.AddNoteAsync(It.IsAny<Note>()))
                              .ReturnsAsync(Result.Failure(errorMessageStub));

            var navigationServiceMock = new Mock<INavigationService>();
            var messengerMock = new Mock<IMessenger>();

            var viewModel = new NewNoteViewModel(noteRepositoryMock.Object,
                                                 navigationServiceMock.Object,
                                                 messengerMock.Object);

            // Act
            await viewModel.SaveCommand.ExecuteAsync(null);

            // Assert
            messengerMock.Verify(messenger => messenger.Send(It.Is<ErrorMessage>(message => message != null),
                                                             It.IsAny<string>()),
                                                             Times.Once());
            navigationServiceMock.Verify(navigation => navigation.PopAsync(), Times.Once());
        }

        [Fact]
        public async Task SaveCommand_ShouldSendErrorMessageWhenGetNoteFails()
        {
            // Arrange
            var noteRepositoryMock = new Mock<INoteRepository>();

            var errorMessageStub = "Failed to get note by id";
            noteRepositoryMock.Setup(noteRepository => noteRepository.GetNoteByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync(Result.Failure<Note>(errorMessageStub));

            var navigationServiceMock = new Mock<INavigationService>();
            var messengerMock = new Mock<IMessenger>();

            var viewModel = new NewNoteViewModel(noteRepositoryMock.Object,
                                                 navigationServiceMock.Object,
                                                 messengerMock.Object);

            // Act
            await viewModel.SaveCommand.ExecuteAsync(null);

            // Assert
            messengerMock.Verify(messenger => messenger.Send(It.Is<ErrorMessage>(message => message != null),
                                                             It.IsAny<string>()),
                                                             Times.Once());
            navigationServiceMock.Verify(navigation => navigation.PopAsync(), Times.Once());
        }
    }
}
