using System.ComponentModel.DataAnnotations;

namespace HFiles.Models.ProfileModels
{
    public class UpdateProfileFormModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
