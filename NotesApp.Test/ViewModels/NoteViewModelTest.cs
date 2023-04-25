using FluentAssertions;
using Moq;
using NotesApp.Models;
using NotesApp.ViewModels;

namespace NotesApp.Test.ViewModels
{
    public class NoteViewModelTest
    {
        [Fact]
        public void Refresh_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            var noteMock = new Mock<Note>();
            var viewModel = new NoteViewModel(noteMock.Object);

            var textPropertyChangedRaised = false;
            var datePropertyChangedRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(NoteViewModel.Text))
                {
                    textPropertyChangedRaised = true;
                } 
                else if (args.PropertyName == nameof(NoteViewModel.Date))
                {
                    datePropertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Refresh();

            // Assert
            textPropertyChangedRaised.Should().BeTrue();
            datePropertyChangedRaised.Should().BeTrue();
        }
    }
}
