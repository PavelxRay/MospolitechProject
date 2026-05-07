using MospolitechProject.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using VersOne.Epub;
using Xamarin.Essentials;

namespace MospolitechProject.Helpers
{
    public class EpubService
    {
        // Список для хранения текста всех глав книги
        public List<string> Chapters { get; set; } = new List<string>();

        public async Task<Book> ParseEpubAsync(string filePath)
        {
            EpubBook epubBook = await EpubReader.ReadBookAsync(filePath);
            string coverPath = null;

            if (epubBook.CoverImage != null)
            {
                string fileName = $"{Guid.NewGuid()}.jpg";
                coverPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                File.WriteAllBytes(coverPath, epubBook.CoverImage);
            }

            return new Book
            {
                Title = epubBook.Title,
                Author = epubBook.Author,
                Description = "",
                CoverUrl = coverPath,
                FilePath = filePath,
                IsReading = true,
                Progress = 0
            };
        }

        // Загрузка всех глав книги в память
        public async Task LoadAllChaptersAsync(string filePath)
        {
            try
            {
                var epubBook = await EpubReader.ReadBookAsync(filePath);
                Chapters.Clear();

                foreach (var chapter in epubBook.ReadingOrder)
                {
                    string html = chapter.Content;

                    // Очистка от CSS и скриптов
                    html = Regex.Replace(html, "<style.*?>.*?</style>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, "<script.*?>.*?</script>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // Удаление тегов
                    string text = Regex.Replace(html, "<.*?>", string.Empty, RegexOptions.Singleline);
                    string cleanedText = System.Net.WebUtility.HtmlDecode(text).Trim();

                    // Добавляем только страницы, где есть текст
                    if (!string.IsNullOrWhiteSpace(cleanedText))
                    {
                        Chapters.Add(cleanedText);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
