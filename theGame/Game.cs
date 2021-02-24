using ProjetZORK.Services;
using ProjetZORK.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjetZORK.theGame
{
    public class Game
    {
        private List<CellDto> map = new List<CellDto>();
        private List<ObjectTypeDto> ObjectTypeDtos = new List<ObjectTypeDto>();
        private List<WeaponDto> WeaponDtos = new List<WeaponDto>();


        private PlayerDto player = new PlayerDto();
        private CellDto cellCurrent = new CellDto();
        private MonsterDto monsterCurrent = new MonsterDto();
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
        private bool exit = false;

        public Game(ZorkService zorkService, int gameId)
        {
            this.zorkService = zorkService;
            this.gameId = gameId;
            Console.Clear();
            getObjectTypes();
            getWeapons();
            getPlayer();
            getMap();

            // If monster alive in game load the monster
            this.monsterCurrent = this.zorkService.MonsterServices.Get(gameId);
            if (this.monsterCurrent.HP > 0)
            {
                fight();
            }

            gameCell();
        }
        public void gameCell()
        {
            this.cellCurrent = this.player.Cell;
            this.genMiniMap();
            Console.WriteLine("##############################################");
            //Console.WriteLine($"Position : x {this.cellCurrent.PosX} | y {this.cellCurrent.PosY}");
            Console.WriteLine($"Description : {this.cellCurrent.Description}");
            Console.WriteLine("##############################################");
            Console.WriteLine("Deplacement :");
            deplacement();
            if(!this.exit) movePlayer();
        }
        public void deplacement()
        {
            Console.Write("Move > ");
            var ch = Console.ReadKey().Key;
            Console.Clear();
            int x = this.cellCurrent.PosX;
            int y = this.cellCurrent.PosY;
            switch (ch)
            {
                case ConsoleKey.Escape:
                    this.exit = true;
                    break;
                case ConsoleKey.UpArrow:
                    y--;
                    break;
                case ConsoleKey.DownArrow:
                    y++;
                    break;
                case ConsoleKey.RightArrow:
                    x++;
                    break;
                case ConsoleKey.LeftArrow:
                    x--;
                    break;
                case ConsoleKey.Spacebar:
                    // INFO PLAYER
                    this.infoPlayer();
                    this.deplacement();
                    break;
                default:
                    this.gameCell();
                    break;
            }
            this.outOfMap(x, y);
        }

        public void outOfMap(int x, int y)
        {
            if ((x < 0 || x > 4) || (y < 0 || y > 4))
            {
                Console.WriteLine("Zone inaccessible...");
                this.gameCell();
            }
            else
            {
                this.cellCurrent.PosX = x;
                this.cellCurrent.PosY = y;
            }
        }

        public void genMiniMap()
        {
            for (int y = 0; y < 5; y++)
            {
                string miniMap = "|";
                for (int x = 0; x < 5; x++)
                {
                    miniMap += $"{(this.cellCurrent.PosY == y && this.cellCurrent.PosX == x ? '*' : ' ')}|";
                }
                Console.WriteLine(miniMap);
            }
        }

        public async void movePlayer()
        {
            this.player = await this.zorkService.PlayerServices.changeCasePlayer(this.cellCurrent.PosX, this.cellCurrent.PosY);

            await trap();
            await treasure();

            Random rnd = new Random();
            if (this.cellCurrent.MonsterRate > rnd.Next(0, 100))
            {
                int rankMonster = this.player.XP;
                if (50 < rnd.Next(0, 100) && ListMonster.Count < 7)
                {
                    rankMonster++;
                }
                Monster m = ListMonster[rankMonster];
                Monster newMonster = new Monster { Name = m.Name, HP = m.HP, AttackRate = m.AttackRate, Group = player.Id };
                Task.Run(async () => {
                    this.monsterCurrent = await this.zorkService.MonsterServices.AddAsync(newMonster);
                }).Wait();
                if (newMonster.Name == "Dragon") {
                    this.annimayionBoss();
                }
                fight();
            }
            gameCell();
        }
        public async Task trap()
        {
            Random rnd = new Random();

            if (this.cellCurrent.trapRate > rnd.Next(0, 100))
            {
                Console.WriteLine("##############################################");
                Console.WriteLine("Vous êtes tombé sur un piège ! Vous avez perdu des HP");
                Console.WriteLine("##############################################");
                this.player.HP -= 5;
                this.player = await this.zorkService.PlayerServices.editUserLifeXP(this.player);
            }
        }
        public async Task treasure()
        {
            if (this.player.Cell.objectGet == true)
            {

                Random rnd = new Random();

                int random = rnd.Next(0, 100);

                if (60 > random)
                {
                    //Generation d'un objet consommable !

                    ObjectTypeDto objectTypeDto = this.ObjectTypeDtos[new Random().Next(0, this.ObjectTypeDtos.Count())];
                    Console.WriteLine("##############################################");
                    Console.WriteLine("You found a treasure !");
                    Console.WriteLine($"                 {objectTypeDto.Name}                   ");
                    Console.WriteLine("##############################################");
                    this.player = await this.zorkService.PlayerServices.addObjectPlayer(objectTypeDto);
                }
                else
                {
                    //Generation d'une arme !

                    WeaponDto weaponDto = this.WeaponDtos[new Random().Next(0, this.WeaponDtos.Count())];
                    Console.WriteLine("##############################################");
                    Console.WriteLine("You found a weapon !");
                    Console.WriteLine($"   {weaponDto.Name} => Power : {weaponDto.AttackPower} ; " +
                        $"AttackRate : {weaponDto.AttackPower} ; MissRate : {weaponDto.MissRate} ;            ");
                    Console.WriteLine("Do you want to recover this weapon? (y for yes)");
                    if (this.player.Weapon != null)
                    {
                        Console.WriteLine($"  Actual Weapon => {this.player.Weapon.Name} => Power : {this.player.Weapon.AttackPower} ; " +
                            $"AttackRate : {this.player.Weapon.AttackPower} ; MissRate : {this.player.Weapon.MissRate} ;            ");
                    }
                    else
                    {
                        Console.WriteLine("You don't have a weapon !");
                    }

                    Console.Write("Weapon Choice > ");
                    var ch = Console.ReadLine();

                    if (ch == "y" || ch == "yes")
                    {
                        this.player = await this.zorkService.PlayerServices.weaponPlayer(weaponDto);
                    }
                }


            }
        }

        public void fight()
        {
            Console.WriteLine("##############################################");
            Console.WriteLine("                  Combat !                    ");
            Console.WriteLine("##############################################");
            Console.WriteLine($"   {player.Name}   VS   {this.monsterCurrent.Name}");
            Console.WriteLine($"   HP: {player.HP}        HP: {this.monsterCurrent.HP}");
            Console.WriteLine($"_____________________________________________\n");
            Console.WriteLine("   1. Attaque         |          2. Défense   ");
            Console.WriteLine("   3. Équipement/Info      |          4. Fuite     ");
            this.actionFight();
        }

        public void actionFight()
        {
            this.player = await this.zorkService.PlayerServices.editUserLifeXP(this.player);

            Console.Write("Fight > ");
            var ch = Console.ReadLine();
            switch (ch)
            {
                /* Attaque */
                case "1":
                    damagePlayer();
                    damageMonster();
                    break;
                /* Défense */
                case "2":
                    damageMonster(10);
                    break;
                /* Équipement */
                case "3":
                    listInventory(this.monsterCurrent);
                    break;
                /* Fuite */
                case "4":
                    Random rnd = new Random();
                    if (50 < rnd.Next(0, 100)) {
                        Console.WriteLine("Vous avez fuit");
                        Task.Run(async () => {
                            await this.zorkService.MonsterServices.DeleteAsync(this.monsterCurrent);
                        }).Wait();
                        return;
                    } else {
                        Console.WriteLine("Vous n'avez pas réussi a fuire...");
                        damageMonster(10);
                    }
                    break;
            }
            if(this.monsterCurrent.HP > 0 && this.player.HP > 0)
            {
                actionFight();
            }
        }

        public void damagePlayer() {
            /* Attaque du Joueur */
            this.monsterCurrent.HP = this.monsterCurrent.HP - this.player.Attack; /* + obgectDmg */
            Task.Run(async () => {
                await this.zorkService.MonsterServices.EditAsync(this.monsterCurrent);
            }).Wait();
            Console.WriteLine($"Vous attaquez ! {this.monsterCurrent.Name} a {this.monsterCurrent.HP} HP");
            if (this.monsterCurrent.HP <= 0)
            {
                Console.WriteLine($"{this.monsterCurrent.Name} est mort...");
                this.player.XP++;
                Task.Run(async () => {
                    await this.zorkService.PlayerServices.editUserLifeXP(this.player);
                }).Wait();
                Task.Run(async () => {
                    await this.zorkService.MonsterServices.DeleteAsync(this.monsterCurrent);
                }).Wait();
                if (this.monsterCurrent.Name == "Dragon")
                {
                    Console.WriteLine("\n\n\n      (¯`·¯`·.¸¸.·´¯`·.¸¸.·´¯`·´¯)\n      ( \\                      / )\n       ( ) Vous avez gagner ! ( )\n        (/                    \\)\n         (.·´¯`·.¸¸.·´¯`·.¸¸.·)");
                    this.exit = true;
                }
            }
        }
        
        public void damageMonster(int defence = 0) {
            /* Attaque du Monstre */
            if (this.monsterCurrent.HP <= 0) return;
            Random rnd = new Random();
            if ((this.monsterCurrent.AttackRate - defence + this.player.Defense) > rnd.Next(0, 100))
            {
                this.player.HP = this.player.HP - 1; /* +++ */
                Task.Run(async () => {
                    await this.zorkService.PlayerServices.editUserLifeXP(this.player);
                }).Wait();
                Console.WriteLine($"{this.monsterCurrent.Name} attaque ! Il vous reste {this.player.HP} HP");
                if (this.player.HP <= 0)
                {
                    Console.WriteLine(" - Game Over - ");
                }
            }
            else
            {
                Console.WriteLine($"{this.monsterCurrent.Name} a raté son attaque...");
                if (defence > 0 && 50 < rnd.Next(0, 100))
                {
                    damagePlayer();
                }
            }

        }

        public void getPlayer()
        {
            this.player = this.zorkService.PlayerServices.Get(this.gameId);
        }
        public void getObjectTypes()
        {
            this.ObjectTypeDtos = this.zorkService.PlayerServices.GetObjectTypeDtos();
        }
        public void getWeapons()
        {
            this.WeaponDtos = this.zorkService.PlayerServices.GetWeaponsDtos();
        }



        public async void listInventory(Monster monster)
        {
            this.headInfo();

            int compteur = 1;
            foreach (ObjectPlayerDto objectPlayerDto in this.player.ObjectInventory)
            {
                ObjectTypeDto objectTypeDto = this.ObjectTypeDtos.Single(ObjectType => ObjectType.Id == objectPlayerDto.ObjectTypeId);

                string Display = $"{compteur} => {objectTypeDto.Name} : ";

                if (objectTypeDto.HPRestoreValue > 0)
                {
                    Display += $" Heal : {objectTypeDto.HPRestoreValue} HP";
                }
                if (objectTypeDto.AttackStrenghBoost > 0)
                {
                    Display += $" Attack Boost {objectTypeDto.AttackStrenghBoost}";
                }
                if (objectTypeDto.DefenseBoost > 0)
                {
                    Display += $"Defense Boost : {objectTypeDto.DefenseBoost}";
                }
                Console.WriteLine(Display);

                compteur++;
            }


            Console.WriteLine("##############################################");
            Console.WriteLine("                  Choice an object :          ");
            Console.WriteLine("##############################################");
            string choice = Console.ReadLine();
            try
            {
                if (int.Parse(choice) >= 1 && int.Parse(choice) <= compteur)
                {
                    ObjectPlayerDto objectPlayerDto = this.player.ObjectInventory.ElementAt(int.Parse(choice) - 1);

                    this.player = await this.zorkService.PlayerServices.removeObjectPlayer(objectPlayerDto);
                    damageMonster(monster);
                }
            }
            catch
            {
                Console.WriteLine("Error !");
            }


        }

        public void headInfo()
        {
            Console.WriteLine("##############################################");
            Console.WriteLine($"                  Name : {this.player.Name}                   ");
            Console.WriteLine($"                  HP :  {this.player.HP}                  ");
            Console.WriteLine($"                  XP :  {this.player.XP}                  ");
            Console.WriteLine($"                  Attack :  {this.player.Attack}                  ");
            Console.WriteLine($"                  Defense :  {this.player.Defense}                  ");

            if (this.player.Weapon != null)
            {
                Console.WriteLine($"                  Weapon :  {this.player.Weapon.Name} => Power : {this.player.Weapon.AttackPower} " +
                    $"; AttackRate : {this.player.Weapon.AttackPower} ; MissRate : {this.player.Weapon.MissRate}                 ");
            }


            Console.WriteLine("______________________________________________");
            Console.WriteLine($"-------------  Objects Inventory ({this.player.ObjectInventory.Count} objects)  -----------");
        }
        public void infoPlayer()
        {
            this.headInfo();
            foreach (ObjectPlayerDto objectPlayerDto in this.player.ObjectInventory)
            {
                ObjectTypeDto objectTypeDto = this.ObjectTypeDtos.Single(ObjectType => ObjectType.Id == objectPlayerDto.ObjectTypeId);

                string Display = $"{objectTypeDto.Name} : ";

                if (objectTypeDto.HPRestoreValue > 0)
                {
                    Display += $" Heal : {objectTypeDto.HPRestoreValue} HP";
                }
                if (objectTypeDto.AttackStrenghBoost > 0)
                {
                    Display += $" Attack Boost {objectTypeDto.AttackStrenghBoost}";
                }
                if (objectTypeDto.DefenseBoost > 0)
                {
                    Display += $"Defense Boost : {objectTypeDto.DefenseBoost}";
                }
                Console.WriteLine(Display);

            }
            Console.WriteLine("______________________________________________");

            Console.WriteLine("##############################################");
        }

        public void getMap()
        {
            this.map = this.zorkService.CellServices.GetAllGameId(this.player.Id);

            foreach (CellDto cell in this.map)
            {
                //Console.WriteLine($"{cell.PosX} {cell.PosY} {cell.Description} {cell.gameId}");
            }
        }

        public void annimayionBoss()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("                                              ,--,  ,.-..");
                Console.WriteLine("                ,                   \\,       '-,-`,'-.' | ._");
                Console.WriteLine("               /|           \\    ,   |\\         }  )/  / `-,',");
                Console.WriteLine("               [ '          |\\  /|   | |        /  \\|  |/`  ,`");
                Console.WriteLine("               | |       ,.`  `,` `, | |  _,...(   (      _',");
                Console.WriteLine("   -ART BY-    \\  \\  __ ,-` `  ,  , `/ |,'      Y     (   \\_L\\");
                Console.WriteLine("    -ZEUS-      \\  \\_\\,``,   ` , ,  /  |         )         _,/");
                Console.WriteLine("                 \\  '  `  ,_ _`_,-,<._.<        /         /");
                Console.WriteLine("                  ', `>.,`  `  `   ,., |_      |         /");
                Console.WriteLine("                    \\/`  `,   `   ,`  | /__,.-`    _,   `\\");
                Console.WriteLine("                -,-..\\  _  \\  `  /  ,  / `._) _,-\\`       \\");
                Console.WriteLine("                 \\_,,.) /\\    ` /  / ) (-,, ``    ,        |");
                Console.WriteLine("                ,` )  | \\_\\       '-`  |  `(               \\");
                Console.WriteLine("               /  /```(   , --, ,' \\   |`<`    ,            |");
                Console.WriteLine("              /  /_,--`\\   <\\  V /> ,` )<_/)  | \\      _____)");
                Console.WriteLine("        ,-, ,`   `   (_,\\ \\    |   /) / __/  /   `----`");
                Console.WriteLine("       (-, \\           ) \\ ('_.-._)/ /,`    /");
                Console.WriteLine("       | /  `          `/ \\\\ V   V, /`     /");
                Console.WriteLine("    ,--\\(        ,     <_/`\\\\     ||      /");
                Console.WriteLine("   (   ,``-     \\/|         \\-A.A-`|     /");
                Console.WriteLine("  ,>,_ )_,..(    )\\          -,,_-`  _--`");
                Console.WriteLine(" (_ \\|`   _,/_  /  \\_            ,--`");
                Console.WriteLine("  \\( `   <.,../`     `-.._   _,-`");
                Console.WriteLine("   `                      ```");
                Thread.Sleep(100);
                Console.Clear();
            }
        }
    }
}
