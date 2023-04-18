using System.ComponentModel.DataAnnotations;

namespace HFiles.Models.FilesModels
{
    public class CreateFileFormModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get;set; }

        [Required]
        public IFormFile File { get; set; }

        [Required]
        public bool? IsPrivate { get;set;}

        //[Required]
        public DateTime? ExpirationDate { get; set; }
    }
}
