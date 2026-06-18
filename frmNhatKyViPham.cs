using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.Threading.Tasks;
// =====================================================================
//  frmNhatKyViPham – Nhật ký vi phạm  (Phân trang + Lọc thời điểm)
// =====================================================================

namespace GiamSatPhongThi
{
    public partial class frmNhatKyViPham : Form
    {
        // ============================================================
        //  HẰNG SỐ KẾT NỐI – dùng chung toàn form
        // ============================================================
        private const string CONN_STR =
            "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";

        // ============================================================
        //  BIẾN TRẠNG THÁI
        // ============================================================
        private DataTable dtMain = new DataTable();   // Bảng dữ liệu đang bind vào grid
        private int selectedLogID = -1;               // LogID của dòng đang chọn

        // ---- Phân trang ----
        private int _currentPage  = 1;
        private int _pageSize     = 10;
        private int _totalRecords = 0;
        private int TotalPages => _totalRecords == 0 ? 1 : (int)Math.Ceiling(_totalRecords / (double)_pageSize);

        // ---- Bộ lọc hiện tại ----
        private string  _keyword     = "";
        private int     _vtID        = -1;
        private bool    _useDate     = false;
        private DateTime _fromDate   = new DateTime(DateTime.Now.Year, 1, 1);
        private DateTime _toDate     = DateTime.Now;

        // ============================================================
        //  CONSTRUCTOR
        // ============================================================
        public frmNhatKyViPham()
        {
            InitializeComponent();
            this.Load += FrmNhatKyViPham_Load;
        }

        // ============================================================
        //  LOAD FORM
        // ============================================================
        private async void FrmNhatKyViPham_Load(object sender, EventArgs e)
        {
            ApplyModernStyling();
            
            // Add Xem Xếp Hạng button first so UI shows it immediately
            Button btnXepHang = new Button();
            btnXepHang.Text = "🏆 Top Cảnh Báo";
            btnXepHang.Size = new Size(140, 35);
            btnXepHang.Location = new Point(btnXemDrive.Right + 15, btnXemDrive.Top);
            StyleButton(btnXepHang, Color.Crimson);
            btnXepHang.Click += BtnXemXepHang_Click;
            btnXemDrive.Parent.Controls.Add(btnXepHang);

            // Let UI paint all controls (buttons, grids, groupboxes) before doing DB work
            await Task.Delay(50);

            await LoadViolationTypesAsync();
            await LoadYearsAsync();
            await LoadDataAsync();
        }

        private void BtnXemXepHang_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    string sql = @"
                        SELECT TOP 10 DoiTuong, COUNT(*) as SoLanViPham, (COUNT(*) * 10) as DiemHanhVi
                        FROM NhatKyViPham
                        WHERE DoiTuong != 'Chưa xác định' AND DoiTuong != 'Hệ thống'
                        GROUP BY DoiTuong
                        ORDER BY SoLanViPham DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        string msg = "🚨 BẢNG VÀNG CẢNH BÁO: TOP 10 THÍ SINH CÓ ĐIỂM HÀNH VI CAO NHẤT 🚨\n\n";
                        msg += "STT | Điểm Khả Nghi | Tần Suất | Họ và Tên\n";
                        msg += "--------------------------------------------------------\n";
                        int i = 1;
                        bool hasData = false;
                        while (reader.Read())
                        {
                            hasData = true;
                            string name = reader["DoiTuong"].ToString();
                            int freq = Convert.ToInt32(reader["SoLanViPham"]);
                            int diem = Convert.ToInt32(reader["DiemHanhVi"]);
                            msg += $"{i,2}  | {diem,12}đ | {freq,7} lần | {name}\n";
                            i++;
                        }
                        
                        if (!hasData)
                        {
                            msg += "Phòng thi hiện tại đang rất trong sạch! Chưa có thí sinh nào bị cảnh báo.";
                        }
                        
