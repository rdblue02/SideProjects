using DiscordBot.Core;
using DiscordBot.Core.Cache;
using DotaBot.Models;
using DotaBot.Requests;
using DotaBot.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotaBot.Managers
{
    public class HeroManager:IDisposable
    {
        private readonly ICacheManager cacheManager;
        private readonly AppTimeManager appTimeManager;
        private readonly CancellationTokenSource heroManagerCancellationToken;
        private Task refreshTask;
        public HeroManager(ICacheManager cacheManager, AppTimeManager appTimeManager)
        {
            this.cacheManager = cacheManager;
            this.appTimeManager = appTimeManager;
            this.heroManagerCancellationToken = new CancellationTokenSource();
        }
        public async Task Initialize()
        {
            HeroIndex heroIndex = new HeroIndex();
            var result = await this.cacheManager.GetObjectFromCache(heroIndex,  out heroIndex);

            if (heroIndex == null || heroIndex.InitializedDate.AddDays(7) < DateTime.Now)
            {
                using (var client = new HttpClient())
                {
                    heroIndex = await FetchHeroIndexFromDotaBuff(client);
                    await cacheManager.UpdateOrAddCachedItem(heroIndex);
                }
            }
            refreshTask = await Task.Factory.StartNew(CheckIfRefreshRequired, TaskCreationOptions.LongRunning);
        }
       
        private async Task CheckIfRefreshRequired()
        {
            while (!this.heroManagerCancellationToken.IsCancellationRequested)
            {
                if (this.appTimeManager.ElapsedTime > TimeSpan.FromDays(7))
                {
                    using (var client = new HttpClient())
                    {
                        var heroIndex = await FetchHeroIndexFromDotaBuff(client);
                        await this.cacheManager.UpdateOrAddCachedItem(heroIndex);
                    }
                    Console.WriteLine($"Elapsed time since last refresh {this.appTimeManager.ElapsedTime}. Refreshing cache values");
                    await this.appTimeManager.ResetTimer();
                }       
            }            
        }

        private async Task<HeroIndex> FetchHeroIndexFromDotaBuff(HttpClient client)
        {
            var heroesTask = new HeroRequests.HeroListRequest().Send(client); 
            var dAndHTask = new HeroRequests.HeroDAndHRequest().Send(client);
            var econTask = new HeroRequests.EconomyRequest().Send(client);
            var winRateTask = new HeroRequests.WinRateRequest().Send(client);       
            var heroMatchUpTasks = new List<Task<(string id,List<HeroMatchUp> matchUps)>>();
            var heroItemTasks = new List<Task<(string id, List<Item> items)>>();        
            var heroes = (await heroesTask).Heroes;

        
            foreach(var hero in heroes)
            {
                var matchUpRequest = new HeroRequests.HeroMatchUpRequest(hero.DotaBuffSanatizedName).Send(hero.Name,client);
                heroMatchUpTasks.Add(matchUpRequest);
                var itemStatRequest = new HeroRequests.HeroItemStatRequest(hero.DotaBuffSanatizedName).Send(hero.Name, client);
                heroItemTasks.Add(itemStatRequest);
            }
          
            var dAndH = await dAndHTask;
            var econ = await econTask;
            var winRate = await winRateTask;

            foreach (var hero in heroes)
            {
                var heroDAndH = dAndH.First(x => x.HeroName.ToLower().Trim() == hero.Name.ToLower().Trim());
                var heroEcon = econ.First(x => x.HeroName.ToLower().Trim() == hero.Name.ToLower().Trim());
                var heroWinRate = winRate.First(x => x.HeroName.ToLower().Trim() == hero.Name.ToLower().Trim());
                hero.HeroStats = new HeroStats()
                {
                      DamagerPerMinute = heroDAndH.DamagerPerMinute,
                      TowerDamagePerMinute = heroDAndH.TowerDamagePerMinute,
                      HealingPerMinute = heroDAndH.HealingPerMinute,
                      GoldPerMinute = heroEcon.GPM,
                      XPPerMinute = heroEcon.XPM,
                      KDRatio =  heroWinRate.KDRatio,
                      PickRate = heroWinRate.PickRate,
                      WinRate = heroWinRate.WinRate
                };                
            }

           var matchUps =  await Task.WhenAll(heroMatchUpTasks);
            foreach(var hero in heroes)
            {
                var heroMatchUps = matchUps.First(x => x.id.ToLower().Trim() == hero.DotaBuffSanatizedName.ToLower().Trim());
                hero.HeroMatchUp = heroMatchUps.matchUps;
            }

            var itemStats = await Task.WhenAll(heroItemTasks);
            foreach (var hero in heroes)
            {
                var heroitemStats = itemStats.First(x => x.id.ToLower().Trim() == hero.DotaBuffSanatizedName.ToLower().Trim());
                hero.ItemStats = heroitemStats.items;
            }
            var heroIndex = new HeroIndex();

            foreach(var hero in heroes)
            {
                heroIndex.Heroes.Add(hero.Name, hero);
            }
            return heroIndex;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.heroManagerCancellationToken.Cancel();
                this.refreshTask.Wait();
                this.heroManagerCancellationToken.Dispose();
                this.refreshTask.Dispose();
            }
        }
    }
}
