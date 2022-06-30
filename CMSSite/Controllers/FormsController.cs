using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CMSSite.Controllers
{
    public class FormsController : Controller
    {
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public FormsController(

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
            var result = await _client.GetAsync<EnumModel>(new Forms().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);

        }


        public async Task<IActionResult> GetAll()
        {
            var result = await _client.GetAsync<Forms>(new Forms().GetType().Name + "/GetAll");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Forms> param)
        {
            var result = await _client.PostAsync<Forms>(new Forms().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Forms postmodel)
        {
            var result = await _client.PostAsync<Forms>(new Forms().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Forms>(new Forms().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Forms>(new Forms().GetType().Name + "/GetRow" + $"?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Forms>(new Forms().GetType().Name + "/Delete" + $"?id={id}");
            return Json(result);
        }


    }
}
