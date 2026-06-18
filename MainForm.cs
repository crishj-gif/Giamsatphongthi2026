using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GiamSatPhongThi
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            // Set styles for smoother drawing and automatic redraw on resize
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            this.FormClosed += (s, e) => Application.Exit();

            // --- Thêm sự kiện hỗ trợ Enter ---
            textBox1.KeyDown += (s, e) => { 
                if (e.KeyCode == Keys.Enter) { 
                    e.SuppressKeyPress = true; 
                    textBox2.Focus(); 
                } 
            };
            
            textBox2.KeyDown += (s, e) => { 
                if (e.KeyCode == Keys.Enter) { 
                    e.SuppressKeyPress = true; 
                    button1.PerformClick(); 
                } 
            };
        }

        // --- CODE VẼ NỀN GRADIENT ---
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color mauTren = Color.RoyalBlue; // "Xanh dương" đậm hơn một chút cho đẹp
            Color mauDuoi = Color.White;
            
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, mauTren, mauDuoi, LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        // --- CODE NÚT ĐĂNG NHẬP ---
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tài khoản và Mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DbConnection db = new DbConnection();
            bool loginSuccess = db.CheckLogin(username, password);

            if (loginSuccess)
            {
                DialogResult modeResult = MessageBox.Show(
                    "Bạn muốn mở phiên giám sát dưới hình thức nào?\n\n" +
                    "YES - THI THẬT: Xóa toàn bộ nhật ký vi phạm cũ và bắt đầu ca thi mới.\n" +
                    "NO  - THI TEST: Giữ nguyên lịch sử vi phạm cũ để kiểm thử.", 
                    "Chọn Chế Độ Giám Sát", 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Question);

                if (modeResult == DialogResult.Cancel)
                {
                    return;
                }

                if (modeResult == DialogResult.Yes)
                {
                    try
                    {
                        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;"))
                        {
                            conn.Open();
                            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("DELETE FROM NhatKyViPham", conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi reset nhật ký: " + ex.Message);
                    }
                }

                MessageBox.Show("Đăng nhập thành công!\nChào mừng Giám thị.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 1. Khởi tạo Form Giám sát
                frmGiamSat formGiamSat = new frmGiamSat();
                
                // 2. Hiển thị Form Giám sát lên màn hình
                formGiamSat.Show();
                
                // 3. Ẩn Form Đăng nhập hiện tại đi
                this.Hide(); 
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu! Vui lòng kiểm tra lại.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                textBox2.PasswordChar = '\0'; // Hiện mật khẩu
            }
            else
            {
                textBox2.PasswordChar = '*'; // Ẩn mật khẩu
            }
        }

        // --- CÁC SỰ KIỆN GIAO DIỆN ---
        private void MainForm_Load(object sender, EventArgs e) 
        { 
            // Làm đẹp TextBox
            textBox1.BorderStyle = BorderStyle.None;
            textBox2.BorderStyle = BorderStyle.None;
            UIHelper.BoTronControl(textBox1, 8);
            UIHelper.BoTronControl(textBox2, 8);

            // Làm đẹp Button
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.Navy;
            button1.ForeColor = Color.White;
            UIHelper.BoTronControl(button1, 15);
        }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}