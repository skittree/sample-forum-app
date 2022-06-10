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

        [HttpGet]
        [Route("{id}/topics")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var topics = await SectionService.GetTopicsById(id);
                return Ok(topics);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(SectionAddEditDto model)
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

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(SectionAddEditDto model, int id)
        {
            if (model == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                await SectionService.EditSection(model, id);
                return Ok();
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
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

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await SectionService.DeleteSection(id);
                return Ok();
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
