using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceReference_RussianPost;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using WebTelegram.Models;

namespace TelegramPostBotMVC.Models
{
	public class RussianPost : Post
	{
		private const string Version = "v 1.1";
		public override async void GetRequestFromTelegramBot(Update request)
		{
			if (request.message == null) { return; }

			long chatId = request.message.chat.id;
			string message = request.message.text;
			
			switch (message)
			{
				case "/start":
					StringBuilder caption = new StringBuilder();

					caption
						.Append("Вас приветствует Бот отслеживания почтовых отправлений! ")
						.AppendLine(Version)
						.Append("Введите номер почтового отправления:");
					
					await Bot.SendTextMessageAsync(chatId, caption.ToString());
					break;
				default:
					//логика обработки трека
					try
					{
						Task<object> response = GetPostData(message);
						SendPostMessageToTelegramBot(request, response.Result);
					}
					catch (Exception ex)
					{
						await Bot.SendTextMessageAsync(chatId, ex.Message);
					}
					break;
			}
		}
		public override async void SendPostMessageToTelegramBot(Update request, object responseFromPostService)
	    {
		    
			getOperationHistoryResponse response = responseFromPostService as getOperationHistoryResponse;
		    if (response == null)
		    {
			    //сообщение об ошибке
			    return;
		    }

			string caption = "";

			StringBuilder sbCaption = new StringBuilder();

			long idChat = request.message.chat.id;

			List<OperationHistoryRecord> operationList = new List<OperationHistoryRecord>(response.OperationHistoryData);

			//operationList.Reverse();

		    DateTime dateTime;
			foreach (var r in operationList)
			{
				StringBuilder sbDateTime = new StringBuilder();

				dateTime = r.OperationParameters.OperDate;

				string day = dateTime.Day < 10 ? "0" + dateTime.Day.ToString() : dateTime.Day.ToString();
				string month = dateTime.Month < 10 ? "0" + dateTime.Month.ToString() : dateTime.Month.ToString();
				string hour = dateTime.Hour < 10 ? "0" + dateTime.Hour.ToString() : dateTime.Hour.ToString();
				string minute = dateTime.Minute < 10 ? "0" + dateTime.Minute.ToString() : dateTime.Minute.ToString();

				sbDateTime
					.Append(day)
					.Append(".")
					.Append(month)
					.Append(".")
					.Append(r.OperationParameters.OperDate.Year)
					.Append(" - ")
					.Append(hour)
					.Append(":")
					.Append(minute);

				string typeName = r.OperationParameters.OperType.Name;

				string attrName = r.OperationParameters.OperAttr.Name;

				string operStatus = string.IsNullOrEmpty(attrName) ? typeName : typeName + "-" + attrName;
				
				string index = r.AddressParameters.OperationAddress.Index;

				string discr = r.AddressParameters.OperationAddress.Description;

				string operLocation = string.IsNullOrEmpty(index) ? discr : index + " " + discr;
				
				

				sbCaption
					.AppendLine("<b>" + sbDateTime + "</b>")
					.AppendLine("<pre>" + operStatus + "</pre>")
					.AppendLine("<code>" + operLocation + "</code>");
					
				await Bot.SendTextMessageAsync(idChat, caption, true, false, 0, null, ParseMode.Html);
			}

			//await bot.SendTextMessageAsync(idChat, seperator);
			string messageText = request.message.text;
			
		    sbCaption.Clear();

		    sbCaption
			    .AppendLine("<b>Дополнительная информация по адресу:</b>")
			    .Append("<a href=\"https://www.pochta.ru/tracking#" + messageText + "\">")
			    .Append(messageText)
			    .Append("</a>");

			await Bot.SendTextMessageAsync(idChat, caption, true, false, 0, null, ParseMode.Html);

			caption = "Введите номер почтового отправления:";
			await Bot.SendTextMessageAsync(idChat, caption, true, false, 0, null, ParseMode.Html);
		}
	    public override async Task<object> GetPostData(string trackNumber)
	    {
			
			OperationHistoryRequest ohr = new OperationHistoryRequest();
			ohr.Barcode = trackNumber;
			ohr.MessageType = 0;

			AuthorizationHeader ah = new AuthorizationHeader();
			ah.login = "xOmeLwsajuASmo";
			ah.password = "R6K4avbste01";


			getOperationHistoryRequest req = new getOperationHistoryRequest(ohr, ah);

			//getOperationHistoryResponse resp = new getOperationHistoryResponse(new OperationHistoryRecord[100]);
			OperationHistory12 history = new OperationHistory12Client(OperationHistory12Client.EndpointConfiguration.OperationHistory12Port);

			return await history.getOperationHistoryAsync(req);

		}
		
	    
    }
}
