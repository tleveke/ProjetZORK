using ProjetZORK.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetZORK
{
    public class Cell : CellDto
    {
        public Cell(int Id, int PosX, int PosY)
        {
            this.Id = Id;
            this.PosX = PosX;
            this.PosY = PosY;
        }

    }
}
