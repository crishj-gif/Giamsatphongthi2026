using System;
using System.Collections.Generic;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GiamSatPhongThi
{
    /// <summary>
    /// Lớp tiện ích GoogleDriveHelper: Hỗ trợ chức năng tải video vi phạm lên Google Drive.
    /// Cấu trúc thư mục trên Drive sẽ như sau:
    ///   NhatKiViPham/
    ///     └── NhatKiViPham_HH-MM_DD-MM-YYYY/   ← Thư mục này tạo 1 lần khi gọi StartSession()
    ///           ├── violation_xxx.avi
    ///           └── ...
    /// </summary>
    public static class GoogleDriveHelper
    {
        // Yêu cầu quyền truy cập vào các file do ứng dụng này tạo ra trên Drive
        private static readonly string[] Scopes    = { DriveService.Scope.DriveFile };
        private static readonly string   AppName   = "GiamSatPhongThi";
        private static readonly string   TokenFolder = "google_token";

        // ── Tên thư mục gốc trên Drive ──────────────────────────────────
        private const string ROOT_FOLDER = "NhatKiViPham";

        // ── Biến cache (lưu trữ tạm thời) theo mẫu Singleton ─────────────
        private static DriveService _service       = null;
        private static string       _rootFolderId  = null;   // ID thư mục gốc trên Google Drive
        private static string       _sessionFolderId = null; // ID thư mục phiên giám sát hiện tại
        // Đối tượng dùng để khóa (lock) khi thao tác với luồng (thread) để tránh xung đột
        private static readonly object _lock = new object();

        // ================================================================
        /// <summary>
        /// Khởi động phiên làm việc – tạo thư mục phiên ngay khi bắt đầu.
        /// Thường được gọi 1 lần khi người dùng nhấn nút "Bắt đầu giám sát AI".
        /// </summary>
        /// <param name="sessionTime">Thời gian bắt đầu phiên giám sát</param>
        // ================================================================
        public static void StartSession(DateTime sessionTime)
        {
            // Kiểm tra xem đã có file xác thực credentials.json chưa
            if (!HasCredentials()) return;

            // Task.Run: Chạy tiến trình tạo thư mục ngầm (ở background thread)
            // Việc này giúp giao diện chính (UI thread) không bị đơ/treo khi ứng dụng đang gọi API Google
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var svc = GetService();
                    // Tạo tên thư mục phiên theo thời gian. Ví dụ: NhatKiViPham_14-30_08-05-2026
                    string sessionName = "NhatKiViPham_" + sessionTime.ToString("HH-mm_dd-MM-yyyy");
                    
                    // Lấy ID thư mục gốc (hoặc tạo mới nếu chưa có)
                    _rootFolderId    = GetOrCreateFolder(svc, ROOT_FOLDER, null);
                    // Lấy ID thư mục phiên nằm trong thư mục gốc
                    _sessionFolderId = GetOrCreateFolder(svc, sessionName, _rootFolderId);
                }
                catch { /* Bỏ qua lỗi nếu Google Drive chưa sẵn sàng hoặc rớt mạng để tránh crash app */ }
            });
        }

        // ================================================================
        /// <summary>
        /// Tải một video từ máy tính (local) lên thư mục phiên hiện tại trên Google Drive.
        /// </summary>
        /// <param name="localFilePath">Đường dẫn tới file video trên máy tính</param>
        /// <param name="driveFileName">Tên file khi lưu trên Google Drive</param>
        /// <returns>Chuỗi URL đường link chia sẻ công khai của video trên Google Drive</returns>
        // ================================================================
        public static string UploadVideo(string localFilePath, string driveFileName)
        {
            var svc = GetService();

            // Đảm bảo thư mục phiên đã có (lazy load - nạp chậm nếu StartSession chưa gọi hoặc chưa chạy xong)
            EnsureSessionFolder(svc);

            // Metadata: Thông tin cấu hình của file sẽ đưa lên Drive (Tên, Thư mục cha, Loại file)
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name    = driveFileName,
                Parents = new List<string> { _sessionFolderId },
                MimeType = "video/x-msvideo" // Kiểu định dạng của file AVI
            };

            FilesResource.CreateMediaUpload request;
            // FileStream: Đọc file video từ ổ cứng
            using (var stream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open))
            {
                request = svc.Files.Create(fileMetadata, stream, "video/x-msvideo");
                request.Fields = "id"; // Chỉ yêu cầu API trả về ID của file sau khi upload xong
                request.Upload();      // Thực thi việc đẩy file lên
            }

            string fileId = request.ResponseBody?.Id;
            if (string.IsNullOrEmpty(fileId))
                throw new Exception("Upload thất bại – Drive không trả về File ID.");

            // Cấp quyền (Permission) cho file vừa upload:
            // Type = "anyone" (bất kỳ ai có link), Role = "reader" (được quyền xem)
            svc.Permissions.Create(
                new Permission { Type = "anyone", Role = "reader" }, fileId).Execute();

            return $"https://drive.google.com/file/d/{fileId}/view";
        }

        // ================================================================
        /// <summary>
        /// Mở đường link (URL) trong trình duyệt web mặc định của Windows.
        /// </summary>
        /// <param name="driveUrl">Link Google Drive cần mở</param>
        // ================================================================
        public static void OpenInBrowser(string driveUrl)
        {
            if (string.IsNullOrWhiteSpace(driveUrl)) return;
            
            // System.Diagnostics.Process.Start: Mở một ứng dụng ngoài (ở đây là trình duyệt web)
            // UseShellExecute = true cho phép hệ điều hành tự tìm ứng dụng mặc định phù hợp với URL
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = driveUrl,
                UseShellExecute = true
            });
        }

        // ================================================================
        /// <summary>
        /// Kiểm tra xem file cấu hình xác thực của Google (credentials.json) có tồn tại hay không.
        /// </summary>
        // ================================================================
        public static bool HasCredentials()
        {
            string credPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "credentials.json");
            return System.IO.File.Exists(credPath);
        }

        // ================================================================
        /// <summary>
        /// Lấy đối tượng kết nối (DriveService) với Google API – Chỉ tiến hành đăng nhập 1 lần (Singleton).
        /// </summary>
        // ================================================================
        private static DriveService GetService()
        {
            if (_service != null) return _service;
            
            // Từ khóa lock: Ngăn chặn nhiều luồng (thread) chạy vào khối code này cùng lúc
            // Điều này phòng trường hợp mở app lên sinh ra nhiều luồng đòi xác thực Google cùng một lúc
            lock (_lock)
            {
                if (_service != null) return _service;

                string credPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "credentials.json");
                if (!System.IO.File.Exists(credPath))
                    throw new System.IO.FileNotFoundException(
                        "Không tìm thấy credentials.json tại: " +
                        AppDomain.CurrentDomain.BaseDirectory);

                UserCredential credential;
                // Đọc file thông tin xác thực
                using (var stream = new System.IO.FileStream(
                    credPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    string tokenPath = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, TokenFolder);
                    
                    // AuthorizeAsync: Hiển thị màn hình đăng nhập Google trên trình duyệt nếu chưa có token,
                    // hoặc tự động đăng nhập lại nếu token đã được lưu tại "tokenPath".
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes, "user",
                        CancellationToken.None,
                        new FileDataStore(tokenPath, true)).Result;
                }

                // Khởi tạo đối tượng DriveService dùng để tương tác với Google Drive API
                _service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName       = AppName,
                });
            }
            return _service;
        }

        // ================================================================
        /// <summary>
        /// Đảm bảo thư mục phiên đã được tạo trên Google Drive. 
        /// Dùng khi người dùng bấm Lưu Vi Phạm nhưng StartSession chưa tạo xong.
        /// </summary>
        // ================================================================
        private static void EnsureSessionFolder(DriveService svc)
        {
            // Nếu đã có ID thư mục thì bỏ qua luôn để tiết kiệm thời gian gọi API
            if (!string.IsNullOrEmpty(_sessionFolderId)) return;
            
            _rootFolderId    = GetOrCreateFolder(svc, ROOT_FOLDER, null);
            string sessionName = "NhatKiViPham_" + DateTime.Now.ToString("HH-mm_dd-MM-yyyy");
            _sessionFolderId = GetOrCreateFolder(svc, sessionName, _rootFolderId);
        }

        // ================================================================
        /// <summary>
        /// Tìm thư mục theo tên trong một thư mục cha (parent). 
        /// Nếu thư mục chưa có mặt trên Google Drive thì tạo mới và trả về ID của thư mục đó.
        /// </summary>
        // ================================================================
        private static string GetOrCreateFolder(DriveService svc, string name, string parentId)
        {
            // Xây dựng câu truy vấn (query): Tìm các file có kiểu 'thư mục' (folder), chưa bị xóa và tên khớp
            string query = $"mimeType='application/vnd.google-apps.folder' " +
                           $"and name='{name}' and trashed=false";
            // Nếu truyền vào ID cha thì thêm điều kiện tìm trong thư mục cha đó
            if (!string.IsNullOrEmpty(parentId))
                query += $" and '{parentId}' in parents";

            var listReq = svc.Files.List();
            listReq.Q      = query;
            listReq.Fields = "files(id,name)"; // Giới hạn chỉ lấy id và tên để tối ưu đường truyền
            listReq.Spaces = "drive";
            
            // Chạy lệnh list lên Google Drive
            var found = listReq.Execute().Files;
            
            if (found != null && found.Count > 0)
                return found[0].Id; // Đã có thư mục → trả về ID luôn

            // Nếu danh sách rỗng (Chưa có) → tạo mới một thư mục
            var folderMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name     = name,
                MimeType = "application/vnd.google-apps.folder"
            };
            if (!string.IsNullOrEmpty(parentId))
                folderMeta.Parents = new List<string> { parentId };

            // Thực thi lệnh Create (tạo mới) trên Google Drive
            var created = svc.Files.Create(folderMeta).Execute();
            return created.Id;
        }
    }
}
