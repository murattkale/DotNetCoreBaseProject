using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class SpecAttrController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ISpecAttrService _ISpecAttrService;
        public SpecAttrController(IUnitOfWork<myDBContext> _uow, ISpecAttrService _ISpecAttrService)
        {
            this._uow = _uow;
            this._ISpecAttrService = _ISpecAttrService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ISpecAttrService.WhereList();
            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<SpecAttr> param)
        {
            var result = _ISpecAttrService.GetPaging(o => (param.selectid > 0 ? o.SpecId == param.selectid : true), true, param, false, o => o.Spec);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(SpecAttr postModel)
        {
            var result = _ISpecAttrService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ISpecAttrService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ISpecAttrService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
