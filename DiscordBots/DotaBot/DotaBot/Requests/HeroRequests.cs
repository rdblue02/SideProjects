using DiscordBot.Core;
using DotaBot.Models;
using DotaBot.Responses;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.Requests
{
    public class HeroRequests
    {
        public class HeroListRequest : DotabuffRequest<HeroResponses.HeroListResponse>
        {
            protected override string Path { get; set; } = "/heroes";
            protected override Task<HeroResponses.HeroListResponse> ParseServerResponse(HttpResponseMessage message)
            {
                var content = Helpers.DecompressStringFromStream(message.Content.ReadAsStream());
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);
                var names = htmlDocument.DocumentNode.SelectNodes("//body/div/div/div/div/section/footer/div/a");

                var response = new HeroResponses.HeroListResponse();
                foreach (var name in names)
                {
                    var hero = new Hero();
                    var namePath = name.GetAttributeValue("href", "");
                    var sanatized = namePath.Substring("/heroes/".Length);
                    
                    hero.DotaBuffSanatizedName = sanatized;
                    hero.Name = name.FirstChild.InnerText;
                    response.Heroes.Add(hero);
                }

                return Task.FromResult(new HeroResponses.HeroListResponse());
            }      
        }

        public class HeroDAndHRequest : DotabuffRequest<List<HeroResponses.HeroDAndHResponse>>
        {
            protected override string Path { get; set; } = "/heroes/damage";

            protected override Task<List<HeroResponses.HeroDAndHResponse>> ParseServerResponse(HttpResponseMessage message)
            {
                var heroHAndDs = new List<HeroResponses.HeroDAndHResponse>();
               
                var content = Helpers.DecompressStringFromStream(message.Content.ReadAsStream());
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);

                var heroNodes = htmlDocument.DocumentNode.SelectNodes("//table/tbody/tr");

                foreach(var node in heroNodes)
                {
                    var heroHAndD = new HeroResponses.HeroDAndHResponse();

                    heroHAndD.HeroName = node.SelectNodes("//td")[1].InnerText;
                    heroHAndD.DamagerPerMinute = node.SelectNodes("//td")[2].InnerText;
                    heroHAndD.TowerDamagePerMinute = node.SelectNodes("//td")[3].InnerText;
                    heroHAndD.HealingPerMinute= node.SelectNodes("//td")[4].InnerText;
                    heroHAndDs.Add(heroHAndD);
                }

                return Task.FromResult(heroHAndDs);
            }
        }

        public class EconomyRequest : DotabuffRequest<List<HeroResponses.EconomyResponse>>
        {
            protected override string Path { get; set; } = "/heroes/economy";

            protected override Task<List<HeroResponses.EconomyResponse>> ParseServerResponse(HttpResponseMessage message)
            {
                var heroEcons = new List<HeroResponses.EconomyResponse>();
                var content = Helpers.DecompressStringFromStream(message.Content.ReadAsStream());
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);

                var heroNodes = htmlDocument.DocumentNode.SelectNodes("//table/tbody/tr");
                foreach (var node in heroNodes)
                {
                    var heroEcon = new HeroResponses.EconomyResponse();

                    heroEcon.HeroName = node.SelectNodes("//td")[1].InnerText;
                    heroEcon.GPM = node.SelectNodes("//td")[2].InnerText;
                    heroEcon.XPM = node.SelectNodes("//td")[3].InnerText; 
                    heroEcons.Add(heroEcon);
                }

                return Task.FromResult(heroEcons);
            }
        }
        public class WinRateRequest : DotabuffRequest<List<HeroResponses.WinRateResponse>>
        {
            protected override string Path { get; set; } = "/heroes/winning";
            protected override Task<List<HeroResponses.WinRateResponse>> ParseServerResponse(HttpResponseMessage message)
            {
                var heroWinRates = new List<HeroResponses.WinRateResponse>();
                var content = Helpers.DecompressStringFromStream(message.Content.ReadAsStream());
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);

                var heroNodes = htmlDocument.DocumentNode.SelectNodes("//table/tbody/tr");
                foreach (var node in heroNodes)
                {
                    var heroWinRate = new HeroResponses.WinRateResponse();

                   heroWinRate.HeroName = node.SelectNodes("//td")[1].InnerText;
                   heroWinRate.WinRate = node.SelectNodes("//td")[2].InnerText;
                   heroWinRate.PickRate= node.SelectNodes("//td")[3].InnerText;
                   heroWinRate.KDRatio = node.SelectNodes("//td")[4].InnerText;
                   heroWinRates.Add(heroWinRate);
                }

                return Task.FromResult(heroWinRates);
            }
        }

        public class HeroMatchUpRequest : DotabuffRequest<List<HeroMatchUp>>
        {
            protected override string Path { get; set; }
            public HeroMatchUpRequest(string requestedHero)
            {
                this.Path = "/heroes/" + requestedHero + "/counters";
            }
            protected override Task<List<HeroMatchUp>> ParseServerResponse(HttpResponseMessage message)
            {
                var matchUps = new List<HeroMatchUp>(); 
                var content = Helpers.DecompressStringFromStream(message.Content.ReadAsStream());
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);
              
                var heroNodes = htmlDocument.DocumentNode.SelectNodes("//section/article/table/tbody").Last().ChildNodes;

                foreach (var node in heroNodes)
                {
                    var matchUp = new HeroMatchUp();

                    matchUp.HeroName = node.SelectNodes("//td")[1].InnerText;
                    matchUp.Advantage= node.SelectNodes("//td")[2].InnerText;
                    matchUp.WinRate = node.SelectNodes("//td")[3].InnerText;
                    matchUps.Add(matchUp);
                }
                return Task.FromResult(matchUps);
            }
        }
        public class HeroItemStatRequest : DotabuffRequest<List<Item>>
        {
            protected override string Path { get; set; }
            public HeroItemStatRequest(string requestedHero)
            {
                this.Path = "/heroes/" + requestedHero + "/items";
            }
            protected override Task<List<Item>> ParseServerResponse(HttpResponseMessage message)
            {
               var itemStats = new List<Item>();

                var content = Helpers.DecompressStringFromStream(message.Content.ReadAsStream());
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);

                var heroNodes = htmlDocument.DocumentNode.SelectNodes("//section/article/table/tbody").Last().ChildNodes;

                foreach (var node in heroNodes)
                {
                    var itemStat = new Item();

                    itemStat.Name = node.SelectNodes("//td")[1].InnerText;
                    itemStat.MatchesUsed = node.SelectNodes("//td")[2].InnerText;
                    itemStat.WinRate = node.SelectNodes("//td")[3].InnerText;
                    itemStats.Add(itemStat);
                }

                return Task.FromResult(itemStats);
            }
        }
    }
}
