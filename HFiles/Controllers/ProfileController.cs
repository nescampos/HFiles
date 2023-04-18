using HFiles.Data;
using HFiles.Models.ProfileModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hashgraph;

namespace HFiles.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private ApplicationDbContext _db;
        private string _gatewayUrl;
        private string _nodeAccountId;
        private string _hederaAccount;
        private string _hederaPublicKey;
        private string _hederaPrivateKey;

        public ProfileController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _gatewayUrl = configuration["HederaNetwork"];
            _nodeAccountId = configuration["HederaNodeAccountId"];
            _hederaAccount = configuration["HederaAccountId"];
            _hederaPrivateKey = configuration["HederaPrivateKey"];
            _hederaPublicKey = configuration["HederaPublicKey"];
        }

        public async Task<IActionResult> Index()
        {
            IndexProfileViewModel model = new IndexProfileViewModel(_db, User.Identity.Name);
            if(model.Profile != null)
            {
                var payerPrivateKey = Hex.ToBytes(_hederaPrivateKey);
                var newPublicKey = Hex.ToBytes(_hederaPublicKey);
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = new Gateway(_gatewayUrl, 0, 0, long.Parse(_nodeAccountId));
                    ctx.Payer = new Address(0, 0, long.Parse(_hederaAccount));
                    ctx.Signatory = new Signatory(payerPrivateKey);
                });
                var account = new Address(0, 0, model.Profile.AccountNum.Value);
                var info = await client.GetAccountInfoAsync(account);
                model.Balance = info.Balance / 100_000_000;
            }
            return View(model);
        }

        public IActionResult Update()
        {
            UpdateProfileViewModel model = new UpdateProfileViewModel(_db, User.Identity.Name);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProfileFormModel Form)
        {
            if (!ModelState.IsValid)
            {
                UpdateProfileViewModel model = new UpdateProfileViewModel(_db, User.Identity.Name);
                model.Form = Form;
                return View(model);
            }

            Profile profile = _db.Profiles.SingleOrDefault(x => x.Username == User.Identity.Name);
            if(profile == null)
            {
                var payerPrivateKey = Hex.ToBytes(_hederaPrivateKey);
                var newPublicKey = Hex.ToBytes(_hederaPublicKey);
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = new Gateway(_gatewayUrl, 0, 0, long.Parse(_nodeAccountId));
                    ctx.Payer = new Address(0, 0, long.Parse(_hederaAccount));
                    ctx.Signatory = new Signatory(payerPrivateKey);
                });
                var createParams = new CreateAccountParams
                {
                    Endorsement = new Endorsement(newPublicKey),
                    InitialBalance = 3_000_000_000
                };
                var account = await client.CreateAccountAsync(createParams);
                var address = account.Address;
                profile = new Profile
                {
                     Name = Form.Name, UpdatedAt = DateTime.Now, Username = User.Identity.Name, AccountNum = address.AccountNum
                };
                _db.Profiles.Add(profile);
                _db.SaveChanges();
            }
            else
            {
                profile.Name = Form.Name;
                profile.UpdatedAt = DateTime.Now;
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
