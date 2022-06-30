using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class UserController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserService _IUserService;
        public UserController(IUnitOfWork<myDBContext> _uow, IUserService _IUserService)
        {
            this._uow = _uow;
            this._IUserService = _IUserService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IUserService.WhereList();
            return Ok(result);
        }

        [HttpGet("ValidateMail")]
        public IActionResult ValidateMail(string Mail, string pass)
        {
            var result = _IUserService.Get(o => o.Mail == Mail && o.Pass == pass, true, false, o => o.ParentUser, o => o.UserAdress);
            return Ok(result);
        }

        [HttpGet("ValidateMailControl")]
        public IActionResult ValidateMailControl(string Mail)
        {
            var result = _IUserService.Get(o => o.Mail == Mail, true, false, o => o.ParentUser, o => o.UserAdress);
            return Ok(result);
        }

        //[AllowAnonymous]
        [HttpGet("Validate")]
        public IActionResult Validate(string user, string pass)
        {
            var result = _IUserService.Get(o => o.UserName == user && o.Pass == pass, true, false, o => o.ParentUser, o => o.UserAdress);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(User postModel)
        {
            var result = _IUserService.InsertOrUpdate(postModel);
            var rs = _uow.SaveChanges();
            result.RType = rs.RType;
            result.Message = rs.Message;
            result.MessageList = rs.MessageList;
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IUserService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.NameSurname }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }




        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<User> param)
        {
            var result = _IUserService.GetPaging(o => (param.selectid > 0 ? o.ParentUserId == param.selectid : true), true, param, false, o => o.ParentUser, o => o.UserAdress);

            return Ok(result);
        }

        [HttpPost("GetPagingOrder")]
        public IActionResult GetPagingOrder(DTParameters<User> param)
        {
            var result = _IUserService.GetPaging(o => (param.selectid > 0 ? o.ParentUserId == param.selectid : true), true, param, false, o => o.ParentUser, o => o.UserAdress, o => o.Order, o => o.Nationality);
            return Ok(result);
        }


        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IUserService.Get(o => o.Id == id, true, false, o => o.ParentUser, o => o.UserAdress);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IUserService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }


        [HttpGet("GetUserStatusType")]
        public IActionResult GetUserStatusType()
        {
            var res = _IUserService.GetUserStatusType();
            return Ok(res);
        }

        [HttpGet("GetGender")]
        public IActionResult GetGender()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(Gender)).Cast<int>()
                .Select(x => new EnumModel { name = ((Gender)x).ToStr(), value = x.ToString(), text = ((Gender)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }


    }
}
