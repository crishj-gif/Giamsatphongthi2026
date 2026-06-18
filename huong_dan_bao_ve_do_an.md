# 📘 Cẩm nang Bảo vệ Đồ án: Trọng tâm Lập trình C# & Database

Tài liệu này tập trung sâu vào kỹ thuật lập trình, tổ chức Code trong WinForms, tương tác với Cơ sở dữ liệu SQL Server (70%), và cách tích hợp thư viện AI vào một ứng dụng thực tế (30%). 

---

## PHẦN 1: TỔ CHỨC KIẾN TRÚC PHẦN MỀM (Architecture)
Hệ thống được thiết kế theo mô hình **Client-Server truyền thống**, kết hợp xử lý AI tại biên (Edge AI):
- **Cơ sở dữ liệu (SQL Server):** Quản lý qua 3 bảng chính: `Candidates`, `ViolationTypes`, `ViolationLogs`.
- **Kết nối Dữ liệu (ADO.NET):** Sử dụng các lớp `SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlDataAdapter` để tương tác. Đặc biệt **bảo mật SQL Injection** bằng cách luôn dùng Parameter (`@note`, `@logID`).

---

## PHẦN 2: PHÂN TÍCH CHUYÊN SÂU MỨC ĐỘ CODE (TRỌNG TÂM VẤN ĐÁP)

Dưới đây là phần "mổ xẻ" chi tiết 2 màn hình xương sống của phần mềm. Hãy bám sát vào các hàm (method) và thư viện (class) này để trả lời khi bị Giảng viên yêu cầu "mở code lên giải thích".

### A. MÀN HÌNH GIÁM SÁT (`frmGiamSat.cs` - Trái tim hệ thống)
Đây là màn hình xử lý đa luồng (Multi-threading) nặng nhất, kết hợp UI và xử lý hình ảnh thời gian thực.

#### 1. Lớp hỗ trợ (Class) `ViolationTracker` (Dòng 13)
- **Class này dùng làm gì?** Đây là bộ não "Trí nhớ ngắn hạn" của phần mềm. Nó cấp phát trên RAM một đối tượng cho từng thí sinh chứa: `ViolationStartTime` (Thời điểm bắt đầu quay ngang), `Frequency` (Số lần tiền án vi phạm), và `CurrentScore` (Điểm khả nghi hiện tại).
- **Triển khai ở đâu:** Được nạp vào biến từ điển `Dictionary<string, ViolationTracker> trackers` (Dòng 22).
- **Hiệu quả:** Thay vì dùng `if/else` đếm giây cứng nhắc (hardcode) và truy vấn SQL Server liên tục (gây nghẽn cổ chai I/O), Class này giúp lưu trữ trạng thái của thí sinh ngay trên RAM cục bộ (In-Memory Tracking). Truy xuất cực kỳ nhanh với độ phức tạp $O(1)$ của từ điển Hash Map.

#### 2. Hàm nghiệp vụ cốt lõi `VideoSource_NewFrame` (Dòng 325)
- **Nhiệm vụ:** Đây là Event Handler được kích hoạt liên tục (15-30 lần/giây) mỗi khi Camera có ảnh mới đưa về. Mọi logic AI đều chạy trong hàm này.
- **Code triển khai nổi bật:**
  - **Dòng 413:** `if (ratio < 0.35f)` - Logic toán học Hình học cơ sở. Đo khoảng cách (Tỉ lệ giữa mũi và 2 mắt). Nếu < 0.35 nghĩa là mặt lệch hẳn sang một bên -> Bắt lỗi "Quay ngang ngửa".
  - **Dòng 450:** `if (bestScore >= 0.363 && bestMatchId != -1)` - So sánh Vector 128 chiều bằng hàm Cosine Similarity của thuật toán SFace. Lấy ngưỡng 0.363 để chốt định danh danh tính.
  - **Dòng 494:** `double S = W * Math.Exp(lambda * t) + alpha * trk.Frequency;` - Nơi **Hàm Tăng Trưởng Mũ** được áp dụng. $t$ là khoảng thời gian (giây) tính bằng `(DateTime.Now - ViolationStartTime)`. Điểm số $S$ sẽ tăng vọt nếu hành vi giữ nguyên sau 3-4 giây. Ngưỡng chốt hạ để ghi DB là `S >= 20`.
