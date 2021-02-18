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

        public List<string> PageName = new List<string>() {
            "Home",
            "Load Game",
            "About\n\nCreated by: Bigeard, Leveque\n",
        };

        public List<List<string>> ItemsMenuName = new List<List<string>>() {
            /* Home */
            new List<string>() {
                "New Game",
                "Load Game",
                "About",
                "Exit",
            },
            /* Load Game */
            new List<string>(),
            /* About */
            new List<string>() {
                "Return",
            },
        };
        public List<List<Action>> ItemsMenuAction = new List<List<Action>>();
        public Launcher(ZorkService zorkService)
        {
            this.zorkService = zorkService;
        }

        public void Start()
        {
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

            string name = "Zork ! ";
            for (int i = 0; i < 4; i++)
            {
                name += name;
                Console.Clear();
                for (int j = 0; j < 20; j++)
                {
                    Thread.Sleep(10);
                    Console.WriteLine(name);
                }
            }
            Console.Clear();
            Console.WriteLine("Press Key !");
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
                        MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        MoveDown();
                        break;
                    case ConsoleKey.Spacebar:
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
            Console.WriteLine("##############################################");
            Console.WriteLine(" Zork - " + PageName[IndexPage]);
            Console.WriteLine("##############################################\n");
            Console.WriteLine("Use the arrow key (Up, Down) to select");
            Console.WriteLine("And press the Spacebar to validate - " + (this.IndexMenu+1));
            Console.WriteLine("______________________________________________\n");
            int index = 0;
            foreach (string e in this.ItemsMenuName[IndexPage]) {
                Console.WriteLine($"[{(index == IndexMenu ? '*' : ' ')}] - {e}\n");
                index++;
            }
            Console.WriteLine("______________________________________________");
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
            foreach (PlayerDto game in games)
            {
                ItemsMenuName[1].Add($"{game.Id}. {game.Name}");
                ItemsMenuAction[1].Add(() => StartGame(game.Id));
            }
            ChangePage(IndexPage, IndexMenu);
        }

        void NewGame()
        {
            Console.WriteLine("New Game (start game)");
            new SetupGame(zorkService);
        }

        void StartGame(int Id)
        {
            Console.WriteLine($"Start Game {Id}");
            Console.WriteLine($"Start Game {Id}");
            Console.WriteLine($"Start Game {Id}");
        }

        void ExitGame()
        {
            Console.WriteLine("Exit Game");
            this.Exit(this, Event);
        }
    }
}