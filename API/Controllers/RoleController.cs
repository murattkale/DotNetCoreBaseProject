using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class RoleController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IRoleService _IRoleService;
        public RoleController(IUnitOfWork<myDBContext> _uow, IRoleService _IRoleService)
        {
            this._uow = _uow;
            this._IRoleService = _IRoleService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IRoleService.WhereList();
            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Role> param)
        {
            var result = _IRoleService.GetPaging(null, true, param, false, o => o.RoleParent);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Role postModel)
        {
            var result = _IRoleService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IRoleService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IRoleService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
