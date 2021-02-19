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
        List<Monster> ListMonster = new List<Monster>() {
            new Monster { Name = "Orc", HP = 2, AttackRate = 85 },
            new Monster { Name = "Murloc", HP = 4, AttackRate = 65 },
            new Monster { Name = "Sorcière", HP = 6, AttackRate = 55 },
            new Monster { Name = "Troll", HP = 6, AttackRate = 85 },
            new Monster { Name = "Harpie", HP = 7, AttackRate = 100 },
            new Monster { Name = "Golem", HP = 12, AttackRate = 35 },
            new Monster { Name = "Dragon", HP = 10, AttackRate = 85 },
        };

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
            this.cellCurrent = this.player.Cell;
            Console.WriteLine("##############################################");
            Console.WriteLine($"                  Vous �tes dans : {this.cellCurrent.PosX} {this.cellCurrent.PosY}, la description : {this.cellCurrent.Description}        ");
            Console.WriteLine("##############################################");
            Console.WriteLine("                  deplacement :    ");
            deplacement();
            movePlayer();
        }
        public void deplacement()
        {
            Console.Write(">");
            var ch = Console.ReadKey().Key;
            switch (ch)
            {
                case ConsoleKey.Escape:
                    Console.WriteLine(" Bye :) ");
                    return;
                case ConsoleKey.UpArrow:
                    this.cellCurrent.PosX++;
                    break;
                case ConsoleKey.DownArrow:
                    this.cellCurrent.PosX--;
                    break;
                case ConsoleKey.RightArrow:
                    this.cellCurrent.PosY++;
                    break;
                case ConsoleKey.LeftArrow:
                    this.cellCurrent.PosY--;
                    break;
                case ConsoleKey.Spacebar:
                    // INFO PLAYER
                    break;
                default:
                    this.deplacement();
                    break;
            }
        }
        public async void movePlayer()
        {
            await this.zorkService.PlayerServices.changeCasePlayer(this.cellCurrent.PosX, this.cellCurrent.PosY);
            Random rnd = new Random();
            if (this.cellCurrent.MonsterRate > rnd.Next(0, 100))
            {
                int rankMonster = this.player.XP;
                if (50 < rnd.Next(0, 100))
                {
                    rankMonster++;
                }
                Monster m = ListMonster[rankMonster];
                Monster newMonster = new Monster { Name = m.Name, HP = m.HP, AttackRate = m.AttackRate };
                fight(newMonster);
            }

            /*this.player = */
            //this.cellCurrent = this.zorkService.CellServices.GetGameIdPosXY(this.cellCurrent.gameId, this.cellCurrent.PosX, this.cellCurrent.PosY);
            //await this.zorkService.PlayerServices.EditAsync(this.player, this.cellCurrent);
            gameCell();
        }

        public void fight(Monster monster)
        {
            Console.WriteLine("##############################################");
            Console.WriteLine("                  Combat !                    ");
            Console.WriteLine("##############################################");
            Console.WriteLine($"   {player.Name}   VS   {monster.Name}   ");
            Console.WriteLine($"   HP: {player.HP}        HP: {monster.HP} ");
            Console.WriteLine($"_____________________________________________\n");
            Console.WriteLine("   1. Attaque         |          2. Défense   ");
            Console.WriteLine("   3. Équipement      |          4. Fuite     ");
            this.actionFight(monster);
        }

        public void actionFight(Monster monster)
        {
            Console.Write(">");
            var ch = Console.ReadLine();
            switch (ch)
            {
                /* Attaque */
                case "1":
                    damagePlayer(monster);
                    damageMonster(monster);
                    break;
                /* Défense */
                case "2":
                    damageMonster(monster, 10);
                    break;
                /* Équipement */
                case "3":
                    /* ... */
                    break;
                /* Fuite */
                case "4":
                    Random rnd = new Random();
                    if (50 < rnd.Next(0, 100)) {
                        Console.WriteLine("Vous avez fuit");
                        return;
                    } else {
                        Console.WriteLine("Vous n'avez pas réussi a fuire...");
                        damageMonster(monster, 10);
                    }
                    break;
            }
            if(monster.HP > 0 && this.player.HP > 0)
            {
                actionFight(monster);
            }
        }

        public void damagePlayer(Monster monster) {
            /* Attaque du Joueur */
            monster.HP = monster.HP - 1; /* + obgectDmg */
            Console.WriteLine($"Vous attaquez ! {monster.Name} a {monster.HP} HP");
            if (monster.HP <= 0)
            {
                Console.WriteLine($"{monster.Name} est mort...");
            }
        }
        
        public void damageMonster(Monster monster, int defence = 0) {
            /* Attaque du Monstre */
            if (monster.HP >= 0) return;
            Random rnd = new Random();
            if ((monster.AttackRate - defence) < rnd.Next(0, 100))
            {
                this.player.HP = this.player.HP - 1; /* +++ */
                Console.WriteLine($"{monster.Name} attaque ! Il vous reste {this.player.HP} HP");
                if (this.player.HP <= 0)
                {
                    Console.WriteLine(" - Game Over - ");
                }
            }
            else
            {
                Console.WriteLine($"{monster.Name} a raté son attaque...");
                if (defence > 0 && 50 < rnd.Next(0, 100))
                {
                    damagePlayer(monster);
                }
            }
        }

        public void getPlayer()
        {
            this.player = this.zorkService.PlayerServices.Get(this.gameId);
            Console.WriteLine(this.player.CellId);
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
