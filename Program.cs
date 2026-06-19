namespace GiamSatPhongThi;

/// <summary>
/// Lớp Program: Điểm bắt đầu (entry point) của toàn bộ ứng dụng Windows Forms.
/// Lớp này chứa phương thức Main(), là hàm đầu tiên được gọi khi chương trình chạy.
/// </summary>
static class Program
{
    /// <summary>
    ///  Điểm bắt đầu chính của ứng dụng.
    ///  [STAThread] chỉ định rằng mô hình luồng COM cho ứng dụng này là Single-Threaded Apartment (STA),
    ///  điều này bắt buộc đối với các ứng dụng Windows Forms có sử dụng giao diện người dùng (UI).
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Khởi tạo các cấu hình mặc định cho Windows Forms ứng dụng (như Visual Styles, rendering).
        // Đây là cú pháp mới từ .NET 6 trở đi.
        ApplicationConfiguration.Initialize();
        
        // Khởi động vòng lặp tin nhắn (message loop) của ứng dụng và hiển thị form chính (MainForm).
        // Vòng lặp này giữ cho chương trình chạy và phản hồi các sự kiện từ người dùng (click chuột, gõ phím...).
        Application.Run(new MainForm());
    }
}