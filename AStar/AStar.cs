using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace AStar
{
    static public class AStar
    {
        static public PictureBox[,] cellMap;
        
        static readonly public List<CellInfo> openCells = new List<CellInfo>();
        static readonly public List<CellInfo> closeCells = new List<CellInfo>();
        
        public class CellInfo
        {
            public int x;
            public int y;
            public int f_value;
            public int h_value;
            
            public CellInfo(int x, int y, int h_value)
            {
                this.x = x;
                this.y = y;
                this.f_value = int.MaxValue;
                this.h_value = h_value;
            }
        }
        
        static bool IsOpenCell(CellInfo cell)
        {
            if (cell == null)
            {
                return true;
            }
            else
            {
                return openCells.Contains(cell);
            }
        }
        
        static bool IsCloseCell(CellInfo cell)
        {
            return closeCells.Contains(cell);
        }
        
        static void Visit(CellInfo cellFrom, CellInfo cellTo)
        {
            if (cellMap == null)
            {
                return;
            }
            
            if (IsCloseCell(cellTo) == true)
            {
                return;
            }
            
            int new_f_value = (cellFrom != null ? cellFrom.f_value : 0) + cellTo.h_value;
            
            if (cellTo.f_value > new_f_value)
            {
                cellTo.f_value = new_f_value;
            
                if (IsOpenCell(cellTo) == false)
                {
                    openCells.Add(cellTo);
                }
                
                cellMap[cellTo.x, cellTo.y].BackColor = Color.Yellow;
            }
        }
        
        static CellInfo GetOptimalCell()
        {
            int min_h_value = int.MaxValue;
            CellInfo optimalCell = null;
            
            foreach (CellInfo cell in openCells)
            {
                if (cell.h_value < min_h_value)
                {
                    min_h_value = cell.h_value;
                    optimalCell = cell;
                }
            }
            
            return optimalCell;
        }
        
        static void OutputMap(CellInfo startCell, CellInfo goalCell)
        {
            for (int y = 0; y < frmMain.MAP_HEIGHT; y++)
            {
                for (int x = 0; x < frmMain.MAP_WIDTH; x++)
                {
                    int f_value = (cellMap[x, y].Tag as CellInfo).f_value;
                    
                    if (f_value == int.MaxValue)
                    {
                        Console.Write("INF".PadLeft(5));
                    }
                    else
                    {
                        Console.Write(f_value.ToString().PadLeft(5));
                    }
                }
                
                Console.WriteLine();
            }
        }

        static public void FindPath(CellInfo startCell, CellInfo goalCell)
        {
            if (cellMap == null)
            {
                return;
            }
            
            for (int y = 0; y < frmMain.MAP_HEIGHT; y++)
            {
                for (int x = 0; x < frmMain.MAP_WIDTH; x++)
                {
                    (cellMap[x, y].Tag as CellInfo).f_value = int.MaxValue;
                }
            }
            
            openCells.Clear();
            closeCells.Clear();
            
            Visit(null, startCell);
            
            while (openCells.Count > 0)
            {
                CellInfo cellFrom = GetOptimalCell();
                
                openCells.Remove(cellFrom);
                closeCells.Add(cellFrom);
                
                int x = cellFrom.x;
                int y = cellFrom.y;
                
                if (x == goalCell.x && y == goalCell.y)
                {
                    OutputMap(startCell, goalCell);
                    return;
                }
                
                if (y - 1 >= 0)
                {
                    // Top-Left
                    if (x - 1 >= 0) 
                    {
                        Visit(cellFrom, cellMap[x - 1, y - 1].Tag as CellInfo);
                    }
                    
                    // Top
                    Visit(cellFrom, cellMap[x, y - 1].Tag as CellInfo);
                        
                    // Top-Right
                    if (x + 1 < frmMain.MAP_WIDTH)
                    {
                        Visit(cellFrom, cellMap[x + 1, y - 1].Tag as CellInfo);
                    }
                }
                
                // Middle-Left
                if (x - 1 >= 0)
                {
                    Visit(cellFrom, cellMap[x - 1, y].Tag as CellInfo);
                }
                
                // Middle-Right
                if (x + 1 < frmMain.MAP_WIDTH)
                {
                    Visit(cellFrom, cellMap[x + 1, y].Tag as CellInfo);
                }
                
                if (y + 1 < frmMain.MAP_HEIGHT)
                {
                    // Bottom-Left
                    if (x - 1 >= 0) 
                    {
                        Visit(cellFrom, cellMap[x - 1, y + 1].Tag as CellInfo);
                    }
                    
                    // Bottom
                    Visit(cellFrom, cellMap[x, y + 1].Tag as CellInfo);
                        
                    // Bottom-Right
                    if (x + 1 < frmMain.MAP_WIDTH)
                    {
                        Visit(cellFrom, cellMap[x + 1, y + 1].Tag as CellInfo);
                    }
                }
                
                Application.DoEvents();
            }
        }
    }
}
