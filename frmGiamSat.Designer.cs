namespace GiamSatPhongThi
{
    partial class frmGiamSat
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tlpMain = new TableLayoutPanel();
            grpCamera = new GroupBox();
            tlpCameraContent = new TableLayoutPanel();
            picCamera = new PictureBox();
            flpButtons = new FlowLayoutPanel();
            btnMoCamera = new Button();
            btnDiemDanh = new Button();
            btnGiamSatAI = new Button();
            btnKetThuc = new Button();
            btnImportExcel = new Button();
            btnXuatExcel = new Button();
            btnImportImages = new Button();
            btnIPCamera = new Button();
            btnPlayVideo = new Button();
            btnSaveVideo = new Button();
            btnQuanLyViPham = new Button();
            tlpRight = new TableLayoutPanel();
            grpDanhSach = new GroupBox();
            dgvDanhSach = new DataGridView();
            grpViPham = new GroupBox();
            dgvViPham = new DataGridView();
            mySqlDataAdapter1 = new MySql.Data.MySqlClient.MySqlDataAdapter();
            tlpMain.SuspendLayout();
            grpCamera.SuspendLayout();
            tlpCameraContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCamera).BeginInit();
            flpButtons.SuspendLayout();
            tlpRight.SuspendLayout();
            grpDanhSach.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDanhSach).BeginInit();
            grpViPham.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvViPham).BeginInit();
            SuspendLayout();
            // 
            // tlpMain
            // 
            tlpMain.ColumnCount = 2;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tlpMain.Controls.Add(grpCamera, 0, 0);
            tlpMain.Controls.Add(tlpRight, 1, 0);
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Location = new Point(0, 0);
            tlpMain.Name = "tlpMain";
            tlpMain.RowCount = 1;
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.Size = new Size(1488, 739);
            tlpMain.TabIndex = 0;
            // 
            // grpCamera
            // 
            grpCamera.Controls.Add(tlpCameraContent);
            grpCamera.Dock = DockStyle.Fill;
            grpCamera.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            grpCamera.Location = new Point(3, 3);
            grpCamera.Name = "grpCamera";
            grpCamera.Size = new Size(1035, 733);
            grpCamera.TabIndex = 0;
            grpCamera.TabStop = false;
            grpCamera.Text = "Camera Giám sát Trực tiếp";
            // 
            // tlpCameraContent
            // 
            tlpCameraContent.ColumnCount = 1;
            tlpCameraContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpCameraContent.Controls.Add(picCamera, 0, 0);
            tlpCameraContent.Controls.Add(flpButtons, 0, 1);
            tlpCameraContent.Dock = DockStyle.Fill;
            tlpCameraContent.Location = new Point(3, 25);
            tlpCameraContent.Name = "tlpCameraContent";
            tlpCameraContent.RowCount = 2;
            tlpCameraContent.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            tlpCameraContent.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tlpCameraContent.Size = new Size(1029, 705);
            tlpCameraContent.TabIndex = 0;
            // 
            // picCamera
            // 
            picCamera.BackColor = Color.Black;
            picCamera.Dock = DockStyle.Fill;
            picCamera.Location = new Point(3, 3);
            picCamera.Name = "picCamera";
            picCamera.Size = new Size(1023, 593);
            picCamera.SizeMode = PictureBoxSizeMode.Zoom;
            picCamera.TabIndex = 0;
            picCamera.TabStop = false;
            // 
            // flpButtons
            // 
            flpButtons.Controls.Add(btnMoCamera);
            flpButtons.Controls.Add(btnDiemDanh);
            flpButtons.Controls.Add(btnGiamSatAI);
            flpButtons.Controls.Add(btnKetThuc);
            flpButtons.Controls.Add(btnImportExcel);
            flpButtons.Controls.Add(btnXuatExcel);
            flpButtons.Controls.Add(btnImportImages);
            flpButtons.Controls.Add(btnIPCamera);
            flpButtons.Controls.Add(btnPlayVideo);
            flpButtons.Controls.Add(btnSaveVideo);
            flpButtons.Controls.Add(btnQuanLyViPham);
            flpButtons.Dock = DockStyle.Fill;
            flpButtons.Location = new Point(3, 602);
            flpButtons.Name = "flpButtons";
            flpButtons.Size = new Size(1023, 100);
            flpButtons.TabIndex = 1;
            // 
            // btnMoCamera
            // 
            btnMoCamera.BackColor = Color.MediumSeaGreen;
            btnMoCamera.FlatStyle = FlatStyle.Flat;
            btnMoCamera.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnMoCamera.ForeColor = Color.White;
            btnMoCamera.Location = new Point(10, 10);
            btnMoCamera.Margin = new Padding(10);
            btnMoCamera.Name = "btnMoCamera";
            btnMoCamera.Size = new Size(150, 50);
            btnMoCamera.TabIndex = 0;
            btnMoCamera.Text = "Bật Camera";
            btnMoCamera.UseVisualStyleBackColor = false;
            btnMoCamera.Click += btnMoCamera_Click_1;
            // 
            // btnDiemDanh
            // 
            btnDiemDanh.BackColor = Color.DodgerBlue;
            btnDiemDanh.FlatStyle = FlatStyle.Flat;
            btnDiemDanh.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnDiemDanh.ForeColor = Color.White;
            btnDiemDanh.Location = new Point(180, 10);
            btnDiemDanh.Margin = new Padding(10);
            btnDiemDanh.Name = "btnDiemDanh";
            btnDiemDanh.Size = new Size(150, 50);
            btnDiemDanh.TabIndex = 1;
            btnDiemDanh.Text = "Điểm danh";
            btnDiemDanh.UseVisualStyleBackColor = false;
            // 
            // btnGiamSatAI
            // 
            btnGiamSatAI.BackColor = Color.Orange;
            btnGiamSatAI.FlatStyle = FlatStyle.Flat;
            btnGiamSatAI.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGiamSatAI.ForeColor = Color.White;
            btnGiamSatAI.Location = new Point(350, 10);
            btnGiamSatAI.Margin = new Padding(10);
            btnGiamSatAI.Name = "btnGiamSatAI";
            btnGiamSatAI.Size = new Size(150, 50);
            btnGiamSatAI.TabIndex = 2;
            btnGiamSatAI.Text = "Giám sát AI";
            btnGiamSatAI.UseVisualStyleBackColor = false;
            // 
            // btnKetThuc
            // 
            btnKetThuc.BackColor = Color.Crimson;
            btnKetThuc.FlatStyle = FlatStyle.Flat;
            btnKetThuc.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnKetThuc.ForeColor = Color.White;
            btnKetThuc.Location = new Point(520, 10);
            btnKetThuc.Margin = new Padding(10);
            btnKetThuc.Name = "btnKetThuc";
            btnKetThuc.Size = new Size(150, 50);
            btnKetThuc.TabIndex = 3;
            btnKetThuc.Text = "Kết thúc";
            btnKetThuc.UseVisualStyleBackColor = false;
            btnKetThuc.Click += btnKetThuc_Click_1;
            // 
            // btnImportExcel
            // 
            btnImportExcel.BackColor = Color.Teal;
            btnImportExcel.FlatStyle = FlatStyle.Flat;
            btnImportExcel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnImportExcel.ForeColor = Color.White;
            btnImportExcel.Location = new Point(690, 10);
            btnImportExcel.Margin = new Padding(10);
            btnImportExcel.Name = "btnImportExcel";
            btnImportExcel.Size = new Size(150, 50);
            btnImportExcel.TabIndex = 4;
            btnImportExcel.Text = "Nhập Excel";
            btnImportExcel.UseVisualStyleBackColor = false;
            btnImportExcel.Click += btnImportExcel_Click;
            // 
            // btnXuatExcel
            // 
            btnXuatExcel.BackColor = Color.Indigo;
            btnXuatExcel.FlatStyle = FlatStyle.Flat;
            btnXuatExcel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnXuatExcel.ForeColor = Color.White;
            btnXuatExcel.Location = new Point(860, 10);
            btnXuatExcel.Margin = new Padding(10);
            btnXuatExcel.Name = "btnXuatExcel";
            btnXuatExcel.Size = new Size(150, 50);
            btnXuatExcel.TabIndex = 5;
            btnXuatExcel.Text = "Xuất Nhật Ký";
            btnXuatExcel.UseVisualStyleBackColor = false;
            btnXuatExcel.Click += btnXuatExcel_Click;
            // 
            // btnImportImages
            // 
            btnImportImages.BackColor = Color.DarkMagenta;
            btnImportImages.FlatStyle = FlatStyle.Flat;
            btnImportImages.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnImportImages.ForeColor = Color.White;
            btnImportImages.Location = new Point(10, 80);
            btnImportImages.Margin = new Padding(10);
            btnImportImages.Name = "btnImportImages";
            btnImportImages.Size = new Size(150, 50);
            btnImportImages.TabIndex = 6;
            btnImportImages.Text = "Import Ảnh(Cả kíp)";
            btnImportImages.UseVisualStyleBackColor = false;
            btnImportImages.Click += btnImportImages_Click;
            // 
            // btnIPCamera
            // 
            btnIPCamera.BackColor = Color.SteelBlue;
            btnIPCamera.FlatStyle = FlatStyle.Flat;
            btnIPCamera.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnIPCamera.ForeColor = Color.White;
            btnIPCamera.Location = new Point(180, 80);
            btnIPCamera.Margin = new Padding(10);
            btnIPCamera.Name = "btnIPCamera";
            btnIPCamera.Size = new Size(150, 50);
            btnIPCamera.TabIndex = 7;
            btnIPCamera.Text = "IP Cam Demo";
            btnIPCamera.UseVisualStyleBackColor = false;
            btnIPCamera.Click += btnIPCamera_Click;
            // 
            // btnPlayVideo
            // 
            btnPlayVideo.BackColor = Color.DeepPink;
            btnPlayVideo.FlatStyle = FlatStyle.Flat;
            btnPlayVideo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnPlayVideo.ForeColor = Color.White;
            btnPlayVideo.Location = new Point(350, 80);
            btnPlayVideo.Margin = new Padding(10);
            btnPlayVideo.Name = "btnPlayVideo";
            btnPlayVideo.Size = new Size(150, 50);
            btnPlayVideo.TabIndex = 8;
            btnPlayVideo.Text = "Xem Lại Video";
            btnPlayVideo.UseVisualStyleBackColor = false;
            btnPlayVideo.Click += btnPlayVideo_Click;
            // 
            // btnSaveVideo
            // 
            btnSaveVideo.BackColor = Color.DarkGoldenrod;
            btnSaveVideo.FlatStyle = FlatStyle.Flat;
            btnSaveVideo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSaveVideo.ForeColor = Color.White;
            btnSaveVideo.Location = new Point(520, 80);
            btnSaveVideo.Margin = new Padding(10);
            btnSaveVideo.Name = "btnSaveVideo";
            btnSaveVideo.Size = new Size(150, 50);
            btnSaveVideo.TabIndex = 9;
            btnSaveVideo.Text = "Lưu Video Xuất";
            btnSaveVideo.UseVisualStyleBackColor = false;
            btnSaveVideo.Click += btnSaveVideo_Click;
            // 
            // btnQuanLyViPham
            // 
            btnQuanLyViPham.BackColor = Color.FromArgb(90, 20, 160);
            btnQuanLyViPham.FlatStyle = FlatStyle.Flat;
            btnQuanLyViPham.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnQuanLyViPham.ForeColor = Color.White;
            btnQuanLyViPham.Location = new Point(690, 80);
            btnQuanLyViPham.Margin = new Padding(10);
            btnQuanLyViPham.Name = "btnQuanLyViPham";
            btnQuanLyViPham.Size = new Size(165, 50);
            btnQuanLyViPham.TabIndex = 10;
            btnQuanLyViPham.Text = "📋 Quản Lý Vi Phạm";
            btnQuanLyViPham.UseVisualStyleBackColor = false;
            btnQuanLyViPham.Click += btnQuanLyViPham_Click;
            // 
            // tlpRight
            // 
            tlpRight.ColumnCount = 1;
            tlpRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpRight.Controls.Add(grpDanhSach, 0, 0);
            tlpRight.Controls.Add(grpViPham, 0, 1);
            tlpRight.Dock = DockStyle.Fill;
            tlpRight.Location = new Point(1044, 3);
            tlpRight.Name = "tlpRight";
            tlpRight.RowCount = 2;
            tlpRight.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tlpRight.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tlpRight.Size = new Size(441, 733);
            tlpRight.TabIndex = 1;
            // 
            // grpDanhSach
            // 
            grpDanhSach.Controls.Add(dgvDanhSach);
            grpDanhSach.Dock = DockStyle.Fill;
            grpDanhSach.Font = new Font("Segoe UI", 10F);
            grpDanhSach.Location = new Point(3, 3);
            grpDanhSach.Name = "grpDanhSach";
            grpDanhSach.Size = new Size(435, 507);
            grpDanhSach.TabIndex = 0;
            grpDanhSach.TabStop = false;
            grpDanhSach.Text = "Danh sách thí sinh";
            // 
            // dgvDanhSach
            // 
            dgvDanhSach.AllowUserToAddRows = false;
            dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDanhSach.BackgroundColor = Color.White;
            dgvDanhSach.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDanhSach.Dock = DockStyle.Fill;
            dgvDanhSach.Location = new Point(3, 21);
            dgvDanhSach.Name = "dgvDanhSach";
            dgvDanhSach.ReadOnly = true;
            dgvDanhSach.Size = new Size(429, 483);
            dgvDanhSach.TabIndex = 0;
            // 
            // grpViPham
            // 
            grpViPham.Controls.Add(dgvViPham);
            grpViPham.Dock = DockStyle.Fill;
            grpViPham.Font = new Font("Segoe UI", 10F);
            grpViPham.Location = new Point(3, 516);
            grpViPham.Name = "grpViPham";
            grpViPham.Size = new Size(435, 214);
            grpViPham.TabIndex = 1;
            grpViPham.TabStop = false;
            grpViPham.Text = "Nhật ký vi phạm";
            // 
            // dgvViPham
            // 
            dgvViPham.AllowUserToAddRows = false;
            dgvViPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvViPham.BackgroundColor = Color.White;
            dgvViPham.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvViPham.Dock = DockStyle.Fill;
            dgvViPham.Location = new Point(3, 21);
            dgvViPham.Name = "dgvViPham";
            dgvViPham.ReadOnly = true;
            dgvViPham.Size = new Size(429, 190);
            dgvViPham.TabIndex = 0;
            // 
            // mySqlDataAdapter1
            // 
            mySqlDataAdapter1.DeleteCommand = null;
            mySqlDataAdapter1.InsertCommand = null;
            mySqlDataAdapter1.SelectCommand = null;
            mySqlDataAdapter1.UpdateCommand = null;
            // 
            // frmGiamSat
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1488, 739);
            Controls.Add(tlpMain);
            Name = "frmGiamSat";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Giám Sát Phòng Thi";
            WindowState = FormWindowState.Maximized;
            Load += frmGiamSat_Load_1;
            tlpMain.ResumeLayout(false);
            grpCamera.ResumeLayout(false);
            tlpCameraContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picCamera).EndInit();
            flpButtons.ResumeLayout(false);
            tlpRight.ResumeLayout(false);
            grpDanhSach.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvDanhSach).EndInit();
            grpViPham.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvViPham).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox grpCamera;
        private System.Windows.Forms.TableLayoutPanel tlpCameraContent;
        private System.Windows.Forms.PictureBox picCamera;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnMoCamera;
        private System.Windows.Forms.Button btnDiemDanh;
        private System.Windows.Forms.Button btnGiamSatAI;
        private System.Windows.Forms.Button btnKetThuc;
        private System.Windows.Forms.Button btnImportExcel;
        private System.Windows.Forms.Button btnXuatExcel;
        private System.Windows.Forms.Button btnImportImages;
        private System.Windows.Forms.Button btnIPCamera;
        private System.Windows.Forms.Button btnPlayVideo;
        private System.Windows.Forms.Button btnSaveVideo;
        private System.Windows.Forms.TableLayoutPanel tlpRight;
        private System.Windows.Forms.GroupBox grpDanhSach;
        private System.Windows.Forms.DataGridView dgvDanhSach;
        private System.Windows.Forms.GroupBox grpViPham;
        private System.Windows.Forms.DataGridView dgvViPham;
        private MySql.Data.MySqlClient.MySqlDataAdapter mySqlDataAdapter1;
        private System.Windows.Forms.Button btnQuanLyViPham;
    }
}