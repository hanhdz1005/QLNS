# HƯỚNG DẪN CÀI ĐẶT VÀ SỬ DỤNG HỆ THỐNG QUẢN LÝ NHÂN SỰ (QLNS - HRMS PRO)

Tài liệu này hướng dẫn chi tiết cách cài đặt môi trường để chạy dự án và cách sử dụng đầy đủ toàn bộ các chức năng trong hệ thống QLNS (HRMS Pro).

---

## PHẦN 1: HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY DỰ ÁN (PROJECT SETUP)

### 1. Yêu cầu môi trường cài đặt
- Hệ điều hành Windows 10/11 hoặc macOS/Linux.
- Bộ công cụ phát triển Backend: **.NET 8.0 SDK** trở lên (có thể dùng Visual Studio 2022 hoặc VS Code).
- Hệ quản trị cơ sở dữ liệu: **Microsoft SQL Server** (bản Developer, Express hoặc LocalDB) và công cụ SQL Server Management Studio (SSMS).
- Bộ công cụ phát triển Frontend: **Node.js phiên bản 20 LTS hoặc 22 LTS** trở lên (đi kèm trình quản lý gói `npm`).
- Thiết bị có gắn Webcam (hoặc Camera laptop) để sử dụng chức năng chấm công nhận diện khuôn mặt Real-time trên trình duyệt Chrome/Edge.

### 2. Các bước cài đặt và chạy Backend (`QLNS.API`)
- **Bước 1 — Cấu hình chuỗi kết nối SQL Server**: Mở file `QLNS/QLNS.API/QLNS.API/appsettings.json`, tìm đến mục `ConnectionStrings.DefaultConnection` và chỉnh sửa lại tên `Server=...` cho khớp với tên SQL Server trên máy của bạn (ví dụ `Server=localhost;Database=QLNS_DB;Trusted_Connection=True;TrustServerCertificate=True;`).
- **Bước 2 — Khởi tạo Database (nếu máy chưa có DB)**: Mở terminal tại thư mục `QLNS/QLNS.API` và chạy lệnh cập nhật migration của Entity Framework Core:
  ```bash
  cd QLNS/QLNS.API
  dotnet ef database update --project QLNS.Infrastructure --startup-project QLNS.API
  ```
- **Bước 3 — Chạy dự án Backend**: Di chuyển vào thư mục `QLNS.API` và chạy ứng dụng:
  ```bash
  cd QLNS.API
  dotnet run
  ```
- **Lưu ý tự động khởi tạo dữ liệu**: Ngay khi lệnh `dotnet run` khởi chạy, hệ thống sẽ tự động thực thi `IdentitySeed.EnsureExtendedSchemaAsync` và `IdentitySeed.SeedRoleAsync` để tự động bổ sung các bảng/cột mở rộng (như bảng đơn làm thêm giờ `OVERTIME_REQUESTS`, cột `RegularHours`, `OvertimeHours`, `ScanCount`, `FaceEmbedding`) và tự động tạo sẵn 6 tài khoản mẫu tương ứng với 6 cấp Role.

### 3. Các bước cài đặt và chạy Frontend (`QLNS.FE`)
- **Bước 1 — Cài đặt thư viện**: Mở một cửa sổ terminal mới, di chuyển vào thư mục `QLNS/QLNS.FE` và cài đặt các gói phụ thuộc:
  ```bash
  cd QLNS/QLNS.FE
  npm install
  ```
- **Bước 2 — Kiểm tra địa chỉ API**: Mặc định Frontend kết nối tới Backend qua địa chỉ cấu hình trong `src/api/axiosClient.ts` (hoặc biến môi trường `VITE_API_BASE_URL`). Đảm bảo cổng порт của Backend khớp với địa chỉ gọi API.
- **Bước 3 — Khởi chạy giao diện phát triển**:
  ```bash
  npm run dev
  ```
- Sau khi chạy lệnh trên, mở trình duyệt tại địa chỉ hiển thị trên terminal (thường là `http://localhost:5173`) để bắt đầu sử dụng hệ thống. Nếu muốn kiểm tra lỗi kiểu dữ liệu hoặc đóng gói bản Production, có thể chạy `npm run type-check` và `npm run build-only`.

