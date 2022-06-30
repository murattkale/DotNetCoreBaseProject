using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class UserRoleController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IUserRoleService _IUserRoleService;
        public UserRoleController(IUnitOfWork<myDBContext> _uow, IUserRoleService _IUserRoleService)
        {
            this._uow = _uow;
            this._IUserRoleService = _IUserRoleService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IUserRoleService.WhereList();
            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<UserRole> param)
        {
            var result = _IUserRoleService.GetPaging(null, true, param, false, o => o.Role,o=>o.User);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(UserRole postModel)
        {
            var result = _IUserRoleService.InsertOrUpdate(postModel);
            var rs = _uow.SaveChanges();

            result.RType = rs.RType;
            result.Message = rs.Message;
            result.MessageList = rs.MessageList;
            result.ResultRow = result.ResultRow;

            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IUserRoleService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IUserRoleService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
