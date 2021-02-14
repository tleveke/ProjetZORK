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
                new Game(zorkService,await this.zorkService.PlayerServices.GenerateMapAsync(width, height, 10, namePlayer));
            }).Wait();
        }
    }
}
