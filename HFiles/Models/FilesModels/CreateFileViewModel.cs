using HFiles.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HFiles.Models.FilesModels
{
    public class CreateFileViewModel
    {
        public CreateFileFormModel Form { get; set; }
        public Profile Profile { get;set; }
        public List<SelectListItem> Visibility { get; set; }

        public CreateFileViewModel(ApplicationDbContext db, string username)
        {
            Profile = db.Profiles.SingleOrDefault(x => x.Username == username);
            Visibility = new List<SelectListItem>();
            Visibility.Add(new SelectListItem { Text = "Public (everyone)", Value = false.ToString() });
            Visibility.Add(new SelectListItem { Text = "Private (just me)", Value = true.ToString() });
        }

        
    }
}