---

## PHẦN 2: DANH SÁCH 6 TÀI KHOẢN PHÂN QUYỀN MẪU (MẬT KHẨU CHUNG: `Admin@1234`)

Hệ thống được phân thành 6 tầng vai trò (Role) từ cao xuống thấp và đã được khởi tạo sẵn các tài khoản sau để kiểm thử:
- **Tầng 1 — Quản trị viên hệ thống (`Admin`)**: Đăng nhập bằng email `admin@gmail.com` (mật khẩu `Admin@1234`). Tài khoản này có toàn quyền quản trị hệ thống, cấu hình chính sách công ty, gán Role cho người dùng và truy cập mọi phân hệ.
- **Tầng 2 — Trưởng phòng Nhân sự (`HRManager`)**: Đăng nhập bằng email `hrmanager@gmail.com` (mật khẩu `Admin@1234`). Tài khoản này quản lý toàn bộ nhân sự, phòng ban, chức vụ, hợp đồng, khen thưởng/kỷ luật, cấu hình chính sách OT/công ty, duyệt đơn nghỉ phép, duyệt đơn OT, xem vòng đời toàn công ty và xem bảng lương.
- **Tầng 3 — Chuyên viên Nhân sự (`HRStaff`)**: Đăng nhập bằng email `hrstaff@gmail.com` (mật khẩu `Admin@1234`). Tài khoản này thực hiện nghiệp vụ hồ sơ nhân viên, phòng ban, chức vụ, hợp đồng, thưởng/phạt, theo dõi chấm công và xem vòng đời nhân sự toàn công ty.
- **Tầng 4 — Kế toán tiền lương (`Accountant`)**: Đăng nhập bằng email `accountant@gmail.com` (mật khẩu `Admin@1234`). Tài khoản này chuyên trách cấu hình bảng lương, chạy tính lương tự động hàng tháng (bao gồm tính lương OT, BHXH, thuế TNCN), quản lý thưởng/phạt và xuất báo cáo quỹ lương.
- **Tầng 5 — Trưởng bộ phận (`DepartmentManager`)**: Đăng nhập bằng email `manager@gmail.com` (mật khẩu `Admin@1234`). Tài khoản này có quyền phê duyệt hoặc từ chối đơn xin nghỉ phép và đơn đăng ký làm thêm giờ (OT) của nhân viên, đồng thời theo dõi danh sách nhân sự, bảng chấm công và vòng đời làm việc của nhân viên.
- **Tầng 6 — Nhân viên (`Employee`)**: Đăng nhập bằng email `employee@gmail.com` (mật khẩu `Admin@1234`). Tài khoản này dùng để quét khuôn mặt chấm công, xem lịch sử chấm công cá nhân, gửi đơn xin nghỉ phép, gửi đơn đăng ký giờ làm thêm (OT), tra cứu phiếu lương cá nhân và xem vòng đời làm việc của chính mình.

---

## PHẦN 3: HƯỚNG DẪN SỬ DỤNG CHI TIẾT TỪNG CHỨC NĂNG TRONG HỆ THỐNG

### 1. Chức năng Đăng nhập, Hồ sơ cá nhân, Đổi mật khẩu và Quét khuôn mặt lần đầu (`/login`, `/profile`)
- Tại màn hình đăng nhập (`/login`), người dùng nhập email và mật khẩu hoặc bấm trực tiếp vào các nút chọn nhanh tài khoản mẫu tương ứng với từng Role để đăng nhập nhanh.
- Ngay sau khi đăng nhập, nếu tài khoản của nhân viên chưa từng đăng ký mẫu sinh trắc học khuôn mặt (`HasRegisteredFace = false`), hệ thống sẽ tự động bật cửa sổ bắt buộc **Quét khuôn mặt lần đầu**. Người dùng chỉ cần cho phép trình duyệt truy cập Camera và giữ khuôn mặt thẳng trong khung elip khoảng 1 giây (không cần bấm nút chụp), thanh tiến trình sẽ tự động chạy từ `0%` lên `100%` và lưu mẫu vector khuôn mặt 128 chiều vào cơ sở dữ liệu để dùng làm gốc đối chiếu cho tất cả các lần chấm công sau.
- Tại trang **Hồ sơ cá nhân (`/profile`)**, người dùng có thể xem thông tin cá nhân, cập nhật lại mẫu khuôn mặt mới bất cứ lúc nào bằng nút **Cập nhật khuôn mặt**, hoặc thực hiện đổi mật khẩu tài khoản.
- Trên thanh Header góc phải trên cùng, người dùng có thể chuyển đổi ngôn ngữ hiển thị giữa **Tiếng Việt (`VI`)** và **Tiếng Anh (`EN`)** theo thời gian thực.

