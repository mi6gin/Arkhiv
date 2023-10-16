using Microsoft.EntityFrameworkCore;
using SammanWebSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SammanWebSite.DataBase
{
    public class PdfnameDbContext : DbContext
    {
        public DbSet<ArchiveItem> ArchiveItems { get; set; }
        public DbSet<PdfFile> PdfFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database/Base/file2.db");
        }

        public PdfnameDbContext()
        {
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string docFolderPath = Path.Combine(currentDirectory, "pdf");
            string pdfFolderPath = docFolderPath;

            // Создайте список категорий
            List<string> categories = new List<string>
            {
                "Протокол",
                "Документ",
                "Соглашение",
                "Заявление",
                "Решение",
                "Устав",
                "Сведения",
                "Лицензия",
                "Доверенность",
                "Закон",
                "Положение",
                "Смета",
                "План"
            };

            Random random = new Random();

            List<string> part1Options = new List<string>
{
    "Протокол",
    "Документ",
    "Соглашение",
    "Заявление",
    "Решение",
    "Устав",
    "Сведения",
    "Лицензия",
    "Доверенность",
    "Закон",
    "Положение",
    "Смета",
    "План",
    "Акт",
    "Запись",
    "Документация",
    "Отчет",
    "Бумага",
    "Запись",
    "Письмо",
    "Текст",
    "Договор",
    "Контракт",
    "Согласие",
    "Участие",
    "Заява",
    "Ходатайство",
    "Обращение",
    "Апелляция",
    "Постановление",
    "Определение",
    "Решение суда",
    "Вердикт",
    "Статут",
    "Регламент",
    "Правила",
    "Конституция",
    "Информация",
    "Данные",
    "Справка",
    "Известия",
    "Разрешение",
    "Разрешительный документ",
    "Удостоверение",
    "Аттестат",
    "Полномочие",
    "Мандат",
    "Уполномоченное лицо",
    "Доверительное письмо",
    "Норма",
    "Правило",
    "Законодательство",
    "Норматив",
    "Статья",
    "Норма",
    "Правило",
    "Пункт",
    "Расчет",
    "Оценка",
    "Бюджет",
    "Планирование расходов",
    "Программа",
    "График",
    "Задача",
    "Проект"
};


            List<string> part2Options = new List<string>
{
    "арбитражного", "налогового", "контрактного", "регуляторного", "корпоративного", "земельного", "административного", "гражданского", "кредитного", "торгового", "строительного", "лицензионного", "договорного", "бухгалтерского", "финансового", "трудового", "судебного", "экологического", "патентного", "авторского", "конституционного", "медицинского"
};

List<string> part3Options = new List<string>
{
    "решения", "предложения", "соглашения", "отчета", "регулирования", "оборудования", "проекта", "разрешения", "акта", "соглашения", "условия", "документа", "задания", "процесса", "стратегии", "анализа", "норматива", "отчета", "учета", "контракта", "закона", "уведомления", "положения", "инструкции", "регламента", "запроса"
};
            List<string> part4Options = new List<string>
            {
                "номер"
            };

            if (Database.EnsureCreated())
            {
                if (Directory.Exists(pdfFolderPath))
                {
                    int fileCount = 1; // Счетчик файлов

                    foreach (var filePath in Directory.GetFiles(pdfFolderPath, "*.*")
                        .Where(file => file.ToLower().EndsWith(".pdf")))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);


                        // Проверьте, не существует ли файл с таким же именем в базе данных.
                        if (!ArchiveItems.Any(pf => pf.PdfFilename == fileName))
                        {
                            // Файл с таким именем не найден, добавьте его в базу данных.
                            var fileContent = File.ReadAllBytes(filePath);

                            // Выберите случайные части для создания PdfFileTruename
                            string randomPart1 = part1Options[random.Next(part1Options.Count)];
                            string randomPart2 = part2Options[random.Next(part2Options.Count)];
                            string randomPart3 = part3Options[random.Next(part3Options.Count)];
                            string randomPart4 = part4Options[random.Next(part4Options.Count)];

                            // Генерируем случайную строку от 6 до 9 символов для каждого элемента PdfFile
                            int randomLength = random.Next(6, 10);
                            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            char[] randomChars = new char[randomLength];
                            for (int i = 0; i < randomLength; i++)
                            {
                                randomChars[i] = characters[random.Next(characters.Length)];
                            }
                            string randomPart5 = new string(randomChars);

                            // Создайте PdfFileTruename
                            string pdfFileTruename = $"{randomPart1} {randomPart2} {randomPart3} {randomPart4} {randomPart5}";

                            // Создайте объект PdfFile и свяжите его с ArchiveItem
                            var pdfFile = new PdfFile
                            {
                                FileContent = fileContent,
                                FileContentPNG = fileContent,
                                FileContentJPG = fileContent,
                                FileContentDOC = fileContent,
                                FileName = fileName
                            };

                            // Выберите случайную категорию
                            string randomCategory = categories[random.Next(categories.Count)];

                            ArchiveItems.Add(new ArchiveItem
                            {
                                PdfFilename = fileName,
                                PdfFileTruename = pdfFileTruename,
                                Category = randomCategory, // Установите случайную категорию
                                PdfFile = pdfFile // Установите связь с PdfFile
                            });

                            fileCount++; // Увеличиваем счетчик
                        }
                    }

                    SaveChanges();
                }
            }
        }
    }
}
