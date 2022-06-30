using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class SpecContentValueController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ISpecContentValueService _ISpecContentValueService;
        public SpecContentValueController(IUnitOfWork<myDBContext> _uow, ISpecContentValueService _ISpecContentValueService)
        {
            this._uow = _uow;
            this._ISpecContentValueService = _ISpecContentValueService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ISpecContentValueService.WhereList();
            return Ok(result);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<SpecContentValue> param)
        {
            var result = _ISpecContentValueService.GetPaging(o => (param.selectid > 0 ? o.SpecId == param.selectid : true), true, param, false, o => o.Spec);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(SpecContentValue postModel)
        {
            var result = _ISpecContentValueService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ISpecContentValueService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ISpecContentValueService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }

        [HttpPost("InsertOrUpdateBulk")]
        public IActionResult InsertOrUpdateBulk(List<SpecContentValue> postModel)
        {
            List<RModel<SpecContentValue>> insertAll = new List<RModel<SpecContentValue>>();
            postModel.ForEach(o =>
            {
                var row = _ISpecContentValueService.Where(oo => oo.ContentPageId == o.ContentPageId && oo.SpecId == o.SpecId, false, false);
                if (row.RType == RType.OK)
                {
                    var rowItem = row.Result.FirstOrDefault();
                    if (rowItem != null)
                    {
                        if (!string.IsNullOrEmpty(rowItem.ContentValue))
                        {
                            rowItem.ContentValue = o.ContentValue;
                            var rowResult = _ISpecContentValueService.Update(rowItem);
                            var res = _uow.SaveChanges();
                        }
                        else
                        {
                            var rowResult = _ISpecContentValueService.Delete(rowItem);
                            var res = _uow.SaveChanges();
                        }

                    }
                    else
                    {
                        var res = _ISpecContentValueService.InsertOrUpdate(o);
                        insertAll.Add(res);
                    }

                }
                else
                {

                }


            });
            return Ok(insertAll);
        }

    }
}
