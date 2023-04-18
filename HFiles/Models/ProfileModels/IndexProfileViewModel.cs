using HFiles.Data;

namespace HFiles.Models.ProfileModels
{
    public class IndexProfileViewModel
    {
        public Profile Profile { get; set; }
        public ulong Balance { get;set; }

        public IEnumerable<UserFile> LastFiles { get; set; }

        public IndexProfileViewModel(ApplicationDbContext db, string? username)
        {
            Profile = db.Profiles.SingleOrDefault(x => x.Username == username);
            LastFiles = db.UserFiles.Where(x => x.Username == username && x.CreatedInNetwork).OrderByDescending(x => x.CreatedAt).Take(5);
        }
    }
}
