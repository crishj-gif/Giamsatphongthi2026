using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Data;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Dnn;
using ExcelDataReader;
using System.IO;
using System.Data.SqlClient;

namespace GiamSatPhongThi
{
    public class ViolationTracker
    {
        public DateTime ViolationStartTime { get; set; } = DateTime.MinValue;
        public DateTime LastSeenViolation { get; set; } = DateTime.MinValue;
        public bool IsViolating { get; set; } = false;
        public int Frequency { get; set; } = 0;
        public double CurrentScore { get; set; } = 0;
    }

    public partial class frmGiamSat : Form
    {
        private Dictionary<string, ViolationTracker> trackers = new Dictionary<string, ViolationTracker>();
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        // Các biến lưu vết vi phạm
        private DataTable dtViPham;
        private VideoWriter autoVideoWriter;
        private bool isRecordingAuto = false;
        private int autoRecordingFrames = 0;
        private DateTime lastPhoneWarning = DateTime.MinValue;
        private DateTime lastProfileWarning = DateTime.MinValue;
        private string lastRecognizedName = "Chưa xác định";
        private DateTime lastRecognizedTime = DateTime.MinValue;
        private DateTime lastNoFaceWarning = DateTime.MinValue;

        // Khai báo các biến toàn cục để chứa "bộ não" nhận diện mặt và điện thoại
        private CascadeClassifier phoneDetector;

        // OpenCV DNN Face Detector (YuNet) và Recognizer (SFace)
        private FaceDetectorYN faceDetector;
        private FaceRecognizerSF faceRecognizer;
        private bool isDnnLoaded = false;
        
        // --- CÁC BIẾN CHO TÍNH NĂNG ĐIỂM DANH GẮN MÁC ---
        private Dictionary<int, string> faceNames = new Dictionary<int, string>();
        private Dictionary<int, Mat> faceFeatures = new Dictionary<int, Mat>();
        private bool isRecognizerTrained = false;

        // Hàm bỏ dấu Tiếng Việt cho OpenCV hiển thị chữ (Khắc phục lỗi Hu???nh Gia H??n)
        private string RemoveAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            string normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (char c in normalizedString)
            {
                System.Globalization.UnicodeCategory unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).Replace("Đ", "D").Replace("đ", "d");
        }

        public frmGiamSat()
        {
            InitializeComponent();
            this.FormClosed += (s, e) => StopCamera();
            this.Load += frmGiamSat_Load;
        }

