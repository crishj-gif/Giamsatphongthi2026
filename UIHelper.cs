using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GiamSatPhongThi
{
    public static class UIHelper
    {
        public static void BoTronControl(Control control, int radius)
        {
            EventHandler resizeHandler = (s, e) =>
            {
                if (control.Width == 0 || control.Height == 0) return;
                Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
                GraphicsPath path = new GraphicsPath();
                int arcWidth = radius * 2;
                
                // Tránh lỗi khi Control bị thu nhỏ hơn kích thước Bo góc
                if (arcWidth > control.Width) arcWidth = control.Width;
                if (arcWidth > control.Height) arcWidth = control.Height;

                path.AddArc(rect.X, rect.Y, arcWidth, arcWidth, 180, 90);
                path.AddArc(rect.X + rect.Width - arcWidth, rect.Y, arcWidth, arcWidth, 270, 90);
                path.AddArc(rect.X + rect.Width - arcWidth, rect.Y + rect.Height - arcWidth, arcWidth, arcWidth, 0, 90);
                path.AddArc(rect.X, rect.Y + rect.Height - arcWidth, arcWidth, arcWidth, 90, 90);
                path.CloseFigure();
                
                control.Region = new Region(path);
            };

            control.Resize += resizeHandler;
            
            // Gọi chạy ngay lần đầu
            resizeHandler(control, EventArgs.Empty);
        }
    }
}
