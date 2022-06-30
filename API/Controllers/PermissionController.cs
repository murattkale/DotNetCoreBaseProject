using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class PermissionController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IPermissionService _IPermissionService;
        public PermissionController(IUnitOfWork<myDBContext> _uow, IPermissionService _IPermissionService)
        {
            this._uow = _uow;
            this._IPermissionService = _IPermissionService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IPermissionService.WhereList();
            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Permission> param)
        {
            var result = _IPermissionService.GetPaging(null, true, param, false, o => o.Role);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Permission postModel)
        {
            var result = _IPermissionService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IPermissionService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IPermissionService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
