using ProjetZORK.Services;
using ProjetZORK.Services.Dto;
using System;
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
            Console.WriteLine(" Choisissez votre nom (entre pour valider)");
            Console.WriteLine("##############################################\n");
            Console.Write("> ");
            var namePlayer = Console.ReadLine();
            Task.Run(async () => {
                new Game(zorkService,await this.zorkService.PlayerServices.GenerateMapAsync(width, height, 10, namePlayer));
            }).Wait();
        }
    }
}
