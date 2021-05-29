using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotaBot.Managers;
using System.Diagnostics;
using DiscordBot.Core.Cache;
using DiscordBot.Core;
using Discord.WebSocket;
using Discord.Commands;
using DiscordBot.Core.Command;
using Microsoft.Extensions.Configuration;

namespace DotaBot
{
    class Program
    {
        public static void Main(string[] args)
         => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();        
            await host.Services.GetRequiredService<StartUp>().Start();
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)            
               .ConfigureServices((_, services) => {                 
                   services.AddSingleton<ICacheManager,JsonCacheManager>();                
                   services.AddSingleton<CommandService>();
                   services.AddSingleton<ICommandOptions, DefaultCommandOptions>();
                   services.AddSingleton<CommandHandler>();
                   services.AddSingleton<DiscordSocketClient>();
                   services.AddSingleton<DiscordBotConfig>();
                   services.AddSingleton<DiscordBotInitializer>(); 
                   services.AddSingleton<AppTimeManager>();
                   services.AddSingleton<HeroManager>();
                   services.AddSingleton<StartUp>();
               });
    }
}
