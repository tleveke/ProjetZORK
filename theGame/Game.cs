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
        private PlayerDto player = new PlayerDto();
        private CellDto cellCurrent = new CellDto();


        private ZorkService zorkService;
        private int gameId;

        public Game(ZorkService zorkService, int gameId)
        {
            this.zorkService = zorkService;
            this.gameId = gameId;
            getPlayer();
            getMap();
            gameCell();
        }
        public void gameCell()
        {
            this.cellCurrent = this.player.currentCell;


            Console.WriteLine("##############################################");
            Console.WriteLine($"                  Vous êtes dans : {this.cellCurrent.PosX} {this.cellCurrent.PosY}, la description : {this.cellCurrent.Description}        ");
            Console.WriteLine("##############################################");
            Console.WriteLine($"                  deplacement :    ");
            Console.Write($">");
            deplacement();
            movePlayer();

        }
        public void deplacement()
        {
            Console.Write($">");
            var ch = Console.ReadLine();
            switch (ch)
            {
                case "goNorth":
                    this.cellCurrent.PosX++;
                    return;
                case "goSouth":
                    this.cellCurrent.PosX--;
                    break;
                case "goEast":
                    this.cellCurrent.PosY++;
                    break;
                case "goWest":
                    this.cellCurrent.PosY--;
                    break;
                default:
                    this.deplacement();
                    break;
            }

        }
        public async void movePlayer()
        {
            await this.zorkService.PlayerServices.changeCasePlayer(this.cellCurrent.PosX, this.cellCurrent.PosY);
            /*this.player = */
            //this.cellCurrent = this.zorkService.CellServices.GetGameIdPosXY(this.cellCurrent.gameId, this.cellCurrent.PosX, this.cellCurrent.PosY);
            //await this.zorkService.PlayerServices.EditAsync(this.player, this.cellCurrent);
            gameCell();
        }
        public void getPlayer()
        {
            this.player = this.zorkService.PlayerServices.Get(this.gameId);
        }

        public void getMap()
        {
            Console.WriteLine($" le game id => {this.player.Id}");
            this.map = this.zorkService.CellServices.GetAllGameId(this.player.Id);

            foreach (CellDto cell in this.map)
            {
                Console.WriteLine($"{cell.PosX} {cell.PosY} {cell.Description} {cell.gameId}");
            }


        }
    }
}
