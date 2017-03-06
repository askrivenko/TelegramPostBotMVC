using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TelegramPostBotMVC.Models;
using WebTelegram.Models;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramPostBotMVC.Controllers
{
    [Route("[controller]")]
    public class RussianPostController : Controller
    {
        // GET: api/values
        [HttpGet]
		public string Get()
		{
			return "Служба отслеживания почтовых отправлений запущена!\nМодуль - Почта России";
		}

		// GET api/values/5
		[HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Update request)
        {
			Post russianPost = new RussianPost();
			russianPost.GetRequestFromTelegramBot(request);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
