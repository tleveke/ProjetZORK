using ProjetZORK.Services;
using ProjetZORK.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetZORK.theGame
{
    public class Game
    {
        private List<CellDto> map = new List<CellDto>();


        private ZorkService zorkService;
        private int gameId;

        public Game(ZorkService zorkService, int gameId)
        {
            this.zorkService = zorkService;
            this.gameId = gameId;
            getMap();

        }

        public void getMap()
        {
            Console.WriteLine($" le game id => {gameId}");
            this.map = this.zorkService.CellServices.GetAllGameId(gameId);

            foreach (CellDto cell in this.map)
            {
                Console.WriteLine($"{cell.PosX} {cell.PosY} {cell.Description} {cell.gameId}");
            }


        }

        public void generateMap(int width, int height, int numObstacle, int gameId)
        {
            //Task.Run(async () => { await this.zorkService.CellServices.AddAsync(new CellDto { PosX = 1, PosY = 1, gameId = 1, Description = "dssdsd", canMoveTo = true, MonsterRate = 0 }); }).Wait();
            //Task.Run(async () => { await this.zorkService.PlayerServices.AddAsync(new PlayerDto { Name = "nnn",XP=1,HP=1,MaxHP=1 }); }).Wait();
            
            
            /*foreach (CellDto cell in this.zorkService.CellServices.GetAllGameId(1) )
            {
                Console.WriteLine(cell.Description);
            }*/
        }
    }
}
