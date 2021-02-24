using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ProjetZORK.Services;
using ProjetZORK.theGame;
using ProjetZORK.Services.Dto;

namespace ProjetZORK
{
    public class Launcher
    {
        public Func<object, object, Task> Exit { get; internal set; }
        public event EventHandler<Task> Event;
        public ZorkService zorkService;
        public int IndexPage = 0;
        public int IndexMenu = 0;
        string name = "       ▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄ ▄▄▄▄▄▄   ▄▄▄   ▄ \n      █       █       █   ▄  █ █   █ █ █\n      █▄▄▄▄   █   ▄   █  █ █ █ █   █▄█ █\n       ▄▄▄▄█  █  █ █  █   █▄▄█▄█      ▄█\n      █ ▄▄▄▄▄▄█  █▄█  █    ▄▄  █     █▄ \n      █ █▄▄▄▄▄█       █   █  █ █    ▄  █\n      █▄▄▄▄▄▄▄█▄▄▄▄▄▄▄█▄▄▄█  █▄█▄▄▄█ █▄█\n";


        public List<string> PageName = new List<string>() {
            "Accueil",
            "Chargement d'un partie",
            "À propos\n\nCréé par : Bigeard, Leveque\n",
        };

        public List<List<string>> ItemsMenuName = new List<List<string>>() {
            /* Home */
            new List<string>() {
                "Nouvelle partie",
                "Chargement d'un partie",
                "À propos",
                "Quitter",
            },
            /* Load Game */
            new List<string>(),
            /* About */
            new List<string>() {
                "Retour",
            },
        };
        public List<List<Action>> ItemsMenuAction = new List<List<Action>>();
        public Launcher(ZorkService zorkService)
        {
            this.zorkService = zorkService;

            this.zorkService.ObjectTypeServices.generateObject();
            this.zorkService.WeaponServices.generateObject();

        }

        public void Start()
        {
            /* Init Menu */
            this.ItemsMenuAction.Add(
            /* Home */
            new List<Action>() {
                NewGame,
                () => LoadGame(1, 0),
                () => ChangePage(2, 0),
                ExitGame,
            });
            /* Load Game */
            this.ItemsMenuAction.Add(new List<Action>());
            /* About */
            this.ItemsMenuAction.Add(new List<Action>() {
                () => ChangePage(0, 2),
            });

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(name);
                Thread.Sleep(100);
            }
            Console.Clear();
            Console.WriteLine(name);
            Console.WriteLine("Appuyez sur la touche !");
            var ch = Console.ReadKey().Key;
            DiplayMenu();
            ActionMenu();
        }

        void ActionMenu()
        {
            bool loopMenu = true;
            while (loopMenu)
            {
            var ch = Console.ReadKey().Key;
                switch(ch)
                {
                    case ConsoleKey.Escape:
                        Console.WriteLine(" Bye :) ");
                        return;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.RightArrow:
                        MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.LeftArrow:
                        MoveDown();
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        loopMenu = false;
                        break;
                }
            }
            ItemsMenuAction[IndexPage][IndexMenu]();
            return;
        }

        void MoveUp() {
            if (this.IndexMenu > 0)
            {
                this.IndexMenu--;
                DiplayMenu();
            }
        }

        void MoveDown() {
            if (IndexMenu < ItemsMenuName[IndexPage].Count()-1)
            {
                this.IndexMenu++;
                DiplayMenu();
            }
        }

        void DiplayMenu () {
            Console.Clear();
            Console.WriteLine("##############################################\n");
            Console.WriteLine(this.name);
            Console.WriteLine($"- {this.PageName[this.IndexPage]}");
            Console.WriteLine("##############################################\n");
            Console.WriteLine("Utilisez les touches fléchées (Haut, Bas) pour sélectionner");
            Console.WriteLine("Et appuyez sur la barre d'espace pour valider - " + (this.IndexMenu+1));
            Console.WriteLine("______________________________________________\n");
            int index = 0;
            foreach (string e in this.ItemsMenuName[this.IndexPage]) {
                Console.WriteLine($"[{(index == IndexMenu ? '*' : ' ')}] - {e}\n");
                index++;
            }
            Console.WriteLine("______________________________________________");
            Console.SetCursorPosition(0, IndexMenu);
        }

        void ChangePage(int IndexPage, int IndexMenu)
        {
            this.IndexPage = IndexPage;
            this.IndexMenu = IndexMenu;
            DiplayMenu();
            ActionMenu();
        }

        void LoadGame(int IndexPage, int IndexMenu)
        {
            ItemsMenuName[1].Clear();
            ItemsMenuAction[1].Clear();
            ItemsMenuName[1].Add("Return");
            ItemsMenuAction[1].Add(() => ChangePage(0, 1));
            List<PlayerDto> games = this.zorkService.PlayerServices.GetAll();
            foreach (PlayerDto game in Enumerable.Reverse(games))
            {
                if (game.isFinish == false && game.HP > 0)
                {
                    ItemsMenuName[1].Add($"{game.Id}. {game.Name}");
                    ItemsMenuAction[1].Add(() => StartGame(game.Id));
                }
            }
            ChangePage(IndexPage, IndexMenu);
        }

        void NewGame()
        {
            new SetupGame(zorkService);
            ChangePage(0, 0);
        }

        void StartGame(int Id)
        {
            new Game(zorkService, Id);
            ChangePage(0, 0);
        }

        void ExitGame()
        {
            Console.Clear();
            Console.WriteLine("Exit Game");
            this.Exit(this, Event);
        }
    }
}