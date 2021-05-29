using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class DiscordBotConfig
    {
       public string Token { get; }

        public DiscordBotConfig(IConfiguration config)
        {
            this.Token = config["discord_token"];
            if (string.IsNullOrEmpty(Token))
            {
                throw new ArgumentNullException("Invalid token");
            }
        }
        
    }
}
