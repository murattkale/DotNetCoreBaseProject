using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class ServiceConfigController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IServiceConfigService _IServiceConfigService;
        public ServiceConfigController(IUnitOfWork<myDBContext> _uow, IServiceConfigService _IServiceConfigService)
        {
            this._uow = _uow;
            this._IServiceConfigService = _IServiceConfigService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IServiceConfigService.WhereList();
            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<ServiceConfig> param)
        {
            var result = _IServiceConfigService.GetPaging(null, true, param, false, o => o.Role, o => o.Parent);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(ServiceConfig postModel)
        {
            var result = _IServiceConfigService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IServiceConfigService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IServiceConfigService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
