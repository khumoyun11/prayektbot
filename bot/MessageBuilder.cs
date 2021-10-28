using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace bot
{
    public class MessageBuilder
    {
        public static ReplyKeyboardMarkup DateButtonEn()
            => new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                            {
                                new List<KeyboardButton>()
                                {
                                    new KeyboardButton(){ Text = "Today" },
                                    new KeyboardButton(){ Text = "Back" },
                                }
                            },
                ResizeKeyboard = true
            };
        public static ReplyKeyboardMarkup DateButtonRu()
            => new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                            {
                                new List<KeyboardButton>()
                                {
                                    new KeyboardButton(){ Text = "Сегодняшний" },
                                    new KeyboardButton(){ Text = "Назад" },
                                }
                            },
                ResizeKeyboard = true
            };
        public static ReplyKeyboardMarkup DateButtonUz()
            => new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                            {
                                new List<KeyboardButton>()
                                {
                                    new KeyboardButton(){ Text = "Bugungi" },
                                    new KeyboardButton(){ Text = "Orqaga" },
                                }
                            },
                ResizeKeyboard = true
            };
        public static ReplyKeyboardMarkup LanguagesButton()
            => new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                            {
                                new List<KeyboardButton>()
                                {
                                    new KeyboardButton(){ Text = "English" },
                                    new KeyboardButton(){ Text = "O'zbek" },
                                    new KeyboardButton(){ Text = "Русский" }
                                }
                            },
                ResizeKeyboard = true
            };
        public static ReplyKeyboardMarkup LocationRequestButtonRu()
            => new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                            {
                                new List<KeyboardButton>()
                                {
                                    new KeyboardButton(){ Text = "Поделиться локацией", RequestLocation = true },
                                    new KeyboardButton(){ Text = "Назад" } 
                                }
                            },
                ResizeKeyboard = true
            };
        public static ReplyKeyboardMarkup LocationRequestButtonEn()
            => new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                            {
                                new List<KeyboardButton>()
                                {
                                    new KeyboardButton(){ Text = "Share location", RequestLocation = true },
                                    new KeyboardButton(){ Text = "Back" } 
                                }
                            },
                ResizeKeyboard = true
            };

        public static ReplyKeyboardMarkup LocationRequestButtonUz()
            => new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                            {
                                new List<KeyboardButton>()
                                {
                                    new KeyboardButton(){ Text = "Lokatsiya jo'natish", RequestLocation = true },
                                    new KeyboardButton(){ Text = "Orqaga" } 
                                }
                            },
                ResizeKeyboard = true
            };
    }
}