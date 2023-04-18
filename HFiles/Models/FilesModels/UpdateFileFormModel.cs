using System.ComponentModel.DataAnnotations;

namespace HFiles.Models.FilesModels
{
    public class UpdateFileFormModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public IFormFile? File { get; set; }

        [Required]
        public bool? IsPrivate { get; set; }
    }
}
