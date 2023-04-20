using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCommBot.Models
{
    /// <summary>
    /// Telegram user
    /// </summary>
    public class TelegramUser
    {
        /// <summary>
        /// User id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// api_id
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// api_hash
        /// </summary>
        public string? Hash { get; set; }

        /// <summary>
        /// phone_number
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// verification_code
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// first_name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// last_name
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// password
        /// </summary>
        public string? Password { get; set; }
    }
}
