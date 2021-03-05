using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using VoteTempsReel.Models;

namespace VoteTempsReel
{
    public class VotingHub : Hub
    {
        
        private PollEntities db = new PollEntities();
        public Polling quiz=new Polling();

            
        public static Dictionary<string, int> poll = new Dictionary<string, int>()
        {
            {"Boudani Aziza",1},
            {"Akhatar Hassan",3},
            {"Naouiss Hamza",2},
            {"Benzid Tarik",3},
            {"Elidris Ali",1},
            {"Moussaid Dounia",2},
            {"Achach Siham",1},
            {"Laoulidi Fatima Zohra",2},
           
        };

        public void send(string name)
        {
            
            poll[name]++;
            string data = JsonConvert.SerializeObject(poll.Select(x => new { name = x.Key, count = x.Value }).ToList());

            Clients.All.showLiveResult(data);
        }
    }
}