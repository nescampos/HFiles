using HFiles.Data;

namespace HFiles.Models.ProfileModels
{
    public class UpdateProfileViewModel
    {
        public UpdateProfileFormModel Form { get; set; }

        public UpdateProfileViewModel(ApplicationDbContext db, string? name)
        {
            var profile = db.Profiles.SingleOrDefault(p => p.Username == name);
            if(profile != null)
            {
                Form = new UpdateProfileFormModel { Name = profile.Name };
            }
        }
    }
}
