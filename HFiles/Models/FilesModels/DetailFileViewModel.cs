using HFiles.Data;

namespace HFiles.Models.FilesModels
{
    public class DetailFileViewModel
    {
        public UserFile UserFile { get; set; }
        public Hashgraph.FileInfo FileInfo { get; set; }

        public DetailFileViewModel(ApplicationDbContext db, long id)
        {
            UserFile = db.UserFiles.SingleOrDefault(x => x.Id == id);
        }
    }
}
