using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CMSSite.Controllers
{
    public class UserAdressController : Controller
    {
        IBaseModel _IBaseModel;
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public UserAdressController(
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

      

        public async Task<IActionResult> GetSelect(string name, string whereCase)
        {
            var result = await _client.GetAsync<EnumModel>(new UserAdress().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<UserAdress> param)
        {
            var result = await _client.PostAsync<UserAdress>(new UserAdress().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(UserAdress postModel)
        {
            if (postModel.Id > 0)
            {
                var rowModel = await _client.GetAsync<UserAdress>(new UserAdress().GetType().Name + $"/GetRow?id={postModel.Id}");
                var row = rowModel.ResultRow;
                row.CountryId = postModel.CountryId;
                row.CityId = postModel.CityId;
                row.Town = postModel.Town;
                row.District = postModel.District;
                row.IsDefault = postModel.IsDefault;
                row.UserId = postModel.UserId;

            }

            var result = await _client.PostAsync<UserAdress>(new UserAdress().GetType().Name + "/InsertOrUpdate", postModel);


            var userRow = await _client.GetAsync<User>(new User().GetType().Name + $"/GetRow?id={postModel.UserId}");

            _IHttpContextAccessor.HttpContext.Session.Set("LoginUser", userRow.ResultRow);

            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<UserAdress>(new UserAdress().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<UserAdress>(new UserAdress().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<UserAdress>(new UserAdress().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }


  

    }
}
