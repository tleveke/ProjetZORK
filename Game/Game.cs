using ProjetZORK.Services;
using ProjetZORK.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetZORK
{
    public class Game
    {
        public List<List<object>> map = new List<List<object>>();

        public ZorkService zorkService;

        public Game(ZorkService zorkService)
        {
            this.zorkService = zorkService;
            //Task.Run(() => generateMap(40, 40, 10));
            generatePlayer();
            //generateMap(40, 40, 10);
        }

        public void generatePlayer()
        {
            Console.Clear();
            Console.WriteLine("##############################################");
            Console.WriteLine("                  Choose Player Name        " );
            Console.WriteLine("##############################################");
            Console.Write("> ");
            var namePlayer = Console.ReadLine();
            Task.Run(async () => {
                generateMap(5, 5, 0, await this.zorkService.PlayerServices.AddAsync(new PlayerDto { Name = namePlayer, XP = 1, HP = 1, MaxHP = 1 }) ); 
            }).Wait();

            //int playerID = await this.zorkService.PlayerServices.AddAsync(new PlayerDto { Name = namePlayer, XP = 1, HP = 1, MaxHP = 1 });

            //generateMap(40, 40, 0, playerID);



        }

        public void generateMap(int width, int height, int numObstacle, int gameId)
        {
            //Task.Run(async () => { await this.zorkService.CellServices.AddAsync(new CellDto { PosX = 1, PosY = 1, gameId = 1, Description = "dssdsd", canMoveTo = true, MonsterRate = 0 }); }).Wait();
            //Task.Run(async () => { await this.zorkService.PlayerServices.AddAsync(new PlayerDto { Name = "nnn",XP=1,HP=1,MaxHP=1 }); }).Wait();
            
            
            /*foreach (CellDto cell in this.zorkService.CellServices.GetAllGameId(1) )
            {
                Console.WriteLine(cell.Description);
            }*/

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Task.Run(async () => { await this.zorkService.CellServices.AddAsync(new CellDto { PosX = x, PosY = y, gameId = gameId, Description = "dssdsd", canMoveTo = true, MonsterRate = 0 }); }).Wait();
                }
            }
            Console.WriteLine("Fin de Génération");
        }
    }
}
