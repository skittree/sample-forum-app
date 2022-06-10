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
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService MessageService;

        public MessagesController(IMessageService messageService)
        {
            MessageService = messageService;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(MessageAddEditDto model, int id)
        {
            if (model == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                await MessageService.EditMessage(model, id);
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
                await MessageService.DeleteMessage(id);
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
