using ASP_201.Data;
using ASP_201.Models.Forum;
using ASP_201.Services.Transliterate;
using ASP_201.Services.Validate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Drawing.Text;
using System.Security.Claims;

namespace ASP_201.Controllers
{
    public class ForumController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ForumController> _logger;
        private readonly IValidateService _validationService;
        private readonly ITransliterationServiceUkr _transliterationService;

        public ForumController(DataContext dataContext, ILogger<ForumController> logger, IValidateService validationService, ITransliterationServiceUkr transliterationService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _validationService = validationService;
            _transliterationService = transliterationService;
        }

        private int counter = 0;
        private int Counter { get => counter++; set => counter = value; }
        public IActionResult Index()
        {
            Counter = 0;
            ForumIndexModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Sections = _dataContext
                    .Sections 
                    .Include(s => s.Author)
                    .Where(s => s.Deleted == null)
                    .OrderBy(s => s.CreatedDt)
                    .AsEnumerable() // IQuerible -> IEnumerated
                    .Select(s => new ForumSectionViewModel()
                    {
                        CreatedAt = s.CreatedDt,
                        Title = s.Title,
                        Description = s.Description,
                        LogoUrl = $"/img/logos/section{Counter}.png",
                        AuthorName = s.Author.RealName,
                        UrlIdString = s.UrlId is null ? s.Id.ToString() : s.UrlId,
                        AuthorAvatarUrl = s.Author.Avatar == null
                        ? "avatars/no-avatar.png"
                        : $"/avatars/{s.Author.Avatar}"
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is String message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive = HttpContext.Session.GetInt32("IsMessagePositive") == 1;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                    };
                    HttpContext.Session.Remove("SavedTitle");
                    HttpContext.Session.Remove("SavedDescription");
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }

        public ViewResult Sections([FromRoute] String id)
        {
            ViewData["id"] = id;
            Guid sectionId;
            try
            {
                sectionId = Guid.Parse(id);
            }
            catch
            {
                sectionId = _dataContext.Sections.First(s => s.UrlId == id).Id;
            }
            ForumSectionsModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                SectionId = id,
                Themes = _dataContext
                .Themes
                .Include(t => t.Author)
                .Where(t => t.Deleted == null && t.SectionId == sectionId)
                .Select(t => new ForumThemeViewModel()
                {
                    Title = t.Title,
                    Description = t.Description,
                    CreatedAt = DateTime.Now,
                    UrlIdString = t.Id.ToString(),
                    SectionId = t.SectionId.ToString(),
                    AuthorName = t.Author.IsRealNamePublic ? t.Author.RealName : t.Author.Login,
                    AuthorAvatarUrl = t.Author.Avatar == null
                        ? "avatars/no-avatar.png"
                        : $"/avatars/{t.Author.Avatar}"
                }).ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is String message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive = HttpContext.Session.GetInt32("IsMessagePositive") == 1;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                    };
                    HttpContext.Session.Remove("SavedTitle");
                    HttpContext.Session.Remove("SavedDescription");
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }
        public IActionResult Themes([FromRoute] String id)
        {
            Guid themesId;
            try
            {
                themesId = Guid.Parse(id);
            }
            catch
            {
                themesId = Guid.Empty; //_dataContext.Sections.First(s => s.UrlId == id).Id;
            }
            var theme = _dataContext.Themes.Find(themesId);
            if(theme == null)
            {
                return NotFound();
            }
            ForumThemesModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                ThemeId = id,
                Title = theme.Title,
                Topics = _dataContext
                .Topics
                .Where(t => t.Deleted == null && t.ThemeId == themesId)
                .Select(t => new ForumTopicViewModel()
                {
                    Title = t.Title,
                    Description = t.Description,
                    CreatedAt = DateTime.Now,
                    UrlIdString = t.Id.ToString()
                }).ToList()


            };
            return View(model);
        }
        public IActionResult Topics([FromRoute] String id)
        {
            Guid topicId;
            try
            {
                topicId = Guid.Parse(id);
            }
            catch
            {
                topicId = Guid.Empty; //_dataContext.Sections.First(s => s.UrlId == id).Id;
            }
            var topic = _dataContext.Topics.Find(topicId);
            if (topic == null)
            {
                return NotFound();
            }
            ForumTopicsModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Title = topic.Title,
                Description = topic.Description,
                TopicId = id,
                Posts = _dataContext
                .Posts
                .Include(p=>p.Author)
                .Include(p => p.Reply)
                .Where(p => p.Deleted == null && p.TopicId == topicId)
                .Select(t => new ForumPostViewModel
                {
                    Content = t.Content,
                    AuthorName = t.Author.IsRealNamePublic ? t.Author.RealName : t.Author.Login,
                    AuthorAvatarUrl = t.Author.Avatar == null
                        ? "avatars/no-avatar.png"
                        : $"/avatars/{t.Author.Avatar}"
                }).ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is String message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive = HttpContext.Session.GetInt32("IsMessagePositive") == 1;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Content = HttpContext.Session.GetString("SavedContent")!,
                        ReplyId = HttpContext.Session.GetString("SavedReply")!
                    };
                    HttpContext.Session.Remove("SavedContent");
                    HttpContext.Session.Remove("SavedReply");
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult CreateSection(ForumSectionFormModel formModel)
        {
            _logger.LogInformation("Title: {t}, Description: {d}",
                formModel.Title, formModel.Description);

            if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage",
                    "Назва не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage",
                    "Опис не може бути порожним");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value
                    );
                    string trans = _transliterationService.Transliterate(formModel.Title);
                    string urlId = trans;
                    int n = 2;
                    while(_dataContext.Sections.Where(s => s.UrlId == urlId).Count() > 0) { urlId = $"{urlId}{n++}"; }
                    _dataContext.Sections.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Title = formModel.Title,
                        Description = formModel.Description,
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        UrlId = urlId
                    });
                    _dataContext.SaveChanges();
                    HttpContext.Session.SetString("CreateSectionMessage",
                        "Додано успішно");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage",
                    "Відмовлено в авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                    HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public RedirectToActionResult CreateTheme(ForumThemeFormModel formModel)
        {
            _logger.LogInformation("Title: {t}, Description: {d}",
                formModel.Title, formModel.Description);

            if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage",
                    "Назва не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage",
                    "Опис не може бути порожним");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value
                    );
                    _dataContext.Themes.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Title = formModel.Title,
                        Description = formModel.Description,
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        SectionId = Guid.Parse(formModel.SectionId)
                    });
                    _dataContext.SaveChanges();
                    HttpContext.Session.SetString("CreateSectionMessage",
                        "Додано успішно");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage",
                    "Відмовлено в авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                    HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
                }
            }
            return RedirectToAction(nameof(Sections), new {id = formModel.SectionId});
        }

        public RedirectToActionResult CreateTopic(ForumTopicFormModel formModel)
        {
            _logger.LogInformation("Title: {t}, Description: {d}",
                formModel.Title, formModel.Description);

            if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage",
                    "Назва не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage",
                    "Опис не може бути порожним");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value
                    );
                    _dataContext.Topics.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Title = formModel.Title,
                        Description = formModel.Description,
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        ThemeId = Guid.Parse(formModel.ThemeId)
                    });
                    _dataContext.SaveChanges();
                    HttpContext.Session.SetString("CreateSectionMessage",
                        "Додано успішно");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage",
                    "Відмовлено в авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                    HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
                }
            }
            return RedirectToAction(nameof(Themes), new { id = formModel.ThemeId });
        }
        public RedirectToActionResult CreatePost(ForumPostFormModel formModel)
        {
            if (!_validationService.Validate(formModel.Content, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage",
                    "Назва не може бути порожною");
                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedContent", formModel.Content ?? String.Empty);
                HttpContext.Session.SetString("SavedReply", formModel.ReplyId ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(
                        HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value
                    );
                    _dataContext.Posts.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Content = formModel.Content,
                        ReplyId = String.IsNullOrEmpty(formModel.ReplyId) ? null : Guid.Parse(formModel.ReplyId),
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        TopicId = Guid.Parse(formModel.TopicId)
                    }); ;
                    _dataContext.SaveChanges();
                    HttpContext.Session.SetString("CreateSectionMessage",
                        "Додано успішно");
                    HttpContext.Session.SetInt32("IsMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage",
                    "Відмовлено в авторизації");
                    HttpContext.Session.SetInt32("IsMessagePositive", 0);
                    HttpContext.Session.SetString("SavedTitle", formModel.Content ?? String.Empty);
                    HttpContext.Session.SetString("SavedDescription", formModel.ReplyId ?? String.Empty);
                }
            }
            return RedirectToAction(nameof(Topics), new { id = formModel.TopicId });
        }
    }
}
