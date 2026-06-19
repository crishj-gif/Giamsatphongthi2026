using System;
using System.Drawing;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    /// <summary>
    /// Form frmMultiCam: Giao diện giả lập trung tâm giám sát đa luồng (nhiều IP Camera cùng lúc).
    /// Hiện tại đang tạo giao diện lưới (grid) 2x2 để mô phỏng 4 camera.
    /// </summary>
    public partial class frmMultiCam : Form
    {
        public frmMultiCam()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Khởi tạo các thành phần giao diện cho form bằng code.
        /// (Không dùng giao diện kéo thả Designer thông thường để tối ưu hóa việc tạo hàng loạt Control)
        /// </summary>
        private void InitializeComponent()
        {
            // TableLayoutPanel: Bảng bố cục, giúp tự động chia đều không gian trên Form
            TableLayoutPanel tlpCameras = new TableLayoutPanel();
            tlpCameras.Dock = DockStyle.Fill; // Đổ đầy toàn bộ Form
            tlpCameras.RowCount = 2;          // 2 hàng
            tlpCameras.ColumnCount = 2;       // 2 cột
            // Thiết lập tỷ lệ kích thước mỗi hàng/cột là 50% (chia đều 4 phần bằng nhau)
            tlpCameras.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpCameras.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpCameras.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpCameras.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            
            // Vòng lặp tạo ra 4 khung Camera tương ứng cho 4 phòng
            for (int i = 0; i < 4; i++)
            {
                // GroupBox: Hộp nhóm có đường viền và tiêu đề
                GroupBox gb = new GroupBox();
                gb.Text = "🎥 IP Camera Demo - Phòng " + (101 + i);
                gb.Dock = DockStyle.Fill;
                gb.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                gb.ForeColor = Color.White;
                gb.Padding = new Padding(10); // Khoảng cách từ viền vào nội dung bên trong
                
                // PictureBox: Dùng để hiển thị khung hình video/hình ảnh của Camera
                PictureBox pb = new PictureBox();
                pb.Dock = DockStyle.Fill;
                pb.BackColor = Color.Black;
                pb.SizeMode = PictureBoxSizeMode.CenterImage;
                
                // Label: Nhãn hiển thị trạng thái đang kết nối trên nền đen của Camera
                Label lblStatus = new Label();
                lblStatus.Text = "Đang kết nối RTSP/IP Stream...\n(IP Cams Demo)";
                lblStatus.ForeColor = Color.LimeGreen;
                lblStatus.BackColor = Color.Black; // Nền đen tiệp màu với PictureBox
                lblStatus.Font = new Font("Segoe UI", 12F);
                lblStatus.AutoSize = true; // Tự động co giãn theo nội dung chữ
                lblStatus.Location = new Point(20, 20); // Vị trí (x=20, y=20) tính từ góc trái trên
                
                // Gắn Label vào PictureBox (để Label nổi lên trên nền PictureBox)
                pb.Controls.Add(lblStatus);
                // Gắn PictureBox vào trong GroupBox
                gb.Controls.Add(pb);
                
                // Đưa GroupBox vào trong bảng TableLayoutPanel.
                // i % 2: Tính cột (0 hoặc 1), i / 2: Tính hàng (0 hoặc 1)
                tlpCameras.Controls.Add(gb, i % 2, i / 2);
            }

            // Gắn TableLayoutPanel (chứa 4 cụm Camera) vào Form chính
            this.Controls.Add(tlpCameras);
            this.Text = "Trung Tâm Giám Sát Đa Luồng (IP Cameras)";
            this.Size = new Size(1280, 800); // Kích thước mặc định của cửa sổ
            this.StartPosition = FormStartPosition.CenterScreen; // Hiển thị Form ra giữa màn hình
            this.BackColor = Color.FromArgb(40, 40, 40); // Đặt màu nền là xám đen
        }
    }
}
