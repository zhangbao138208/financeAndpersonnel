using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DncZeus.Api.Services
{
    public class TelegramService
    {
        private readonly TelegramBotClient _telegramBot;
        public TelegramService()
        {
            this._telegramBot = new TelegramBotClient("1312555643:AAHUVb4lGKx8yrQt8kSK_hbx7KtC4ydjcsQ");
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="telegramId">编号</param>
        /// <param name="msg">消息内容</param>
        /// <returns></returns>
        public async Task SendTextMessageAsync(string telegramId, string msg)
        {



            foreach (var number in telegramId.Split(','))
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        var chat = new ChatId(string.IsNullOrEmpty(number) ? -1001485277950 : Convert.ToInt64(number));
                        var resultMessage = await _telegramBot.SendTextMessageAsync(chat, msg);
                        // LogHelper.Information($"小飞机：{number} {msg}\r\n{resultMessage.ToJson()}");
                    });
                }
                catch (Exception e)
                {
                    // LogHelper.WriteLog(e.InnerException?.Message ?? e.Message, e);
                }
                finally
                {
                    Thread.Sleep(1);
                }
            }


        }
    }
}
