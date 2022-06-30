using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CMSSite.Controllers
{
    public class UserController : Controller
    {
        IBaseModel _IBaseModel;
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;
        ISendMail _ISendMail;

        public UserController(
            IBaseModel _IBaseModel,
            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client,
            ISendMail _ISendMail
            )
        {
            this._ISendMail = _ISendMail;
            this._IBaseModel = _IBaseModel;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("profile")]
        public async Task<IActionResult> profile()
        {
            var result = await _client.GetAsync<User>(new User().GetType().Name + $"/GetRow?id={SessionRequest._User.Id}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> Company()
        {
            var result = await _client.GetAsync<User>(new User().GetType().Name + $"/GetRow?id={SessionRequest._User.Id}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var result = await _client.GetAsync<User>(new User().GetType().Name + $"/GetRow?id={SessionRequest._User.Id}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetSelect(string name, string whereCase)
        {
            var result = await _client.GetAsync<EnumModel>(new User().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);
        }

        public async Task<IActionResult> GetGender()
        {
            var result = await _client.GetAsync<EnumModel>(new User().GetType().Name + $"/GetGender");
            return Json(result.ResultList);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<User> param)
        {
            var result = await _client.PostAsync<User>(new User().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(User postmodel)
        {
            postmodel.Mail = postmodel.Mail.Trim();
            if (SessionRequest.LoginUser == null || postmodel.Id < 1)
            {
                var userMailControl = await _client.GetAsync<User>($"User/ValidateMailControl?Mail={postmodel.Mail}");
                if (userMailControl.ResultRow != null)
                    return Json("duplicate");
            }

            var result = await _client.PostAsync<User>(new User().GetType().Name + "/InsertOrUpdate", postmodel);
            _IHttpContextAccessor.HttpContext.Session.Set("LoginUser", result.ResultRow);
            return Json(result);
        }




        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<User>(new User().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<User>(new User().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<User>(new User().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> GetUserStatusType()
        {
            var result = await _client.GetAsync<EnumModel>(new User().GetType().Name + $"/GetUserStatusType");
            return Json(result.ResultList);
        }


        //public async Task<IActionResult> Validate(string UserName, string Pass)
        //{
        //    var result = await _client.GetAsync<User>($"User/Validate?user={UserName}&pass={Pass}");

        //    if (result.RType == RType.OK && result.ResultRow != null)
        //    {
        //        _IHttpContextAccessor.HttpContext.Session.Set("LanguageId", SessionRequest.LanguageId);
        //        result.ResultRow.LanguageId = SessionRequest.LanguageId;
        //        _IBaseModel.LanguageId = SessionRequest.LanguageId;
        //        _IBaseModel.CreaUser = result.ResultRow.Id;
        //        _IHttpContextAccessor.HttpContext.Session.Set("LoginUser", result.ResultRow);

        //        var Languages = await _client.PostAsync<Lang>($"Lang/GetPaging", new DTParameters<Lang>());
        //        _IHttpContextAccessor.HttpContext.Session.Set("Languages", Languages.ResultPaging.data);

        //        return Json(result);
        //    }
        //    else
        //    {
        //        return Json("");
        //    }
        //}

        public async Task<IActionResult> ValidateMail(string Mail, string Pass)
        {
            var result = await _client.GetAsync<User>($"User/ValidateMail?Mail={Mail}&pass={Pass}");

            if (result.RType == RType.OK && result.ResultRow != null)
            {
                _IHttpContextAccessor.HttpContext.Session.Set("LanguageId", SessionRequest.LanguageId);
                result.ResultRow.LanguageId = SessionRequest.LanguageId;
                _IBaseModel.LanguageId = SessionRequest.LanguageId;
                _IBaseModel.CreaUser = result.ResultRow.Id;
                _IHttpContextAccessor.HttpContext.Session.Set("LoginUser", result.ResultRow);

                var _order = await _client.GetAsync<Order>($"Order/GetUserOrder?UserId={SessionRequest.LoginUser.Id}");
                _IHttpContextAccessor.HttpContext.Session.Set("myOrder", _order.ResultRow);
                if (_order.ResultRow != null && _order.ResultRow.OrderStatus == OrderStatus.Odendi)
                {
                    _IHttpContextAccessor.HttpContext.Session.SetInt32("raceUser", 1);
                }


                //var Languages = await _client.PostAsync<Lang>($"Lang/GetPaging", new DTParameters<Lang>());
                //_IHttpContextAccessor.HttpContext.Session.Set("Languages", Languages.ResultPaging.data);

                return Json(result);
            }
            else
            {
                return Json("");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IList<IFormFile> files)
        {
            string filename = "";
            foreach (IFormFile source in files.Take(1))
            {

                var fileContent = ContentDispositionHeaderValue.Parse(source.ContentDisposition);
                filename = fileContent.FileName.clearFileName("_", "user");

                var physicalPath = Path.Combine(_IHostingEnvironment.WebRootPath, "uploads", filename);


                using (FileStream output = System.IO.File.Create(physicalPath))
                    await source.CopyToAsync(output);
            }

            var result = await _client.GetAsync<User>(new User().GetType().Name + $"/GetRow?id={SessionRequest.LoginUser.Id}");

            result.ResultRow.HealtReportUrl = filename;
            var resultRow = await _client.PostAsync<User>(new User().GetType().Name + "/InsertOrUpdate", result.ResultRow);
            _IHttpContextAccessor.HttpContext.Session.Set("LoginUser", resultRow.ResultRow);


            if (SessionRequest.myOrder?.Id > 0)
            {
                var _orderRow = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={SessionRequest.myOrder.Id}");
                var _order = _orderRow.ResultRow;
                _order.ConfirmStatus = ConfirmStatus.Değerlendirmede;

                var UpdateOrder = await _client.PostAsync<Order>(new Order().GetType().Name + "/InsertOrUpdate", _order);
                _IHttpContextAccessor.HttpContext.Session.Set("myOrder", _order);

            }

            return Json("ok");
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            return _IHostingEnvironment.WebRootPath + "\\uploads\\" + filename;
        }


    }
}