        private void frmGiamSat_Load(object sender, EventArgs e)
        {
            // --- CẬP NHẬT GIAO DIỆN BO TRÒN HIỆN ĐẠI ---
            UIHelper.BoTronControl(btnMoCamera, 15);
            UIHelper.BoTronControl(btnDiemDanh, 15);
            UIHelper.BoTronControl(btnGiamSatAI, 15);
            UIHelper.BoTronControl(btnKetThuc, 15);
            UIHelper.BoTronControl(btnImportExcel, 15);
            UIHelper.BoTronControl(btnXuatExcel, 15);
            UIHelper.BoTronControl(btnImportImages, 15);
            UIHelper.BoTronControl(btnIPCamera, 15);
            UIHelper.BoTronControl(btnPlayVideo, 15);
            UIHelper.BoTronControl(btnSaveVideo, 15);
            UIHelper.BoTronControl(btnQuanLyViPham, 15);
            UIHelper.BoTronControl(picCamera, 20);
            
            // Gắn sự kiện click cho nút điểm danh
            btnDiemDanh.Click += btnDiemDanh_Click;
            
            // BẤM ĐÚP ĐỂ THÊM ẢNH DATABASE
            dgvDanhSach.CellDoubleClick += DgvDanhSach_CellDoubleClick;

            // Tải bộ phát hiện điện thoại Haar Cascade
            try
            {
                string phonePath = Application.StartupPath + "\\haarcascade_phone.xml";
                if (System.IO.File.Exists(phonePath))
                {
                    phoneDetector = new CascadeClassifier(phonePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không tìm thấy file Haar Cascade XML: " + ex.Message, "Báo lỗi");
            }

            // Tải bộ phát hiện khuôn mặt OpenCV DNN ở luồng nền để tránh làm đơ giao diện
            this.Text = "⏳ ĐANG KHỞI TẠO CẤU HÌNH AI FACE DNN...";
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    EnsureDnnModelFiles();
                    string yunetPath = Path.Combine(Application.StartupPath, "face_detection_yunet_2023mar.onnx");
                    string sfacePath = Path.Combine(Application.StartupPath, "face_recognition_sface_2021dec.onnx");

                    if (File.Exists(yunetPath) && File.Exists(sfacePath))
                    {
                        faceDetector = new FaceDetectorYN(yunetPath, "", new Size(320, 320), 0.5f, 0.3f, 5000);
                        faceRecognizer = new FaceRecognizerSF(sfacePath, "", Emgu.CV.Dnn.Backend.Default, Emgu.CV.Dnn.Target.Cpu);
                        isDnnLoaded = true;
                    }
                }
                catch { }

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (isDnnLoaded)
                        {
                            this.Text = "✅ ĐÃ KÍCH HOẠT DNN FACE DETECTION THÀNH CÔNG - SẴN SÀNG!";
                        }
                        else
                        {
                            this.Text = "⚠️ LỖI NẠP DNN - ĐÃ CHUYỂN SANG DUAL HAAR CASCADE!";
                        }
                    }));
                }
            });

            // Khởi tạo bảng nhật ký vi phạm
            dtViPham = new DataTable();
            dtViPham.Columns.Add("Thời gian", typeof(string));
            dtViPham.Columns.Add("Đối tượng", typeof(string));
            dtViPham.Columns.Add("Hành vi", typeof(string));
            dgvViPham.DataSource = dtViPham;

            // Gọi hàm lấy dữ liệu lên DataGridView
            LoadDanhSachThiSinh();
        }

        // Cập nhật lên UI và đồng bộ vào DATABASE nhật ký an toàn
        private void LogViPham(string doiTuong, string hanhVi)
        {
            if (this.InvokeRequired)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        int typeId = 0;
                        if (hanhVi == "Sử dụng điện thoại") typeId = 1;
                        else if (hanhVi == "Quay ngang ngửa") typeId = 2;
                        else if (hanhVi == "Khuất tầm nhìn / Bỏ vị trí") typeId = 4;

                        if (typeId > 0)
                        {
                            string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                string query = "INSERT INTO ViolationLogs (TypeID, ViolationTime, IsConfirmed, ProctorNote) VALUES (@typeId, @time, 0, @note)";
                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@typeId", typeId);
                                    cmd.Parameters.AddWithValue("@time",   DateTime.Now);
                                    cmd.Parameters.AddWithValue("@note",   "AI tự động phát hiện (" + doiTuong + "): " + hanhVi);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch { }
                });

                this.BeginInvoke(new Action(() => LogViPham(doiTuong, hanhVi)));
                return;
            }

            if (dtViPham != null)
            {
                DataRow row = dtViPham.NewRow();
                row["Thời gian"] = DateTime.Now.ToString("HH:mm:ss");
                row["Đối tượng"] = doiTuong;
                row["Hành vi"]   = hanhVi;
                dtViPham.Rows.InsertAt(row, 0);
                if (dtViPham.Rows.Count > 50)
                    dtViPham.Rows.RemoveAt(dtViPham.Rows.Count - 1);
                dgvViPham.ClearSelection();
            }
        }

        // Lấy LogID mới nhất (gắn vào video sau khi ghi xong)
        private int GetLatestLogId()
        {
            try
            {
                string conn = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                using (var c = new SqlConnection(conn))
                {
                    c.Open();
                    using (var cmd = new SqlCommand("SELECT TOP 1 LogID FROM ViolationLogs ORDER BY ViolationTime DESC", c))
                        return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return -1; }
        }

        private void LoadDanhSachThiSinh()
        {
            try
            {
                DbConnection db = new DbConnection();
                dgvDanhSach.DataSource = db.LoadDanhSachThiSinh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị dữ liệu: " + ex.Message);
            }
        }

        // Đổi tên thành _1 để khớp với Designer
        private void btnMoCamera_Click_1(object sender, EventArgs e)
        {
            if (btnMoCamera.Text == "Bật Camera")
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy Webcam kết nối với máy thiết bị!", "Thiếu Camera", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                StopCamera();
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();

                // Tạo thư mục phiên trên Google Drive ngay khi bật camera
                GoogleDriveHelper.StartSession(DateTime.Now);

                // Biến đổi UI thành nút dừng tắt
                btnMoCamera.Text = "Tắt Camera";
                btnMoCamera.BackColor = Color.IndianRed;
            }
            else
            {
                StopCamera();
                if (picCamera.Image != null)
                {
                    picCamera.Image.Dispose();
                    picCamera.Image = null; // Trả lại khung đen
                }
                
                // Trả về UI mặc định
                btnMoCamera.Text = "Bật Camera";
                btnMoCamera.BackColor = Color.MediumSeaGreen;
            }
        }

        private void TriggerAutoRecord(Size frameSize)
        {
            if (!isRecordingAuto)
            {
                try
                {
                    if (autoVideoWriter != null)
                    {
                        autoVideoWriter.Dispose();
                        autoVideoWriter = null;
                    }
                    // Tên file = thời điểm vi phạm để tránh đè lẫn
                    string fileName  = "violation_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".avi";
                    string localPath = System.IO.Path.Combine(Application.StartupPath, "violations", fileName);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(localPath));

                    int fourcc = VideoWriter.Fourcc('M', 'J', 'P', 'G');
                    autoVideoWriter = new VideoWriter(localPath, fourcc, 15, frameSize, true);
                    isRecordingAuto     = true;
                    autoRecordingFrames = 0;
                    _currentViolationVideoPath = localPath;
                    _currentViolationVideoName = fileName;
                }
                catch { }
            }
        }

        // Đường dẫn + tên file video đang ghi
        private string _currentViolationVideoPath = "";
        private string _currentViolationVideoName  = "";

        // Được gọi sau khi dừng ghi – upload lên Drive ngay nền
        private void FinishAndUploadVideo(int logId)
        {
            string localPath = _currentViolationVideoPath;
            string fileName  = _currentViolationVideoName;
            _currentViolationVideoPath = "";
            _currentViolationVideoName  = "";

            if (string.IsNullOrEmpty(localPath) || !System.IO.File.Exists(localPath)) return;

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    if (!GoogleDriveHelper.HasCredentials())
                    {
                        // Chưa có credentials – chỉ lưu path cục bộ
                        SaveSnapshotPath(logId, localPath);
                        return;
                    }

                    string driveLink = GoogleDriveHelper.UploadVideo(localPath, fileName);
                    SaveSnapshotPath(logId, driveLink);

                    // Xóa file cục bộ sau khi đã upload thành công (tiết kiệm dung lượng)
                    try { System.IO.File.Delete(localPath); } catch { }
                }
                catch
                {
                    // Nếu upload lỗi, giữ lại file cục bộ và lưu path cục bộ
                    SaveSnapshotPath(logId, localPath);
                }
            });
        }

        private void SaveSnapshotPath(int logId, string path)
        {
            if (logId <= 0) return;
            try
            {
                string conn = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                using (var c = new SqlConnection(conn))
                {
                    c.Open();
                    using (var cmd = new SqlCommand("UPDATE ViolationLogs SET SnapshotPath=@p WHERE LogID=@id", c))
                    {
                        cmd.Parameters.AddWithValue("@p",  path);
                        cmd.Parameters.AddWithValue("@id", logId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (Bitmap original = (Bitmap)eventArgs.Frame.Clone())
                {
                    Bitmap bitmap = original.Clone(new Rectangle(0, 0, original.Width, original.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    using (Image<Bgr, Byte> currentFrame = bitmap.ToImage<Bgr, Byte>())
                    {
                        if (isDnnLoaded && faceDetector != null)
                        {
                            // 1. Chuyển ảnh màu sang ảnh xám (để dùng cho phone detector nếu cần)
                            Mat grayFrame = new Mat();
                            CvInvoke.CvtColor(currentFrame, grayFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                            
                            List<Rectangle> listMatThang = new List<Rectangle>();
                            List<Rectangle> listMatNghieng = new List<Rectangle>();

                            // A. SỬ DỤNG YUNET FACE DETECTION (Chính xác cao, trả về 5 điểm mốc khuôn mặt)
                            faceDetector.InputSize = currentFrame.Size;
                            Mat faces = new Mat();
                            faceDetector.Detect(currentFrame.Mat, faces);
                            
                            int numFaces = faces.Rows;
                            float[] faceData = new float[15];

                            for (int i = 0; i < numFaces; i++)
                            {
                                faces.Row(i).CopyTo(faceData);
                                
                                int x = (int)faceData[0];
                                int y = (int)faceData[1];
                                int w = (int)faceData[2];
                                int h = (int)faceData[3];
                                
                                // Đảm bảo không vượt quá viền ảnh
                                x = Math.Max(0, x);
                                y = Math.Max(0, y);
                                w = Math.Min(currentFrame.Width - x, w);
                                h = Math.Min(currentFrame.Height - y, h);
                                Rectangle rect = new Rectangle(x, y, w, h);
                                
                                // Landmarks
                                float x_re = faceData[4]; // Right eye
                                float x_le = faceData[6]; // Left eye
                                float x_nt = faceData[8]; // Nose tip
                                float confidence = faceData[14];
                                
                                if (confidence < 0.75f) continue; // Bỏ qua mảng tóc, gáy (độ tự tin thấp)
                                
                                // Thuật toán kiểm tra mặt thẳng hay nghiêng dựa trên mốc khuôn mặt
                                // Tính khoảng cách ngang từ mũi đến 2 mắt
                                float dist1 = Math.Abs(x_nt - x_re);
                                float dist2 = Math.Abs(x_nt - x_le);
                                float maxDist = Math.Max(dist1, dist2);
                                float minDist = Math.Min(dist1, dist2);
                                
                                bool isStraight = true;
                                if (maxDist > 0)
                                {
                                    float ratio = minDist / maxDist;
                                    // Nếu tỉ lệ < 0.35, nghĩa là mũi lệch hẳn sang một bên -> Quay ngang ngửa
                                    if (ratio < 0.35f) 
                                    {
                                        isStraight = false;
                                    }
                                }

                                if (isStraight)
                                {
                                    listMatThang.Add(rect);
                                    
                                    // Tiến hành Face Recognition bằng SFace (Chỉ nhận diện khi mặt thẳng)
                                    string matchedName = "Nguoi la";
                                    
                                    if (isRecognizerTrained && faceRecognizer != null && faceFeatures.Count > 0)
                                    {
                                        try
                                        {
                                            Mat alignedFace = new Mat();
                                            faceRecognizer.AlignCrop(currentFrame.Mat, faces.Row(i), alignedFace);
                                            
                                            Mat feature = new Mat();
                                            faceRecognizer.Feature(alignedFace, feature);
                                            
                                            // So sánh với DB (Cosine similarity, threshold >= 0.363)
                                            double bestScore = 0.0;
                                            int bestMatchId = -1;
                                            
                                            foreach (var kvp in faceFeatures)
                                            {
                                                double score = faceRecognizer.Match(feature, kvp.Value, FaceRecognizerSF.DisType.Cosine);
                                                if (score > bestScore)
                                                {
                                                    bestScore = score;
                                                    bestMatchId = kvp.Key;
                                                }
                                            }
                                            
                                            if (bestScore >= 0.363 && bestMatchId != -1)
                                            {
                                                matchedName = faceNames[bestMatchId] + " (" + Math.Round(bestScore, 2) + ")";
                                                lastRecognizedName = faceNames[bestMatchId];
                                                lastRecognizedTime = DateTime.Now;
                                            }
                                        }
                                        catch { }
                                    }
                                    
                                    CvInvoke.Rectangle(currentFrame, rect, new MCvScalar(0, 255, 0), 2);
                                    string labelText = RemoveAccents(matchedName);
                                    CvInvoke.PutText(currentFrame, labelText, new Point(rect.X, rect.Y - 10), 
                                        Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.7, new MCvScalar(0, 255, 0), 2);
                                }
                                else
                                {
                                    listMatNghieng.Add(rect);
                                    CvInvoke.Rectangle(currentFrame, rect, new MCvScalar(0, 0, 255), 2);
                                    CvInvoke.PutText(currentFrame, "CANH BAO: Quay ngang ngua!", new Point(rect.X, rect.Y - 10), 
                                        Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 2);
                                    
                                    string violatorName = "Chưa xác định";
                                    if ((DateTime.Now - lastRecognizedTime).TotalSeconds < 10)
                                    {
                                        violatorName = lastRecognizedName;
                                    }

                                    if (!trackers.ContainsKey(violatorName)) trackers[violatorName] = new ViolationTracker();
                                    ViolationTracker trk = trackers[violatorName];

                                    if (!trk.IsViolating)
                                    {
                                        trk.IsViolating = true;
                                        trk.ViolationStartTime = DateTime.Now;
                                    }
                                    trk.LastSeenViolation = DateTime.Now;
                                    
                                    double t = (DateTime.Now - trk.ViolationStartTime).TotalSeconds;
                                    double W = 5.0; // Quay ngang ngửa
                                    double lambda = 0.5;
                                    double alpha = 10.0;
                                    
                                    double S = W * Math.Exp(lambda * t) + alpha * trk.Frequency;
                                    trk.CurrentScore = Math.Min(100.0, S);
                                    
                                    if (trk.CurrentScore >= 20.0) // Ngưỡng kích hoạt lưu DB
                                    {
                                        LogViPham(violatorName, "Quay ngang ngửa");
                                        trk.Frequency += 1;
                                        trk.IsViolating = false; // Reset to avoid spamming
                                        trk.CurrentScore = 0;
                                        TriggerAutoRecord(currentFrame.Size);
                                    }
                                }
                            }

                            // Phát hiện điện thoại (dùng Haar cascade truyền thống trên ảnh xám thu nhỏ)
                            Rectangle[] dienThoai = new Rectangle[0];
                            if (phoneDetector != null)
                            {
                                Mat smallGrayFrame = new Mat();
                                double scale = 2.0;
                                CvInvoke.Resize(grayFrame, smallGrayFrame, new Size((int)(grayFrame.Width / scale), (int)(grayFrame.Height / scale)));
                                Rectangle[] dtTemp = phoneDetector.DetectMultiScale(smallGrayFrame, 1.2, 5, Size.Empty);
                                
                                dienThoai = new Rectangle[dtTemp.Length];
                                for (int i = 0; i < dtTemp.Length; i++)
                                {
                                    dienThoai[i] = new Rectangle(
                                        (int)(dtTemp[i].X * scale),
                                        (int)(dtTemp[i].Y * scale),
                                        (int)(dtTemp[i].Width * scale),
                                        (int)(dtTemp[i].Height * scale)
                                    );
                                }
                                smallGrayFrame.Dispose();
                            }

                            // XỬ LÝ PHÁT HIỆN ĐIỆN THOẠI
                            if (dienThoai.Length > 0)
                            {
                                foreach (var rect in dienThoai)
                                {
                                    CvInvoke.Rectangle(currentFrame, rect, new MCvScalar(255, 0, 0), 2);
                                    CvInvoke.PutText(currentFrame, "CANH BAO: Phat hien Dien thoai!", new Point(rect.X, rect.Y - 10), 
                                        Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.7, new MCvScalar(255, 0, 0), 2);
                                }
                                
                                string violatorName = "Chưa xác định";
                                if ((DateTime.Now - lastRecognizedTime).TotalSeconds < 10)
                                {
                                    violatorName = lastRecognizedName;
                                }

                                if (!trackers.ContainsKey(violatorName)) trackers[violatorName] = new ViolationTracker();
                                ViolationTracker trk = trackers[violatorName];

                                if (!trk.IsViolating)
                                {
                                    trk.IsViolating = true;
                                    trk.ViolationStartTime = DateTime.Now;
                                }
                                trk.LastSeenViolation = DateTime.Now;
                                
                                double t = (DateTime.Now - trk.ViolationStartTime).TotalSeconds;
                                double W = 30.0; // Điện thoại nặng hơn
                                double lambda = 0.5;
                                double alpha = 10.0;
                                
                                double S = W * Math.Exp(lambda * t) + alpha * trk.Frequency;
                                trk.CurrentScore = Math.Min(100.0, S);
                                
                                if (trk.CurrentScore >= 20.0)
                                {
                                    LogViPham(violatorName, "Sử dụng điện thoại");
                                    trk.Frequency += 1;
                                    trk.IsViolating = false; 
                                    trk.CurrentScore = 0;
                                    TriggerAutoRecord(currentFrame.Size);
                                }
                            }

                            // TRẠNG THÁI 3: KHÔNG TÌM THẤY MẶT (BỎ VỊ TRÍ)
                            if (listMatThang.Count == 0 && listMatNghieng.Count == 0 && dienThoai.Length == 0)
                            {
                                CvInvoke.PutText(currentFrame, "CANH BAO: Khuat tam nhin / Quay ra sau / Bo vi tri!", new Point(10, 30), 
                                    Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.8, new MCvScalar(0, 165, 255), 2);
                                    
                                if ((DateTime.Now - lastNoFaceWarning).TotalSeconds > 5)
                                {
                                    LogViPham("Hệ thống", "Khuất tầm nhìn / Bỏ vị trí");
                                    lastNoFaceWarning = DateTime.Now;
                                    TriggerAutoRecord(currentFrame.Size);
                                }
                            }
                            
                            // Reset tracker nếu không còn vi phạm liên tục quá 1.5 giây
                            foreach (var trk in trackers.Values)
                            {
                                if (trk.IsViolating && (DateTime.Now - trk.LastSeenViolation).TotalSeconds > 1.5)
                                {
                                    trk.IsViolating = false;
                                    trk.CurrentScore = trk.Frequency * 10.0; // Về base score
                                }
                            }
                            
                            grayFrame.Dispose();
                        }
                        else
                        {
                            CvInvoke.PutText(currentFrame, "CHUA NAP DUOC CAU HINH AI!", new Point(40, 50), 
                                Emgu.CV.CvEnum.FontFace.HersheySimplex, 1.0, new MCvScalar(0, 0, 255), 2);
                        }
                        
                        // Vẽ chấm xanh để biết code đang chạy
                        CvInvoke.Circle(currentFrame, new Point(20, 20), 10, new MCvScalar(0, 255, 0), -1);

                        // ---- GHI HÌNH ----
                        if (isRecordingAuto && autoVideoWriter != null)
                        {
                            try
                            {
                                autoVideoWriter.Write(currentFrame.Mat);
                                autoRecordingFrames++;
                                if (autoRecordingFrames >= 75) // 5 giây @ 15 FPS
                                {
                                    autoVideoWriter.Dispose();
                                    autoVideoWriter   = null;
                                    isRecordingAuto   = false;

                                    // Lấy LogID mới nhất để gắn video
                                    int latestLogId = GetLatestLogId();
                                    FinishAndUploadVideo(latestLogId);
                                }

                                // Hiển thị REC ●
                                CvInvoke.PutText(currentFrame, "REC \u25cf", new Point(currentFrame.Width - 110, 40),
                                    Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.9, new MCvScalar(0, 0, 255), 2);
                            }
                            catch { isRecordingAuto = false; }
                        }

                        // Ghi đè lại ảnh bitmap bằng ảnh đã vẽ từ AI để hiển thị lên PictureBox
                        bitmap.Dispose();
                        bitmap = currentFrame.ToBitmap();
                    }

                    // Update UI an toàn, dùng BeginInvoke thay vì Invoke để tránh Deadlock khi tắt camera
                    if (!picCamera.IsDisposed && !this.IsDisposed)
                    {
                        picCamera.BeginInvoke(new Action(() =>
                        {
                            if (!picCamera.IsDisposed)
                            {
                                if (picCamera.Image != null) picCamera.Image.Dispose();
                                picCamera.Image = bitmap;
                            }
                            else
                            {
                                bitmap.Dispose();
                            }
                        }));
                    }
                    else
                    {
                        bitmap.Dispose();
                    }
                }
            }
            catch { }
        }

        private void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;
            }
        }

        // Giữ lại các nút cũ
        private void btnDiemDanh_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Để hệ thống điểm danh tự động, hãy đảm bảo bạn đã 'Thêm ảnh' (Bấm đúp vào học sinh) và nhấn OK để hệ thống tự động quét AI.", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // --- SỰ KIỆN TẢI ẢNH TỪ MÁY TÍNH VÀO SQL ---
        private void DgvDanhSach_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string studentCode = dgvDanhSach.Rows[e.RowIndex].Cells["Mã Thí Sinh"]?.Value?.ToString();
            string studentName = dgvDanhSach.Rows[e.RowIndex].Cells["Họ Tên"]?.Value?.ToString();

            if (string.IsNullOrEmpty(studentCode)) return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Chọn ảnh khuôn mặt đại diện cho " + studentName + " (Lưu ý: Hệ thống sẽ tự nhận diện khuôn mặt trong ảnh bạn tải lên)";
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Đọc ảnh màu để hỗ trợ xử lý DNN (Color = 1)
                    Mat uploadMat = CvInvoke.Imread(ofd.FileName, (Emgu.CV.CvEnum.ImreadModes)1);
                    if (uploadMat.IsEmpty) throw new Exception("Không đọc được ảnh.");
                    
                    if (isDnnLoaded)
                    {
                        string yunetPath = System.IO.Path.Combine(Application.StartupPath, "face_detection_yunet_2023mar.onnx");
                        if (System.IO.File.Exists(yunetPath))
                        {
                            using (FaceDetectorYN localDetector = new FaceDetectorYN(yunetPath, "", uploadMat.Size, 0.5f, 0.3f, 5000))
                            {
                                Mat faces = new Mat();
                                localDetector.Detect(uploadMat, faces);
                                if (faces.Rows == 0)
                                {
                                    MessageBox.Show("Không phát hiện khuôn mặt nào trong ảnh này, hãy chọn ảnh khác rõ mặt hơn!", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                    }

                    // Lưu ảnh gốc (có nén) vào DB để hàm TrainFaceRecognizer tự động AlignCrop
                    using (Bitmap faceBmp = uploadMat.ToBitmap())
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap saveBmp = faceBmp;
                        if (faceBmp.Width > 800)
                        {
                            saveBmp = new Bitmap(faceBmp, new Size(800, faceBmp.Height * 800 / faceBmp.Width));
                        }
                        
                        saveBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();

                        string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "UPDATE Candidates SET FaceImageData = @image WHERE StudentCode = @code";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@image", imageBytes);
                                cmd.Parameters.AddWithValue("@code", studentCode);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    if (dgvDanhSach.Columns.Contains("Trạng thái"))
                        dgvDanhSach.Rows[e.RowIndex].Cells["Trạng thái"].Value = "Đã có ảnh DB";
                        
                    MessageBox.Show("Tiếp nhận ảnh thành công! Đang thiết lập cấu hình khuôn mặt mới...", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TrainFaceRecognizer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- HUẤN LUYỆN KHUÔN MẶT DB ---
        private void TrainFaceRecognizer()
        {
            try
            {
                string yunetPath = System.IO.Path.Combine(Application.StartupPath, "face_detection_yunet_2023mar.onnx");
                string sfacePath = System.IO.Path.Combine(Application.StartupPath, "face_recognition_sface_2021dec.onnx");
                if (!System.IO.File.Exists(yunetPath) || !System.IO.File.Exists(sfacePath)) return;
                
                using (FaceDetectorYN localDetector = new FaceDetectorYN(yunetPath, "", new System.Drawing.Size(320, 320), 0.5f, 0.3f, 5000))
                using (FaceRecognizerSF localRecognizer = new FaceRecognizerSF(sfacePath, "", Emgu.CV.Dnn.Backend.Default, Emgu.CV.Dnn.Target.Cpu))
                {
                    string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT CandidateID, FullName, FaceImageData FROM Candidates WHERE FaceImageData IS NOT NULL";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            faceNames.Clear();
                            faceFeatures.Clear();

                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["CandidateID"]);
                                string name = reader["FullName"].ToString();
                                byte[] imgBytes = (byte[])reader["FaceImageData"];

                                using (MemoryStream ms = new MemoryStream(imgBytes))
                                using (Bitmap bmp = new Bitmap(ms))
                                using (Image<Bgr, byte> bgrImage = bmp.ToImage<Bgr, byte>())
                                {
                                    Mat imgMat = bgrImage.Mat;
                                    
                                    localDetector.InputSize = imgMat.Size;
                                    Mat faces = new Mat();
                                    localDetector.Detect(imgMat, faces);
                                    
                                    if (faces.Rows > 0)
                                    {
                                        Mat firstFace = faces.Row(0);
                                        Mat alignedFace = new Mat();
                                        localRecognizer.AlignCrop(imgMat, firstFace, alignedFace);
                                        
                                        Mat feature = new Mat();
                                        localRecognizer.Feature(alignedFace, feature);
                                        
                                        faceFeatures[id] = feature.Clone();
                                        faceNames[id] = name;
                                    }
                                }
                            }

                            if (faceFeatures.Count > 0)
                            {
                                isRecognizerTrained = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Gọi Silent
            }
        }
        private void btnGiamSatAI_Click(object sender, EventArgs e) { }
        private void btnKetThuc_Click(object sender, EventArgs e) { }

        private void btnQuanLyViPham_Click(object sender, EventArgs e)
        {
            frmNhatKyViPham frm = new frmNhatKyViPham();
            frm.Show();
        }

        private void btnKetThuc_Click_1(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn kết thúc giám sát và quay lại đăng nhập?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                // Mở lại MainForm đang bị Hide phía sau đầu tiên để mượt mà nhất
                foreach (Form f in Application.OpenForms)
                {
                    if (f is MainForm)
                    {
                        f.Show();
                        break;
                    }
                }
                
                this.Hide();  
                StopCamera(); 
                
                // Tắt form Giám sát này hoàn toàn khỏi bộ nhớ
                this.Close();
            }
        }

        private void frmGiamSat_Load_1(object sender, EventArgs e)
        {
            TrainFaceRecognizer();
            LoadViolationFrequencies();
        }

        private void LoadViolationFrequencies()
        {
            try
            {
                string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT DoiTuong, COUNT(*) as Freq FROM NhatKyViPham GROUP BY DoiTuong";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["DoiTuong"].ToString();
                            int freq = Convert.ToInt32(reader["Freq"]);
                            if (!trackers.ContainsKey(name)) trackers[name] = new ViolationTracker();
                            trackers[name].Frequency = freq;
                        }
                    }
                }
            }
            catch { }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                
                // Cần thiết lập Encoding để đọc file Excel cũ không bị lỗi "No data is available for encoding 1252"
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                try
                {
                    // 1. Đọc file Excel (Bật chế độ FileShare.ReadWrite để không bị lỗi nếu file đang mở)
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                            });

                            DataTable dtExcel = result.Tables[0]; // Lấy Sheet đầu tiên

                            // 2. Lưu vào SQL Server
                            string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();

                                // XÓA TOÀN BỘ DỮ LIỆU CŨ TRONG BẢNG CANDIDATES TRƯỚC KHI THÊM MỚI
                                string deleteQuery = "DELETE FROM Candidates";
                                using (SqlCommand cmdDel = new SqlCommand(deleteQuery, conn))
                                {
                                    cmdDel.ExecuteNonQuery();
                                }

                                // Duyệt từng dòng trong file Excel để Insert vào DB
                                foreach (DataRow row in dtExcel.Rows)
                                {
                                    // Kiểm tra xem dòng có rỗng không
                                    if(row.IsNull(0) || string.IsNullOrWhiteSpace(row[0].ToString())) continue;

                                    string studentCode = row[0].ToString(); // Cột 1: Mã SV
                                    string fullName = row[1].ToString();    // Cột 2: Họ Tên
                                    string className = row[2].ToString();   // Cột 3: Lớp

                                    string query = "INSERT INTO Candidates (StudentCode, FullName, Class) VALUES (@code, @name, @class)";
                                    using (SqlCommand cmd = new SqlCommand(query, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@code", studentCode);
                                        cmd.Parameters.AddWithValue("@name", fullName);
                                        cmd.Parameters.AddWithValue("@class", className);
                                        
                                        try { cmd.ExecuteNonQuery(); } 
                                        catch { /* Bỏ qua nếu lỗi trùng SBD */ }
                                    }
                                }
                            }
                            MessageBox.Show("Nhập dữ liệu Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Load lại danh sách lên lưới rạng rỡ nha!
                            LoadDanhSachThiSinh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đọc file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dtViPham == null || dtViPham.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu vi phạm để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (Mở bằng Excel)|*.csv|All files|*.*";
            sfd.FileName = "NhatKyViPham_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // Thêm BOM để Excel việt hóa không bị lỗi phông
                        sw.Write('\uFEFF');
                        
                        // Header
                        for (int i = 0; i < dtViPham.Columns.Count; i++)
                        {
                            sw.Write("\"" + dtViPham.Columns[i].ColumnName + "\"");
                            if (i < dtViPham.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // Rows
                        foreach (DataRow row in dtViPham.Rows)
                        {
                            for (int i = 0; i < dtViPham.Columns.Count; i++)
                            {
                                string value = row[i]?.ToString() ?? "";
                                value = value.Replace("\"", "\"\""); 
                                sw.Write($"\"{value}\"");
                                if (i < dtViPham.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }
                    MessageBox.Show("Xuất nhật ký thành công! Bạn có thể mở trực tiếp file này bằng máy tính.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lưu file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImportImages_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Chọn nhiều ảnh khuôn mặt của lớp (Tên file học sinh = SBD)";
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            ofd.Multiselect = true;
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int countSuccess = 0;
                string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (string filePath in ofd.FileNames)
                    {
                        try
                        {
                            string studentCode = System.IO.Path.GetFileNameWithoutExtension(filePath);
                            Mat uploadMat = CvInvoke.Imread(filePath, (Emgu.CV.CvEnum.ImreadModes)1);
                            if (uploadMat.IsEmpty) continue;

                            if (isDnnLoaded)
                            {
                                string yunetPath = System.IO.Path.Combine(Application.StartupPath, "face_detection_yunet_2023mar.onnx");
                                if (System.IO.File.Exists(yunetPath))
                                {
                                    using (FaceDetectorYN localDetector = new FaceDetectorYN(yunetPath, "", uploadMat.Size, 0.5f, 0.3f, 5000))
                                    {
                                        Mat faces = new Mat();
                                        localDetector.Detect(uploadMat, faces);
                                        if (faces.Rows == 0) continue;
                                    }
                                }
                            }

                            using (Bitmap faceBmp = uploadMat.ToBitmap())
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Bitmap saveBmp = faceBmp;
                                if (faceBmp.Width > 800)
                                {
                                    saveBmp = new Bitmap(faceBmp, new Size(800, faceBmp.Height * 800 / faceBmp.Width));
                                }
                                
                                saveBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                byte[] imageBytes = ms.ToArray();

                                string query = "UPDATE Candidates SET FaceImageData = @image WHERE StudentCode = @code";
                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@image", imageBytes);
                                    cmd.Parameters.AddWithValue("@code", studentCode);
                                    int rows = cmd.ExecuteNonQuery();
                                    if (rows > 0) countSuccess++;
                                }
                            }
                        }
                        catch { /* Bỏ qua lỗi ảnh đơn lẻ */ }
                    }
                }
                MessageBox.Show($"Hoàn tất tải lên! Đã chiết xuất khuôn mặt tự động và lưu thành công cho {countSuccess} thí sinh vào CSDL.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Load lại danh sách và huấn luyện lại AI Model mới
                LoadDanhSachThiSinh();
                TrainFaceRecognizer();
            }
        }

        private void btnIPCamera_Click(object sender, EventArgs e)
        {
            frmMultiCam mcam = new frmMultiCam();
            mcam.Show(); // Để là show thì có thể view độc lập để tiện báo cáo như ý bạn trình bày
        }

        private void btnPlayVideo_Click(object sender, EventArgs e)
        {
            string tempFile = System.IO.Path.Combine(Application.StartupPath, "temp_violation.avi");
            if (System.IO.File.Exists(tempFile))
            {
                if (isRecordingAuto)
                {
                    MessageBox.Show("Vui lòng đợi vài giây vì hệ thống đang bận ghi lại video sai phạm mới nhất...", "Đang bận", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = tempFile,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể mở video. " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Chưa có đoạn ghi hình vi phạm nào hợp lệ!", "Trống", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSaveVideo_Click(object sender, EventArgs e)
        {
            string tempFile = System.IO.Path.Combine(Application.StartupPath, "temp_violation.avi");
            if (!System.IO.File.Exists(tempFile))
            {
                MessageBox.Show("Chưa có video vi phạm nào diễn ra để lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (isRecordingAuto)
            {
                MessageBox.Show("Hệ thống đang ghi dở 1 đoạn vi phạm 5 giây cuối cùng. Vui lòng thử lại sau khoảnh khắc vài giây!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Lưu chứng cứ vi phạm phòng thi để báo cáo";
            sfd.Filter = "AVI Video|*.avi";
            sfd.FileName = "BangChung_ViPham_" + DateTime.Now.ToString("HHmmss_ddMMyyyy") + ".avi";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.File.Copy(tempFile, sfd.FileName, true);
                    MessageBox.Show("Đã trích xuất đoạn video bằng chứng 5 giây thành công!", "Báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lưu file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- CÁC HÀM TRỢ GIÚP CHO OPENCV DNN FACE DETECTION ---
        private void EnsureDnnModelFiles()
        {
            try
            {
                string yunetPath = Path.Combine(Application.StartupPath, "face_detection_yunet_2023mar.onnx");
                string sfacePath = Path.Combine(Application.StartupPath, "face_recognition_sface_2021dec.onnx");

                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    if (!File.Exists(yunetPath))
                    {
                        var data = client.GetByteArrayAsync("https://github.com/opencv/opencv_zoo/raw/main/models/face_detection_yunet/face_detection_yunet_2023mar.onnx").Result;
                        File.WriteAllBytes(yunetPath, data);
                    }

                    if (!File.Exists(sfacePath))
                    {
                        var data = client.GetByteArrayAsync("https://github.com/opencv/opencv_zoo/raw/main/models/face_recognition_sface/face_recognition_sface_2021dec.onnx").Result;
                        File.WriteAllBytes(sfacePath, data);
                    }
                }
            }
            catch { }
        }
    }
}
