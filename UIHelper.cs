using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    /// <summary>
    /// Lớp tiện ích (Helper) UIHelper chứa các hàm hỗ trợ việc tùy biến giao diện.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Làm bo tròn các góc của một Control (Button, Panel, Form...).
        /// </summary>
        /// <param name="control">Control cần được bo góc.</param>
        /// <param name="radius">Bán kính bo góc (pixel).</param>
        public static void BoTronControl(Control control, int radius)
        {
            // Định nghĩa một Event Handler sẽ chạy mỗi khi Control bị thay đổi kích thước (Resize).
            EventHandler resizeHandler = (s, e) =>
            {
                // Bỏ qua nếu Control chưa có kích thước thực tế.
                if (control.Width == 0 || control.Height == 0) return;
                
                Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
                GraphicsPath path = new GraphicsPath();
                int arcWidth = radius * 2;
                
                // Tránh lỗi ném ra ngoại lệ (exception) khi Control bị thu nhỏ hơn kích thước vòng cung bo góc.
                // Nếu arcWidth lớn hơn cạnh của Control thì ta gán bằng cạnh của Control.
                if (arcWidth > control.Width) arcWidth = control.Width;
                if (arcWidth > control.Height) arcWidth = control.Height;

                // Vẽ 4 góc bo tròn bằng cách sử dụng AddArc (vẽ cung tròn).
                // Góc trên bên trái (Góc phần tư thứ 2, từ góc 180 độ quét 90 độ)
                path.AddArc(rect.X, rect.Y, arcWidth, arcWidth, 180, 90);
                // Góc trên bên phải
                path.AddArc(rect.X + rect.Width - arcWidth, rect.Y, arcWidth, arcWidth, 270, 90);
                // Góc dưới bên phải
                path.AddArc(rect.X + rect.Width - arcWidth, rect.Y + rect.Height - arcWidth, arcWidth, arcWidth, 0, 90);
                // Góc dưới bên trái
                path.AddArc(rect.X, rect.Y + rect.Height - arcWidth, arcWidth, arcWidth, 90, 90);
                
                // Đóng đường viền lại thành một hình khép kín
                path.CloseFigure();
                
                // Thay đổi vùng hiển thị (Region) của Control thành hình dạng vừa vẽ (đã được bo góc).
                // Các phần thừa bên ngoài của Control sẽ bị cắt bỏ (không vẽ ra).
                control.Region = new Region(path);
            };

            // Gán sự kiện Resize
            control.Resize += resizeHandler;
            
            // Gọi chạy ngay lần đầu tiên để áp dụng bo góc mà không cần đợi Resize.
            resizeHandler(control, EventArgs.Empty);
        }
    }
}
