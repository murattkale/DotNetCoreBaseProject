using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class SpecController : Controller
    {
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public SpecController(

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

        public async Task<IActionResult> GetSelect(string name, string whereCase)
        {
            var result = await _client.GetAsync<EnumModel>(new Spec().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Spec> param)
        {
            var result = await _client.PostAsync<Spec>(new Spec().GetType().Name + $"/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Spec postmodel)
        {
            var result = await _client.PostAsync<Spec>(new Spec().GetType().Name + $"/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Spec>(new Spec().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Spec>(new Spec().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Spec>(new Spec().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }

        public async Task<IActionResult> GetSpecType()
        {
            var result = await _client.GetAsync<EnumModel>(new Spec().GetType().Name + $"/GetSpecType");
            return Json(result.ResultList);
        }

        public async Task<IActionResult> GetSpecListType()
        {
            var result = await _client.GetAsync<EnumModel>(new Spec().GetType().Name + $"/GetSpecListType");
            return Json(result.ResultList);
        }

        public async Task<IActionResult> GetSpecTypeList(SpecType SpecType)
        {
            var result = await _client.GetAsync<Spec>(new Spec().GetType().Name + $"/GetSpecTypeList?SpecType={SpecType}");
            return Json(result);
        }


       





    }
}
