using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class OrderDetailController : Controller
    {
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public OrderDetailController(

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

        public IActionResult IndexOther()
        {
            return View();
        }

        public async Task<IActionResult> GetSelect(string name, string whereCase)
        {
            var result = await _client.GetAsync<EnumModel>(new OrderDetail().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);

        }


        public async Task<IActionResult> GetAll()
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + "/GetAll");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<OrderDetail> param)
        {
            var result = await _client.PostAsync<OrderDetail>(new OrderDetail().GetType().Name + "/GetPaging", param);

            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> GetPagingOther(DTParameters<OrderDetail> param)
        {
            var result = await _client.PostAsync<OrderDetail>(new OrderDetail().GetType().Name + "/GetPaging", param);

            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(OrderDetail postmodel)
        {
            var result = await _client.PostAsync<OrderDetail>(new OrderDetail().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + "/GetRow" + $"?id={id}");
            return Json(result);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + "/Delete" + $"?id={id}");
            return Json(result);
        }

        public async Task<IActionResult> GetOrderDetailStatus()
        {
            var result = await _client.GetAsync<EnumModel>(new OrderDetail().GetType().Name + $"/GetOrderDetailStatus");
            return Json(result.ResultList);
        }

    }
}
