using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class ServiceConfigAuthController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IServiceConfigAuthService _IServiceConfigAuthService;
        public ServiceConfigAuthController(IUnitOfWork<myDBContext> _uow, IServiceConfigAuthService _IServiceConfigAuthService)
        {
            this._uow = _uow;
            this._IServiceConfigAuthService = _IServiceConfigAuthService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IServiceConfigAuthService.WhereList();
            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<ServiceConfigAuth> param)
        {
            var result = _IServiceConfigAuthService.GetPaging(null, true, param, false, o => o.Role, o => o.Permission, o => o.ServiceConfig, o => o.Permission, o => o.Users);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(ServiceConfigAuth postModel)
        {
            var result = _IServiceConfigAuthService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IServiceConfigAuthService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IServiceConfigAuthService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
