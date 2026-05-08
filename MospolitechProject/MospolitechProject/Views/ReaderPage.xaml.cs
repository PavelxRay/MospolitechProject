using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using MospolitechProject.Models;
using MospolitechProject.Services; // Нужно для DatabaseService

namespace MospolitechProject.Views
{
    public partial class ReaderPage : ContentPage
    {
        private readonly DatabaseService _dbService = new DatabaseService();
        private readonly Book _currentBook;
        private int _currentChapterIndex = 0;

        public ReaderPage(Book book)
        {
            InitializeComponent();
            _currentBook = book;
            Title = _currentBook.Title;
            
            // Начинаем с того места, где остановились
            _currentChapterIndex = _currentBook.Progress;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadBookContent();
        }

        private async Task LoadBookContent()
        {
            ReaderLoader.IsRunning = true;
            await _dbService.Init(); // Инициализируем БД
            await LoadCurrentChapter();
            ReaderLoader.IsRunning = false;
        }

        private async Task LoadCurrentChapter()
        {
            // Берем текст из БД по ID книги и индексу главы
            var chapter = await _dbService.GetChapter(_currentBook.Id, _currentChapterIndex);
            
            if (chapter != null)
            {
                ContentLabel.Text = chapter.Text;
                ChapterLabel.Text = $"Глава {_currentChapterIndex + 1}";
                
                // Скроллим в начало при смене главы
                await MainScroll.ScrollToAsync(0, 0, false);

                // Сохраняем прогресс в модель и в БД
                _currentBook.Progress = _currentChapterIndex;
                await _dbService.UpdateBook(_currentBook);
            }
            else if (_currentChapterIndex > 0)
            {
                // Если главы нет (конец книги), откатываем индекс назад
                _currentChapterIndex--;
                await DisplayAlert("Конец", "Это последняя глава", "ОК");
            }
        }

        // Логика кнопок теперь просто меняет индекс и дергает БД
        private async void OnNextClicked(object sender, EventArgs e)
        {
            _currentChapterIndex++;
            await LoadCurrentChapter();
        }

        private async void OnPrevClicked(object sender, EventArgs e)
        {
            if (_currentChapterIndex > 0)
            {
                _currentChapterIndex--;
                await LoadCurrentChapter();
            }
        }
    }
}
