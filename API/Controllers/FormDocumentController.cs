using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class FormDocumentController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IFormDocumentService _IFormDocumentService;
        public FormDocumentController(IUnitOfWork<myDBContext> _uow, IFormDocumentService _IFormDocumentService)
        {
            this._uow = _uow;
            this._IFormDocumentService = _IFormDocumentService;
        }

      

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<FormDocument> param)
        {
            var result = _IFormDocumentService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(FormDocument postModel)
        {
            var result = _IFormDocumentService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IFormDocumentService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IFormDocumentService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    }
}
