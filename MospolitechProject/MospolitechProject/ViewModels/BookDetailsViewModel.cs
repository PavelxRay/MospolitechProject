using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MospolitechProject.Models;
using MospolitechProject.Services;
using Xamarin.Forms;

namespace MospolitechProject.ViewModels
{
    public class BookDetailsViewModel : BindableObject
    {
        private readonly DatabaseService _dbService = new DatabaseService();
        private List<Chapter> _allChapters = new List<Chapter>();
        private string _searchText = "";
        private string _selectedFilter = "Все";

        public Book CurrentBook { get; set; }
        public ObservableCollection<Chapter> FilteredChapters { get; set; } = new ObservableCollection<Chapter>();

        // Цвета для табов (подсветка активного)
        public Color AllTabColor => _selectedFilter == "Все" ? Color.FromHex("#6200EE") : Color.Transparent;
        public Color ReadTabColor => _selectedFilter == "Прочитано" ? Color.FromHex("#6200EE") : Color.Transparent;
        public Color NotReadTabColor => _selectedFilter == "Непрочитано" ? Color.FromHex("#6200EE") : Color.Transparent;

        // Расчет времени чтения (примерно 200 слов в минуту)
        public string EstimatedTime { get; set; } = "-- мин";

        public async Task LoadData(int bookId)
        {
            await _dbService.Init();
            var books = await _dbService.GetBooks();
            CurrentBook = books.FirstOrDefault(b => b.Id == bookId);

            _allChapters = await _dbService.GetChaptersByBook(bookId);

            CalculateReadingTime();
            ApplyFilters();
            OnPropertyChanged(null); // Обновить весь UI
        }

        private void CalculateReadingTime()
        {
            // Считаем общее кол-во слов во всех главах
            int totalWords = _allChapters.Sum(c => c.Text.Split(' ').Length);
            int minutes = totalWords / 200; // средняя скорость чтения
            EstimatedTime = $"{minutes} мин";
        }

        public void ApplyFilters(string filter = null, string search = null)
        {
            if (filter != null) _selectedFilter = filter;
            if (search != null) _searchText = search.ToLower();

            var result = _allChapters.AsEnumerable();

            // Фильтр по статусу
            if (_selectedFilter == "Прочитано") result = result.Where(c => c.IsCompleted);
            else if (_selectedFilter == "Непрочитано") result = result.Where(c => !c.IsCompleted);

            // Поиск по названию
            if (!string.IsNullOrWhiteSpace(_searchText))
                result = result.Where(c => c.Title.ToLower().Contains(_searchText));

            FilteredChapters.Clear();
            foreach (var ch in result) FilteredChapters.Add(ch);

            OnPropertyChanged(nameof(AllTabColor));
            OnPropertyChanged(nameof(ReadTabColor));
            OnPropertyChanged(nameof(NotReadTabColor));
        }

        public async Task ToggleReadStatus(Chapter chapter)
        {
            chapter.IsCompleted = !chapter.IsCompleted;
            await _dbService.UpdateChapter(chapter);
            ApplyFilters(); // Обновляем список
        }
    }
}
