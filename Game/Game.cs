using ProjetZORK.Services;
using ProjetZORK.Services.Dto;
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

        public ZorkService zorkService;

        public Game(ZorkService zorkService)
        {
            this.zorkService = zorkService;
            //Task.Run(() => generateMap(40, 40, 10));
            generateMap(40, 40, 10);
        }

        public void generateMap(int width, int height, int numObstacle)
        {
            //Task.Run(async () => { await this.zorkService.CellServices.AddAsync(new CellDto { PosX = 1, PosY = 1, gameId = 1, Description = "dssdsd", canMoveTo = true, MonsterRate = 0 }); }).Wait();
            //Task.Run(async () => { await this.zorkService.PlayerServices.AddAsync(new PlayerDto { Name = "nnn",XP=1,HP=1,MaxHP=1 }); }).Wait();
            foreach (CellDto cell in this.zorkService.CellServices.GetAllGameId(1) )
            {
                Console.WriteLine(cell.Description);
            }
            /*
             
            Microsoft.EntityFrameworkCore.Database.Command[20102]
      Failed executing DbCommand (152ms) [Parameters=[@p0='?' (Size = 4000), @p1='?' (DbType = Int32), @p2='?' (DbType = Int32), @p3='?' (DbType = Int32), @p4='?' (DbType = Boolean), @p5='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
      SET NOCOUNT ON;
      INSERT INTO [Cells] ([Description], [MonsterRate], [PosX], [PosY], [canMoveTo], [gameId])
      VALUES (@p0, @p1, @p2, @p3, @p4, @p5);
      SELECT [Id]
      FROM [Cells]
      WHERE @@ROWCOUNT = 1 AND [Id] = scope_identity();
fail: Microsoft.EntityFrameworkCore.Update[10000]
      An exception occurred in the database while saving changes for context type 'ProjetZORK.DataAccessLayer.ZorkManagerDbContext'.
      Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while updating the entries. See the inner exception for details.
       ---> Microsoft.Data.SqlClient.SqlException (0x80131904): Nom d'objet 'Cells' non valide.
             
             */

            /*for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Task.Run(async () => { await this.zorkService.CellServices.AddAsync(new CellDto { PosX = x, PosY = y, gameId = 1, Description = "dssdsd", canMoveTo = true, MonsterRate = 0 }); }).Wait();
                    //await this.zorkService.CellServices.AddAsync(new CellDto { PosX = x, PosY = y, gameId = 1, Description = "dssdsd", canMoveTo = true, MonsterRate = 0 });
                }
            }*/
        }
    }
}