- **Hiệu quả xử lý đa luồng:** Hàm này thuộc thư viện AForge.Video, tự động chạy trong **Background Worker Thread**. Điều này giúp Main UI Thread (giao diện kéo thả form) không bao giờ bị đơ (Not Responding) kể cả khi AI mất tới 50ms để xử lý một bức ảnh.

#### 3. Hàm nghiệp vụ `TriggerAutoRecord` (Dòng ~580)
- **Nhiệm vụ:** Bắt bằng chứng video.
- **Class sử dụng:** `VideoWriter` của bộ Emgu.CV (OpenCV).
- **Cách thức hoạt động:** Khi sinh viên vi phạm, hàm này bật biến `isRecordingAuto = true`. Lúc này vòng lặp ở **Dòng 561** sẽ bắt đầu ghi liên tục các ảnh tĩnh (Frame) nối vào nhau tạo thành 1 file MP4 dài đúng 75 frame (tương đương 5 giây).
- **Hiệu quả cực cao:** Thay vì ghi hình 24/7 khiến rác ổ cứng (nhược điểm của mọi camera truyền thống), hệ thống này sử dụng tư duy "Event-Triggered Recording" (Ghi hình kích hoạt bằng sự kiện). Giúp tiết kiệm 99% dung lượng ổ cứng cho nhà trường.

---

### B. MÀN HÌNH QUẢN LÝ VI PHẠM (`frmNhatKyViPham.cs` - Form khoe Kỹ Thuật)
Màn hình này cho thấy tư duy thiết kế phần mềm doanh nghiệp (Enterprise CRUD).

