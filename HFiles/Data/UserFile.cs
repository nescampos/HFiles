using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HFiles.Data
{
    public class UserFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        [Required]
        public bool CreatedInNetwork { get; set; }

        //[Required]
        public string? HederaId { get; set; }

        public DateTime? ExpirationDate { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public bool? IsPrivate { get; set; }
    }
}
