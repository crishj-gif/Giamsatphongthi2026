using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    /// <summary>
    /// Lớp DbConnection: Quản lý kết nối cơ sở dữ liệu SQL Server.
    /// Chứa các phương thức mở, đóng kết nối và thực thi các câu lệnh truy vấn cơ bản.
    /// </summary>
    public class DbConnection
    {
        // Sử dụng connection string cho SQL Server
        // Server: .\SQLEXPRESS (Mặc định thường dùng) hoặc Localhost\SQLEXPRESS
        // Database: ExamMonitoringDB
        // Integrated Security=True (Windows Authentication - Xác thực bằng tài khoản Windows hiện tại)
        // TrustServerCertificate=True (Bỏ qua kiểm tra chứng chỉ SSL của server)
        private string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";

        private SqlConnection connection;

        /// <summary>
        /// Khởi tạo một đối tượng kết nối mới với chuỗi kết nối đã định nghĩa.
        /// </summary>
        public DbConnection()
        {
            connection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// Mở kết nối đến cơ sở dữ liệu.
        /// </summary>
        /// <returns>Trả về true nếu mở thành công, ngược lại trả về false (và hiện thông báo lỗi).</returns>
        public bool OpenConnection()
        {
            try
            {
                // Chỉ mở khi trạng thái kết nối đang đóng
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không kết nối được Database SQL Server!\nHãy kiểm tra lại:\n1. Server name đã đúng chưa (thử dùng .\\SQLEXPRESS)?\n2. Database 'ExamMonitoringDB' đã có chưa?\n\nLỗi chi tiết: " + ex.Message, "Lỗi Kết Nối Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Đóng kết nối cơ sở dữ liệu nếu đang mở.
        /// </summary>
        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        /// <summary>
        /// Kiểm tra thông tin đăng nhập của người dùng.
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu (đã được băm/hash hoặc text tùy hệ thống)</param>
        /// <returns>Trả về true nếu tài khoản và mật khẩu khớp trong DB, false nếu sai.</returns>
        public bool CheckLogin(string username, string password)
        {
            bool isValid = false;
            // Sử dụng bảng SystemUsers theo Database mới. Tham số hóa (@user, @pass) để tránh SQL Injection.
            string query = "SELECT Count(*) FROM SystemUsers WHERE Username = @user AND PasswordHash = @pass";

            if (this.OpenConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    // ExecuteScalar trả về giá trị của cột đầu tiên của dòng đầu tiên trong kết quả truy vấn.
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0) isValid = true; // Tìm thấy user
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi truy vấn: " + ex.Message);
                }
                finally
                {
                    // Luôn luôn đóng kết nối trong khối finally để đảm bảo không bị rò rỉ tài nguyên
                    this.CloseConnection();
                }
            }
            return isValid;
        }
        
        /// <summary>
        /// Tải danh sách thí sinh từ cơ sở dữ liệu để hiển thị lên DataGridView.
        /// </summary>
        /// <returns>Đối tượng DataTable chứa dữ liệu danh sách thí sinh.</returns>
        public DataTable LoadDanhSachThiSinh()
        {
            DataTable dt = new DataTable();
            // Lấy dữ liệu từ bảng Candidates và đổi tên cột để hiển thị đẹp rạng rỡ trên màn hình
            // Sử dụng câu lệnh CASE WHEN để chuyển đổi dữ liệu ảnh thành chuỗi trạng thái trực quan
            string query = @"
                SELECT 
                    StudentCode AS N'Mã Thí Sinh',
                    FullName AS N'Họ Tên',
                    Class AS N'Lớp',
                    CASE WHEN FaceImageData IS NOT NULL THEN N'Đã có ảnh DB' ELSE '' END AS N'Trạng thái'
                FROM Candidates
            ";

            if (this.OpenConnection())
            {
                try
                {
                    // SqlDataAdapter đóng vai trò như cầu nối lấy dữ liệu từ DB đổ vào DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lấy danh sách thí sinh: " + ex.Message);
                }
                finally
                {
                    this.CloseConnection();
                }
            }
            return dt;
        }
    }
}