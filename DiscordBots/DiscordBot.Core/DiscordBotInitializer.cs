using Discord;
using Discord.Audio;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Core.Cache;
using DiscordBot.Core.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class DiscordBotInitializer
    {
        DiscordSocketClient client;
        CommandHandler commandHandler;
        ICacheManager cacheManager;
        DiscordBotConfig options;
 
        public DiscordBotInitializer(DiscordSocketClient client,
                                        CommandHandler commandHandler,
                                        ICacheManager cacheManager,
                                        DiscordBotConfig options)
        {
            this.client = client;
            client.Log += Log;
            this.commandHandler = commandHandler;
            this.cacheManager = cacheManager;
            this.options = options;
         
        }
        public async Task Initialize()
        {        
            var startTasks = new List<Task>()
            {
                cacheManager.Initialize(),
                StartClient(),
                commandHandler.InstallCommandsAsync()
            };

            await Task.WhenAll(startTasks);  
            
        }
        private async Task StartClient()
        {
            await client.LoginAsync(TokenType.Bot, options.Token);
            await client.StartAsync();
        }
         
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
  
    }
}
