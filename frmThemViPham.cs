using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    /// <summary>
    /// Lớp frmThemViPham: Hiển thị một hộp thoại (Dialog) nhỏ để giám thị THÊM THỦ CÔNG một nhật ký vi phạm.
    /// Thay vì AI tự nhận diện, giám thị có thể tự tay ghi nhận lỗi của thí sinh.
    /// Trả về DialogResult.OK khi việc thêm (Insert) vào CSDL thành công.
    /// </summary>
    public class frmThemViPham : Form
    {
        // Chuỗi kết nối tới cơ sở dữ liệu SQL Server cục bộ
        private const string CONN_STR =
            "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";

        // ---- Các Controls (Thành phần giao diện) ----
        private Label      lblTitle, lblMaSV, lblLoai, lblGhiChu; // Nhãn văn bản
        private ComboBox   cmbCandidates, cmbViolationType;       // Hộp chọn (Dropdown list)
        private TextBox    txtGhiChu;                             // Ô nhập chữ
        private CheckBox   chkConfirm;                            // Ô đánh dấu kiểm
        private Button     btnLuu, btnHuy;                        // Nút bấm
        private DateTimePicker dtpTime;                           // Bộ chọn ngày giờ
        private Label      lblTime;

        public frmThemViPham()
        {
            // Gọi hàm tự xây dựng giao diện bằng code
            BuildUI();
            
            // Đăng ký sự kiện: Khi Form vừa được tải lên thì chạy hàm FrmThemViPham_Load
            this.Load += FrmThemViPham_Load;
        }

        /// <summary>
        /// Tạo giao diện động hoàn toàn bằng code C# (Không sử dụng Designer kéo thả).
        /// Cách này giúp lập trình viên kiểm soát chính xác vị trí và kích thước của từng Control.
        /// </summary>
        private void BuildUI()
        {
            this.Text            = "➕ Thêm Nhật Ký Vi Phạm Thủ Công"; // Tiêu đề cửa sổ
            this.Size            = new Size(560, 420);                 // Kích thước ngang x dọc
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent; // Hiện giữa form cha
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; // Cửa sổ cố định, không thể kéo giãn
            this.MaximizeBox     = false; // Ẩn nút phóng to
            this.MinimizeBox     = false; // Ẩn nút thu nhỏ
            this.Font            = new Font("Segoe UI", 10f); // Phông chữ chung cho toàn bộ Form

            // Khởi tạo Tiêu đề chính
            lblTitle = new Label
            {
                Text      = "THÊM VI PHẠM THỦ CÔNG",
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                Dock      = DockStyle.Top, // Đưa nhãn lên dính sát lề trên
                Height    = 50,
                TextAlign = ContentAlignment.MiddleCenter // Chữ căn giữa
            };

            // Biến căn lề và tính toán khoảng cách
            int leftX = 30, valX = 180, rowH = 44;
            int row1 = 70, row2 = row1 + rowH, row3 = row2 + rowH,
                row4 = row3 + rowH, row5 = row4 + rowH;

            // Dòng 1: Chọn Mã SV / Thí sinh
            lblMaSV = MakeLabel("Thí sinh:", leftX, row1);
            cmbCandidates = new ComboBox
            {
                Location     = new Point(valX, row1 - 2),
                Size         = new Size(320, 28),
                DropDownStyle = ComboBoxStyle.DropDownList, // Không cho gõ chữ vào combobox, chỉ được chọn
            };

            // Dòng 2: Chọn Loại vi phạm
            lblLoai = MakeLabel("Loại vi phạm:", leftX, row2);
            cmbViolationType = new ComboBox
            {
                Location     = new Point(valX, row2 - 2),
                Size         = new Size(320, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            // Dòng 3: Chọn Thời gian vi phạm
            lblTime = MakeLabel("Thời gian:", leftX, row3);
            dtpTime = new DateTimePicker
            {
                Location      = new Point(valX, row3 - 2),
                Size          = new Size(320, 28),
                Format        = DateTimePickerFormat.Custom,    // Định dạng tùy chỉnh
                CustomFormat  = "dd/MM/yyyy  HH:mm:ss",         // Ngày tháng và Giờ phút giây
                Value         = DateTime.Now,                   // Mặc định lấy giờ hiện tại
            };

            // Dòng 4: Nhập Ghi chú
            lblGhiChu = MakeLabel("Ghi chú:", leftX, row4);
            txtGhiChu = new TextBox
            {
                Location    = new Point(valX, row4 - 2),
                Size        = new Size(320, 28),
            };

            // Dòng 5: Đánh dấu xác nhận luôn
            chkConfirm = new CheckBox
            {
                Text      = "Xác nhận vi phạm ngay (IsConfirmed = 1)",
                Location  = new Point(valX, row5),
                Size      = new Size(320, 26),
                Font      = new Font("Segoe UI", 9.5f)
            };

            // Nút bấm: Lưu
            btnLuu = new Button
            {
                Text      = "💾  Lưu vi phạm",
                Location  = new Point(180, row5 + 40),
                Size      = new Size(150, 38),
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor    = Cursors.Hand // Biến con trỏ chuột thành hình bàn tay khi đưa vào
            };
            btnLuu.Click += BtnLuu_Click; // Gắn sự kiện khi bấm nút

            // Nút bấm: Hủy
            btnHuy = new Button
            {
                Text      = "✖  Hủy bỏ",
                Location  = new Point(345, row5 + 40),
                Size      = new Size(120, 38),
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            // Sử dụng biểu thức Lambda (=>) để gán lệnh ngắn gọn: Gán DialogResult = Cancel và Đóng cửa sổ
            btnHuy.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // Thêm tất cả Control vào Form
            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                lblTitle, lblMaSV, cmbCandidates,
                lblLoai, cmbViolationType,
                lblTime, dtpTime,
                lblGhiChu, txtGhiChu,
                chkConfirm, btnLuu, btnHuy
            });
        }

        /// <summary>
        /// Hàm hỗ trợ khởi tạo Label nhanh gọn, tránh lặp lại code nhiều lần (Cú pháp expression-bodied method =>).
        /// </summary>
        private Label MakeLabel(string text, int x, int y) => new Label
        {
            Text      = text,
            Location  = new Point(x, y + 4),
            Size      = new Size(145, 24),
            Font      = new Font("Segoe UI", 9.5f)
        };

        /// <summary>
        /// Sự kiện kích hoạt khi Form chuẩn bị hiện lên. Sẽ gọi CSDL để tải dữ liệu lên ComboBox.
        /// </summary>
        private void FrmThemViPham_Load(object sender, EventArgs e)
        {
            LoadCandidates();      // Lấy danh sách thí sinh
            LoadViolationTypes();  // Lấy danh mục loại vi phạm
        }

        /// <summary>
        /// Truy vấn CSDL bảng Candidates để lấy danh sách sinh viên đưa vào ComboBox.
        /// </summary>
        private void LoadCandidates()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    // Tạo một cột ảo 'Display' bằng cách ghép nối (Nối chuỗi trong SQL dùng dấu +) Mã SV và Họ Tên
                    string sql = "SELECT CandidateID, StudentCode + N' - ' + FullName AS Display FROM Candidates ORDER BY StudentCode";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt); // Đổ dữ liệu từ SQL vào DataTable
                    
                    cmbCandidates.DataSource    = dt;         // Nguồn dữ liệu
                    cmbCandidates.DisplayMember = "Display";  // Cột sẽ hiển thị ra cho người dùng xem
                    cmbCandidates.ValueMember   = "CandidateID"; // Cột ID ẩn bên dưới dùng để lưu vào CSDL
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách thí sinh:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Truy vấn bảng ViolationTypes để lấy danh mục các lỗi vi phạm (ví dụ: dùng điện thoại, quay bài).
        /// </summary>
        private void LoadViolationTypes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    string sql = "SELECT TypeID, TypeName FROM ViolationTypes ORDER BY TypeName";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    
                    cmbViolationType.DataSource    = dt;
                    cmbViolationType.DisplayMember = "TypeName";
                    cmbViolationType.ValueMember   = "TypeID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được loại vi phạm:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Sự kiện khi ấn nút Lưu vi phạm.
        /// Tiến hành kiểm tra dữ liệu đầu vào (Validation) và Insert vào CSDL.
        /// </summary>
        private void BtnLuu_Click(object sender, EventArgs e)
        {
            // --- VALIDATION: KIỂM TRA ĐẦU VÀO ---
            if (cmbCandidates.SelectedValue == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn thí sinh!", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbViolationType.SelectedValue == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn loại vi phạm!", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- LƯU VÀO CƠ SỞ DỮ LIỆU ---
            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    // Câu lệnh INSERT, dùng tham số (@cid, @vtid...) để tránh SQL Injection
                    string sql = @"
                        INSERT INTO ViolationLogs
                            (CandidateID, TypeID, ViolationTime, ProctorNote, IsConfirmed)
                        VALUES
                            (@cid, @vtid, @time, @note, @confirmed)
                    ";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid",       cmbCandidates.SelectedValue);
                        
                        // Xử lý lấy TypeID một cách an toàn (tránh lỗi ép kiểu nếu dữ liệu trả về bị lệch loại)
                        int typeID = -1;
                        if (cmbViolationType.SelectedItem is DataRowView drv)
                            typeID = Convert.ToInt32(drv["TypeID"]);
                            
                        cmd.Parameters.AddWithValue("@vtid",      typeID);
                        cmd.Parameters.AddWithValue("@time",      dtpTime.Value); // Lấy giá trị ngày giờ từ DateTimePicker
                        cmd.Parameters.AddWithValue("@note",      txtGhiChu.Text.Trim()); // Xóa khoảng trắng thừa ở 2 đầu chuỗi
                        cmd.Parameters.AddWithValue("@confirmed", chkConfirm.Checked ? 1 : 0); // Toán tử 3 ngôi: Checked là 1, ko Checked là 0
                        
                        // Chạy câu truy vấn không trả về bảng (Dùng cho Insert/Update/Delete)
                        cmd.ExecuteNonQuery();
                    }
                }
                
                // Trả kết quả báo thành công cho form cha (nơi gọi form này lên) và tự đóng cửa sổ
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
