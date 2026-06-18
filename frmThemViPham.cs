using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    /// <summary>
    /// Dialog nhỏ cho phép giám thị THÊM THỦ CÔNG một nhật ký vi phạm.
    /// Trả về DialogResult.OK khi Insert thành công.
    /// </summary>
    public class frmThemViPham : Form
    {
        private const string CONN_STR =
            "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";

        // ---- Controls ----
        private Label      lblTitle, lblMaSV, lblLoai, lblGhiChu;
        private ComboBox   cmbCandidates, cmbViolationType;
        private TextBox    txtGhiChu;
        private CheckBox   chkConfirm;
        private Button     btnLuu, btnHuy;
        private DateTimePicker dtpTime;
        private Label      lblTime;

        public frmThemViPham()
        {
            BuildUI();
            this.Load += FrmThemViPham_Load;
        }

        private void BuildUI()
        {
            this.Text            = "➕ Thêm Nhật Ký Vi Phạm Thủ Công";
            this.Size            = new Size(560, 420);
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Font            = new Font("Segoe UI", 10f);

            lblTitle = new Label
            {
                Text      = "THÊM VI PHẠM THỦ CÔNG",
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                Dock      = DockStyle.Top,
                Height    = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            int leftX = 30, valX = 180, rowH = 44;
            int row1 = 70, row2 = row1 + rowH, row3 = row2 + rowH,
                row4 = row3 + rowH, row5 = row4 + rowH;

            // Mã SV / Thí sinh
            lblMaSV = MakeLabel("Thí sinh:", leftX, row1);
            cmbCandidates = new ComboBox
            {
                Location     = new Point(valX, row1 - 2),
                Size         = new Size(320, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            // Loại vi phạm
            lblLoai = MakeLabel("Loại vi phạm:", leftX, row2);
            cmbViolationType = new ComboBox
            {
                Location     = new Point(valX, row2 - 2),
                Size         = new Size(320, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
            };

            // Thời gian
            lblTime = MakeLabel("Thời gian:", leftX, row3);
            dtpTime = new DateTimePicker
            {
                Location      = new Point(valX, row3 - 2),
                Size          = new Size(320, 28),
                Format        = DateTimePickerFormat.Custom,
                CustomFormat  = "dd/MM/yyyy  HH:mm:ss",
                Value         = DateTime.Now,
            };

            // Ghi chú
            lblGhiChu = MakeLabel("Ghi chú:", leftX, row4);
            txtGhiChu = new TextBox
            {
                Location    = new Point(valX, row4 - 2),
                Size        = new Size(320, 28),
            };

            // Xác nhận
            chkConfirm = new CheckBox
            {
                Text      = "Xác nhận vi phạm ngay (IsConfirmed = 1)",
                Location  = new Point(valX, row5),
                Size      = new Size(320, 26),
                Font      = new Font("Segoe UI", 9.5f)
            };

            // Nút Lưu
            btnLuu = new Button
            {
                Text      = "💾  Lưu vi phạm",
                Location  = new Point(180, row5 + 40),
                Size      = new Size(150, 38),
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnLuu.Click += BtnLuu_Click;

            // Nút Hủy
            btnHuy = new Button
            {
                Text      = "✖  Hủy bỏ",
                Location  = new Point(345, row5 + 40),
                Size      = new Size(120, 38),
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnHuy.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                lblTitle, lblMaSV, cmbCandidates,
                lblLoai, cmbViolationType,
                lblTime, dtpTime,
                lblGhiChu, txtGhiChu,
                chkConfirm, btnLuu, btnHuy
            });
        }

        private Label MakeLabel(string text, int x, int y) => new Label
        {
            Text      = text,
            Location  = new Point(x, y + 4),
            Size      = new Size(145, 24),
            Font      = new Font("Segoe UI", 9.5f)
        };

        private void FrmThemViPham_Load(object sender, EventArgs e)
        {
            LoadCandidates();
            LoadViolationTypes();
        }

        private void LoadCandidates()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    string sql = "SELECT CandidateID, StudentCode + N' - ' + FullName AS Display FROM Candidates ORDER BY StudentCode";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cmbCandidates.DataSource    = dt;
                    cmbCandidates.DisplayMember = "Display";
                    cmbCandidates.ValueMember   = "CandidateID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách thí sinh:\n" + ex.Message);
            }
        }

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

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            // --- VALIDATION ---
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

            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO ViolationLogs
                            (CandidateID, TypeID, ViolationTime, ProctorNote, IsConfirmed)
                        VALUES
                            (@cid, @vtid, @time, @note, @confirmed)
                    ";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid",       cmbCandidates.SelectedValue);
                        // TypeID: đọc từ DataRowView an toàn
                        int typeID = -1;
                        if (cmbViolationType.SelectedItem is DataRowView drv)
                            typeID = Convert.ToInt32(drv["TypeID"]);
                        cmd.Parameters.AddWithValue("@vtid",      typeID);
                        cmd.Parameters.AddWithValue("@time",      dtpTime.Value);
                        cmd.Parameters.AddWithValue("@note",      txtGhiChu.Text.Trim());
                        cmd.Parameters.AddWithValue("@confirmed", chkConfirm.Checked ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
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
