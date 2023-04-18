using HFiles.Data;
using Microsoft.AspNetCore.Mvc;
using Hashgraph;
using Microsoft.AspNetCore.Authorization;
using HFiles.Models.FilesModels;

namespace HFiles.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private ApplicationDbContext _db;
        private string _gatewayUrl;
        private string _nodeAccountId;
        private string _hederaAccount;
        private string _hederaPublicKey;
        private string _hederaPrivateKey;

        public FilesController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _gatewayUrl = configuration["HederaNetwork"];
            _nodeAccountId = configuration["HederaNodeAccountId"];
            _hederaAccount = configuration["HederaAccountId"];
            _hederaPrivateKey = configuration["HederaPrivateKey"];
            _hederaPublicKey = configuration["HederaPublicKey"];
        }
        public IActionResult Index(string? name, int? page, string? error)
        {
            IndexFilesViewModel model = new IndexFilesViewModel(_db);
            model.Profile = _db.Profiles.SingleOrDefault(x => x.Username == User.Identity.Name);
            model.Files = _db.UserFiles.Where(x => x.Username == User.Identity.Name && x.CreatedInNetwork).OrderByDescending(x => x.CreatedAt);
            if(!string.IsNullOrWhiteSpace(name))
            {
                model.Files = model.Files.Where(x => x.Name.Contains(name) || x.Description.Contains(name));
            }

            page = page.HasValue ? page : 1;
            model.Files = model.Files.Skip(page == 1 ?0: 20 * (page.Value-1)).Take(20);
            model.Page = page;
            model.Name = name;
            model.Error = error;
            return View(model);
        }

        public IActionResult Create()
        {
            CreateFileViewModel model = new CreateFileViewModel(_db, User.Identity.Name);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFileFormModel Form)
        {
            CreateFileViewModel model = new CreateFileViewModel(_db, User.Identity.Name);
            if(Form.File != null && Form.File.Length == 0)
            {
                ModelState.AddModelError("Form.File", "The file is invalid.");
            }
            if (Form.File != null && Form.File.Length > 6000)
            {
                ModelState.AddModelError("Form.File", "The file is too big. You can upload files with 6 KB of size.");
            }
            if (!ModelState.IsValid)
            {
                model.Form = Form;
                return View(model);
            }
            try
            {
                var payerPrivateKey = Hex.ToBytes(_hederaPrivateKey);
                var newPublicKey = Hex.ToBytes(_hederaPublicKey);
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = new Gateway(_gatewayUrl, 0, 0, long.Parse(_nodeAccountId));
                    ctx.Payer = new Address(0, 0, model.Profile.AccountNum.Value);
                    ctx.Signatory = new Signatory(payerPrivateKey);
                });
                using (var memoryStream = new MemoryStream())
                {
                    await Form.File.CopyToAsync(memoryStream);
                    Endorsement endorsement = new Endorsement(newPublicKey);
                    CreateFileParams createFileParams = new CreateFileParams()
                    {
                          Contents = memoryStream.ToArray(), Signatory = new Signatory(payerPrivateKey),
                          Endorsements = new Endorsement[] { endorsement },
                          Memo = Form.Description, Expiration = DateTime.UtcNow.AddDays(89)
                    };
                    FileReceipt fileReceipt = await client.CreateFileAsync(createFileParams);
                    if(fileReceipt != null)
                    {
                        UserFile userFile = new UserFile
                        {
                            CreatedAt = DateTime.UtcNow,
                            CreatedInNetwork = true,
                            HederaId = fileReceipt.File.AccountNum.ToString(),
                            Name = Form.Name,
                            Username = User.Identity.Name,
                            ExpirationDate = Form.ExpirationDate,
                            FileName = Form.File.FileName,
                            ContentType = Form.File.ContentType,
                            Description = Form.Description,
                            IsPrivate = Form.IsPrivate
                        };
                        _db.UserFiles.Add(userFile);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form.File", "There is an error with uploading the file. Try again later.");
                model.Form = Form;
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Update(long id)
        {
            UpdateFileViewModel model = new UpdateFileViewModel(_db, id);
            if (model.UserFile.Username != User.Identity.Name)
            {
                return RedirectToAction("Index", new { error = "no access" });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateFileFormModel Form)
        {
            UpdateFileViewModel model = new UpdateFileViewModel(_db, Form.Id);
            if (Form.File != null && Form.File.Length == 0)
            {
                ModelState.AddModelError("Form.File", "The file is invalid.");
            }
            if (Form.File != null && Form.File.Length > 6000)
            {
                ModelState.AddModelError("Form.File", "The file is too big. You can upload files with 6 KB of size.");
            }
            if (!ModelState.IsValid)
            {
                model.Form = Form;
                return View(model);
            }
            try
            {
                var payerPrivateKey = Hex.ToBytes(_hederaPrivateKey);
                var newPublicKey = Hex.ToBytes(_hederaPublicKey);
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = new Gateway(_gatewayUrl, 0, 0, long.Parse(_nodeAccountId));
                    ctx.Payer = new Address(0, 0, model.Profile.AccountNum.Value);
                    ctx.Signatory = new Signatory(payerPrivateKey);
                });

                UserFile userFile = _db.UserFiles.SingleOrDefault(x => x.Id == Form.Id);
                userFile.Name = Form.Name;
                userFile.IsPrivate = Form.IsPrivate;
                userFile.Description = Form.Description;
                if (Form.File != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Form.File.CopyToAsync(memoryStream);
                        Endorsement endorsement = new Endorsement(newPublicKey);
                        UpdateFileParams updateFileParams = new UpdateFileParams()
                        {
                            Contents = memoryStream.ToArray(),
                            Signatory = new Signatory(payerPrivateKey),
                            Endorsements = new Endorsement[] { endorsement },
                            Memo = Form.Description,
                            File = new Address(0, 0, long.Parse(model.UserFile.HederaId))
                        };
                        TransactionReceipt fileReceipt = await client.UpdateFileAsync(updateFileParams);
                        if(fileReceipt != null)
                        {
                            userFile.FileName = Form.File.FileName;
                            userFile.ContentType = Form.File.ContentType;
                        }
                    }
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form.File", "There is an error updating the file. Try again later.");
                model.Form = Form;
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(long id)
        {
            DetailFileViewModel model = new DetailFileViewModel(_db, id);
            if(model.UserFile == null || !model.UserFile.CreatedInNetwork)
            {
                return RedirectToAction("Index", new { error="no file" });
            }
            if(model.UserFile.IsPrivate.GetValueOrDefault(false) && model.UserFile.Username != User.Identity.Name)
            {
                return RedirectToAction("Index", new { error = "no access" });
            }
            return View(model);
        }

        public async Task<IActionResult> Download(long id)
        {
            Profile profile = _db.Profiles.SingleOrDefault(x => x.Username == User.Identity.Name);
            UserFile userFile = _db.UserFiles.SingleOrDefault(x => x.Id == id);
            if (userFile == null || !userFile.CreatedInNetwork)
            {
                return RedirectToAction("Index", new { error = "no file" });
            }
            if (userFile.IsPrivate.GetValueOrDefault(false) && userFile.Username != User.Identity.Name)
            {
                return RedirectToAction("Index", new { error = "no access" });
            }
            var payerPrivateKey = Hex.ToBytes(_hederaPrivateKey);
            await using var client = new Client(ctx =>
            {
                ctx.Gateway = new Gateway(_gatewayUrl, 0, 0, long.Parse(_nodeAccountId));
                ctx.Payer = new Address(0, 0, profile.AccountNum.Value);
                ctx.Signatory = new Signatory(payerPrivateKey);
            });
            var file = new Address(0, 0, long.Parse(userFile.HederaId));
            var bytes = await client.GetFileContentAsync(file);
            return File(bytes.ToArray(), userFile.ContentType, userFile.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            Profile profile = _db.Profiles.SingleOrDefault(x => x.Username == User.Identity.Name);
            UserFile userFile = _db.UserFiles.SingleOrDefault(x => x.Id == id);
            if (userFile == null || !userFile.CreatedInNetwork)
            {
                return RedirectToAction("Index", new { error = "no file" });
            }
            if (userFile.IsPrivate.GetValueOrDefault(false) && userFile.Username != User.Identity.Name)
            {
                return RedirectToAction("Index", new { error = "no access" });
            }
            var payerPrivateKey = Hex.ToBytes(_hederaPrivateKey);
            await using var client = new Client(ctx =>
            {
                ctx.Gateway = new Gateway(_gatewayUrl, 0, 0, long.Parse(_nodeAccountId));
                ctx.Payer = new Address(0, 0, profile.AccountNum.Value);
                ctx.Signatory = new Signatory(payerPrivateKey);
            });
            var file = new Address(0, 0, long.Parse(userFile.HederaId));
            var bytes = await client.DeleteFileAsync(file);
            userFile.CreatedInNetwork = false;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Share()
        {
            return View();
        }
    }
}
