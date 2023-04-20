using AutoCommBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace AutoCommBot.Providers
{
    public class TelegramProvider : ITelegramProvider
    {
        private static WTelegram.Client _client;

        public async Task Initialize(string api_id, string api_hash)
        {
             _client = new WTelegram.Client(Convert.ToInt32(api_id), api_hash);
        }

        public async Task<string> LogIn(string loginInfo)
        {
            if(_client.User == null)
            {
                switch (await _client.Login(loginInfo)) // returns which config is needed to continue login
                {
                    case "verification_code": 
                        return "На ваш номер телефона был выслан код подтверждения. Введите его без пробелов и запятых после знака $, в формате: $123456"; 
                    case "name":
                        return "Также дополнительно нужно указать ваше имя и фамилию, как указанно в вашем профиле, через пробел, после знака -, в формате: -Иван Иванов";
                    case "password":
                        return "В заключении укажите пароль от аккаунта, без проблов и запятых, после знака !, в формате: !Ahdebbwu765";
                    default: 
                        return "Пользователь авторизирован!";
                }
            }
            return "Авторизация не требуется";
        }
    }
}
