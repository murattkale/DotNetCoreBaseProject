using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class UserController : Controller
    {
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public UserController(

            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {
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

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<User> param)
        {
            var result = await _client.PostAsync<User>(new User().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> GetPagingOrder(DTParameters<User> param)
        {
            var result = await _client.PostAsync<User>(new User().GetType().Name + "/GetPagingOrder", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(User postmodel)
        {
            var result = await _client.PostAsync<User>(new User().GetType().Name + "/InsertOrUpdate", postmodel);
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


        public async Task<IActionResult> GetGender()
        {
            var result = await _client.GetAsync<EnumModel>(new User().GetType().Name + $"/GetGender");
            return Json(result.ResultList);
        }


    }
}