                        MessageBox.Show(msg, "Top Điểm Hành Vi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu xếp hạng: " + ex.Message);
            }
        }

        // ============================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void ApplyModernStyling()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;

            // ---- Set uniform font (ko in nghiêng, cùng 1 loại) ----
            Font uniformFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            SetUniformFont(this, uniformFont);

            // ---- Tách rời các panel (rời rạc hơn, bớt liền khối) ----
            // We remove Dock and use Anchor with explicit Locations to create margins
            pnlSearch.Dock = DockStyle.None;
            pnlSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlSearch.Location = new Point(12, 12);
            pnlSearch.Size = new Size(1176, 75);

            pnlDateFilter.Dock = DockStyle.None;
            pnlDateFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDateFilter.Location = new Point(12, 100); // Cách pnlSearch 13px
            pnlDateFilter.Size = new Size(1176, 65);

            grpGrid.Dock = DockStyle.None;
            grpGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpGrid.Location = new Point(12, 180); // Cách pnlDateFilter 15px
            grpGrid.Size = new Size(1176, 400);

            pnlDetail.Dock = DockStyle.None;
            pnlDetail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlDetail.Location = new Point(12, 595); // Cách grpGrid 15px
            pnlDetail.Size = new Size(1176, 100);

            pnlButtons.Dock = DockStyle.None;
            pnlButtons.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlButtons.Location = new Point(12, 710); // Cách pnlDetail 15px
            pnlButtons.Size = new Size(1176, 75);

            // ---- Button Colors (màu khác nhau vào màu cơ bản) ----
            StyleButton(btnTimKiem, Color.DodgerBlue);
            StyleButton(btnLamMoi, Color.Gray);
            StyleButton(btnLocNgay, Color.Teal);
            StyleButton(btnThem, Color.MediumSeaGreen);
            StyleButton(btnSua, Color.Orange);
            StyleButton(btnXoa, Color.Crimson);
            StyleButton(btnXuatExcel, Color.Indigo);
            StyleButton(btnXemDrive, Color.DeepPink);

            // ---- DataGridView ----
            dgvLog.RowHeadersVisible   = false;
            dgvLog.AllowUserToAddRows  = false;
            dgvLog.ReadOnly            = true;
            dgvLog.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.RowTemplate.Height  = 32;
        }

        private void SetUniformFont(Control parent, Font font)
        {
            parent.Font = font;
            foreach (Control c in parent.Controls)
            {
                SetUniformFont(c, font);
            }
        }

        private void StyleButton(Button btn, Color bg)
        {
            btn.UseVisualStyleBackColor = false;
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
        }

        // ============================================================
        //  HELPER: Đọc ViolationTypeID từ ComboBox an toàn
        //  (tránh InvalidCastException khi DataSource là DataTable)
        // ============================================================
        private int GetSelectedViolationTypeID()
        {
            // Khi ComboBox.DataSource = DataTable, SelectedItem là DataRowView
            if (cmbLoaiViPham.SelectedItem is DataRowView drv)
            {
                // Schema thực tế dùng TypeID
                string colName = drv.Row.Table.Columns.Contains("TypeID") ? "TypeID" : "ViolationTypeID";
                return Convert.ToInt32(drv[colName]);
            }
            if (cmbLoaiViPham.SelectedValue is int intVal)
                return intVal;
            return -1;
        }

        // ============================================================
        //  LOAD DANH SÁCH NĂM TỪ DB
        // ============================================================
        private async Task LoadYearsAsync()
        {
            try
            {
                DataTable dt = new DataTable();
                await Task.Run(() => 
                {
                    using (SqlConnection conn = new SqlConnection(CONN_STR))
                    {
                        conn.Open();
                        string sql = "SELECT DISTINCT YEAR(ViolationTime) AS Nam FROM ViolationLogs ORDER BY Nam DESC";
                        SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                        da.Fill(dt);
                    }
                });

                cmbNam.Items.Clear();
                cmbNam.Items.Add("-- Tất cả --");
                foreach (DataRow dr in dt.Rows)
                    cmbNam.Items.Add(dr["Nam"].ToString());
                cmbNam.SelectedIndex = 0;
            }
            catch { cmbNam.Items.Add("-- Tất cả --"); cmbNam.SelectedIndex = 0; }
        }

        // ============================================================
        //  2. TẢI DỮ LIỆU – Server-side paging + lọc ngày
        // ============================================================
        private async Task LoadDataAsync()
        {
            try
            {
                // ------- WHERE clause động -------
                string where = "WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(_keyword))
                    where += " AND (c.FullName LIKE @kw OR c.StudentCode LIKE @kw)";
                if (_vtID > 0)
                    where += " AND vl.TypeID = @vtID";
                if (_useDate)
                    where += " AND CAST(vl.ViolationTime AS DATE) BETWEEN @from AND @to";

                // ------- Đếm tổng bản ghi -------
                string sqlCount = $@"
                    SELECT COUNT(*) FROM ViolationLogs vl
                    LEFT JOIN Candidates     c  ON vl.CandidateID = c.CandidateID
                    LEFT JOIN ViolationTypes vt ON vl.TypeID      = vt.TypeID
                    {where}";

                // ------- SELECT có phân trang (OFFSET-FETCH) -------
                int offset = (_currentPage - 1) * _pageSize;
                string sqlData = $@"
                    SELECT
                        vl.LogID         AS [Mã Log],
                        c.StudentCode    AS [Mã SV],
                        c.FullName       AS [Họ và Tên],
                        c.Class          AS [Lớp],
                        vt.TypeName      AS [Loại Vi Phạm],
                        vl.ViolationTime AS [Thời Gian],
                        CASE WHEN vl.IsConfirmed=1 THEN N'✔ Đã xác nhận' ELSE N'○ Chưa xác nhận' END AS [Trạng Thái],
                        vl.ProctorNote   AS [Ghi Chú Giám Thị],
                        vl.SnapshotPath  AS [Ảnh Vi Phạm]
                    FROM ViolationLogs vl
                    LEFT JOIN Candidates     c  ON vl.CandidateID = c.CandidateID
                    LEFT JOIN ViolationTypes vt ON vl.TypeID      = vt.TypeID
                    {where}
                    ORDER BY vl.ViolationTime DESC
                    OFFSET {offset} ROWS FETCH NEXT {_pageSize} ROWS ONLY";

                int totalRecords = 0;
                DataTable dt = new DataTable();

                await Task.Run(() => 
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(CONN_STR))
                        {
                            conn.Open();
                            // Tự động fix dữ liệu cũ: Điền CandidateID dựa trên tên trong ProctorNote do AI sinh ra
                            string fixSql = @"
                                UPDATE ViolationLogs 
                                SET CandidateID = (
                                    SELECT TOP 1 CandidateID 
                                    FROM Candidates 
                                    WHERE FullName = SUBSTRING(ProctorNote, 23, CHARINDEX(')', ProctorNote) - 23)
                                )
                                WHERE CandidateID IS NULL 
                                  AND ProctorNote LIKE N'AI tự động phát hiện (%)%';
                            ";
                            using (SqlCommand cmdFix = new SqlCommand(fixSql, conn))
                            {
                                cmdFix.ExecuteNonQuery();
                            }
                        }
                    } catch { }

                    using (SqlConnection conn = new SqlConnection(CONN_STR))
                    {
                        conn.Open();

                        // -- Đếm tổng --
                        SqlCommand cmdCount = new SqlCommand(sqlCount, conn);
                        BindParams(cmdCount);
                        totalRecords = (int)cmdCount.ExecuteScalar();

                        // -- Lấy trang hiện tại --
                        SqlCommand cmdData = new SqlCommand(sqlData, conn);
                        BindParams(cmdData);
                        SqlDataAdapter da = new SqlDataAdapter(cmdData);
                        da.Fill(dt);
                    }
                });

                _totalRecords = totalRecords;
                dtMain = dt;

                dgvLog.DataSource = dtMain;
                if (dgvLog.Columns.Contains("Mã Log"))    dgvLog.Columns["Mã Log"].Visible = false;
                if (dgvLog.Columns.Contains("Ảnh Vi Phạm")) dgvLog.Columns["Ảnh Vi Phạm"].Visible = false;
                SetColumnWidths();

                // -- Cập nhật UI phân trang --
                int from = _totalRecords == 0 ? 0 : offset + 1;
                int to   = Math.Min(offset + _pageSize, _totalRecords);
                lblTongSo.Text  = $"Tổng số: {_totalRecords} vi phạm";
                lblPage.Text    = $"Trang {_currentPage} / {TotalPages}";
                lblPageInfo.Text = $"Hiển thị {from} - {to} trong {_totalRecords} bản ghi";
                btnFirst.Enabled = btnPrev.Enabled = (_currentPage > 1);
                btnNext.Enabled  = btnLast.Enabled  = (_currentPage < TotalPages);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Gắn parameters dùng chung
        private void BindParams(SqlCommand cmd)
        {
            if (!string.IsNullOrWhiteSpace(_keyword))
                cmd.Parameters.AddWithValue("@kw", "%" + _keyword + "%");
            if (_vtID > 0)
                cmd.Parameters.AddWithValue("@vtID", _vtID);
            if (_useDate)
            {
                cmd.Parameters.AddWithValue("@from", _fromDate.Date);
                cmd.Parameters.AddWithValue("@to",   _toDate.Date);
            }
        }

        private void SetColumnWidths()
        {
            var widths = new (string col, int w)[]
            {
                ("Mã SV",            90),
                ("Họ và Tên",       180),
                ("Lớp",              80),
                ("Loại Vi Phạm",    160),
                ("Thời Gian",       140),
                ("Trạng Thái",      130),
                ("Ghi Chú Giám Thị",200),
            };
            foreach (var (col, w) in widths)
                if (dgvLog.Columns.Contains(col))
                    dgvLog.Columns[col].Width = w;
        }

        // ============================================================
        //  3. TẢI LOẠI VI PHẠM VÀO COMBOBOX
        // ============================================================
        private async Task LoadViolationTypesAsync()
        {
            try
            {
                DataTable dtVT = new DataTable();
                await Task.Run(() => 
                {
                    using (SqlConnection conn = new SqlConnection(CONN_STR))
                    {
                        conn.Open();
                        string sql = "SELECT TypeID, TypeName FROM ViolationTypes ORDER BY TypeName";
                        SqlDataAdapter da  = new SqlDataAdapter(sql, conn);
                        da.Fill(dtVT);
                    }
                });

                // Thêm dòng "-- Tất cả --" đầu tiên
                DataRow drAll = dtVT.NewRow();
                drAll["TypeID"]   = -1;
                drAll["TypeName"] = "-- Tất cả loại vi phạm --";
                dtVT.Rows.InsertAt(drAll, 0);

                cmbLoaiViPham.DataSource    = dtVT;
                cmbLoaiViPham.DisplayMember = "TypeName";
                cmbLoaiViPham.ValueMember   = "TypeID";
                cmbLoaiViPham.SelectedIndex = 0;
            }
            catch
            {
                // Nếu bảng ViolationTypes chưa có dữ liệu → vẫn chạy bình thường
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("TypeID",   typeof(int));
                dtEmpty.Columns.Add("TypeName", typeof(string));
                DataRow dr = dtEmpty.NewRow();
                dr["TypeID"]   = -1;
                dr["TypeName"] = "-- Tất cả loại vi phạm --";
                dtEmpty.Rows.Add(dr);
                cmbLoaiViPham.DataSource    = dtEmpty;
                cmbLoaiViPham.DisplayMember = "TypeName";
                cmbLoaiViPham.ValueMember   = "TypeID";
            }
        }

        // ============================================================
        //  4. SỰ KIỆN CHỌN DÒNG → ĐỔ VÀO PANEL CHỈNH SỬA
        // ============================================================
        private void DgvLog_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLog.CurrentRow == null) return;

            DataRowView drv = (DataRowView)dgvLog.CurrentRow.DataBoundItem;
            if (drv == null) return;

            selectedLogID = Convert.ToInt32(drv["Mã Log"]);
            txtGhiChu.Text    = drv["Ghi Chú Giám Thị"]?.ToString() ?? "";
            chkXacNhan.Checked = drv["Trạng Thái"].ToString().Contains("✔");

            // Hiển thị thông tin sinh viên lên label phụ
            lblInfoSV.Text = $"🎓 {drv["Họ và Tên"]} ({drv["Mã SV"]}) | 🚨 {drv["Loại Vi Phạm"]} | 🕐 {drv["Thời Gian"]}";
        }

        // ============================================================
        //  5. TÌM KIẾM
        // ============================================================
        private async void BtnTimKiem_Click(object sender, EventArgs e)
        {
            _keyword     = txtTimKiem.Text.Trim();
            _vtID        = GetSelectedViolationTypeID();
            _currentPage = 1;
            await LoadDataAsync();
        }

        private void TxtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnTimKiem_Click(sender, e); }
        }

        // ============================================================
        //  6. LỌC THEO LOẠI VI PHẠM
        // ============================================================
        private async void CmbLoaiViPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            _vtID        = GetSelectedViolationTypeID();
            _currentPage = 1;
            await LoadDataAsync();
        }

        // ============================================================
        //  6b. LỌC THEO NGÀY / NĂM
        // ============================================================
        private async void ChkLocNgay_CheckedChanged(object sender, EventArgs e)
        {
            bool on = chkLocNgay.Checked;
            dtpTuNgay.Enabled  = on;
            dtpDenNgay.Enabled = on;
            cmbNam.Enabled     = on;
            btnLocNgay.Enabled = on;
            if (!on) { _useDate = false; _currentPage = 1; await LoadDataAsync(); }
        }

        private void CmbNam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!chkLocNgay.Checked) return;
            if (cmbNam.SelectedIndex <= 0) return; // "-- Tất cả --"
            if (int.TryParse(cmbNam.SelectedItem?.ToString(), out int year))
            {
                dtpTuNgay.Value = new DateTime(year, 1, 1);
                dtpDenNgay.Value = new DateTime(year, 12, 31);
            }
        }

        private async void BtnLocNgay_Click(object sender, EventArgs e)
        {
            if (dtpTuNgay.Value.Date > dtpDenNgay.Value.Date)
            {
                MessageBox.Show("⚠️ Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!",
                    "Sai khoảng ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _useDate    = true;
            _fromDate   = dtpTuNgay.Value.Date;
            _toDate     = dtpDenNgay.Value.Date;
            _currentPage = 1;
            await LoadDataAsync();
        }

        // ============================================================
        //  7. PHÂN TRANG
        // ============================================================
        private async void BtnFirst_Click(object sender, EventArgs e) { _currentPage = 1;          await LoadDataAsync(); }
        private async void BtnPrev_Click (object sender, EventArgs e) { if (_currentPage > 1)          { _currentPage--; await LoadDataAsync(); } }
        private async void BtnNext_Click (object sender, EventArgs e) { if (_currentPage < TotalPages) { _currentPage++; await LoadDataAsync(); } }
        private async void BtnLast_Click (object sender, EventArgs e) { _currentPage = TotalPages; await LoadDataAsync(); }

        private async void CmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cmbPageSize.SelectedItem?.ToString(), out int ps))
            { _pageSize = ps; _currentPage = 1; await LoadDataAsync(); }
        }

        // ============================================================
        //  8. LÀM MỚI (Reset tất cả bộ lọc)
        // ============================================================
        private async void BtnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text             = "";
            cmbLoaiViPham.SelectedIndex = 0;
            chkLocNgay.Checked          = false;
            selectedLogID               = -1;
            txtGhiChu.Text              = "";
            chkXacNhan.Checked          = false;
            lblInfoSV.Text              = "Chọn một dòng vi phạm để xem chi tiết và chỉnh sửa.";
            _keyword = ""; _vtID = -1; _useDate = false; _currentPage = 1;
            await LoadDataAsync();
        }

        // ============================================================
        //  8. THÊM THỦ CÔNG
        // ============================================================
        private async void BtnThem_Click(object sender, EventArgs e)
        {
            // Mở dialog Thêm mới
            using (frmThemViPham dlg = new frmThemViPham())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    await LoadDataAsync();   // Tải lại sau khi thêm
                    MessageBox.Show("✅ Đã thêm nhật ký vi phạm thành công!", "Thành công",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // ============================================================
        //  9. SỬA (Cập nhật ProctorNote + IsConfirmed)
        // ============================================================
        private async void BtnSua_Click(object sender, EventArgs e)
        {
            // --- VALIDATION: Phải chọn dòng trước ---
            if (selectedLogID < 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn một dòng vi phạm trên bảng trước khi lưu!",
                                "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE ViolationLogs
                        SET ProctorNote = @note,
                            IsConfirmed = @confirmed
                        WHERE LogID = @logID
                    ";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@note",      txtGhiChu.Text.Trim());
                        cmd.Parameters.AddWithValue("@confirmed", chkXacNhan.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@logID",     selectedLogID);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Tải lại lưới sau khi sửa
                await LoadDataAsync();

                MessageBox.Show("✅ Đã cập nhật ghi chú và trạng thái xác nhận thành công!",
                                "Cập nhật thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        //  10. XÓA
        // ============================================================
        private async void BtnXoa_Click(object sender, EventArgs e)
        {
            // --- VALIDATION: Phải chọn dòng trước ---
            if (selectedLogID < 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn một dòng vi phạm cần xóa trên bảng!",
                                "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- XÁC NHẬN TRƯỚC KHI XÓA ---
            DialogResult confirm = MessageBox.Show(
                "🗑️ Bạn có chắc chắn muốn xóa nhật ký vi phạm này không?\n\n" +
                "(Hành động này không thể hoàn tác!)",
                "Xác nhận Xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(CONN_STR))
                {
                    conn.Open();
                    string sql = "DELETE FROM ViolationLogs WHERE LogID = @logID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@logID", selectedLogID);
                        cmd.ExecuteNonQuery();
                    }
                }

                selectedLogID      = -1;
                txtGhiChu.Text     = "";
                chkXacNhan.Checked = false;
                lblInfoSV.Text     = "Chọn một dòng vi phạm để xem chi tiết và chỉnh sửa.";

                await LoadDataAsync();

                MessageBox.Show("✅ Đã xóa nhật ký vi phạm thành công!",
                                "Xóa thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        //  11. XUẤT EXCEL (.xlsx) – dùng ClosedXML
        // ============================================================
        private void BtnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dtMain == null || dtMain.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter   = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"NhatKyViPham_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                Title    = "Chọn nơi lưu file Excel"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (var wb  = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Nhật Ký Vi Phạm");

                    // ---- Tiêu đề ----
                    ws.Cell(1, 1).Value = "NHẬT KÝ VI PHẠM - HỆ THỐNG GIÁM SÁT PHÒNG THI";
                    var titleRange = ws.Range(1, 1, 1, 8);
                    titleRange.Merge();
                    titleRange.Style
                        .Font.SetBold(true)
                        .Font.SetFontSize(14)
                        .Font.SetFontColor(XLColor.White)
                        .Fill.SetBackgroundColor(XLColor.FromArgb(30, 50, 120))
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ws.Cell(2, 1).Value = $"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    ws.Range(2, 1, 2, 8).Merge().Style
                        .Font.SetItalic(true)
                        .Font.SetFontColor(XLColor.Gray)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    // ---- Header cột ----
                    string[] headers = { "STT", "Mã SV", "Họ và Tên", "Lớp",
                                         "Loại Vi Phạm", "Thời Gian", "Trạng Thái", "Ghi Chú Giám Thị" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        var cell = ws.Cell(4, c + 1);
                        cell.Value = headers[c];
                        cell.Style
                            .Font.SetBold(true)
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(60, 100, 200))
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    }

                    // ---- Dữ liệu ----
                    string[] cols = { "Mã SV", "Họ và Tên", "Lớp",
                                      "Loại Vi Phạm", "Thời Gian", "Trạng Thái", "Ghi Chú Giám Thị" };
                    for (int r = 0; r < dtMain.Rows.Count; r++)
                    {
                        DataRow dr  = dtMain.Rows[r];
                        int     row = r + 5;

                        ws.Cell(row, 1).Value = r + 1; // STT

                        for (int c = 0; c < cols.Length; c++)
                        {
                            var val = dr[cols[c]]?.ToString() ?? "";
                            ws.Cell(row, c + 2).Value = val;
                        }

                        // Tô màu xen kẽ
                        var rowRange = ws.Range(row, 1, row, headers.Length);
                        rowRange.Style
                            .Fill.SetBackgroundColor(r % 2 == 0
                                ? XLColor.FromArgb(230, 240, 255)
                                : XLColor.White)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    }

                    // ---- Tự căn chỉnh độ rộng cột ----
                    ws.Columns().AdjustToContents();
                    ws.Column(3).Width = 25; // Họ và Tên
                    ws.Column(8).Width = 35; // Ghi chú

                    // ---- Đóng băng hàng tiêu đề ----
                    ws.SheetView.FreezeRows(4);

                    wb.SaveAs(sfd.FileName);
                }

                MessageBox.Show($"✅ Xuất Excel thành công!\n📂 File lưu tại:\n{sfd.FileName}",
                                "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Hỏi mở file ngay
                DialogResult openNow = MessageBox.Show(
                    "Bạn có muốn mở file Excel vừa xuất ngay bây giờ không?",
                    "Mở File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (openNow == DialogResult.Yes)
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName        = sfd.FileName,
                        UseShellExecute = true
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ============================================================
        //  XEM VIDEO TRÊN GOOGLE DRIVE
        // ============================================================
        private void BtnXemDrive_Click(object sender, EventArgs e)
        {
            if (dgvLog.CurrentRow == null)
            {
                MessageBox.Show("⚠️ Vui lòng chọn một dòng vi phạm trước!",
                    "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRowView drv = (DataRowView)dgvLog.CurrentRow.DataBoundItem;
            string snapshotPath = drv["Ảnh Vi Phạm"]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(snapshotPath))
            {
                MessageBox.Show("⚠️ Vi phạm này chưa có video đính kèm.\n\n" +
                    "Video chỉ được ghi khi AI phát hiện vi phạm lúc camera đang bật.",
                    "Không có video", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (snapshotPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // Là link Drive → mở trình duyệt
                GoogleDriveHelper.OpenInBrowser(snapshotPath);
            }
            else if (System.IO.File.Exists(snapshotPath))
            {
                // Là đường dẫn local (chưa upload hoặc upload lỗi)
                DialogResult ans = MessageBox.Show(
                    "File video này đang được lưu trên máy tính (chưa upload Drive).\n\n" +
                    "Đường dẫn: " + snapshotPath + "\n\n" +
                    "Bạn muốn mở file video ngay bây giờ không?",
                    "Video cục bộ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ans == DialogResult.Yes)
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    { FileName = snapshotPath, UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("⚠️ Không tìm thấy file video.\n\nPath: " + snapshotPath,
                    "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
