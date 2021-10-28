using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using bot.Entity;
using bot.HttpClients;
using bot.Services;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Topten.RichTextKit;

namespace bot
{
    public class Handlers
    {
        private readonly ILogger<Handlers> _logger;
        private readonly IStorageService _storage;
        private readonly ICacheService _cache;

        public Handlers(
            ILogger<Handlers> logger, 
            IStorageService storage,
            ICacheService cache)
        {
            _logger = logger;
            _storage = storage;
            _cache = cache;
        }

        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ctoken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException => $"Error occured with Telegram Client: {exception.Message}",
                _ => exception.Message
            };

            _logger.LogCritical(errorMessage);

            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ctoken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(client, update.Message),
                UpdateType.EditedMessage => BotOnMessageEdited(client, update.EditedMessage),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(client, update.CallbackQuery),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(client, update.InlineQuery),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(client, update.ChosenInlineResult),
                _ => UnknownUpdateHandlerAsync(client, update)
            };

            try
            {
                await handler;
            }
            catch(Exception e)
            {

            }
        }

        private async Task BotOnMessageEdited(ITelegramBotClient client, Message editedMessage)
        {
            throw new NotImplementedException();
        }

        private async Task UnknownUpdateHandlerAsync(ITelegramBotClient client, Update update)
        {
            throw new NotImplementedException();
        }

        private async Task BotOnChosenInlineResultReceived(ITelegramBotClient client, ChosenInlineResult chosenInlineResult)
        {
            throw new NotImplementedException();
        }

        private async Task BotOnInlineQueryReceived(ITelegramBotClient client, InlineQuery inlineQuery)
        {
            throw new NotImplementedException();
        }

        private async Task BotOnCallbackQueryReceived(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }
        

        private async Task BotOnMessageReceived(ITelegramBotClient client, Message message)
        {
            if(message.Type == MessageType.Location && message.Location != null)
            {
                var result = await _cache.GetOrUpdatePrayerTimeAsync(message.Chat.Id, message.Location.Latitude, message.Location.Longitude);
                var times = result.prayerTime;

                // await client.SendPhotoAsync(
                //     chatId: message.Chat.Id,
                //     getImageFile(times));
                
                // _logger.LogInformation("hi");

                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: getTimeString(times),
                    parseMode: ParseMode.Markdown,
                    replyMarkup: MessageBuilder.LocationRequestButtonEn());
            }
            switch(message.Text)
            {
                case "/start": 
                    await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            parseMode: ParseMode.Markdown,
                            text: "English | O'zbek | Русский?",
                            replyMarkup: MessageBuilder.LanguagesButton());   
                    if(!await _storage.ExistsAsync(message.Chat.Id))
                    {
                        var user = new BotUser(
                            chatId: message.Chat.Id,
                            username: message.From.Username,
                            fullname: $"{message.From.FirstName} {message.From.LastName}",
                            latitude: message.Location.Latitude,
                            longitude: message.Location.Longitude,
                            address: string.Empty,
                            language: message.From.LanguageCode);

                        var result = await _storage.InsertUserAsync(user);

                        if(result.IsSuccess)
                        {
                            _logger.LogInformation($"New user added: {message.Chat.Id}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"User exists");
                    }
                    await client.DeleteMessageAsync(
                        chatId: message.Chat.Id,
                        messageId: message.MessageId); break;
                case "English":
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        parseMode: ParseMode.Markdown,
                        text: "Assalomu aleykum, Can you share your location?",
                        replyMarkup: MessageBuilder.LocationRequestButtonEn());
                }; break;

                case "Русский":
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        parseMode: ParseMode.Markdown,
                        text: "Ассалому алейкум, Можете отправить вашу локацию?",
                        replyMarkup: MessageBuilder.LocationRequestButtonRu());
                }; break;
                case "O'zbek":
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        parseMode: ParseMode.Markdown,
                        text: "Assalomu aleykum, Lokatsiyangizni jo'nata olasizmi?",
                        replyMarkup: MessageBuilder.LocationRequestButtonUz());
                    
                }; break;
                case "Back":
                await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            parseMode: ParseMode.Markdown,
                            text: "Select language?",
                            replyMarkup: MessageBuilder.LanguagesButton()); break;
                case "Назад":
                await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            parseMode: ParseMode.Markdown,
                            text: "Выберите язык?",
                            replyMarkup: MessageBuilder.LanguagesButton()); break;
                case "Orqaga":
                await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            parseMode: ParseMode.Markdown,
                            text: "Tilni tanglang?",
                            replyMarkup: MessageBuilder.LanguagesButton()); break; 
                // case "Today":
                // var res = await _cache.GetOrUpdatePrayerTimeAsync(message.Chat.Id, message.Location.Latitude, message.Location.Longitude);
                // var times = res.prayerTime;
                // await client.SendTextMessageAsync(
                //     chatId: message.Chat.Id,
                //     text: getTimeString(times),
                //     replyMarkup: MessageBuilder.LocationRequestButtonEn()); break;
            }    
        }
        public Stream getImageFile(Models.PrayerTime times)
        {
            var text = getTimeString(times);
            using (var surface = SKSurface.Create(new SKImageInfo(500, 500)))
            {
                Draw(surface, text);
                using var image = surface.Snapshot();

                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                return data.AsStream();
            }
        }

        public void Draw(SKSurface surface, string text)
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.LightGray);

            // Find the canvas bounds
            var canvasBounds = canvas.DeviceClipBounds;

            // Create the text block
            var tb = new TextBlock();

            // Configure layout properties
            tb.MaxWidth = canvasBounds.Width * 0.8f;
            tb.Alignment = TextAlignment.Left;

            var style = new Style()
            {
                FontSize = 24
            };

            // Add text to the text block
            tb.AddText(text, style);

            // Paint the text block
            tb.Paint(canvas, new SKPoint(canvasBounds.Width * 0.1f, canvasBounds.Height * 0.1f));
        }
        public string getTimeString(Models.PrayerTime times)
            => $" *Fajr*: {times.Fajr}\n*Sunrise*: {times.Sunrise}\n*Dhuhr*: {times.Dhuhr}\n*Asr*: {times.Asr}\n*Maghrib*: {times.Maghrib}\n*Isha*: {times.Isha}\n*Midnight*: {times.Midnight}\n\n*Method*: {times.CalculationMethod}";
    

        // private string prayertimeF_En(Models.PrayerTime times)
        //     => $"Fajr: {times.Fajr}\nSunrise: {times.Sunrise}\nDhuhr: {times.Dhuhr}\nAsr: {times.Asr}\nMaghrib: {times.Maghrib}\nIsha: {times.Isha}\n\nMethod: {times.CalculationMethod}";
        // private string prayertimeF_Uz(Models.PrayerTime times)
        //     => $"Bomdod: {times.Fajr}\nQuyosh chiqishi: {times.Sunrise}\nPeshin: {times.Dhuhr}\nAsr: {times.Asr}\nShom: {times.Maghrib}\nXufton: {times.Isha}\n\nMethod: {times.CalculationMethod}";
        // private string prayertimeF_Ru(Models.PrayerTime times)
        //     => $"Бомдод: {times.Fajr}\nВосход: {times.Sunrise}\nПешин: {times.Dhuhr}\nАср: {times.Asr}\nШом: {times.Maghrib}\nХуфтон: {times.Isha}\n\nMethod: {times.CalculationMethod}";
    
    }
}