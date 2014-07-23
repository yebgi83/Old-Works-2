namespace AStar
{
    partial class frmMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdRandomize = new System.Windows.Forms.Button();
            this.mapBaseCell = new System.Windows.Forms.PictureBox();
            this.panelMap = new System.Windows.Forms.Panel();
            this.cmdFindPath = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mapBaseCell)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdRandomize
            // 
            this.cmdRandomize.Location = new System.Drawing.Point(12, 12);
            this.cmdRandomize.Name = "cmdRandomize";
            this.cmdRandomize.Size = new System.Drawing.Size(90, 23);
            this.cmdRandomize.TabIndex = 0;
            this.cmdRandomize.Text = "Randomize";
            this.cmdRandomize.UseVisualStyleBackColor = true;
            this.cmdRandomize.Click += new System.EventHandler(this.cmdRandomize_Click);
            // 
            // mapBaseCell
            // 
            this.mapBaseCell.BackColor = System.Drawing.Color.White;
            this.mapBaseCell.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapBaseCell.Location = new System.Drawing.Point(534, 12);
            this.mapBaseCell.Name = "mapBaseCell";
            this.mapBaseCell.Size = new System.Drawing.Size(24, 24);
            this.mapBaseCell.TabIndex = 1;
            this.mapBaseCell.TabStop = false;
            this.mapBaseCell.Visible = false;
            this.mapBaseCell.Click += new System.EventHandler(this.mapBaseCell_Click);
            // 
            // panelMap
            // 
            this.panelMap.Location = new System.Drawing.Point(12, 42);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(546, 549);
            this.panelMap.TabIndex = 2;
            // 
            // cmdFindPath
            // 
            this.cmdFindPath.Location = new System.Drawing.Point(108, 12);
            this.cmdFindPath.Name = "cmdFindPath";
            this.cmdFindPath.Size = new System.Drawing.Size(90, 23);
            this.cmdFindPath.TabIndex = 3;
            this.cmdFindPath.Text = "Find path";
            this.cmdFindPath.UseVisualStyleBackColor = true;
            this.cmdFindPath.Click += new System.EventHandler(this.cmdFindPath_Click);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(570, 602);
            this.Controls.Add(this.cmdFindPath);
            this.Controls.Add(this.panelMap);
            this.Controls.Add(this.mapBaseCell);
            this.Controls.Add(this.cmdRandomize);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "A* Test (Path finding algorithm)";
            ((System.ComponentModel.ISupportInitialize)(this.mapBaseCell)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdRandomize;
        private System.Windows.Forms.PictureBox mapBaseCell;
        private System.Windows.Forms.Panel panelMap;
        private System.Windows.Forms.Button cmdFindPath;
    }
}