### 2. Chức năng Tổng quan hệ thống - Dashboard (`/dashboard`)
- Khi truy cập trang chủ Dashboard, hệ thống hiển thị các thẻ chỉ số thống kê tổng quan theo thời gian thực gồm tổng số nhân sự, số nhân sự đang làm việc, tổng số phòng ban, số đơn từ đang chờ duyệt, số lượt chấm công trong tháng, tổng quỹ lương thực chi và mức lương trung bình.
- Khu vực biểu đồ bên dưới thể hiện tỷ trọng phân bổ nhân sự theo từng phòng ban giúp người quản lý nắm bắt quy mô tổ chức.
- Khu vực **Cảnh báo nhân sự (HR Alerts)** tự động liệt kê danh sách các hợp đồng lao động sắp hết hạn trong vòng 30 ngày, danh sách nhân viên sắp kết thúc thời gian thử việc và danh sách nhân viên có sinh nhật trong tháng hiện tại.

### 3. Chức năng Cài đặt Chính sách Công ty, Cấu hình Lương OT, Hệ số OT và Giờ làm việc (`/settings`)
- Truy cập menu **Cài đặt (`/settings`)** (dành cho `Admin` và `HRManager` chỉnh sửa; các Role khác có thể xem chính sách đang áp dụng), chức năng này được chia làm 3 tab cấu hình chính sách nghiệp vụ:
- **Tab 1 — Cấu hình Chính sách Làm thêm giờ & Lương OT (`Chính sách OT`)**:
  - Cho phép chọn **Chế độ đãi ngộ OT mặc định của công ty** gồm 3 hình thức: **Quy đổi Nghỉ bù (`CompensatoryLeave`)**, **Ghi nhận cống hiến / KPI (`RecognitionOnly`)**, hoặc **Tính thành Tiền lương OT (`Paid`)**.
  - Cung cấp 2 công tắc phân quyền linh hoạt: Bật/tắt việc **cho phép nhân viên tự chọn hình thức đãi ngộ OT** khi tạo đơn đăng ký (nếu tắt, đơn OT của nhân viên sẽ khóa cứng theo chế độ mặc định của công ty) và bật/tắt việc **cho phép người duyệt (`DepartmentManager` / `HRManager` / `Admin`) chuyển đổi hình thức đãi ngộ OT** (ví dụ chuyển từ nghỉ bù sang trả tiền lương OT) ngay lúc duyệt đơn.
  - Cho phép cài đặt các định mức và **hệ số nhân lương OT (`Multiplier`)** gồm: Giới hạn số giờ OT tối đa trong 1 tháng (mặc định `40h`), số giờ OT quy đổi thành 1 ngày nghỉ bù (mặc định `8h`), **Hệ số lương OT ngày thường (`Weekday Multiplier`, mặc định `1.5x` tương ứng 150%)**, **Hệ số lương OT cuối tuần (`Weekend Multiplier`, mặc định `2.0x` tương ứng 200%)**, và **Hệ số lương OT ngày Lễ/Tết (`Holiday Multiplier`, mặc định `3.0x` tương ứng 300%)**.
