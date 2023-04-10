using SQLite;

namespace NotesApp.Models
{
    [Table("Notes")]
    public class Note
    {
        [PrimaryKey, AutoIncrement] 
        public int Id { get; set; }

        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
