using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class FormsController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IFormsService _IFormsService;
        public FormsController(IUnitOfWork<myDBContext> _uow, IFormsService _IFormsService)
        {
            this._uow = _uow;
            this._IFormsService = _IFormsService;
        }



        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Forms> param)
        {
            var result = _IFormsService.GetPaging(o => (param.selectid > 0 ? o.FormTypeId == param.selectid : true), true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Forms postModel)
        {
            var result = _IFormsService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IFormsService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IFormsService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    }
}
