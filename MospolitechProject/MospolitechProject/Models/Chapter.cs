using SQLite;

namespace MospolitechProject.Models
{
    public class Chapter
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int BookId { get; set; } // Привязка к книге
        public int Index { get; set; }  // Номер главы
        public string Title { get; set; }
        public string Text { get; set; } // Текст главы
    }
}
