using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class UserAdressController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserAdressService _IUserAdressService;
        public UserAdressController(IUnitOfWork<myDBContext> _uow, IUserAdressService _IUserAdressService)
        {
            this._uow = _uow;
            this._IUserAdressService = _IUserAdressService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IUserAdressService.WhereList(null, true, false, o => o.User);
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IUserAdressService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<UserAdress> param)
        {
            var result = _IUserAdressService.GetPaging(null, true, param, false, o => o.User);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(UserAdress postModel)
        {
            var result = _IUserAdressService.InsertOrUpdate(postModel);
            var rs = _uow.SaveChanges();
            result.RType = rs.RType;
            result.Message = rs.Message;
            result.MessageList = rs.MessageList;
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IUserAdressService.Get(o => o.Id == id, true, false, o => o.User);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IUserAdressService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    }
}