- **Tab 2 — Cài đặt Giờ làm việc & Chấm công (`Giờ làm & Chấm công`)**: Cho phép thiết lập giờ bắt đầu/kết thúc ca sáng (mặc định `08:00 - 12:00`), ca chiều (mặc định `13:00 - 17:30`), mốc giờ ân hạn đi muộn buổi sáng (mặc định `08:15`), số ngày công chuẩn trong tháng (mặc định `22` hoặc `26` ngày) và tùy chọn có nghỉ ngày Thứ Bảy hay không.
- **Tab 3 — Cài đặt Chính sách Tính lương & Nghỉ phép (`Lương & Nghỉ phép`)**: Cho phép cài đặt số ngày phép năm mặc định của nhân viên (mặc định `12` ngày/năm) và bật/tắt mặc định 3 tùy chọn khi chạy bảng lương: **Tự động cộng tiền lương OT vào bảng lương**, **Tự động khấu trừ Bảo hiểm bắt buộc (10.5%)**, và **Tự động khấu trừ Thuế TNCN lũy tiến**. Sau khi điều chỉnh, bấm **Lưu cài đặt** (hoặc **Khôi phục mặc định**).

### 4. Chức năng Chấm công Khuôn mặt Real-Time Tự động (`/attendance`)
- Truy cập menu **Chấm công (`/attendance`)**, bấm nút **Bật Camera** tại khu vực máy quét khuôn mặt thời gian thực.
- **Cơ chế quét tự động không cần bấm chụp**: Khi nhân viên đứng trước camera, hệ thống tự động phát hiện khuôn mặt trong khung ngắm elip, tự động khóa mục tiêu từ `0%` đến `100%` trong khoảng 1 giây và gửi dữ liệu lên Server đối sánh sinh trắc học. Sau khi nhận diện thành công, bảng kết quả bên phải hiển thị ngay họ tên, mã nhân viên, độ khớp khuôn mặt (%), giờ quét đầu, giờ quét cuối, tổng số lần quét trong ngày, giờ công hành chính và giờ OT đã duyệt, sau đó tự động mở khóa sau 4 giây để người tiếp theo vào quét.
- **Cách hệ thống xử lý khi quét nhiều lần trong cùng 1 ngày**:
  - Lần quét đầu tiên trong ngày được hệ thống ghi nhận làm **Giờ vào đầu (`CheckIn`)**.
  - Từ lần quét thứ 2, thứ 3 trở đi trong cùng ngày, hệ thống giữ nguyên **Giờ vào đầu (`CheckIn`)**, cập nhật thời점 quét mới nhất làm **Giờ ra cuối (`CheckOut`)**, đồng thời tăng bộ đếm **Lượt quét (`#ScanCount`)**.
  - **Cách tính Giờ công hành chính (`RegularHours`)**: Hệ thống chỉ tính phần thời gian nằm trong khung giờ làm việc hành chính chuẩn (`08:00 – 12:00` và `13:00 – 17:30`) giữa lần quét đầu và lần quét cuối, tối đa **8.0 giờ/ngày**. Dù nhân viên quét sớm trước 08:00 hay quét muộn sau 17:30 nhưng không có đơn OT được duyệt thì giờ công hành chính vẫn chỉ tính tối đa 8.0 giờ.
  - **Cách cộng Giờ làm thêm (`OvertimeHours`)**: Hệ thống kiểm tra xem ngày hôm đó nhân viên có đơn đăng ký OT nào đã được phê duyệt (`Approved`) hay không; nếu có, hệ thống chỉ cộng thêm **đúng bằng số giờ OT đã đăng ký và được duyệt** vào cột giờ OT của ngày công đó.
- Ngoài quét tự động, cấp quản lý (`Admin`, `HRManager`, `HRStaff`) còn có thể tra cứu bảng nhật ký chấm công toàn công ty theo bộ lọc ngày/trạng thái hoặc thêm/chỉnh sửa bản ghi chấm công khi cần thiết; nhân viên thường xem lịch sử chấm công của riêng mình ở bảng bên dưới.

### 5. Chức năng Quản lý Nghỉ phép, Đăng ký giờ OT và Phê duyệt OT (`/leaves`)
- **Quản lý Nghỉ phép & Quỹ phép năm**:
  - Ở phần trên của trang `/leaves`, nhân viên xem được các thẻ thống kê quỹ phép năm của mình (tổng ngày phép được cấp, số ngày đã nghỉ, số ngày đang chờ duyệt và số ngày phép còn lại).
  - Để xin nghỉ phép, nhân viên bấm nút **Tạo đơn nghỉ phép**, chọn loại nghỉ phép (Phép năm, Nghỉ ốm, Nghỉ việc riêng, Nghỉ không lương...), chọn từ ngày đến ngày, nhập lý do và gửi đơn.
  - Cấp quản lý (`DepartmentManager`, `HRManager`, `Admin`) chuyển sang tab **Duyệt đơn nghỉ phép** để bấm **Duyệt** hoặc **Từ chối** đơn của nhân viên. Khi đơn được duyệt, hệ thống tự động trừ quỹ phép và tính ngày nghỉ có lương vào bảng lương tháng. Ngoài ra HR/Admin có thể bấm nút **Cấu hình quỹ phép** để cấp hoặc điều chỉnh số ngày phép năm cho từng nhân viên.
