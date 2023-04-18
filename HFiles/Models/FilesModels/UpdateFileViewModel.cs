using HFiles.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HFiles.Models.FilesModels
{
    public class UpdateFileViewModel
    {
        public UserFile UserFile { get; set; }
        public UpdateFileFormModel Form { get; set; }
        public List<SelectListItem> Visibility { get; set; }
        public Profile Profile { get; set; }

        public UpdateFileViewModel(ApplicationDbContext db, long id)
        {
            UserFile = db.UserFiles.SingleOrDefault(x => x.Id == id);
            Profile = db.Profiles.SingleOrDefault(x => x.Username == UserFile.Username);
            Form = new UpdateFileFormModel() { Id = id, Description = UserFile.Description, IsPrivate = UserFile.IsPrivate, Name = UserFile.Name };
            Visibility = new List<SelectListItem>();
            Visibility.Add(new SelectListItem { Text = "Public (everyone)", Value = false.ToString() });
            Visibility.Add(new SelectListItem { Text = "Private (just me)", Value = true.ToString() });
        }
    }
}
