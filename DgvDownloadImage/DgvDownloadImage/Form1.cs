using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DgvDownloadImage
{
    public partial class frmMain : Form
    {
        private readonly string[] urlList = 
        {
            "http://lh5.ggpht.com/_mrb7w4gF8Ds/TCpetKSqM1I/AAAAAAAAD2c/Qef6Gsqf12Y/s144-c/_DSC4374%20copy.jpg",
            "http://lh5.ggpht.com/_Z6tbBnE-swM/TB0CryLkiLI/AAAAAAAAVSo/n6B78hsDUz4/s144-c/_DSC3454.jpg",
            "http://lh3.ggpht.com/_GEnSvSHk4iE/TDSfmyCfn0I/AAAAAAAAF8Y/cqmhEoxbwys/s144-c/_MG_3675.jpg",
            "http://lh6.ggpht.com/_Nsxc889y6hY/TBp7jfx-cgI/AAAAAAAAHAg/Rr7jX44r2Gc/s144-c/IMGP9775a.jpg",
            "http://lh3.ggpht.com/_lLj6go_T1CQ/TCD8PW09KBI/AAAAAAAAQdc/AqmOJ7eg5ig/s144-c/Juvenile%20Gannet%20despute.jpg",
            "http://lh6.ggpht.com/_ZN5zQnkI67I/TCFFZaJHDnI/AAAAAAAABVk/YoUbDQHJRdo/s144-c/P9250508.JPG",
            "http://lh4.ggpht.com/_XjNwVI0kmW8/TCOwNtzGheI/AAAAAAAAC84/SxFJhG7Scgo/s144-c/0014.jpg",
            "http://lh6.ggpht.com/_lnDTHoDrJ_Y/TBvKsJ9qHtI/AAAAAAAAG6g/Zll2zGvrm9c/s144-c/000007.JPG",
            "http://lh6.ggpht.com/_qvCl2efjxy0/TCIVI-TkuGI/AAAAAAAAOUY/vbk9MURsv48/s144-c/DSC_0844.JPG",
            "http://lh4.ggpht.com/_TPlturzdSE8/TBv4ugH60PI/AAAAAAAAMsI/p2pqG85Ghhs/s144-c/_MG_3963.jpg",
            "http://lh4.ggpht.com/_4f1e_yo-zMQ/TCe5h9yN-TI/AAAAAAAAXqs/8X2fIjtKjmw/s144-c/IMG_1786.JPG",
            "http://lh6.ggpht.com/_iFt5VZDjxkY/TB9rQyWnJ4I/AAAAAAAADpU/lP2iStizJz0/s144-c/DSCF1014.JPG",
            "http://lh5.ggpht.com/_hepKlJWopDg/TB-_WXikaYI/AAAAAAAAElI/715k4NvBM4w/s144-c/IMG_0075.JPG",
            "http://lh6.ggpht.com/_OfRSx6nn68g/TCzsQic_z3I/AAAAAAABOOI/5G4Kwzb2qhk/s144-c/EASTER%20ISLAND_Hanga%20Roa_31.5.08_46.JPG",
            "http://lh6.ggpht.com/_ZGv_0FWPbTE/TB-_GLhqYBI/AAAAAAABVxs/cVEvQzt0ke4/s144-c/IMG_1288_hf.jpg",
            "http://lh6.ggpht.com/_a29lGRJwo0E/TBqOK_tUKmI/AAAAAAAAVbw/UloKpjsKP3c/s144-c/31012332.jpg",
            "http://lh3.ggpht.com/_55Lla4_ARA4/TB6xbyxxJ9I/AAAAAAABTWo/GKe24SwECns/s144-c/Bluebird%20049.JPG",
            "http://lh3.ggpht.com/_iVnqmIBYi4Y/TCaOH6rRl1I/AAAAAAAA1qg/qeMerYQ6DYo/s144-c/Kwiat_100626_0016.jpg",
            "http://lh6.ggpht.com/_QFsB_q7HFlo/TCItd_2oBkI/AAAAAAAAFsk/4lgJWweJ5N8/s144-c/3705226938_d6d66d6068_o.jpg",
            "http://lh5.ggpht.com/_JTI0xxNrKFA/TBsKQ9uOGNI/AAAAAAAChQg/z8Exh32VVTA/s144-c/CRW_0015-composite.jpg",
            "http://lh6.ggpht.com/_loGyjar4MMI/S-InVNkTR_I/AAAAAAAADJY/Fb5ifFNGD70/s144-c/Moving%20Rock.jpg",
            "http://lh4.ggpht.com/_L7i4Tra_XRY/TBtxjScXLqI/AAAAAAAAE5o/ue15HuP8eWw/s144-c/opera%20house%20II.jpg",
            "http://lh3.ggpht.com/_rfAz5DWHZYs/S9cstBTv1iI/AAAAAAAAeYA/EyZPUeLMQ98/s144-c/DSC_6425.jpg",
            "http://lh6.ggpht.com/_iGI-XCxGLew/S-iYQWBEG-I/AAAAAAAACB8/JuFti4elptE/s144-c/norvig-polar-bear.jpg",
            "http://lh3.ggpht.com/_M3slUPpIgmk/SlbnavqG1cI/AAAAAAAACvo/z6-CnXGma7E/s144-c/mf_003.jpg",
            "http://lh4.ggpht.com/_loGyjar4MMI/S-InQvd_3hI/AAAAAAAADIw/dHvCFWfyHxQ/s144-c/Rainbokeh.jpg",
            "http://lh4.ggpht.com/_yy6KdedPYp4/SB5rpK3Zv0I/AAAAAAAAOM8/mokl_yo2c9E/s144-c/Point%20Reyes%20road%20.jpg",
            "http://lh5.ggpht.com/_6_dLVKawGJA/SMwq86HlAqI/AAAAAAAAG5U/q1gDNkmE5hI/s144-c/mobius-glow.jpg",
            "http://lh3.ggpht.com/_QFsB_q7HFlo/TCItc19Jw3I/AAAAAAAAFs4/nPfiz5VGENk/s144-c/4551649039_852be0a952_o.jpg",
            "http://lh6.ggpht.com/_TQY-Nm7P7Jc/TBpjA0ks2MI/AAAAAAAABcI/J6ViH98_poM/s144-c/IMG_6517.jpg",
            "http://lh3.ggpht.com/_rfAz5DWHZYs/S9cLAeKuueI/AAAAAAAAeYU/E19G8DOlJRo/s144-c/DSC_4397_8_9_tonemapped2.jpg",
            "http://lh4.ggpht.com/_TQY-Nm7P7Jc/TBpi6rKfFII/AAAAAAAABbg/79FOc0Dbq0c/s144-c/david_lee_sakura.jpg",
            "http://lh3.ggpht.com/_TQY-Nm7P7Jc/TBpi8EJ4eDI/AAAAAAAABb0/AZ8Cw1GCaIs/s144-c/Hokkaido%20Swans.jpg",
            "http://lh3.ggpht.com/_1aZMSFkxSJI/TCIjB6od89I/AAAAAAAACHM/CLWrkH0ziII/s144-c/079.jpg",
            "http://lh5.ggpht.com/_loGyjar4MMI/S-InWuHkR9I/AAAAAAAADJE/wD-XdmF7yUQ/s144-c/Colorado%20River%20Sunset.jpg",
            "http://lh3.ggpht.com/_0YSlK3HfZDQ/TCExCG1Zc3I/AAAAAAAAX1w/9oCH47V6uIQ/s144-c/3138923889_a7fa89cf94_o.jpg",
            "http://lh6.ggpht.com/_K29ox9DWiaM/TAXe4Fi0xTI/AAAAAAAAVIY/zZA2Qqt2HG0/s144-c/IMG_7100.JPG",
            "http://lh6.ggpht.com/_0YSlK3HfZDQ/TCEx16nJqpI/AAAAAAAAX1c/R5Vkzb8l7yo/s144-c/4235400281_34d87a1e0a_o.jpg",
            "http://lh4.ggpht.com/_8zSk3OGcpP4/TBsOVXXnkTI/AAAAAAAAAEo/0AwEmuqvboo/s144-c/yosemite_forrest.jpg",
            "http://lh4.ggpht.com/_6_dLVKawGJA/SLZToqXXVrI/AAAAAAAAG5k/7fPSz_ldN9w/s144-c/coastal-1.jpg",
            "http://lh4.ggpht.com/_WW8gsdKXVXI/TBpVr9i6BxI/AAAAAAABhNg/KC8aAJ0wVyk/s144-c/IMG_6233_1_2-2.jpg",
            "http://lh3.ggpht.com/_loGyjar4MMI/S-InS0tJJSI/AAAAAAAADHU/E8GQJ_qII58/s144-c/Windmills.jpg",
            "http://lh4.ggpht.com/_loGyjar4MMI/S-InbXaME3I/AAAAAAAADHo/4gNYkbxemFM/s144-c/Frantic.jpg",
            "http://lh5.ggpht.com/_loGyjar4MMI/S-InKAviXzI/AAAAAAAADHA/NkyP5Gge8eQ/s144-c/Rice%20Fields.jpg",
            "http://lh3.ggpht.com/_loGyjar4MMI/S-InZA8YsZI/AAAAAAAADH8/csssVxalPcc/s144-c/Seahorse.jpg",
            "http://lh3.ggpht.com/_syQa1hJRWGY/TBwkCHcq6aI/AAAAAAABBEg/R5KU1WWq59E/s144-c/Antelope.JPG",
            "http://lh5.ggpht.com/_MoEPoevCLZc/S9fHzNgdKDI/AAAAAAAADwE/UAno6j5StAs/s144-c/c84_7083.jpg",
            "http://lh4.ggpht.com/_DJGvVWd7IEc/TBpRsGjdAyI/AAAAAAAAFNw/rdvyRDgUD8A/s144-c/Free.jpg",
            "http://lh6.ggpht.com/_iO97DXC99NY/TBwq3_kmp9I/AAAAAAABcz0/apq1ffo_MZo/s144-c/IMG_0682_cp.jpg",
            "http://lh4.ggpht.com/_7V85eCJY_fg/TBpXudG4_PI/AAAAAAAAPEE/8cHJ7G84TkM/s144-c/20100530_120257_0273-Edit-2.jpg" 
        };

        private int? firstDisplayIndex = null;
        private int? lastDisplayIndex = null;

        private Dictionary<int, Thread> downloadThreadPool = new Dictionary<int, Thread>();

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            InitializeDataGridView();
        }

        private void dgvImgs_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                // 다운로드 범위
                firstDisplayIndex = dgvImgs.FirstDisplayedScrollingRowIndex;
                lastDisplayIndex = dgvImgs.FirstDisplayedScrollingRowIndex + dgvImgs.DisplayedRowCount(true) - 1;
            
                // 다운로드 시도
                tmrDownload.Enabled = true;
            }
        }

        private void InitializeDataGridView()
        {
            DataGridView dataGridView = this.dgvImgs;

            // DataGridView 초기화
            dgvImgs.AllowUserToAddRows = false;
            dgvImgs.AllowUserToDeleteRows = false;
            dgvImgs.AllowUserToOrderColumns = false;
            dgvImgs.AllowUserToResizeColumns = false;
            dgvImgs.AllowUserToResizeRows = false;
            dgvImgs.BorderStyle = BorderStyle.FixedSingle;
            dgvImgs.RowTemplate.Height = 64;
            dgvImgs.RowHeadersVisible = false;
            dgvImgs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvImgs.ScrollBars = ScrollBars.Vertical;
            
            // 컬럼 초기화
            dgvImgs.Columns.Clear();

            // 컬럼 1 
            dgvImgs.Columns.Add
            (
                new DataGridViewTextBoxColumn
                {
                    Width = dgvImgs.Width - 144,
                    ReadOnly = true,
                    HeaderText = "URL"
                }
            );

            // 컬럼 2
            dgvImgs.Columns.Add
            (
                new DataGridViewImageColumn
                {
                    Width = 144,
                    ReadOnly = true,
                    HeaderText = "Image"
                }
            );

            // 데이터
            foreach (string urlItem in urlList)
            {
                dgvImgs.Rows.Add(urlItem, null);
            }

            // 다운로드 범위 
            firstDisplayIndex = dgvImgs.FirstDisplayedScrollingRowIndex;
            lastDisplayIndex = dgvImgs.FirstDisplayedScrollingRowIndex + dgvImgs.DisplayedRowCount(true) - 1;
            
            // 다운로드 시도
            tmrDownload.Enabled = true;
        }

        private Image LoadImage(string url)
        {
            try
            {
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();

                return Image.FromStream(response.GetResponseStream());
            }
            catch
            {
                return null;
            }
        }

        private void tmrDownload_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.firstDisplayIndex.HasValue == false)
                {
                    return;
                }
                else if (this.lastDisplayIndex.HasValue == false)
                {
                    return;
                }
                else
                {
                    for (int key = this.firstDisplayIndex.Value; key < this.lastDisplayIndex; key++)
                    {
                        if (this.downloadThreadPool.ContainsKey(key) == true)
                        {
                            continue;
                        }
                        else
                        {
                            Thread thread = new Thread
                            (
                                (_key) =>
                                {
                                    if (this.dgvImgs[1, (int)_key].Value == null)
                                    {
                                        this.dgvImgs[1, (int)_key].Value = LoadImage(this.dgvImgs[0, (int)_key].Value as string);
                                    }
                                }
                            );

                            thread.IsBackground = true;
                            thread.Start(key);

                            downloadThreadPool.Add(key, thread);
                        }
                    }

                    while (true)
                    {
                        int? removeKey = null;

                        foreach (int key in this.downloadThreadPool.Keys)
                        {
                            if (downloadThreadPool[key].ThreadState != ThreadState.Running)
                            {
                                removeKey = key;
                                break;
                            }
                        }

                        if (removeKey.HasValue == true)
                        {
                            this.downloadThreadPool.Remove(removeKey.Value);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            finally
            {
                tmrDownload.Enabled = false;
            }
        }
    }
}
