using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class OrderController : Controller
    {
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public OrderController(

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
            var result = await _client.GetAsync<EnumModel>(new Order().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);

        }


        public async Task<IActionResult> GetAll()
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + "/GetAll");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Order> param)
        {
            var result = await _client.PostAsync<Order>(new Order().GetType().Name + "/GetPaging", param);

            var rs = result.ResultPaging;
            rs.data = rs.data.Where(o=>o.OrderStatus == OrderStatus.Odendi).ToList();
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> GetPagingOther(DTParameters<Order> param)
        {
            var result = await _client.PostAsync<Order>(new Order().GetType().Name + "/GetPaging", param);

            var rs = result.ResultPaging;
            rs.data = rs.data.Where(o => o.OrderStatus != OrderStatus.Odendi).ToList();
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Order postmodel)
        {
            var result = await _client.PostAsync<Order>(new Order().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + "/GetRow" + $"?id={id}");
            return Json(result);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + "/Delete" + $"?id={id}");
            return Json(result);
        }

        public async Task<IActionResult> GetOrderStatus()
        {
            var result = await _client.GetAsync<EnumModel>(new Order().GetType().Name + $"/GetOrderStatus");
            return Json(result.ResultList);
        }

        public async Task<IActionResult> GetConfirmStatus()
        {
            var result = await _client.GetAsync<EnumModel>(new Order().GetType().Name + $"/GetConfirmStatus");
            return Json(result.ResultList);
        }

    }
}