- **Đăng ký làm thêm giờ (OT) dành cho Nhân viên**:
  - Kéo xuống khu vực **Quản lý Làm thêm giờ (Overtime - OT)** trên trang `/leaves` và bấm nút **Đăng ký OT**.
  - Trong cửa sổ đăng ký, nhân viên chọn **Ngày làm thêm**, chọn **Loại ngày OT** (Ngày thường, Cuối tuần, hoặc Ngày Lễ/Tết — hệ thống cũng tự động nhận diện nếu ngày chọn rơi vào cuối tuần), hoặc bấm các nút chọn nhanh ca OT mẫu (Tối 2h `17:30-19:30`, Tối 3h `18:00-21:00`, Nửa ngày cuối tuần `4h`, Cả ngày cuối tuần `8h`).
  - Nhập **Giờ bắt đầu**, **Giờ kết thúc** và **Số giờ OT đăng ký (`RegisteredHours`)**: Hệ thống tự động tính số giờ gợi ý từ khoảng thời gian bắt đầu – kết thúc và cho phép nhân viên nhập chính xác số giờ OT đăng ký.
  - Chọn **Hình thức đãi ngộ OT** (Nghỉ bù, Ghi nhận KPI, hoặc Tính tiền lương OT theo hệ số `1.5x / 2.0x / 3.0x` tùy theo chính sách công ty đã mở hay khóa ở mục Cài đặt), xem trước quyền lợi quy đổi ngay trên form, nhập lý do tăng ca rồi bấm **Gửi đơn đăng ký OT**.
- **Phê duyệt và Điều chỉnh đơn OT dành cho `DepartmentManager`, `HRManager`, `Admin`**:
  - Người duyệt chuyển sang tab **Đơn OT chờ duyệt** trong khu vực Overtime để xem danh sách các đơn đăng ký OT của nhân viên.
  - Bấm nút **Xem xét / Duyệt** trên dòng đơn OT để mở cửa sổ phê duyệt chi tiết (`OvertimeReviewModal`). Tại đây, hệ thống hiển thị đối chiếu trực tiếp dữ liệu **Giờ quét khuôn mặt thực tế (`CheckIn -> CheckOut`)** của nhân viên trong ngày đó để người quản lý kiểm chứng.
  - Người duyệt có thể giữ nguyên hoặc điều chỉnh lại **Số giờ OT phê duyệt**, **Loại ngày OT**, và **Hình thức đãi ngộ OT** (ví dụ quyết định tính thành **Tiền lương OT** hay **Nghỉ bù** nếu chính sách công ty cho phép người duyệt ghi đè), nhập ghi chú phê duyệt rồi bấm **Phê duyệt OT** (hoặc **Từ chối**).
  - Ngay khi bấm **Phê duyệt OT**, hệ thống tự động cập nhật đúng số giờ OT đã duyệt vào bảng chấm công của nhân viên trong ngày đó; nếu đơn OT thuộc diện **Tính tiền lương OT (`Paid`)**, hệ thống sẽ tự động nhân với hệ số lương OT tương ứng (`1.5x`, `2.0x`, `3.0x`) khi chạy bảng lương cuối tháng.

