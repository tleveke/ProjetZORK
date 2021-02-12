using ProjetZORK.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace ProjetZORK
{
    public class Launcher
    {
        public Func<object, object, Task> Exit { get; internal set; }

        public event EventHandler<Task> Event;

        public void Start()
        {
            Console.Clear();
            Console.WriteLine("Zork !");
            Thread.Sleep(2000);
            Console.Clear();
            Console.WriteLine("Menu");
            Console.WriteLine("______________________________________________");
        }
    }
}