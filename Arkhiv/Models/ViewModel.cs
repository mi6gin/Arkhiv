using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SammanWebSite.Models
{
    public class PdfFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[]? FileContent { get; set; }
        public byte[]? FileContentPNG { get; set; } // Допустимо null
        public byte[]? FileContentJPG { get; set; } // Допустимо null
        public byte[]? FileContentDOC { get; set; } // Допустимо null
        public DateTime? DateCreated { get; set; }
    }

    public class ArchiveItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string PdfFilename { get; set; }

        public string PdfFileTruename { get; set; }

        public string Category { get; set; }

        // Навигационное свойство для связи с PdfFile
        public PdfFile PdfFile { get; set; }
    }

    public class PdfFileViewModel
    {
        public string DocFileName { get; set; }

        public IFormFile? PdfFile { get; set; }
        public IFormFile? DocFile { get; set; }
        public IFormFile? JpgFile { get; set; }
        public IFormFile? PngFile { get; set; }


        public List<string> Categores { get; set; }

        public DateTime DateCreate { get; set; }
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        

    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите имя пользователя")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите пароль")]
        public string Password { get; set; }
    }


}
