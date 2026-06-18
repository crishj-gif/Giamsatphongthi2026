namespace GiamSatPhongThi
{
    partial class frmNhatKyViPham
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlSearch        = new System.Windows.Forms.GroupBox();
            this.lblSearch        = new System.Windows.Forms.Label();
            this.txtTimKiem       = new System.Windows.Forms.TextBox();
            this.btnTimKiem       = new System.Windows.Forms.Button();
            this.lblFilter        = new System.Windows.Forms.Label();
            this.cmbLoaiViPham    = new System.Windows.Forms.ComboBox();
            this.btnLamMoi        = new System.Windows.Forms.Button();
            this.lblTongSo        = new System.Windows.Forms.Label();
            // --- Date filter controls ---
            this.pnlDateFilter    = new System.Windows.Forms.GroupBox();
            this.lblTuNgay        = new System.Windows.Forms.Label();
            this.dtpTuNgay        = new System.Windows.Forms.DateTimePicker();
            this.lblDenNgay       = new System.Windows.Forms.Label();
            this.dtpDenNgay       = new System.Windows.Forms.DateTimePicker();
            this.lblNamHoc        = new System.Windows.Forms.Label();
            this.cmbNam           = new System.Windows.Forms.ComboBox();
            this.btnLocNgay       = new System.Windows.Forms.Button();
            this.chkLocNgay       = new System.Windows.Forms.CheckBox();
            // --- Pagination controls ---
            this.pnlPage          = new System.Windows.Forms.Panel();
            this.btnFirst         = new System.Windows.Forms.Button();
            this.btnPrev          = new System.Windows.Forms.Button();
            this.lblPage          = new System.Windows.Forms.Label();
            this.btnNext          = new System.Windows.Forms.Button();
            this.btnLast          = new System.Windows.Forms.Button();
            this.lblPageSize      = new System.Windows.Forms.Label();
            this.cmbPageSize      = new System.Windows.Forms.ComboBox();
            this.lblPageInfo      = new System.Windows.Forms.Label();
            this.dgvLog           = new System.Windows.Forms.DataGridView();
            this.pnlDetail        = new System.Windows.Forms.GroupBox();
            this.lblInfoSV        = new System.Windows.Forms.Label();
            this.lblGhiChu        = new System.Windows.Forms.Label();
            this.txtGhiChu        = new System.Windows.Forms.TextBox();
            this.chkXacNhan       = new System.Windows.Forms.CheckBox();
            this.pnlButtons       = new System.Windows.Forms.GroupBox();
            this.grpGrid = new System.Windows.Forms.GroupBox();
            this.btnThem          = new System.Windows.Forms.Button();
            this.btnSua           = new System.Windows.Forms.Button();
            this.btnXoa           = new System.Windows.Forms.Button();
            this.btnXuatExcel = new System.Windows.Forms.Button();
            this.btnXemDrive  = new System.Windows.Forms.Button();
            this.pnlSearch.SuspendLayout();
            this.pnlDateFilter.SuspendLayout();
            this.pnlPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
            this.pnlDetail.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpGrid.SuspendLayout();
            this.SuspendLayout();

            // ============================================================
            // ============================================================



            // ============================================================
            // pnlSearch  (thanh tìm kiếm & lọc)
            // ============================================================
            this.pnlSearch.Controls.Add(this.lblTongSo);
            this.pnlSearch.Controls.Add(this.btnLamMoi);
            this.pnlSearch.Controls.Add(this.cmbLoaiViPham);
            this.pnlSearch.Controls.Add(this.lblFilter);
            this.pnlSearch.Controls.Add(this.btnTimKiem);
            this.pnlSearch.Controls.Add(this.txtTimKiem);
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(12, 12);
            this.pnlSearch.Size = new System.Drawing.Size(1176, 75);
            this.pnlSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSearch.Text = "Tìm kiếm & Bộ lọc";
            this.pnlSearch.Name      = "pnlSearch";
            this.pnlSearch.Padding   = new System.Windows.Forms.Padding(15, 0, 15, 0);


            // lblSearch
            this.lblSearch.AutoSize  = true;
            this.lblSearch.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblSearch.Location  = new System.Drawing.Point(20, 34);
            this.lblSearch.Name      = "lblSearch";
            this.lblSearch.Text      = "Họ tên / Mã SV:";

            // txtTimKiem
            this.txtTimKiem.Font        = new System.Drawing.Font("Segoe UI", 10f);
            this.txtTimKiem.Location    = new System.Drawing.Point(128, 30);
            this.txtTimKiem.Name        = "txtTimKiem";
            this.txtTimKiem.Size        = new System.Drawing.Size(220, 28);
            this.txtTimKiem.KeyDown    += new System.Windows.Forms.KeyEventHandler(this.TxtTimKiem_KeyDown);

            // btnTimKiem
            this.btnTimKiem.Location = new System.Drawing.Point(356, 30);
            this.btnTimKiem.Name     = "btnTimKiem";
            this.btnTimKiem.Size     = new System.Drawing.Size(100, 28);
            this.btnTimKiem.TabIndex = 1;
            this.btnTimKiem.Text     = "🔍 Tìm kiếm";
            this.btnTimKiem.Click   += new System.EventHandler(this.BtnTimKiem_Click);

            // lblFilter
            this.lblFilter.AutoSize  = true;
            this.lblFilter.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblFilter.Location  = new System.Drawing.Point(475, 34);
            this.lblFilter.Name      = "lblFilter";
            this.lblFilter.Text      = "Lọc loại vi phạm:";

            // cmbLoaiViPham
            this.cmbLoaiViPham.DropDownStyle      = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLoaiViPham.Font               = new System.Drawing.Font("Segoe UI", 9.5f);
            this.cmbLoaiViPham.Location           = new System.Drawing.Point(592, 30);
            this.cmbLoaiViPham.Name               = "cmbLoaiViPham";
            this.cmbLoaiViPham.Size               = new System.Drawing.Size(240, 28);
            this.cmbLoaiViPham.SelectedIndexChanged += new System.EventHandler(this.CmbLoaiViPham_SelectedIndexChanged);

            // btnLamMoi
            this.btnLamMoi.Location = new System.Drawing.Point(842, 30);
            this.btnLamMoi.Name     = "btnLamMoi";
            this.btnLamMoi.Size     = new System.Drawing.Size(100, 28);
            this.btnLamMoi.TabIndex = 2;
            this.btnLamMoi.Text     = "🔄 Làm mới";
            this.btnLamMoi.Click   += new System.EventHandler(this.BtnLamMoi_Click);

            // lblTongSo
            this.lblTongSo.AutoSize  = true;
            this.lblTongSo.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lblTongSo.Location  = new System.Drawing.Point(960, 38);
            this.lblTongSo.Name      = "lblTongSo";
            this.lblTongSo.Text      = "Tổng số: 0 vi phạm";

            // ============================================================
            // pnlDateFilter  (lọc theo thời điểm / năm)
            // ============================================================
            this.pnlDateFilter.Controls.Add(this.chkLocNgay);
            this.pnlDateFilter.Controls.Add(this.lblTuNgay);
            this.pnlDateFilter.Controls.Add(this.dtpTuNgay);
            this.pnlDateFilter.Controls.Add(this.lblDenNgay);
            this.pnlDateFilter.Controls.Add(this.dtpDenNgay);
            this.pnlDateFilter.Controls.Add(this.lblNamHoc);
            this.pnlDateFilter.Controls.Add(this.cmbNam);
            this.pnlDateFilter.Controls.Add(this.btnLocNgay);
            this.pnlDateFilter.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlDateFilter.Location = new System.Drawing.Point(12, 93);
            this.pnlDateFilter.Size = new System.Drawing.Size(1176, 65);
            this.pnlDateFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDateFilter.Text = "Lọc theo ngày";
            this.pnlDateFilter.Name      = "pnlDateFilter";
            this.pnlDateFilter.Padding   = new System.Windows.Forms.Padding(15, 0, 15, 0);

            this.chkLocNgay.AutoSize  = true;
            this.chkLocNgay.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.chkLocNgay.Location  = new System.Drawing.Point(20, 25);
            this.chkLocNgay.Name      = "chkLocNgay";
            this.chkLocNgay.Text      = "📅 Lọc theo ngày";
            this.chkLocNgay.CheckedChanged += new System.EventHandler(this.ChkLocNgay_CheckedChanged);

            this.lblTuNgay.AutoSize  = true;
            this.lblTuNgay.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblTuNgay.Location  = new System.Drawing.Point(175, 26);
            this.lblTuNgay.Name      = "lblTuNgay";
            this.lblTuNgay.Text      = "Từ ngày:";

            this.dtpTuNgay.Format    = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTuNgay.Location  = new System.Drawing.Point(245, 22);
            this.dtpTuNgay.Name      = "dtpTuNgay";
            this.dtpTuNgay.Size      = new System.Drawing.Size(120, 26);
            this.dtpTuNgay.Enabled   = false;
            this.dtpTuNgay.Value     = new System.DateTime(DateTime.Now.Year, 1, 1);

            this.lblDenNgay.AutoSize  = true;
            this.lblDenNgay.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblDenNgay.Location  = new System.Drawing.Point(378, 26);
            this.lblDenNgay.Name      = "lblDenNgay";
            this.lblDenNgay.Text      = "Đến ngày:";

            this.dtpDenNgay.Format   = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDenNgay.Location = new System.Drawing.Point(453, 22);
            this.dtpDenNgay.Name     = "dtpDenNgay";
            this.dtpDenNgay.Size     = new System.Drawing.Size(120, 26);
            this.dtpDenNgay.Enabled  = false;
            this.dtpDenNgay.Value    = System.DateTime.Now;

            this.lblNamHoc.AutoSize  = true;
            this.lblNamHoc.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblNamHoc.Location  = new System.Drawing.Point(590, 26);
            this.lblNamHoc.Name      = "lblNamHoc";
            this.lblNamHoc.Text      = "Năm:";

            this.cmbNam.DropDownStyle      = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNam.Font               = new System.Drawing.Font("Segoe UI", 9.5f);
            this.cmbNam.Location           = new System.Drawing.Point(628, 22);
            this.cmbNam.Name               = "cmbNam";
            this.cmbNam.Size               = new System.Drawing.Size(100, 26);
            this.cmbNam.Enabled            = false;
            this.cmbNam.SelectedIndexChanged += new System.EventHandler(this.CmbNam_SelectedIndexChanged);

            this.btnLocNgay.Location = new System.Drawing.Point(742, 21);
            this.btnLocNgay.Name     = "btnLocNgay";
            this.btnLocNgay.Size     = new System.Drawing.Size(110, 28);
            this.btnLocNgay.Text     = "🔎 Áp dụng lọc";
            this.btnLocNgay.Enabled  = false;
            this.btnLocNgay.Click   += new System.EventHandler(this.BtnLocNgay_Click);

            // ============================================================
            // pnlPage  (thanh phân trang)
            // ============================================================
            this.pnlPage.Controls.Add(this.btnFirst);
            this.pnlPage.Controls.Add(this.btnPrev);
            this.pnlPage.Controls.Add(this.lblPage);
            this.pnlPage.Controls.Add(this.btnNext);
            this.pnlPage.Controls.Add(this.btnLast);
            this.pnlPage.Controls.Add(this.lblPageSize);
            this.pnlPage.Controls.Add(this.cmbPageSize);
            this.pnlPage.Controls.Add(this.lblPageInfo);
            this.pnlPage.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPage.Height    = 42;
            this.pnlPage.Name      = "pnlPage";

            this.btnFirst.Location = new System.Drawing.Point(15, 8);
            this.btnFirst.Name     = "btnFirst";
            this.btnFirst.Size     = new System.Drawing.Size(36, 28);
            this.btnFirst.Text     = "|◀";
            this.btnFirst.Click   += new System.EventHandler(this.BtnFirst_Click);

            this.btnPrev.Location = new System.Drawing.Point(55, 8);
            this.btnPrev.Name     = "btnPrev";
            this.btnPrev.Size     = new System.Drawing.Size(36, 28);
            this.btnPrev.Text     = "◀";
            this.btnPrev.Click   += new System.EventHandler(this.BtnPrev_Click);

            this.lblPage.AutoSize  = false;
            this.lblPage.Font      = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.lblPage.Location  = new System.Drawing.Point(95, 12);
            this.lblPage.Name      = "lblPage";
            this.lblPage.Size      = new System.Drawing.Size(130, 20);
            this.lblPage.Text      = "Trang 1 / 1";
            this.lblPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnNext.Location = new System.Drawing.Point(229, 8);
            this.btnNext.Name     = "btnNext";
            this.btnNext.Size     = new System.Drawing.Size(36, 28);
            this.btnNext.Text     = "▶";
            this.btnNext.Click   += new System.EventHandler(this.BtnNext_Click);

            this.btnLast.Location = new System.Drawing.Point(269, 8);
            this.btnLast.Name     = "btnLast";
            this.btnLast.Size     = new System.Drawing.Size(36, 28);
            this.btnLast.Text     = "▶|";
            this.btnLast.Click   += new System.EventHandler(this.BtnLast_Click);

            this.lblPageSize.AutoSize  = true;
            this.lblPageSize.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblPageSize.Location  = new System.Drawing.Point(320, 13);
            this.lblPageSize.Name      = "lblPageSize";
            this.lblPageSize.Text      = "Dòng/trang:";

            this.cmbPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPageSize.Font          = new System.Drawing.Font("Segoe UI", 9.5f);
            this.cmbPageSize.Location      = new System.Drawing.Point(410, 8);
            this.cmbPageSize.Name          = "cmbPageSize";
            this.cmbPageSize.Size          = new System.Drawing.Size(70, 26);
            this.cmbPageSize.Items.AddRange(new object[] { "10", "20", "50", "100" });
            this.cmbPageSize.SelectedIndex = 0;
            this.cmbPageSize.SelectedIndexChanged += new System.EventHandler(this.CmbPageSize_SelectedIndexChanged);

            this.lblPageInfo.AutoSize  = false;
            this.lblPageInfo.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblPageInfo.Location  = new System.Drawing.Point(495, 13);
            this.lblPageInfo.Name      = "lblPageInfo";
            this.lblPageInfo.Size      = new System.Drawing.Size(300, 20);
            this.lblPageInfo.Text      = "Hiển thị 0 - 0 trong 0 bản ghi";

            // ============================================================
            // dgvLog  (DataGridView chính)
            // ============================================================
            this.dgvLog.AllowUserToAddRows    = false;
            this.dgvLog.AllowUserToDeleteRows = false;
            this.dgvLog.ReadOnly              = true;
            this.dgvLog.Dock                  = System.Windows.Forms.DockStyle.Fill;
            this.dgvLog.Name                  = "dgvLog";
            this.dgvLog.SelectionMode         = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLog.MultiSelect           = false;
            this.dgvLog.SelectionChanged     += new System.EventHandler(this.DgvLog_SelectionChanged);

            // ============================================================
            // pnlDetail  (panel chỉnh sửa phía dưới)
            // ============================================================
            this.pnlDetail.Controls.Add(this.chkXacNhan);
            this.pnlDetail.Controls.Add(this.txtGhiChu);
            this.pnlDetail.Controls.Add(this.lblGhiChu);
            this.pnlDetail.Controls.Add(this.lblInfoSV);
            this.pnlDetail.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDetail.Location = new System.Drawing.Point(12, 510);
            this.pnlDetail.Size = new System.Drawing.Size(1176, 120);
            this.pnlDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetail.Text = "Thông tin chi tiết";
            this.pnlDetail.Name      = "pnlDetail";
            this.pnlDetail.Padding   = new System.Windows.Forms.Padding(15, 8, 15, 8);


            this.lblInfoSV.AutoSize  = false;
            this.lblInfoSV.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Italic);
            this.lblInfoSV.Location  = new System.Drawing.Point(20, 28);
            this.lblInfoSV.Name      = "lblInfoSV";
            this.lblInfoSV.Size      = new System.Drawing.Size(950, 22);
            this.lblInfoSV.Text      = "Chọn một dòng vi phạm để xem chi tiết và chỉnh sửa.";

            this.lblGhiChu.AutoSize  = true;
            this.lblGhiChu.Font      = new System.Drawing.Font("Segoe UI", 9f);
            this.lblGhiChu.Location  = new System.Drawing.Point(20, 60);
            this.lblGhiChu.Name      = "lblGhiChu";
            this.lblGhiChu.Text      = "Ghi chú giám thị:";

            this.txtGhiChu.Font        = new System.Drawing.Font("Segoe UI", 10f);
            this.txtGhiChu.Location    = new System.Drawing.Point(145, 56);
            this.txtGhiChu.Name        = "txtGhiChu";
            this.txtGhiChu.Size        = new System.Drawing.Size(450, 28);

            this.chkXacNhan.AutoSize  = true;
            this.chkXacNhan.Font      = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.chkXacNhan.Location  = new System.Drawing.Point(615, 58);
            this.chkXacNhan.Name      = "chkXacNhan";
            this.chkXacNhan.Text      = "✔  Xác nhận vi phạm (IsConfirmed)";

            // ============================================================
            // pnlButtons  (hàng nút hành động)
            // ============================================================
            this.pnlButtons.Controls.Add(this.btnThem);
            this.pnlButtons.Controls.Add(this.btnSua);
            this.pnlButtons.Controls.Add(this.btnXoa);
            this.pnlButtons.Controls.Add(this.btnXuatExcel);
            this.pnlButtons.Controls.Add(this.btnXemDrive);
            this.pnlButtons.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(12, 636);
            this.pnlButtons.Size = new System.Drawing.Size(1176, 75);
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlButtons.Text = "Thao tác";
            this.pnlButtons.Name      = "pnlButtons";
            this.pnlButtons.Padding   = new System.Windows.Forms.Padding(15, 8, 15, 8);

            this.btnThem.Location = new System.Drawing.Point(20, 25);
            this.btnThem.Name     = "btnThem";
            this.btnThem.Size     = new System.Drawing.Size(150, 36);
            this.btnThem.TabIndex = 10;
            this.btnThem.Text     = "➕  Thêm thủ công";
            this.btnThem.Click   += new System.EventHandler(this.BtnThem_Click);

            this.btnSua.Location = new System.Drawing.Point(185, 25);
            this.btnSua.Name     = "btnSua";
            this.btnSua.Size     = new System.Drawing.Size(160, 36);
            this.btnSua.TabIndex = 11;
            this.btnSua.Text     = "💾  Lưu chỉnh sửa";
            this.btnSua.Click   += new System.EventHandler(this.BtnSua_Click);

            this.btnXoa.Location = new System.Drawing.Point(360, 25);
            this.btnXoa.Name     = "btnXoa";
            this.btnXoa.Size     = new System.Drawing.Size(150, 36);
            this.btnXoa.TabIndex = 12;
            this.btnXoa.Text     = "🗑️  Xóa vi phạm";
            this.btnXoa.Click   += new System.EventHandler(this.BtnXoa_Click);

            this.btnXuatExcel.Location = new System.Drawing.Point(530, 25);
            this.btnXuatExcel.Name     = "btnXuatExcel";
            this.btnXuatExcel.Size     = new System.Drawing.Size(180, 36);
            this.btnXuatExcel.TabIndex = 13;
            this.btnXuatExcel.Text     = "📊  Xuất Excel (.xlsx)";
            this.btnXuatExcel.Click   += new System.EventHandler(this.BtnXuatExcel_Click);

            this.btnXemDrive.Location = new System.Drawing.Point(725, 25);
            this.btnXemDrive.Name     = "btnXemDrive";
            this.btnXemDrive.Size     = new System.Drawing.Size(200, 36);
            this.btnXemDrive.TabIndex = 14;
            this.btnXemDrive.Text     = "▶  Xem Video trên Drive";
            this.btnXemDrive.Click   += new System.EventHandler(this.BtnXemDrive_Click);

            // ============================================================
            // Form chính
            // ============================================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.grpGrid);
            this.Controls.Add(this.pnlDetail);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlDateFilter);
            this.Controls.Add(this.pnlSearch);
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize     = new System.Drawing.Size(1100, 720);
            this.Name            = "frmNhatKyViPham";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = "Quản Lý Nhật Ký Vi Phạm";
            this.WindowState     = System.Windows.Forms.FormWindowState.Maximized;

            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlDateFilter.ResumeLayout(false);
            this.pnlDateFilter.PerformLayout();
            this.pnlPage.ResumeLayout(false);
            this.pnlPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            this.pnlDetail.ResumeLayout(false);
            this.pnlDetail.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.grpGrid.ResumeLayout(false);
            // 
            // grpGrid
            // 
            this.grpGrid.Controls.Add(this.dgvLog);
            this.grpGrid.Controls.Add(this.pnlPage);
            this.grpGrid.Location = new System.Drawing.Point(12, 164);
            this.grpGrid.Name = "grpGrid";
            this.grpGrid.Size = new System.Drawing.Size(1176, 340);
            this.grpGrid.TabIndex = 99;
            this.grpGrid.TabStop = false;
            this.grpGrid.Text = "Danh sách nhật ký vi phạm";
            this.grpGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.ResumeLayout(false);
        }

        #endregion

        // Controls
        private System.Windows.Forms.GroupBox         pnlSearch;
        private System.Windows.Forms.Label         lblSearch;
        private System.Windows.Forms.TextBox       txtTimKiem;
        private System.Windows.Forms.Button        btnTimKiem;
        private System.Windows.Forms.Label         lblFilter;
        private System.Windows.Forms.ComboBox      cmbLoaiViPham;
        private System.Windows.Forms.Button        btnLamMoi;
        private System.Windows.Forms.Label         lblTongSo;
        // Date filter
        private System.Windows.Forms.GroupBox         pnlDateFilter;
        private System.Windows.Forms.CheckBox      chkLocNgay;
        private System.Windows.Forms.Label         lblTuNgay;
        private System.Windows.Forms.DateTimePicker dtpTuNgay;
        private System.Windows.Forms.Label         lblDenNgay;
        private System.Windows.Forms.DateTimePicker dtpDenNgay;
        private System.Windows.Forms.Label         lblNamHoc;
        private System.Windows.Forms.ComboBox      cmbNam;
        private System.Windows.Forms.Button        btnLocNgay;
        // Pagination
        private System.Windows.Forms.Panel         pnlPage;
        private System.Windows.Forms.Button        btnFirst;
        private System.Windows.Forms.Button        btnPrev;
        private System.Windows.Forms.Label         lblPage;
        private System.Windows.Forms.Button        btnNext;
        private System.Windows.Forms.Button        btnLast;
        private System.Windows.Forms.Label         lblPageSize;
        private System.Windows.Forms.ComboBox      cmbPageSize;
        private System.Windows.Forms.Label         lblPageInfo;
        private System.Windows.Forms.DataGridView  dgvLog;
        private System.Windows.Forms.GroupBox         pnlDetail;
        private System.Windows.Forms.Label         lblInfoSV;
        private System.Windows.Forms.Label         lblGhiChu;
        private System.Windows.Forms.TextBox       txtGhiChu;
        private System.Windows.Forms.CheckBox      chkXacNhan;
        private System.Windows.Forms.GroupBox         pnlButtons;
        private System.Windows.Forms.GroupBox grpGrid;
        private System.Windows.Forms.Button        btnThem;
        private System.Windows.Forms.Button        btnSua;
        private System.Windows.Forms.Button        btnXoa;
        private System.Windows.Forms.Button        btnXuatExcel;
        private System.Windows.Forms.Button        btnXemDrive;
    }
}
