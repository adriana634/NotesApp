using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NotesApp.Messages
{
    public class NoteDeletedMessage : ValueChangedMessage<int>
    {
        public NoteDeletedMessage(int noteId) : base(noteId)
        {
        }
    }
}
