using SQLite;

namespace MospolitechProject.Models
{
    public class Book
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverUrl { get; set; }
        public string Description { get; set; }
        public int Progress { get; set; }
        
        // Вот эти поля нужны для логики экранов:
        public bool IsReading { get; set; }   // Пойдет в "Читаю сейчас"
        public bool IsFinished { get; set; }  // Пойдет в "Прочитано"
        public bool IsFavorite { get; set; }  // Для сердечка
        
        public string FilePath { get; set; }
    }
}
