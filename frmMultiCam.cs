using System;
using System.Drawing;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    public partial class frmMultiCam : Form
    {
        public frmMultiCam()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            TableLayoutPanel tlpCameras = new TableLayoutPanel();
            tlpCameras.Dock = DockStyle.Fill;
            tlpCameras.RowCount = 2;
            tlpCameras.ColumnCount = 2;
            tlpCameras.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpCameras.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpCameras.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpCameras.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            
            for (int i = 0; i < 4; i++)
            {
                GroupBox gb = new GroupBox();
                gb.Text = "🎥 IP Camera Demo - Phòng " + (101 + i);
                gb.Dock = DockStyle.Fill;
                gb.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                gb.ForeColor = Color.White;
                gb.Padding = new Padding(10);
                
                PictureBox pb = new PictureBox();
                pb.Dock = DockStyle.Fill;
                pb.BackColor = Color.Black;
                pb.SizeMode = PictureBoxSizeMode.CenterImage;
                
                Label lblStatus = new Label();
                lblStatus.Text = "Đang kết nối RTSP/IP Stream...\n(IP Cams Demo)";
                lblStatus.ForeColor = Color.LimeGreen;
                lblStatus.BackColor = Color.Black;
                lblStatus.Font = new Font("Segoe UI", 12F);
                lblStatus.AutoSize = true;
                lblStatus.Location = new Point(20, 20);
                
                pb.Controls.Add(lblStatus);
                gb.Controls.Add(pb);
                
                tlpCameras.Controls.Add(gb, i % 2, i / 2);
            }

            this.Controls.Add(tlpCameras);
            this.Text = "Trung Tâm Giám Sát Đa Luồng (IP Cameras)";
            this.Size = new Size(1280, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 40, 40);
        }
    }
}
