using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class ServiceConfigAuthController : Controller
    {
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public ServiceConfigAuthController(

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
            var result = await _client.GetAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + $"/GetRow?id={SessionRequest._User.Id}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> Company()
        {
            var result = await _client.GetAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + $"/GetRow?id={SessionRequest._User.Id}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> ServiceConfigAuths()
        {
            var result = await _client.GetAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + $"/GetRow?id={SessionRequest._User.Id}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetSelect(string name, string whereCase)
        {
            var result = await _client.GetAsync<EnumModel>(new ServiceConfigAuth().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<ServiceConfigAuth> param)
        {
            var result = await _client.PostAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(ServiceConfigAuth postmodel)
        {
            var result = await _client.PostAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<ServiceConfigAuth>(new ServiceConfigAuth().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> GetServiceConfigAuthStatusType()
        {
            var result = await _client.GetAsync<EnumModel>(new ServiceConfigAuth().GetType().Name + $"/GetServiceConfigAuthStatusType");
            return Json(result.ResultList);
        }


    }
}
