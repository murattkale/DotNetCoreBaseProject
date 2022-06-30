using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CMSSite.Controllers
{
    public class ProductController : Controller
    {
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public ProductController(

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

        public async Task<IActionResult> GetSelect(ProductType ProductType)
        {
            var result = await _client.GetAsync<EnumModel>(new Product().GetType().Name + $"/GetSelect?ProductType={ProductType}");
            return Json(result.ResultList);

        }


        public async Task<IActionResult> GetAll(ProductType ProductType)
        {
            var result = await _client.GetAsync<Product>(new Product().GetType().Name + $"/GetAll?ProductType={ProductType}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Product> param)
        {
            var result = await _client.PostAsync<Product>(new Product().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Product postmodel)
        {
            var result = await _client.PostAsync<Product>(new Product().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Product>(new Product().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Product>(new Product().GetType().Name + "/GetRow" + $"?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Product>(new Product().GetType().Name + "/Delete" + $"?id={id}");
            return Json(result);
        }


    }
}
