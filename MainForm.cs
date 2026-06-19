using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GiamSatPhongThi
{
    /// <summary>
    /// Form MainForm: Màn hình đăng nhập của ứng dụng Giám sát phòng thi.
    /// Cho phép người dùng (giám thị) nhập tài khoản, mật khẩu để vào hệ thống.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            // Set styles for smoother drawing and automatic redraw on resize
            // Cài đặt này giúp màn hình mượt mà hơn khi vẽ (đặc biệt là lúc vẽ màu nền Gradient)
            // DoubleBuffered giúp giảm hiện tượng nháy màn hình (flickering) khi vẽ lại giao diện
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            
            // Sự kiện: Khi Form đăng nhập đóng, thoát hoàn toàn ứng dụng (nếu không app vẫn chạy ngầm)
            this.FormClosed += (s, e) => Application.Exit();

            // --- Thêm sự kiện hỗ trợ Enter ---
            // Bắt sự kiện khi người dùng gõ phím trong ô Tài khoản (textBox1)
            textBox1.KeyDown += (s, e) => { 
                if (e.KeyCode == Keys.Enter) { // Nếu phím gõ là phím Enter
                    e.SuppressKeyPress = true; // Ngăn tiếng "beep" mặc định của Windows
                    textBox2.Focus();          // Chuyển con trỏ soạn thảo sang ô Mật khẩu
                } 
            };
            
            // Bắt sự kiện khi người dùng gõ phím trong ô Mật khẩu (textBox2)
            textBox2.KeyDown += (s, e) => { 
                if (e.KeyCode == Keys.Enter) { 
                    e.SuppressKeyPress = true; 
                    button1.PerformClick();    // Tự động nhấn nút Đăng nhập (button1)
                } 
            };
        }

        // --- CODE VẼ NỀN GRADIENT ---
        /// <summary>
        /// Ghi đè phương thức OnPaint để vẽ màu nền của Form chuyển sắc (Gradient).
        /// Sự kiện OnPaint tự động kích hoạt mỗi khi giao diện cần được vẽ lại (ví dụ khi mở form, thay đổi kích thước).
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color mauTren = Color.RoyalBlue; // "Xanh dương" đậm hơn một chút cho đẹp
            Color mauDuoi = Color.White;
            
            // LinearGradientBrush: Cọ vẽ tạo hiệu ứng chuyển từ màu trên xuống màu dưới
            // ClientRectangle: Diện tích thực tế bên trong của Form
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, mauTren, mauDuoi, LinearGradientMode.Vertical))
            {
                // FillRectangle: Tô màu hình chữ nhật (toàn bộ Form) bằng cọ vừa tạo
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        // --- CODE NÚT ĐĂNG NHẬP ---
        /// <summary>
        /// Sự kiện xảy ra khi người dùng nhấn chuột vào nút Đăng nhập (button1).
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // Lấy văn bản người dùng đã nhập
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Kiểm tra xem người dùng có bỏ trống ô nào không
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tài khoản và Mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng lại, không chạy code phía dưới nữa
            }

            // Gọi hàm kiểm tra thông tin đăng nhập trong cơ sở dữ liệu
            DbConnection db = new DbConnection();
            bool loginSuccess = db.CheckLogin(username, password);

            if (loginSuccess)
            {
                // Hiện hộp thoại hỏi người dùng chế độ chạy
                DialogResult modeResult = MessageBox.Show(
                    "Bạn muốn mở phiên giám sát dưới hình thức nào?\n\n" +
                    "YES - THI THẬT: Xóa toàn bộ nhật ký vi phạm cũ và bắt đầu ca thi mới.\n" +
                    "NO  - THI TEST: Giữ nguyên lịch sử vi phạm cũ để kiểm thử.", 
                    "Chọn Chế Độ Giám Sát", 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Question);

                if (modeResult == DialogResult.Cancel)
                {
                    return; // Người dùng hủy, không làm gì cả
                }

                // Nếu chọn THI THẬT thì xóa sạch dữ liệu nhật ký cũ
                if (modeResult == DialogResult.Yes)
                {
                    try
                    {
                        // Khởi tạo và mở kết nối tới database
                        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;"))
                        {
                            conn.Open();
                            // Thực thi câu lệnh SQL DELETE xóa sạch các hàng trong bảng NhatKyViPham
                            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("DELETE FROM NhatKyViPham", conn))
                            {
                                cmd.ExecuteNonQuery(); // Lệnh thực thi thay đổi (không trả về dữ liệu)
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi reset nhật ký: " + ex.Message);
                    }
                }

                MessageBox.Show("Đăng nhập thành công!\nChào mừng Giám thị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 1. Khởi tạo Form Giám sát chính
                frmGiamSat formGiamSat = new frmGiamSat();
                
                // 2. Hiển thị Form Giám sát lên màn hình
                formGiamSat.Show();
                
                // 3. Ẩn Form Đăng nhập hiện tại đi (Không đóng lại vì nếu đóng thì ứng dụng sẽ tắt)
                this.Hide(); 
            }
            else
            {
                // Nếu sai tài khoản mật khẩu
                MessageBox.Show("Sai tài khoản hoặc mật khẩu! Vui lòng kiểm tra lại.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus(); // Chuyển con trỏ về ô mật khẩu
                textBox2.SelectAll(); // Bôi đen mật khẩu sai để người dùng dễ xóa
            }
        }

        /// <summary>
        /// Sự kiện xảy ra khi người dùng tích/bỏ tích vào ô "Hiển thị mật khẩu" (chkShowPassword)
        /// </summary>
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                textBox2.PasswordChar = '\0'; // Ký tự \0 (null) giúp hiển thị ký tự như bình thường
            }
            else
            {
                textBox2.PasswordChar = '*'; // Ẩn mật khẩu bằng dấu sao
            }
        }

        // --- CÁC SỰ KIỆN GIAO DIỆN ---
        /// <summary>
        /// Sự kiện chạy 1 lần duy nhất khi Form đang được tải lên (trước khi hiển thị ra màn hình).
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e) 
        { 
            // Làm đẹp TextBox: Xóa viền bao quanh
            textBox1.BorderStyle = BorderStyle.None;
            textBox2.BorderStyle = BorderStyle.None;
            // Gọi hàm tiện ích (UIHelper) để làm bo góc các ô nhập liệu
            UIHelper.BoTronControl(textBox1, 8);
            UIHelper.BoTronControl(textBox2, 8);

            // Làm đẹp Button: Biến thành nút dạng phẳng (Flat), xóa viền, chỉnh màu
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.Navy;
            button1.ForeColor = Color.White;
            // Bo góc mạnh cho nút bấm
            UIHelper.BoTronControl(button1, 15);
        }
        
        // Các sự kiện click không sử dụng nhưng có thể sinh ra do Designer tự tạo
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}