#### 1. Hàm nghiệp vụ `LoadData` (Dòng 183)
- **Nhiệm vụ:** Tải danh sách từ SQL lên DataGridView, cho phép Lọc (Filter) và Phân trang (Paging).
- **Class sử dụng:** ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`).
- **Triển khai Code ăn điểm:**
  - **Kỹ thuật Truy vấn Động (Dynamic Query - Dòng 188):** Khởi tạo `string where = "WHERE 1=1"`. Sau đó nối chuỗi `AND` dần dần nếu người dùng gõ tìm kiếm (`_keyword`) hoặc chọn bộ lọc ngày tháng.
  - **Kỹ thuật Phân trang Server-Side (Dòng 221):** Sử dụng câu lệnh T-SQL tiêu chuẩn `OFFSET {offset} ROWS FETCH NEXT {_pageSize} ROWS ONLY`. Kết hợp toán tử truyền tham số an toàn `@kw` để chống Hacker tấn công SQL Injection.
- **Hiệu quả:** Tuyệt đối an toàn trước Hacker. Server-Side Paging đảm bảo dù Database có 1 triệu hay 10 triệu bản ghi, RAM phần mềm cũng không bao giờ bị tràn (Memory Leak) vì máy tính chỉ kéo đúng 10 dòng dữ liệu tương ứng với Trang hiện tại hiển thị.

#### 2. Hàm nghiệp vụ `BtnXemXepHang_Click` (Dòng 68)
- **Nhiệm vụ:** Hiển thị Leaderboard Top 10 đối tượng vi phạm.
- **Cách sử dụng:** Sử dụng truy vấn gom nhóm `GROUP BY` thẳng trên Database.
- **Triển khai ở Dòng 76:** 
  `SELECT TOP 10 DoiTuong, COUNT(*) as SoLanViPham, (COUNT(*) * 10) as DiemHanhVi ... GROUP BY DoiTuong ORDER BY SoLanViPham DESC`
- **Hiệu quả:** Thay vì kéo hết dữ liệu về ứng dụng C# bằng vòng lặp `for/foreach` để đếm tần suất (cực kỳ tốn kém), việc "Đẩy" gánh nặng tính toán xuống Engine của SQL Server giúp hệ thống truy xuất kết quả ra bảng xếp hạng chỉ trong tích tắc 0.001 giây.

#### 3. Hàm nghiệp vụ `BtnXuatExcel_Click` (Dòng 564)
- **Nhiệm vụ:** Xuất báo cáo chứng cứ dưới dạng Excel.
- **Class sử dụng:** `XLWorkbook`, `IXLWorksheet` (Thư viện ClosedXML).
- **Cách hoạt động:** Mở `SaveFileDialog` lấy đường dẫn của người dùng. Sau đó khởi tạo WorkBook (File rỗng) và duyệt vòng lặp qua từng dòng của lưới `dtMain.Rows` để ghi giá trị Cell vào.
- **Hiệu quả:** Không sử dụng thư viện `Microsoft.Office.Interop.Excel` cũ kỹ (yêu cầu máy nhà trường phải cài sẵn MS Excel mới chạy được). `ClosedXML` ghi thẳng file nhị phân `.xlsx` ở tầng thấp nên xuất file cực nhanh, hoạt động hoàn hảo trên các máy tính trắng tinh chưa cài Office.

---

## PHẦN 3: BỘ CÂU HỎI VẤN ĐÁP KỸ THUẬT NÊN HỌC THUỘC

**❓ Câu 1: Tại sao form Nhật Ký Vi Phạm không dùng DataSet/DataAdapter kéo hết dữ liệu về DataGridView mà phải viết câu lệnh SQL phức tạp có OFFSET?**
> **Trả lời:** Em sử dụng kỹ thuật Phân trang dưới Database (Server-Side Paging). Nếu phòng thi lưu 10.000 dòng log, việc kéo hết dữ liệu về RAM sẽ làm tràn bộ nhớ và treo phần mềm (Lag UI). Việc viết SQL có `OFFSET ... FETCH NEXT ... ROWS ONLY` ép SQL Server chỉ trả về đúng 10 dòng (1 page) để hiển thị, giúp phần mềm chạy mượt mà ngay cả khi database phình to.

**❓ Câu 2: Giải thích cách em chống SQL Injection trong phần mềm của em?**
> **Trả lời:** Xuyên suốt mã nguồn, em tuyệt đối KHÔNG nối chuỗi String trực tiếp từ `TextBox` vào câu lệnh SQL. Em sử dụng đối tượng `SqlCommand` và truyền Data qua Parameters (Ví dụ: `cmd.Parameters.AddWithValue("@kw", "%" + _keyword + "%")`). Việc này giao phó cho Engine ADO.NET tự động mã hóa chuỗi, vô hiệu hóa hoàn toàn các ký tự độc hại như dấu nháy đơn `'` hay mã rác `OR 1=1`.

**❓ Câu 3: Làm sao em phân biệt được sinh viên mỏi cổ quay 1 giây với sinh viên quay 3 giây để nhìn bài bạn?**
> **Trả lời:** Hệ thống của em không đếm giây bằng `if/else` mà xây dựng một Class `ViolationTracker` lưu trên RAM và áp dụng **Mô hình tính điểm khả nghi** bằng Hàm tăng trưởng Mũ ($y = e^x$). Nếu sinh viên chỉ ngoái nhìn 1 giây, hàm sẽ sinh ra điểm cực thấp. Nhưng ở giây thứ 3, điểm số sẽ nhân lên đột biến theo đường cong mũ, chạm trần 20 điểm thì Còi sẽ báo. Hơn nữa, những người từng có lịch sử "Tái phạm" sẽ bị áp dụng thêm Hệ số phạt $\alpha$ khiến điểm chạm trần cực nhanh chỉ sau nửa giây.

**❓ Câu 4: Nhược điểm của phần mềm em hiện tại là gì?**
> **Trả lời:** Nhược điểm lớn nhất là việc bắt "Sử dụng điện thoại" vẫn đang sử dụng Haar Cascade (Machine Learning cổ điển). Thuật toán này quét ảnh bằng trượt cửa sổ (Sliding Window) khá chậm và dễ nhận diện nhầm các vật hình chữ nhật (như máy tính cầm tay Casio) thành điện thoại. Hướng phát triển tương lai là em sẽ thay thế Haar Cascade bằng một mô hình Deep Learning YOLOv8 Nano siêu nhẹ để chuyên bắt vật thể chính xác hơn.
