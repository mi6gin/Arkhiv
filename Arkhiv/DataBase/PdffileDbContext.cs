using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SammanWebSite.Models;

namespace SammanWebSite.DataBase
{
    public class PdffileDbContext : DbContext
    {
        public DbSet<PdfFile> PdfFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database/Base/pdf.db");
        }

        public PdffileDbContext()
        {
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string docFolderPath = Path.Combine(currentDirectory, "pdf");
            string pdfFolderPath = docFolderPath;

            if (Database.EnsureCreated())
            {
                if (Directory.Exists(pdfFolderPath))
                {
                    var fileFormats = new Dictionary<string, PdfFile>();
                    var random = new Random();

                    foreach (var filePath in Directory.GetFiles(pdfFolderPath, "*.*")
                        .Where(file => file.ToLower().EndsWith(".pdf") || file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".doc")))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);

                        var startDate = new DateTime(2000, 1, 1);
                        var endDate = new DateTime(2022, 12, 31);
                        var randomDate = startDate.AddDays(random.Next((endDate - startDate).Days));

                        if (fileFormats.ContainsKey(fileName))
                        {
                            var pdfFile = fileFormats[fileName];

                            if (filePath.EndsWith(".pdf"))
                            {
                                pdfFile.FileContent = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".jpg"))
                            {
                                pdfFile.FileContentPNG = File.ReadAllBytes(filePath); 
                            }
                            else if (filePath.EndsWith(".png"))
                            {
                                pdfFile.FileContentJPG = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".doc"))
                            {
                                pdfFile.FileContentDOC = File.ReadAllBytes(filePath);
                            }
                        }
                        else
                        {
                            var pdfFile = new PdfFile
                            {
                                FileName = fileName,
                                DateCreated = randomDate
                            };

                            if (filePath.EndsWith(".pdf"))
                            {
                                pdfFile.FileContent = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".jpg"))
                            {
                                pdfFile.FileContentPNG = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".png"))
                            {
                                pdfFile.FileContentJPG = File.ReadAllBytes(filePath);
                            }
                            else if (filePath.EndsWith(".doc"))
                            {
                                pdfFile.FileContentDOC = File.ReadAllBytes(filePath);
                            }

                            fileFormats[fileName] = pdfFile;
                        }
                    }

                    PdfFiles.AddRange(fileFormats.Values);
                    SaveChanges();
                }
            }
        }
    }
}
