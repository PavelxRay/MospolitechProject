using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VersOne.Epub;
using HtmlAgilityPack;
using MospolitechProject.Models;
using Xamarin.Essentials;

namespace MospolitechProject.Helpers
{
    public class EpubService
    {
        public List<(string Title, string Text)> Chapters { get; set; } = new List<(string, string)>();

        public async Task<Book> ParseEpubAsync(string filePath)
        {
            EpubBook epubBook = await EpubReader.ReadBookAsync(filePath);

            // Передаем название всей книги в ExtractChapters для очистки дублей
            ExtractChapters(epubBook);

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
                Progress = 0,
                TotalChapters = Chapters.Count
            };
        }

        private void ExtractChapters(EpubBook book)
        {
            Chapters.Clear();
            int counter = 1;

            foreach (var resource in book.ReadingOrder)
            {
                string html = resource.Content;
                if (string.IsNullOrWhiteSpace(html)) continue;

                // Передаем book.Title в метод очистки
                var (title, cleanText) = CleanHtmlAndExtractTitle(html, $"Глава {counter}", book.Title);

                if (cleanText.Length > 50)
                {
                    Chapters.Add((title, cleanText));
                    counter++;
                }
            }
        }

        private (string Title, string Text) CleanHtmlAndExtractTitle(string html, string fallbackTitle, string bookTitle)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // 1. Удаляем скрипты и стили
                var scripts = doc.DocumentNode.SelectNodes("//script|//style|//meta|//link");
                if (scripts != null) foreach (var n in scripts) n.Remove();

                // 2. Ищем и УДАЛЯЕМ заголовок главы (чтобы не было в тексте)
                string extractedTitle = fallbackTitle;
                var titleNode = doc.DocumentNode.SelectSingleNode("//h1|//h2|//h3|//h4|//b|//strong");

                // Если не нашли h-теги, берем самый первый значимый блок
                if (titleNode == null) titleNode = doc.DocumentNode.SelectSingleNode("//p|//div");

                if (titleNode != null)
                {
                    string pTitle = HtmlEntity.DeEntitize(titleNode.InnerText).Trim();
                    if (pTitle.Length > 2 && pTitle.Length < 120)
                    {
                        extractedTitle = pTitle;
                        titleNode.Remove(); // Удаляем узел из дерева
                    }
                }

                // 3. СОХРАНЯЕМ АБЗАЦЫ: Заменяем закрывающие теги блоков на переносы строк
                var blockNodes = doc.DocumentNode.SelectNodes("//p|//div|//br|//h1|//h2|//h3");
                if (blockNodes != null)
                {
                    foreach (var node in blockNodes)
                    {
                        // Вставляем перенос строки после каждого абзаца
                        node.ParentNode.InsertAfter(doc.CreateTextNode("\n\n"), node);
                    }
                }

                // 4. Получаем текст с сохраненными переносами
                string cleanText = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText).Trim();

                // 5. УДАЛЯЕМ НАЗВАНИЕ КНИГИ, если оно прилипло в начале (как на скриншоте)
                if (!string.IsNullOrEmpty(bookTitle))
                {
                    // Отрезаем название книги, если текст с него начинается
                    if (cleanText.StartsWith(bookTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        cleanText = cleanText.Substring(bookTitle.Length).Trim();
                    }
                }

                // 6. ФИНАЛЬНАЯ ЧИСТКА (твои регулярки)
                cleanText = Regex.Replace(cleanText, @"\d+/\d+", ""); // Страницы
                cleanText = Regex.Replace(cleanText, @"\d{1,2}:\d{2}", ""); // Время

                // Схлопываем только горизонтальные пробелы, сохраняя наши \n (переносы)
                cleanText = Regex.Replace(cleanText, @"[ \t]+", " ");

                // Убираем лишние пустые строки (больше двух подряд)
                cleanText = Regex.Replace(cleanText, @"(\n\s*){3,}", "\n\n");

                return (extractedTitle, cleanText.Trim());
            }
            catch
            {
                return (fallbackTitle, Regex.Replace(html, "<.*?>", " ").Trim());
            }
        }
    }
}
