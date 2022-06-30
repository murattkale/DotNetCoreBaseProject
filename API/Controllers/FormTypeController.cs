using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class FormTypeController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IFormTypeService _IFormTypeService;
        public FormTypeController(IUnitOfWork<myDBContext> _uow, IFormTypeService _IFormTypeService)
        {
            this._uow = _uow;
            this._IFormTypeService = _IFormTypeService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IFormTypeService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IFormTypeService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<FormType> param)
        {
            var result = _IFormTypeService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(FormType postModel)
        {
            var result = _IFormTypeService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IFormTypeService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IFormTypeService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    }
}
