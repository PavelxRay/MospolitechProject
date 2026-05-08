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
        // Список очищенных глав для отображения
        public List<string> Chapters { get; set; } = new List<string>();

        public async Task<Book> ParseEpubAsync(string filePath)
        {
            EpubBook epubBook = await EpubReader.ReadBookAsync(filePath);

            // Наполняем список Chapters
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
                Progress = 0
            };
        }

        private void ExtractChapters(EpubBook book)
        {
            Chapters.Clear();

            // Проходим по всем элементам книги в порядке чтения
            foreach (var resource in book.ReadingOrder)
            {
                string html = resource.Content;
                if (string.IsNullOrWhiteSpace(html)) continue;

                // Очищаем HTML и получаем чистый текст
                string cleanText = CleanHtml(html);

                // Добавляем, если в главе есть хоть какой-то осмысленный текст
                if (cleanText.Length > 10)
                {
                    Chapters.Add(cleanText);
                }
            }
        }

        private string CleanHtml(string html)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Удаляем технические теги (как в твоем Kotlin-коде)
                var nodesToRemove = doc.DocumentNode.SelectNodes("//script|//style|//link|//meta");
                if (nodesToRemove != null)
                {
                    foreach (var node in nodesToRemove) node.Remove();
                }

                // Извлекаем текст и декодируем спецсимволы (типа &nbsp;)
                string text = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText).Trim();

                // Применяем твои регулярки для очистки от мусора
                text = Regex.Replace(text, @"\d+/\d+", ""); // Номера страниц
                text = Regex.Replace(text, @"\s+", " ");    // Лишние пробелы

                return text.Trim();
            }
            catch
            {
                // Если HtmlAgilityPack не справился, просто чистим теги регуляркой
                return Regex.Replace(html, "<.*?>", string.Empty).Trim();
            }
        }
    }
}
