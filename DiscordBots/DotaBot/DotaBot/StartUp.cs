  using Discord;
using Discord.Audio;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Core.Cache;
using DiscordBot.Core.Command;
using DotaBot.Managers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    public class StartUp
    {
        private readonly DiscordBotInitializer discordBotInitializer;
        private readonly HeroManager heroManager;

        public StartUp(DiscordBotInitializer discordBotInitializer,
                       ICacheManager cacheManager,
                       HeroManager heroManager)
        {
            this.discordBotInitializer = discordBotInitializer;
            this.heroManager = heroManager;
        }

        public async Task Start()
        {
            await discordBotInitializer.Initialize();
            await heroManager.Initialize();
        }    
    }
}
