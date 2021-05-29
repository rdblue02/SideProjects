﻿using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Core.Cache;
using DiscordBot.Core.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace OuterHeavenBot
{
    class Program
    {
        public static void Main(string[] args)
         => new Program().MainAsync(args).GetAwaiter().GetResult();


        public async Task MainAsync(string[] args)
        {
            try
            {
                using IHost host = CreateHostBuilder(args).Build();
                await host.Services.GetRequiredService<StartUp>().Start();
                await host.RunAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
             
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((_, services) => {
                   services.AddSingleton<ICacheManager, JsonCacheManager>();
                   services.AddSingleton<ICommandOptions,CustomCommandOptions>();
                   services.AddSingleton<CommandService>();
                   services.AddSingleton<DiscordSocketClient>();
                   services.AddSingleton<DiscordBotConfig>();
                   services.AddSingleton<DiscordBotInitializer>();
                   services.AddSingleton<CommandHandler>();
                   services.AddSingleton(new Random((int)DateTime.Now.Ticks));
                   services.AddSingleton<StartUp>();
               });
    }
}
