using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class BaseController : Controller
    {
        IBaseModel _IBaseModel;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public BaseController(
           IBaseModel _IBaseModel,
            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {

            this._IBaseModel = _IBaseModel;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            _IHttpContextAccessor.HttpContext.Session.Set("_user", "");
            _IHttpContextAccessor.HttpContext.Session.Set("config", "");
            _IHttpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login1", "Base");
        }

        public IActionResult Report()
        {
            return View();
        }


        public async Task<IActionResult> Login1()
        {
            var result = await _client.GetAsync<SiteConfig>("SiteConfig/GetBy");

            if (_IHostingEnvironment.IsDevelopment())
                result.ResultRow.layoutUrl = "";

            _IHttpContextAccessor.HttpContext.Session.Set("config", result.ResultRow);

            //if (_IHostingEnvironment.IsDevelopment())
            //    await Validate("admin", "123_*1");

            if (SessionRequest._User != null)
                return View("Index");
            else
                return View();
        }

        public async Task<IActionResult> Validate(string user, string pass)
        {
            var result = await _client.GetAsync<User>($"User/Validate?user={user}&pass={pass}");

            if (result.RType == RType.OK && result.ResultRow != null)
            {
                _IHttpContextAccessor.HttpContext.Session.Set("LanguageId", SessionRequest.LanguageId);
                result.ResultRow.LanguageId = SessionRequest.LanguageId;
                _IBaseModel.LanguageId = SessionRequest.LanguageId;
                _IBaseModel.CreaUser = result.ResultRow.Id;
                _IHttpContextAccessor.HttpContext.Session.Set("_user", result.ResultRow);

                var Languages = await _client.PostAsync<Lang>($"Lang/GetPaging", new DTParameters<Lang>());
                _IHttpContextAccessor.HttpContext.Session.Set("Languages", Languages.ResultPaging.data);


                var FormTypeList = await _client.GetAsync<FormType>($"FormType/GetAll");
                _IHttpContextAccessor.HttpContext.Session.Set("FormType", FormTypeList.ResultList);


                return Json(result);
            }
            else
            {
                return Json("");
            }
        }

        public JsonResult SetLanguage(int id)
        {
            _IHttpContextAccessor.HttpContext.Session.SetInt32("LanguageId", id);
            _IBaseModel.LanguageId = SessionRequest.LanguageId;
            return Json("OK");
        }



    }
}
