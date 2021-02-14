using ProjetZORK.Services;
using ProjetZORK.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetZORK.theGame
{
    class SetupGame
    {
        public ZorkService zorkService;
        PlayerDto PlayerDto = null;
        private int width = 5;
        private int height = 5;
        private int gameId = -1;

        public SetupGame(ZorkService zorkService)
        {
            this.zorkService = zorkService;
            generatePlayer();
        }

        public void generatePlayer()
        {
            Console.Clear();
            Console.WriteLine("##############################################");
            Console.WriteLine("                  Choose Player Name        ");
            Console.WriteLine("##############################################");
            Console.WriteLine("##############################################");
            Console.WriteLine("##############################################");
            Console.Write("> ");

            Random rnd = new Random();

            var namePlayer = Console.ReadLine();
            Task.Run(async () => {



                this.PlayerDto = new PlayerDto { Name = namePlayer, XP = 1, HP = 1, MaxHP = 1 };
                this.gameId = await this.zorkService.PlayerServices.AddAsync(this.PlayerDto);

                generateMap(width, height, 0, this.gameId);

                int posXPlayer = rnd.Next(1, width);  // creates a number between 1 and width
                int posYPlayer = rnd.Next(1, height);   // creates a number between 1 and height
                Console.WriteLine(posXPlayer);
                Console.WriteLine(posYPlayer);

                CellDto cell = this.zorkService.CellServices.GetGameIdPosXY(this.gameId, posXPlayer, posYPlayer);
                PlayerDto playerDto = this.zorkService.PlayerServices.Get(this.gameId);

                await this.zorkService.PlayerServices.EditAsync(playerDto, cell);

            }).Wait();
        }
        public void generateMap(int width, int height, int numObstacle, int gameId)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Task.Run(async () => { await this.zorkService.CellServices.AddAsync(new CellDto { PosX = x, PosY = y, gameId = gameId, Description = "dssdsd", canMoveTo = true, MonsterRate = 0 }); }).Wait();
                }
            }
            Console.WriteLine("Fin de Génération de la map");

            //new Game(this.zorkService, gameId);
        }
    }
}
