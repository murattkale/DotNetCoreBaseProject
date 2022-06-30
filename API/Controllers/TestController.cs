using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class TestController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ITestService _ITestService;
        public TestController(IUnitOfWork<myDBContext> _uow, ITestService _ITestService)
        {
            this._uow = _uow;
            this._ITestService = _ITestService;
        }

        [HttpPost("Send")]
        public IActionResult Send(Test postModel)
        {
            var row = _ITestService.Where(o => o.Aile.ToLower() == postModel.Aile.ToLower(), false).Result.FirstOrDefault();
            if (row != null)
            {
                row.Sayi = postModel.Sayi;
                var result = _ITestService.InsertOrUpdate(row);
                var res = _uow.SaveChanges();
                return Ok("dup");
            }

            var result1 = _ITestService.InsertOrUpdate(postModel);
            var res1 = _uow.SaveChanges();

            return Ok(res1);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ITestService.WhereList().ResultList.Select(o => new { Aile = o.Aile, Sayi = o.Sayi, KayıtTarihi = o.CreaDate, DuzenlemeTarihi = o.ModDate });
            return Ok(result);
        }

        [HttpGet("RowDelete")]
        public IActionResult RowDelete(string Aile)
        {
            var row = _ITestService.Where(o => o.Aile.ToLower() == Aile.ToLower(), false).Result.FirstOrDefault();
            var result = _ITestService.Delete(row);
            var res = _uow.SaveChanges();
            return Ok(result);
        }



    }
}
