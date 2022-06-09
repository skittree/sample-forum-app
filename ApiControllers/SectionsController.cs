using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task3.Services;
using Task3.DtoModels;

namespace Task3.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly ISectionService SectionService;

        public SectionsController(ISectionService sectionService)
        {
            SectionService = sectionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var sections = await SectionService.GetAllSections();
                return Ok(sections);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(SectionAddDto model)
        {
            if (model == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                await SectionService.AddSection(model);
                return Ok();
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
