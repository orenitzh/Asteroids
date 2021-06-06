using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asteroids.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        [HttpGet("history")]
        public List<Player> Pull()
        {
            List<Player> players = DB.PullData<Player>("SELECT PlayerID, PlayerName FROM Players", (dr) => new Player
            {
                PlayerID = dr.GetInt32(0),
                PlayerName = dr.GetString(1),
                PlayerBestScore = getPlayerBestScore(dr.GetInt32(0))
        });
            players.Sort((a, b) => b.PlayerBestScore - a.PlayerBestScore);
            return players;
        }

        [HttpPost("inputPlayer")]

        public Player InputPlayer([FromForm] string PlayerName)
        {
            List<Player> players = DB.PullData<Player>("SELECT PlayerID, PlayerName FROM Players WHERE PlayerName=@PlayerName", (dr) => new Player
            {
                PlayerID = dr == null ? 0 : dr.GetInt32(0),
                PlayerName = dr == null ? "" : dr.GetString(1),
                PlayerBestScore = 0,
            }, (cmd) => cmd.Parameters.AddWithValue("@PlayerName", PlayerName));

            if(players.Count() == 0)
            {
                DB.Modify("INSERT INTO Players (PlayerName) VALUES(@PlayerName)", (cmd) => cmd.Parameters.AddWithValue("@PlayerName", PlayerName));
                players = DB.PullData<Player>("SELECT PlayerID, PlayerName FROM Players WHERE PlayerName=@PlayerName", (dr) => new Player
                {
                    PlayerID = dr.GetInt32(0),
                    PlayerName = dr.GetString(1),
                }, (cmd) => cmd.Parameters.AddWithValue("@PlayerName", PlayerName));
            }

            if(players.Count() > 0)
            {
                Player player = players[0];

                player.PlayerBestScore = getPlayerBestScore(player.PlayerID);

                return player;
            }

            

            return null;
        }
        [HttpPost("updateName")]

        public Player updatePlayer([FromForm] int PlayerID, [FromForm] string PlayerName)
        {
            DB.Modify("Update Players Set PlayerName=@PlayerName where PlayerID=@PlayerID",
                (cmd) => {
                    cmd.Parameters.AddWithValue("@PlayerID", PlayerID);
                    cmd.Parameters.AddWithValue("@PlayerName", PlayerName);
                });

            List<Player> players = DB.PullData<Player>("SELECT PlayerID, PlayerName FROM Players WHERE PlayerID=@PlayerID", (dr) => new Player
            {
                PlayerID = dr == null ? 0 : dr.GetInt32(0),
                PlayerName = dr == null ? "" : dr.GetString(1),
                PlayerBestScore = 0,
            }, (cmd) => cmd.Parameters.AddWithValue("@PlayerID", PlayerID));

           
            if (players.Count() > 0)
            {
                Player player = players[0];

                player.PlayerBestScore = getPlayerBestScore(player.PlayerID);

                return player;
            }



            return null;
        }

        [HttpPost("saveHistory")]

        public bool saveHistory([FromForm] int PlayerID, [FromForm] int Score)
        {


            return DB.Modify("INSERT INTO History (PlayerID, Score, DateTime) VALUES(@PlayerID, @Score, @dateTime)", 
                (cmd) => {
                    cmd.Parameters.AddWithValue("@PlayerID", PlayerID);
                    cmd.Parameters.AddWithValue("@Score", Score);
                    cmd.Parameters.AddWithValue("@dateTime", DateTime.Now);
                }) == 1;
        }
        [HttpPost("deletePlayer")]

        public bool Delete([FromForm] int PlayerID)
        {
            return DB.Modify("DELETE FROM Players WHERE PlayerID=@PlayerID", 
                (cmd) => cmd.Parameters.AddWithValue("@PlayerID", PlayerID))==1;
        }
        private int getPlayerBestScore(int playerId)
        {
            try
            {
                //get best score
                List<History> histories = DB.PullData<History>("SELECT Max(Score) as BestScore FROM History WHERE PlayerID=@PlayerID", (dr) => new History
                {
                    Score = dr == null ? 0 : dr.GetInt32(0),
                }, (cmd) => cmd.Parameters.AddWithValue("@PlayerID", playerId));

                if (histories.Count() > 0)
                {
                    return histories[0].Score;
                }
            }
            catch(Exception e)
            {
                return 0;
            }
            
            return 0;
        }
    }
}
