using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using ProjetZORK.Services;

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
            new List<string>() {
                "Return",
            },
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
                () => ChangePage(1, 0),
                () => ChangePage(2, 0),
                ExitGame,
            });
            this.ItemsMenuAction.Add(
            /* Load Game */
            new List<Action>() {
                () => ChangePage(0, 1),
            });
            this.ItemsMenuAction.Add(
            /* About */
            new List<Action>() {
                () => ChangePage(0, 2),
            });

            Console.Clear();
            for (int i = 0; i < 20; i++) Console.WriteLine("Zork !");
            Thread.Sleep(600);
            Console.Clear();
            for (int i = 0; i < 20; i++) Console.WriteLine("Zork ! Zork !");
            Thread.Sleep(600);
            Console.Clear();
            for (int i = 0; i < 20; i++) Console.WriteLine("Zork ! Zork ! Zork !");
            Thread.Sleep(600);
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
            Console.WriteLine("                  " + PageName[IndexPage]);
            Console.WriteLine("##############################################\n");
            Console.WriteLine("Use the arrow key (Up, Down) to select");
            Console.WriteLine("And press the Spacebar to validate - " + (this.IndexMenu+1));
            Console.WriteLine("______________________________________________\n");
            int index = 0;
            foreach (string e in this.ItemsMenuName[IndexPage]) {
                if (index == IndexMenu) {
                    Console.WriteLine($"[*] - {e}");
                } else {
                    Console.WriteLine($"[ ] - {e}");
                }
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

        void NewGame()
        {
            Console.WriteLine("New Game (start game)");
            new Game(zorkService);
        }

        static void ExitGame()
        {
            Console.WriteLine("Exit Game");
            return;
        }
    }
}