### 6. Chức năng Vòng đời làm việc của nhân viên (`/lifecycle` và `/lifecycle/:id`)
- Truy cập menu **Vòng đời nhân sự (`/lifecycle`)**:
  - Nếu đăng nhập bằng tài khoản **Nhân viên thường (`Employee`, `Accountant`)**, trang sẽ hiển thị trực tiếp **Vòng đời làm việc của chính người dùng đó**, tuyệt đối không xem được thông tin của người khác.
  - Nếu đăng nhập bằng tài khoản **Quản lý (`Admin`, `HRManager`, `HRStaff`, `DepartmentManager`)**, trang sẽ hiển thị **Danh sách tổng quan vòng đời của toàn bộ nhân viên trong công ty** kèm bộ lọc tìm kiếm theo tên/mã nhân viên, lọc theo phòng ban và thanh tiến độ tóm tắt 3 giai đoạn của từng người. Khi bấm nút **Xem chi tiết** ở bất kỳ nhân viên nào, hệ thống sẽ chuyển sang trang chi tiết `/lifecycle/:id` để hiển thị toàn bộ thông tin chuyên sâu của nhân viên đó.
- **Tự động cập nhật chỉ số theo tháng**:
  - Trên góc phải của Banner hồ sơ nhân viên có bộ chọn **Tháng** và **Năm**. Khi người dùng thay đổi tháng hoặc năm, toàn bộ số tháng thâm niên, thời gian từng giai đoạn, chỉ số KPI chấm công/lương và các biểu đồ bên dưới sẽ tự động tính toán lại tính đến thời điểm cuối tháng được chọn.
- **Các nội dung hiển thị trên trang Vòng đời làm việc**:
  - **Phân tách 3 giai đoạn làm việc**: Hiển thị 3 thẻ liệt kê rõ thời gian **Thực tập (`Internship`)**, **Thử việc (`Probation`)** và **Làm chính thức (`Official`)** của nhân viên tại công ty (bao gồm số tháng làm việc ở từng giai đoạn, ngày bắt đầu, ngày kết thúc, số lượng hợp đồng và trạng thái giai đoạn hiện tại).
  - **Hệ thống 6 thẻ chỉ số KPI**: Hiển thị Tổng thâm niên làm việc (tháng), Giờ công hành chính trong tháng đang chọn, Giờ OT đã duyệt trong tháng đang chọn, Tỷ lệ đi làm đúng giờ (%), Lương thực lĩnh (Net) của tháng đang chọn và Tổng số hợp đồng đã ký.
  - **Hệ thống 4 biểu đồ tượng trưng (SVG Symbolic Charts)**: Bao gồm biểu đồ thanh ngang thể hiện tỷ trọng thời gian giữa Thực tập – Thử việc – Chính thức; biểu đồ đường thể hiện xu hướng thu nhập và thâm niên trong 6 tháng gần nhất; biểu đồ cột chồng so sánh Giờ công hành chính và Giờ OT đã duyệt qua 6 tháng; và biểu đồ vòng tròn (Donut) thể hiện tỷ lệ đi làm đúng giờ trong tháng.
  - **Trục thời gian sự nghiệp (Career Timeline)**: Liệt kê theo trình tự thời gian từ lúc nhân viên gia nhập công ty, các mốc ký hợp đồng Thực tập, Thử việc, Chính thức cho đến các quyết định Khen thưởng và Kỷ luật.

### 7. Chức năng Quản lý Nhân sự (`/employees`)
- Dành cho `Admin`, `HRManager`, `HRStaff`, `Accountant`, `DepartmentManager` (trong đó HR và Admin có quyền thêm/sửa/xóa).
- Cho phép tìm kiếm nhân viên theo từ khóa (họ tên, mã nhân viên, email, số điện thoại), lọc theo phòng ban, chức vụ, trạng thái làm việc và bấm **Xuất CSV** để tải danh sách nhân sự.
- Khi bấm **Thêm nhân viên** hoặc **Chỉnh sửa / Xem chi tiết**, hệ thống mở cửa sổ hồ sơ được chia thành 4 tab rõ ràng: Tab thông tin chung (mã NV, họ tên, ngày sinh, giới tính, phòng ban, chức vụ, ngày vào làm, trạng thái), Tab pháp lý (số CCCD, ngày cấp, nơi cấp, mã số thuế cá nhân, số sổ BHXH), Tab ngân hàng (số tài khoản, tên ngân hàng, chi nhánh) và Tab liên hệ khẩn cấp (họ tên người thân, mối quan hệ, số điện thoại).

