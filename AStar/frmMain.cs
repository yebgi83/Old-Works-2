using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace AStar
{
    public partial class frmMain : Form
    {
        public const int RANDOM_TRYCOUNT = 200;
        public const int MAP_WIDTH = 20;
        public const int MAP_HEIGHT = 20;
        
        private PictureBox startCell = null;
        private PictureBox goalCell = null;
                
        public frmMain()
        {
            InitializeComponent();
            InitializeMap(MAP_WIDTH, MAP_HEIGHT);
        }
        
        private PictureBox CreateMapCell()
        {
            PictureBox newMapCell = new PictureBox();
            PropertyInfo[] properties = this.mapBaseCell.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            
            foreach (PropertyInfo propInfo in properties)
            {
                if (propInfo.CanWrite == true)
                {
                    if (propInfo.Name != "WindowTarget")
                    {
                        propInfo.SetValue(newMapCell, propInfo.GetValue(this.mapBaseCell, null), null);
                    }
                }
            }
            
            return newMapCell;
        }
        
        private void InitializeMap(int width, int height)
        {
            AStar.cellMap = new PictureBox[width, height];
           
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    AStar.cellMap[x, y] = CreateMapCell();
                    AStar.cellMap[x, y].Left = (x * (this.mapBaseCell.Width + this.mapBaseCell.Margin.Right)) + this.mapBaseCell.Margin.Left;
                    AStar.cellMap[x, y].Top = (y * (this.mapBaseCell.Height + this.mapBaseCell.Margin.Bottom)) + this.mapBaseCell.Margin.Top;
                    AStar.cellMap[x, y].Visible = true;
                    AStar.cellMap[x, y].Click += mapBaseCell_Click;
                    AStar.cellMap[x, y].Tag = new AStar.CellInfo(x, y, 1);

                    panelMap.Controls.Add(AStar.cellMap[x, y]);
                }
            }
            
            this.startCell = AStar.cellMap[0, 0];
            this.goalCell = AStar.cellMap[MAP_WIDTH - 1, MAP_HEIGHT - 1];
            
            this.startCell.BackColor = Color.Red;
            this.goalCell.BackColor = Color.Blue;
        }

        private void mapBaseCell_Click(object sender, EventArgs e)
        {
            if ((sender as Control).BackColor == Color.White)
            {
                (sender as Control).BackColor = Color.Black;
                ((sender as Control).Tag as AStar.CellInfo).h_value = 1000;
            }
            else if ((sender as Control).BackColor == Color.Black)
            {
                (sender as Control).BackColor = Color.White;
                ((sender as Control).Tag as AStar.CellInfo).h_value = 1;
            }
        }

        private void cmdRandomize_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = 0; y < MAP_HEIGHT; y++)
                {
                    AStar.cellMap[x, y].BackColor = Color.White;
                    (AStar.cellMap[x, y].Tag as AStar.CellInfo).h_value = 1;
                }
            }
            
            this.startCell.BackColor = Color.Red;
            this.goalCell.BackColor = Color.Blue;
            
            Random random = new Random();

            for (int tryCount = 1; tryCount <= RANDOM_TRYCOUNT; tryCount++)
            {
                int x = random.Next(1, MAP_WIDTH);
                int y = random.Next(1, MAP_HEIGHT);
                
                if (AStar.cellMap[x, y].BackColor != Color.White)
                {
                    tryCount--;
                    continue;
                }
                 
                mapBaseCell_Click
                (
                    AStar.cellMap[x, y],
                    EventArgs.Empty
                );
            }
        }

        private void cmdFindPath_Click(object sender, EventArgs e)
        {
            AStar.FindPath
            (
                this.startCell.Tag as AStar.CellInfo, 
                this.goalCell.Tag as AStar.CellInfo
            );
        }
    }
}
