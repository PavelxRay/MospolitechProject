using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using MospolitechProject.Models;
using MospolitechProject.Services;

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
            await _dbService.Init();
            await LoadCurrentChapter();
            ReaderLoader.IsRunning = false;
        }

        private async Task LoadCurrentChapter()
        {
            var chapter = await _dbService.GetChapter(_currentBook.Id, _currentChapterIndex);

            if (chapter != null)
            {
                ContentLabel.Text = chapter.Text;
                ChapterLabel.Text = !string.IsNullOrEmpty(chapter.Title)
                    ? chapter.Title
                    : $"Глава {_currentChapterIndex + 1}";

                await MainScroll.ScrollToAsync(0, 0, false);

                // Помечаем главу прочитанной
                if (!chapter.IsCompleted)
                {
                    chapter.IsCompleted = true;
                    await _dbService.UpdateChapter(chapter);
                }

                // Обновляем прогресс в объекте
                _currentBook.Progress = _currentChapterIndex;

                // Если дошли до конца — меняем статусы
                if (_currentChapterIndex == _currentBook.TotalChapters - 1)
                {
                    _currentBook.IsFinished = true;
                    _currentBook.IsReading = false;
                }

                // Вызов сохранения в БД в конце метода
                await _dbService.UpdateBook(_currentBook);
            }
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            if (_currentChapterIndex < _currentBook.TotalChapters - 1)
            {
                _currentChapterIndex++;
                await LoadCurrentChapter();
            }
            else
            {
                CustomAlert.IsVisible = true;
            }
        }

        // 2. Обработчик кнопки "Остаться"
private void OnAlertCancelClicked(object sender, EventArgs e)
{
    CustomAlert.IsVisible = false;
}

// 3. Обработчик кнопки "В библиотеку"
private async void OnAlertConfirmClicked(object sender, EventArgs e)
{
    CustomAlert.IsVisible = false;
    await Navigation.PopAsync();
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
