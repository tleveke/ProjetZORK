using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetZORK
{
    public class Game
    {
        public List<List<object>> map = new List<List<object>>();

        public Game()
        {
            generateMap(40, 40, 10);
        }

        void generateMap(int width, int height, int numObstacle)
        {
            for (int y = 0; y < height; y++)
            {
                this.map.Add(new List<object>());
            
                for (int x = 0; x < width; x++)
                {
                    this.map[y].Add(new Cell(y+x, x, y));
                }
            }
        }
    }
}
