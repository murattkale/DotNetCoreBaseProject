using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CMSSite.Controllers
{
    public class DistrictController : Controller
    {
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public DistrictController(

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

        public async Task<IActionResult> GetSelect(int CityId)
        {
            var result = await _client.GetAsync<EnumModel>(new District().GetType().Name + $"/GetSelect?CityId={CityId}");
            return Json(result.ResultList);

        }


        public async Task<IActionResult> GetAll(int CityId)
        {
            var result = await _client.GetAsync<District>(new District().GetType().Name + "/GetAll?CityId={CityId}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<District> param)
        {
            var result = await _client.PostAsync<District>(new District().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(District postmodel)
        {
            var result = await _client.PostAsync<District>(new District().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<District>(new District().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<District>(new District().GetType().Name + "/GetRow" + $"?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<District>(new District().GetType().Name + "/Delete" + $"?id={id}");
            return Json(result);
        }


    }
}
