using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    public class DbConnection
    {
        // Sử dụng connection string cho SQL Server
        // Server: .\SQLEXPRESS (Mặc định thường dùng) hoặc Localhost\SQLEXPRESS
        // Database: DB_GiamSatPhongThi
        // Integrated Security=True (Windows Authentication)
        private string connectionString = "Server=.\\SQLEXPRESS;Database=ExamMonitoringDB;Integrated Security=True;TrustServerCertificate=True;";

        private SqlConnection connection;

        public DbConnection()
        {
            connection = new SqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
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

        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        // Hàm kiểm tra đăng nhập
        public bool CheckLogin(string username, string password)
        {
            bool isValid = false;
            // Sử dụng bảng SystemUsers theo Database mới
            string query = "SELECT Count(*) FROM SystemUsers WHERE Username = @user AND PasswordHash = @pass";

            if (this.OpenConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0) isValid = true; // Tìm thấy user
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi truy vấn: " + ex.Message);
                }
                finally
                {
                    this.CloseConnection();
                }
            }
            return isValid;
        }
        // Hàm lấy danh sách thí sinh từ bảng Candidates
        public DataTable LoadDanhSachThiSinh()
        {
            DataTable dt = new DataTable();
            // Lấy dữ liệu từ bảng Candidates và đổi tên cột để hiển thị đẹp rạng rỡ trên màn hình
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