using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MospolitechProject.Models;
using MospolitechProject.Helpers;

namespace MospolitechProject.Views
{
    public partial class ReaderPage : ContentPage
    {
        private readonly EpubService _epubService = new EpubService();
        private readonly Book _currentBook;
        private int _currentChapterIndex = 0;

        public ReaderPage(Book book)
        {
            InitializeComponent();
            _currentBook = book;
            Title = _currentBook.Title;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadBookContent();
        }

        private async Task LoadBookContent()
        {
            if (string.IsNullOrEmpty(_currentBook.FilePath)) return;

            ReaderLoader.IsRunning = true;

            // Загружаем все главы книги через сервис
            await _epubService.LoadAllChaptersAsync(_currentBook.FilePath);

            if (_epubService.Chapters.Count > 0)
            {
                UpdatePage(0);
            }
            else
            {
                ContentLabel.Text = "Не удалось загрузить содержимое книги.";
            }

            ReaderLoader.IsRunning = false;
        }

        private void UpdatePage(int index)
        {
            if (index >= 0 && index < _epubService.Chapters.Count)
            {
                _currentChapterIndex = index;
                ContentLabel.Text = _epubService.Chapters[index];
                ChapterLabel.Text = $"Стр. {index + 1} из {_epubService.Chapters.Count}";

                // Сбрасываем прокрутку в начало страницы
                MainScroll.ScrollToAsync(0, 0, false);
            }
        }

        private void OnNextClicked(object sender, EventArgs e) => UpdatePage(_currentChapterIndex + 1);

        private void OnPrevClicked(object sender, EventArgs e) => UpdatePage(_currentChapterIndex - 1);
    }
}
