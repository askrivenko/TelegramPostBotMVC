using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using WebTelegram.Models;

namespace TelegramPostBotMVC.Models
{
    public abstract class Post
    {
		public TelegramBotClient Bot { get; set; }
		public abstract void GetRequestFromTelegramBot(Update request);
		public abstract void SendPostMessageToTelegramBot(Update request, Object responseFromPostService);
		public abstract Task<object> GetPostData(string trackNumber);
		protected Post()
		{
			Bot = new TelegramBotClient("352425198:AAGm-foe_-29YU2067-MblBWwod3PFjF8q0");
		}
		
	}
}
