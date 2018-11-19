using ElateService.BLL.Interfaces;
using ElateService.BLL.ModelsDTO;
using ElateService.BLL.PaginationDTO;
using NinjectApi.Models;
using NinjectApi.Safe_execution;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace NinjectApi.Controllers
{
    [RoutePrefix("api/Indents"), APIException]
    public class IndentController : ApiController
    {
        private IIndentService _indentService;

        public IndentController(IIndentService service)
        {
            _indentService = service;
        }


        ///<summary>
        ///Returns a list of indents represented on first page
        ///</summary>
        ///<responce code="200">Indents returned</responce>
        ///<responce code="404">Indents not returned</responce>
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Indents returned", typeof(IEnumerable<IndentDTO>))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Indents not returned")]
        [Route("")]
        [Authorize]
        public async Task<IHttpActionResult> Get()
        {
            IndentDTOPage singleIndentPageInfo = await _indentService.GetIndentsPerPage(1, 5, null);
            var indents = singleIndentPageInfo.IndentsOnPage;

            if (indents != null)
            {
                return Ok(indents);
            }

            return NotFound();
        }
    }
}
