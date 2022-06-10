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

/*        [HttpGet]
        public async Task<IActionResult> Get()
        {

        }

        [HttpGet]
        [Route("{id}/topics")]
        public async Task<IActionResult> Get(int id)
        {

        }

        [HttpPost]
        public async Task<IActionResult> Post(SectionAddEditDto model)
        {

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(SectionAddEditDto model, int id)
        {

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

        }*/
    }
}
