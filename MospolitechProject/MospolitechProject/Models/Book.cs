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
        
        public bool IsReading { get; set; }   // "Читаю сейчас"
        public bool IsFinished { get; set; }  // "Прочитано"
        public bool IsFavorite { get; set; }
        
        public string FilePath { get; set; }

        public int TotalChapters { get; set; } // Общее кол-во глав в книге

        [Ignore] // Этот параметр не идет в БД, он только для UI
        public double ProgressPercent => TotalChapters > 0 ? (double)Progress / (TotalChapters - 1) : 0;

        [Ignore]
        public string ProgressText => TotalChapters > 0 ? $"{(int)(ProgressPercent * 100)}%" : "0%";

    }
}
