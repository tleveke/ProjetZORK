using ProjetZORK.Services.Dto;


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