### 8. Chức năng Quản lý Phòng ban (`/departments`) và Chức vụ (`/positions`)
- Tại trang **Phòng ban (`/departments`)**, người quản lý có thể tìm kiếm, thêm mới, chỉnh sửa hoặc xóa phòng ban (mã phòng ban, tên phòng ban, mô tả).
- Tại trang **Chức vụ (`/positions`)**, người quản lý thiết lập danh mục các chức vụ trong công ty kèm theo **Mức lương cơ bản định mức (`BaseSalary`)** cho từng chức vụ. Hệ thống tự động kiểm tra ràng buộc toàn vẹn và ngăn chặn việc xóa phòng ban hoặc chức vụ đang có nhân sự hoạt động.

### 9. Chức năng Quản lý Hợp đồng lao động (`/contracts`)
- Truy cập menu **Hợp đồng (`/contracts`)** để tra cứu, lọc và lập hợp đồng lao động cho nhân viên.
- Khi bấm **Thêm hợp đồng**, chọn nhân viên, nhập mã hợp đồng, chọn loại hợp đồng (`Thực tập - Internship`, `Thử việc - Probation`, `Có thời hạn - Fixed-Term`, hoặc `Vô thời hạn - Indefinite`), ngày bắt đầu, ngày kết thúc, mức lương thỏa thuận và phụ cấp. Dữ liệu thời hạn của các hợp đồng này đồng thời được liên kết trực tiếp để tự động tính toán thời gian Thực tập – Thử việc – Chính thức trên trang Vòng đời nhân viên.

### 10. Chức năng Quản lý Khen thưởng và Kỷ luật (`/bonus-penalty`)
- Truy cập menu **Thưởng / Phạt (`/bonus-penalty`)** để ghi nhận các quyết định khen thưởng (`Bonus`) hoặc xử lý kỷ luật (`Penalty`) đối với nhân viên.
- Khi tạo mới quyết định, người dùng chọn nhân viên, chọn loại quyết định (Thưởng hoặc Phạt), nhập số tiền, ngày quyết định và lý do chi tiết. Tất cả các khoản thưởng và phạt trong tháng sẽ được hệ thống tự động tổng hợp cộng/trừ trực tiếp khi chạy bảng lương của tháng đó.

### 11. Chức năng Quản lý Cấu trúc Bảng lương (`/salaries`)
- Truy cập menu **Cấu trúc lương (`/salaries`)** (dành cho `Admin`, `HRManager`, `Accountant`), chức năng này cho phép tra cứu, thêm mới hoặc điều chỉnh trực tiếp từng bản ghi lương của nhân viên theo kỳ tháng/năm (bao gồm phụ cấp, thưởng, phạt, ngày công thực tế, ngày nghỉ có lương, tiền lương OT, bảo hiểm và thuế) trong trường hợp kế toán cần điều chỉnh đặc thù cho một cá nhân.

