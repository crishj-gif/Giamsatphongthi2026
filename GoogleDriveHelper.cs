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
    /// Upload video vi phạm lên Google Drive với cấu trúc thư mục:
    ///   NhatKiViPham/
    ///     └── NhatKiViPham_HH-MM_DD-MM-YYYY/   ← tạo 1 lần khi StartSession()
    ///           ├── violation_xxx.avi
    ///           └── ...
    /// </summary>
    public static class GoogleDriveHelper
    {
        private static readonly string[] Scopes    = { DriveService.Scope.DriveFile };
        private static readonly string   AppName   = "GiamSatPhongThi";
        private static readonly string   TokenFolder = "google_token";

        // ── Tên thư mục gốc trên Drive ──────────────────────────────────
        private const string ROOT_FOLDER = "NhatKiViPham";

        // ── Cache singleton ──────────────────────────────────────────────
        private static DriveService _service       = null;
        private static string       _rootFolderId  = null;   // ID thư mục gốc
        private static string       _sessionFolderId = null; // ID thư mục phiên hiện tại
        private static readonly object _lock = new object();

        // ================================================================
        //  PUBLIC: Khởi động phiên – tạo thư mục phiên ngay khi bắt đầu
        //  Gọi 1 lần khi người dùng nhấn "Bắt đầu giám sát AI"
        // ================================================================
        public static void StartSession(DateTime sessionTime)
        {
            if (!HasCredentials()) return;

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var svc = GetService();
                    // Tên thư mục phiên: NhatKiViPham_14-30_08-05-2026
                    string sessionName = "NhatKiViPham_" + sessionTime.ToString("HH-mm_dd-MM-yyyy");
                    _rootFolderId    = GetOrCreateFolder(svc, ROOT_FOLDER, null);
                    _sessionFolderId = GetOrCreateFolder(svc, sessionName, _rootFolderId);
                }
                catch { /* Bỏ qua nếu Drive chưa sẵn sàng */ }
            });
        }

        // ================================================================
        //  PUBLIC: Upload video vào thư mục phiên hiện tại
        //  Trả về link Google Drive chia sẻ công khai
        // ================================================================
        public static string UploadVideo(string localFilePath, string driveFileName)
        {
            var svc = GetService();

            // Đảm bảo thư mục phiên đã có (lazy nếu StartSession chưa được gọi)
            EnsureSessionFolder(svc);

            // Metadata: đặt video vào thư mục phiên
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name    = driveFileName,
                Parents = new List<string> { _sessionFolderId },
                MimeType = "video/x-msvideo"
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open))
            {
                request = svc.Files.Create(fileMetadata, stream, "video/x-msvideo");
                request.Fields = "id";
                request.Upload();
            }

            string fileId = request.ResponseBody?.Id;
            if (string.IsNullOrEmpty(fileId))
                throw new Exception("Upload thất bại – Drive không trả về File ID.");

            // Cấp quyền anyone-can-view
            svc.Permissions.Create(
                new Permission { Type = "anyone", Role = "reader" }, fileId).Execute();

            return $"https://drive.google.com/file/d/{fileId}/view";
        }

        // ================================================================
        //  PUBLIC: Mở link trong trình duyệt mặc định
        // ================================================================
        public static void OpenInBrowser(string driveUrl)
        {
            if (string.IsNullOrWhiteSpace(driveUrl)) return;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = driveUrl,
                UseShellExecute = true
            });
        }

        // ================================================================
        //  PUBLIC: Kiểm tra credentials.json tồn tại chưa
        // ================================================================
        public static bool HasCredentials()
        {
            string credPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "credentials.json");
            return System.IO.File.Exists(credPath);
        }

        // ================================================================
        //  PRIVATE: Lấy DriveService – chỉ đăng nhập 1 lần
        // ================================================================
        private static DriveService GetService()
        {
            if (_service != null) return _service;
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
                using (var stream = new System.IO.FileStream(
                    credPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    string tokenPath = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, TokenFolder);
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes, "user",
                        CancellationToken.None,
                        new FileDataStore(tokenPath, true)).Result;
                }

                _service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName       = AppName,
                });
            }
            return _service;
        }

        // ================================================================
        //  PRIVATE: Đảm bảo thư mục phiên đã có (lazy init)
        // ================================================================
        private static void EnsureSessionFolder(DriveService svc)
        {
            if (!string.IsNullOrEmpty(_sessionFolderId)) return;
            _rootFolderId    = GetOrCreateFolder(svc, ROOT_FOLDER, null);
            string sessionName = "NhatKiViPham_" + DateTime.Now.ToString("HH-mm_dd-MM-yyyy");
            _sessionFolderId = GetOrCreateFolder(svc, sessionName, _rootFolderId);
        }

        // ================================================================
        //  PRIVATE: Tìm thư mục theo tên (trong parent), nếu chưa có thì tạo
        // ================================================================
        private static string GetOrCreateFolder(DriveService svc, string name, string parentId)
        {
            // Tìm thư mục đã tồn tại
            string query = $"mimeType='application/vnd.google-apps.folder' " +
                           $"and name='{name}' and trashed=false";
            if (!string.IsNullOrEmpty(parentId))
                query += $" and '{parentId}' in parents";

            var listReq = svc.Files.List();
            listReq.Q      = query;
            listReq.Fields = "files(id,name)";
            listReq.Spaces = "drive";
            var found = listReq.Execute().Files;
            if (found != null && found.Count > 0)
                return found[0].Id; // Đã có → trả về ID

            // Chưa có → tạo mới
            var folderMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name     = name,
                MimeType = "application/vnd.google-apps.folder"
            };
            if (!string.IsNullOrEmpty(parentId))
                folderMeta.Parents = new List<string> { parentId };

            var created = svc.Files.Create(folderMeta).Execute();
            return created.Id;
        }
    }
}
