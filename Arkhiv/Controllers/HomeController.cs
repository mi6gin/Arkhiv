using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SammanWebSite.DataBase;
using SammanWebSite.Models;
using System.Security.Claims;
using System.Net.Mime;

namespace SammanWebSite.Controllers
{
    public class Home : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Adminpanel()
        {
            return View();
        }
        [Authorize]
        public IActionResult Downloader()
        {
            var model = new PdfFileViewModel(); // Создайте экземпляр модели
            return View(model); // Передайте модель в представление
        }
        public IActionResult PdfView(int id)
        {
            var pdfFileDbContext = new SammanWebSite.DataBase.PdffileDbContext();

            var pdfFile = pdfFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);
            if (pdfFile != null)
            {
                return View(pdfFile);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult ShowPdf(int id)
        {
            var pdfFileDbContext = new SammanWebSite.DataBase.PdffileDbContext();

            var pdfFile = pdfFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);

            if (pdfFile != null)
            {
                return File(pdfFile.FileContent, "application/pdf", pdfFile.FileName);
            }
            else
            {
                // Обработка случая, если PDF-файл не найден
                return NotFound();
            }
        }

        public IActionResult Delete(int id)
        {

            using (var docFileDbContext = new PdffileDbContext())
            {
                var docFile = docFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);

                if (docFile != null)
                {
                    docFileDbContext.PdfFiles.Remove(docFile);
                    docFileDbContext.SaveChanges();

                }
            }
            using (var docFileDbContext = new PdfnameDbContext())
            {
                var docFile = docFileDbContext.ArchiveItems.FirstOrDefault(pf => pf.Id == id);
                var docFile1 = docFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);

                if (docFile != null)
                {
                    var pdfFile = docFile.PdfFile;
                    docFileDbContext.ArchiveItems.Remove(docFile);
                    docFileDbContext.PdfFiles.Remove(docFile1);
                    docFileDbContext.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ViewPdf(int id)
        {
            var pdfFileDbContext = new SammanWebSite.DataBase.PdffileDbContext();

            var pdfFile = pdfFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);

            if (pdfFile != null)
            {
                // Устанавливаем Content-Disposition заголовок в "inline" для отображения PDF в iframe
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = pdfFile.FileName
                }.ToString());

                return File(pdfFile.FileContent, "application/pdf");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult DownloadPNG(int id)
        {
            var pdfFileDbContext = new SammanWebSite.DataBase.PdffileDbContext();

            var pdfFile = pdfFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);

            if (pdfFile != null)
            {
                // Устанавливаем Content-Disposition заголовок в "attachment" для скачивания PDF
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = false,
                    FileName = pdfFile.FileName
                }.ToString());

                return File(pdfFile.FileContentPNG, "application/pdf");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult DownloadJPG(int id)
        {
            var pdfFileDbContext = new SammanWebSite.DataBase.PdffileDbContext();

            var pdfFile = pdfFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);

            if (pdfFile != null)
            {
                // Устанавливаем Content-Disposition заголовок для скачивания файла с расширением .jpg
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = false,
                    FileName = pdfFile.FileName + ".jpg" // Добавляем .jpg к имени файла
                }.ToString());

                // Указываем MIME-тип как "image/jpeg" для JPEG изображения
                return File(pdfFile.FileContentJPG, "image/jpeg");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }


        [HttpGet]
        public IActionResult DownloadDOC(int id)
        {
            var pdfFileDbContext = new SammanWebSite.DataBase.PdffileDbContext();

            var pdfFile = pdfFileDbContext.PdfFiles.FirstOrDefault(pf => pf.Id == id);

            if (pdfFile != null)
            {
                // Устанавливаем Content-Disposition заголовок в "attachment" для скачивания PDF
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = false,
                    FileName = pdfFile.FileName
                }.ToString());

                return File(pdfFile.FileContentDOC, "application/doc");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }


        [HttpPost]
        public IActionResult AddPdfFile(PdfFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var _pdfFileDbContext = new PdffileDbContext())
                    using (var _pdfNameDbContext = new PdfnameDbContext())
                    {
                        string docFileName = model.DocFileName;
                        byte[] docFileBytesPdf;
                        byte[] docFileBytesDoc;
                        byte[] docFileBytesJpg;
                        byte[] docFileBytesPng;

                        using (var memoryStream = new MemoryStream())
                        {

                            model.PdfFile?.CopyTo(memoryStream);
                            docFileBytesPdf = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {

                            model.DocFile?.CopyTo(memoryStream);
                            docFileBytesDoc = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {

                            model.JpgFile?.CopyTo(memoryStream);
                            docFileBytesJpg = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {

                            model.PngFile?.CopyTo(memoryStream);
                            docFileBytesPng = memoryStream.ToArray();
                        }

                        var docFile = new PdfFile
                        {
                            FileName = docFileName,
                            FileContent = docFileBytesPdf,
                            FileContentDOC = docFileBytesDoc,
                            FileContentPNG = docFileBytesJpg,
                            FileContentJPG = docFileBytesPng,
                            DateCreated = model.DateCreate
                        };

                        _pdfFileDbContext.PdfFiles.Add(docFile);
                        _pdfFileDbContext.SaveChanges();

                        foreach (var category in model.Categores)
                        {
                            var archiveItem = new ArchiveItem
                            {
                                PdfFilename = docFileName,
                                PdfFileTruename = model.DocFileName,
                                Category = category,
                                PdfFile = docFile
                            };
                            _pdfNameDbContext.ArchiveItems.Add(archiveItem);
                        }

                        _pdfNameDbContext.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении данных: " + ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using var db = new AccountDbContext();

                // Найти пользователя по имени
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null)
                {
                    // Проверить хэш пароля
                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        // Успешная аутентификация - установить данные пользователя в сессию
                        HttpContext.Session.SetString("Username", user.Username);
                        HttpContext.Session.SetInt32("IdUser", user.Id);

                        // Остальной код, если есть

                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // Остальной код, если есть

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Неверные имя пользователя или пароль");
                return RedirectToAction("LoginError", "Home");
            }

            return RedirectToAction("LoginError", "Home");
        }





    }

}
