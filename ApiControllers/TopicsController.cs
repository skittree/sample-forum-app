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
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService TopicService;

        public TopicsController(ITopicService topicService)
        {
            TopicService = topicService;
        }

        [HttpGet]
        [Route("{id}/messages")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var messages = await TopicService.GetMessagesByTopicId(id);
                return Ok(messages);
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
        [Route("{id}/messages")]
        public async Task<IActionResult> Post(MessageAddEditDto model, int id)
        {
            if (model == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                await TopicService.AddMessageByTopicId(model, id);
                return Ok();
            }
            catch (KeyNotFoundException ae)
            {
                return NotFound(ae.Message);
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
        public async Task<IActionResult> Put(TopicAddEditDto model, int id)
        {
            if (model == null)
            {
                return BadRequest("object was null");
            }
            try
            {
                await TopicService.EditTopic(model, id);
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
                await TopicService.DeleteTopic(id);
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
