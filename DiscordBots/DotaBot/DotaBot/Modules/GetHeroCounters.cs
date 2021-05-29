using Discord;
using Discord.Commands;
using DiscordBot.Core.Cache;
using DotaBot.Managers;
using DotaBot.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotaBot.Modules
{
    public class GetHeroCounters : ModuleBase<SocketCommandContext>
    {
        ICacheManager cacheManager;
        public GetHeroCounters(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        [Summary("Lists heros that counter the entered hero")]
        [Command("counters", true ,RunMode = RunMode.Async)]
        public async Task Counters([Summary("Show counters for")] string hero)
        {
            if (string.IsNullOrWhiteSpace(hero))
            {
                await ReplyAsync("Valid hero name required");
            }
            else
            {
                var heroIndex = await cacheManager.GetObjectFromCache(new HeroIndex());
                var heroInfo = heroIndex.Heroes.Where(x => x.Key.ToLower().Contains(hero.ToLower())).Select(x=>x.Value).ToList();

                if (!heroInfo.Any())
                {
                    await ReplyAsync($"No counters found for {hero}");
                }
                else if (heroInfo.Count > 1)
                {
                    await ReplyAsync($"Found more than one hero {string.Join(", ", heroInfo.Select(x => x.Name).ToList())} ");
                }
                else
                {
                    var counterInfo = heroInfo.First();

                    //var message = $"Found the following counters for {counterInfo.Name} {Environment.NewLine}Countering Hero | Disadvantage" +
                    //    $"{Environment.NewLine}{string.Join(Environment.NewLine,counterInfo.Counters.Select(x=>$"{x.CounteringHeroName}|{x.Disadvantage}"))}";
                  //  await ReplyAsync(message);
                  
                }
               
            }
            
        }
    }
}
