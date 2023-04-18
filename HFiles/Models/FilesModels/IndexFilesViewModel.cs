using HFiles.Data;

namespace HFiles.Models.FilesModels
{
    public class IndexFilesViewModel
    {
        private ApplicationDbContext db;
        public IEnumerable<UserFile> Files { get; set; }
        public Profile Profile { get; set; }
        public string? Name { get; set; }
        public string? Error { get; set; }
        public int? Page { get; set; }

        public IndexFilesViewModel(ApplicationDbContext db)
        {
            this.db = db;
        }

        
    }
}
