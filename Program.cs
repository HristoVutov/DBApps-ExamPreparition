using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DatabaseFootball
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new FootballEntities();
            //                1
            //var teams = context.Teams.Select(t => t.TeamName);
            //foreach (var item in teams)
            //{
            //    Console.WriteLine(item);
            //}
            
            //-----------------------------------------------------

            //                2
            //var leagues = context.Leagues
            //    .OrderBy(l => l.LeagueName)
            //    .Select(l => new { 
            //    leagueName = l.LeagueName,
            //    teams = l.Teams
            //    .OrderBy(t => t.TeamName)
            //    .Select(t => t.TeamName)
            //});

            //var serializer = new JavaScriptSerializer();
            //var riverJson = serializer.Serialize(leagues.ToList());
            //System.IO.File.WriteAllText(@"D:\DB\leagues.json", riverJson);

            //------------------------------------------------------------------

            //                    3

            //var internationalMatches = context.InternationalMatches
            //    .OrderBy(im => im.MatchDate)
            //    .ThenBy(im => im.HomeCountry.CountryName)
            //    .ThenBy(im => im.AwayCountry.CountryName)
            //    .Select(im => new
            //    {
            //        AwayCountryName = im.AwayCountry.CountryName,
            //        im.AwayCountryCode,
            //        HomeCountryName = im.HomeCountry.CountryName,
            //        im.HomeCountryCode,
            //        im.League.LeagueName,
            //        im.HomeGoals,
            //        im.AwayGoals,
            //        im.MatchDate
            //    });

            //var xmlMatches = new XElement("matches");
            //foreach (var item in internationalMatches)
            //{
                
                
            //    var xmlMatch = new XElement("match");
                
            //    if (item.MatchDate != null)
            //    {
            //        var matchDate = matchElementFormat(item.MatchDate);
            //        xmlMatch.Add(matchDate);
            //    }
            //    var homeCountry = new XElement("home-country",item.HomeCountryName);
            //    var awayCountry = new XElement("away-country",item.AwayCountryName);
            //    homeCountry.Add(new XAttribute("code", item.HomeCountryCode));
            //    awayCountry.Add(new XAttribute("code", item.AwayCountryCode));
            //    xmlMatch.Add(homeCountry);
            //    xmlMatch.Add(awayCountry);
            //    if (item.HomeGoals != null)
            //    {
            //        var score = new XElement("score",item.HomeGoals + "-" + item.AwayGoals);
            //        xmlMatch.Add(score);
            //    }

            //    if (item.LeagueName != null)
            //    {
            //        var league = new XElement("league", item.LeagueName);
            //        xmlMatch.Add(league);
            //    }
                
               
            //    xmlMatches.Add(xmlMatch);
            //}

            //var xmlDoc = new XDocument(xmlMatches);
            //xmlDoc.Save("international-matches.xml");

            //---------------------------------------------------------------------------------

            //                 4

            var xmlDoc = new XmlDocument();
            xmlDoc.Load("leagues-teams-import.xml");
            var root = xmlDoc.DocumentElement;
            int id = 1;
            
            

            foreach (XmlNode league in root.ChildNodes)
            {
                Console.WriteLine("Processing league #{0} ...",id++);
                XmlNode leagueNameNode = league.SelectSingleNode("league-name");
                string leagueName = "";
                IQueryable<League> leag;
                if (leagueNameNode != null)
                {
                     leagueName = leagueNameNode.InnerText;
                    if (context.Leagues.Any(l => l.LeagueName == leagueName))
                    {
                        Console.WriteLine("Existing league: " + leagueName);
                        leag = context.Leagues.Where(l => l.LeagueName == leagueName);
                    }
                    else
                    {
                        Console.WriteLine("Created league: " + leagueName);
                        leag = new League
                        {
                            LeagueName = leagueName
                        };
                    }
                   
                }

                
                XmlNode teams = league.SelectSingleNode("teams");
                if (teams != null)
                {
                    
                    foreach (XmlNode team in teams.ChildNodes)
                    {
                        var teamName = team.Attributes[0].InnerText;
                        string teamCountry = null;
                        Team contTeam = new Team{
                            TeamName = teamName,
                            CountryCode = CounryCodeByName(teamCountry).ToString()
                        };
                        if (team.Attributes["country"] != null)
                        {
                            teamCountry = team.Attributes["country"].InnerText;
                        }
                        if (context.Teams.Any(l => l.TeamName == teamName && l.Country.CountryName == teamCountry))
                        {
                            teamCountry = teamCountry ?? "no country";
                            Console.WriteLine("Existing team: {0} ({1})",teamName,teamCountry);
                        }
                        else
                        {
                            teamCountry = teamCountry ?? "no country";
                            Console.WriteLine("Created team: {0} ({1})", teamName, teamCountry);
                        }
                        if(leagueName!=""){
                            if(leag){
                                Console.WriteLine("Existing team in league: {0} belongs to {1}", teamName, leagueName);
                            }
                            else
                            {
                                
                                Console.WriteLine("Added team to league: {0} to league {1}", teamName, leagueName);
                            }
                        }
                        
                    }
                }
                Console.WriteLine();
            }
                    
              

        }

        private static IQueryable<string> CounryCodeByName(string name)
        {
             var context = new FootballEntities();
             return context.Countries.Where(c => c.CountryName == name).Select(c => c.CountryCode);
        }

        private static XAttribute matchElementFormat(DateTime? date)
        {
            DateTime dat = date.Value;
            if (dat.TimeOfDay.TotalSeconds == 0)
            {
                return new XAttribute("date", dat.ToString("dd-MMM-yyyy"));
            }
            else
            {
                return new XAttribute("date-time", dat.ToString("dd-MMM-yyyy hh:mm"));
            }
        }
    }
}
