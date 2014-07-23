namespace DgvDownloadImage
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
            this.components = new System.ComponentModel.Container();
            this.dgvImgs = new System.Windows.Forms.DataGridView();
            this.tmrDownload = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImgs)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvImgs
            // 
            this.dgvImgs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvImgs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvImgs.Location = new System.Drawing.Point(0, 0);
            this.dgvImgs.Name = "dgvImgs";
            this.dgvImgs.RowTemplate.Height = 23;
            this.dgvImgs.Size = new System.Drawing.Size(506, 484);
            this.dgvImgs.TabIndex = 0;
            this.dgvImgs.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvImgs_Scroll);
            // 
            // tmrDownload
            // 
            this.tmrDownload.Tick += new System.EventHandler(this.tmrDownload_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 484);
            this.Controls.Add(this.dgvImgs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImgs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvImgs;
        private System.Windows.Forms.Timer tmrDownload;
    }
}

