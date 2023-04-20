using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCommBot.Providers
{
    public interface ITelegramProvider
    {
        Task Initialize(string api_id, string api_hash);
        Task<string> LogIn(string loginInfo);
    }
}