### 12. Chức năng Tính lương tự động hàng loạt, Cài đặt tính lương OT và Xem Phiếu lương (`/payroll`, `/my-payslip`)
- **Chạy Tính lương tự động hàng tháng (`/payroll`)** (dành cho `Admin`, `HRManager`, `Accountant`):
  - Truy cập menu **Bảng tính lương (`/payroll`)**, bấm nút **Tính lương tự động**.
  - Trong cửa sổ **Cài đặt tham số tính lương (`PayrollGenerateModal`)**, người dùng chọn **Tháng**, **Năm**, nhập **Số ngày công chuẩn trong tháng** (ví dụ `22` hoặc `26` ngày) và tích chọn 5 tùy chọn cấu hình tính lương:
    1. **Tính lương theo dữ liệu chấm công thực tế (`calculateAttendance`)**: Tự động quy đổi lương cơ bản theo tỷ lệ ngày công hành chính thực tế + ngày nghỉ phép có lương chia cho ngày công chuẩn.
    2. **Bao gồm tiền lương làm thêm giờ OT (`includeOvertimePay`)**: Khi bật tùy chọn này, hệ thống tự động quét toàn bộ các đơn OT đã được phê duyệt (`Approved`) thuộc chế độ **Tính tiền lương (`Paid`)** trong tháng của từng nhân viên, lấy **đúng số giờ OT đã đăng ký (`RegisteredHours`)** nhân với đơn giá giờ lương cơ bản (`BaseSalary / (StandardWorkingDays * 8)`) và nhân với **hệ số OT tương ứng (`1.5x` ngày thường, `2.0x` cuối tuần, `3.0x` ngày lễ)**. Phần thu nhập tăng thêm do làm thêm giờ cũng được tự động tách phần miễn thuế TNCN theo đúng quy định pháp luật.
    3. **Khấu trừ Bảo hiểm bắt buộc (`includeInsurance`)**: Tự động trích trừ `10.5%` lương cơ bản (`8% BHXH + 1.5% BHYT + 1% BHTN`).
    4. **Khấu trừ Thuế thu nhập cá nhân (`includeTax`)**: Tự động trừ giảm trừ gia cảnh bản thân (`11.000.000 VNĐ/tháng`), phụ cấp ăn trưa miễn thuế, phần lương OT miễn thuế, bảo hiểm bắt buộc và tính thuế TNCN theo biểu lũy tiến từng phần 7 bậc từ `5%` đến `35%`.
    5. **Lưu kết quả vào Cơ sở dữ liệu (`saveToDatabase`)**: Lưu chốt bảng lương của kỳ đó vào bảng `SALARIES` (nếu bỏ tích thì hệ thống chỉ chạy mô phỏng xem trước mà chưa ghi đè vào DB).
  - Sau khi bấm **Chạy tính lương**, hệ thống hiển thị 4 thẻ tổng hợp quỹ lương toàn công ty (Tổng lương Gross, Tổng thực lĩnh Net, Tổng thuế TNCN, Tổng trích nộp Bảo hiểm) và danh sách lương từng nhân viên. Bấm vào từng nhân viên để xem **Phiếu lương chi tiết (`PayrollPayslipModal`)** hoặc bấm **Xuất CSV** để tải bảng lương.
- **Xem Phiếu lương cá nhân (`/my-payslip`)** (dành cho mọi nhân viên):
  - Nhân viên truy cập menu **Phiếu lương của tôi (`/my-payslip`)**, chọn tháng và năm để xem chi tiết từng khoản thu nhập (lương cơ bản theo ngày công, phụ cấp, số giờ OT & tiền lương OT, tiền thưởng) và từng khoản khấu trừ (bảo hiểm, thuế TNCN, tiền phạt) cùng số tiền thực lĩnh cuối cùng (`Net Salary`).

### 13. Chức năng Báo cáo Thống kê và Xuất dữ liệu CSV (`/reports`)
- Truy cập menu **Báo cáo (`/reports`)** (dành cho cấp quản lý và kế toán):
  - **Tab Báo cáo Quỹ lương**: Chọn tháng và năm để thống kê tổng số nhân viên được chi trả, tổng quỹ lương cơ bản, tổng quỹ lương thực chi (Net), mức lương trung bình, mức lương cao nhất và thấp nhất trong tháng, kèm nút **Xuất CSV báo cáo lương**.
  - **Tab Báo cáo Nghỉ phép**: Chọn khoảng thời gian từ ngày đến ngày để thống kê tổng số đơn nghỉ phép, số đơn chờ duyệt, đã duyệt, từ chối, đã hủy cùng thanh biểu đồ tỷ lệ trực quan, kèm nút **Xuất CSV báo cáo nghỉ phép**.
  - Ở góc phải phía trên còn có nút **Xuất toàn bộ danh sách nhân sự CSV** để tải nhanh hồ sơ toàn công ty.

### 14. Chức năng Quản trị Phân quyền 6 tầng Role và Quyền hạn (`/roles`)
- Dành riêng cho `Admin` tại menu **Phân quyền (`/roles`)**:
  - Cho phép xem danh sách toàn bộ các nhóm vai trò (6 cấp Role: `Admin`, `HRManager`, `HRStaff`, `Accountant`, `DepartmentManager`, `Employee`), tạo thêm Role mới nếu cần và cấu hình các quyền chi tiết (`Role Claims`).
  - Cho phép xem danh sách tất cả tài khoản người dùng trong hệ thống và bấm nút **Gán Role** để cấp mới hoặc thu hồi bất kỳ Role nào cho từng tài khoản nhân viên.
