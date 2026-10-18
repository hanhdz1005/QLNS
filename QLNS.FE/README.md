# HRMS Pro - Hệ Thống Quản Lý Nhân Sự (Frontend)

Hệ thống quản lý nhân sự chuyên nghiệp, trực quan và hiện đại được xây dựng bằng **Vue 3**, **Vite**, **TypeScript**, **Tailwind CSS v4**, và **Pinia**, kết nối đầy đủ toàn bộ hệ thống API backend .NET 8 trong `QLNS.API`.

---

## 🚀 Các Tính Năng Đã Triển Khai & Kết Nối API

### 1. 📊 Tổng Quan & Bảng Điều Khiển (Dashboard)
- **KPI Metrics**: Tổng số nhân sự, số lượng đang làm việc, số phòng ban, đơn nghỉ phép chờ duyệt, tổng lượt chấm công trong tháng, ngân sách quỹ lương tháng trước và mức lương bình quân.
- **Cảnh báo nhân sự (HR Alerts)**: Hợp đồng sắp hết hạn trong 30 ngày, sinh nhật nhân viên trong tháng, nhân sự sắp hoàn thành thời gian thử việc.
- **Biểu đồ phân bổ nhân lực theo phòng ban**: Thanh tiến độ trực quan theo tỷ lệ % nhân viên từng phòng.
- **Trích xuất nhanh**: Tải tệp CSV danh sách nhân sự và tệp JSON tổng quan.

### 2. 👥 Quản Lý Hồ Sơ Nhân Viên (Employee Management)
- Xem danh sách nhân viên dạng bảng phân trang, tìm kiếm theo tên hoặc mã nhân viên, lọc theo phòng ban.
- Thêm mới & Chỉnh sửa hồ sơ nhân viên đầy đủ 4 nhóm thông tin:
  1. **Thông tin chung**: Họ tên, Email, Số điện thoại, Ngày sinh, Giới tính, Địa chỉ, Phòng ban, Chức vụ, Ngày gia nhập, Quản lý trực tiếp (Manager).
  2. **Định danh & Thuế**: Số CCCD/CMND, Ngày cấp, Nơi cấp, Mã số thuế TNCN, Số sổ BHXH, Số người phụ thuộc giảm trừ gia cảnh.
  3. **Tài khoản ngân hàng**: Số tài khoản, Ngân hàng, Chi nhánh ngân hàng.
  4. **Liên hệ khẩn cấp**: Họ tên người liên hệ, Số điện thoại, Mối quan hệ.
- Hộp thoại xem chi tiết hồ sơ cá nhân và xóa nhân viên với xác nhận an toàn.
- Xuất dữ liệu nhân sự ra định dạng CSV.

### 3. 🏢 Quản Lý Phòng Ban & Cơ Cấu Tổ Chức (Departments)
- Danh sách phòng ban với mã phòng, mô tả chức năng nhiệm vụ, ngày thành lập.
- Thêm mới, chỉnh sửa và xóa phòng ban.

### 4. 🎖️ Quản Lý Chức Vụ & Cấp Bậc (Positions)
- Danh mục chức danh và mức lương chuẩn vị trí theo VNĐ.
- Thêm, sửa, xóa chức vụ.

### 5. 📝 Quản Lý Hợp Đồng Lao Động (Contracts)
- Phân loại hợp đồng: Thử việc, Có thời hạn (1 - 3 năm), Không thời hạn, Thực tập, Bán thời gian.
- Quản lý mức lương ký kết và mức đóng bảo hiểm xã hội.
- Đính kèm tệp scan hợp đồng (PDF / hình ảnh) qua `FileController`.
- Bộ lọc thông minh: Xem riêng các hợp đồng sắp hết hạn trong vòng 30 ngày.

### 6. ⏰ Chấm Công & Điểm Danh QR Kiosk (Attendance)
- **Kiosk Mode QR Code Động**: Sinh mã QR động từ backend (`/api/Attendance/qr-token`), tự động làm mới mã sau mỗi 30 giây với thanh đếm ngược thời gian thực.
- Nhân viên quét mã QR trên thiết bị di động để Check-in / Check-out tự động ghi nhận giờ vào, giờ ra và số giờ công.
- Hỗ trợ nút Demo Check-in tức thì cho tài khoản đang đăng nhập.
- Bảng nhật ký chấm công toàn công ty và tab tra cứu bảng chấm công cá nhân theo từng tháng/năm.
- Chức năng ghi nhận công thủ công dành cho quản trị viên.

### 7. 🏖️ Quản Lý Nghỉ Phép & Phê Duyệt (Leaves)
- Quản lý quỹ phép năm: Hiển thị tổng số ngày phép, số ngày đã dùng và số ngày phép còn lại.
- Tạo đơn xin nghỉ phép theo nhiều loại hình (Phép năm, Nghỉ ốm đau, Thai sản, Nghỉ chế độ, Không lương).
- **Hàng đợi xét duyệt đơn (Approvals)**: Dành cho Admin/Leader duyệt nhanh (Approve) hoặc từ chối (Reject) chỉ với 1 click.
- Điều chỉnh hạn mức quỹ phép năm cho từng nhân sự.

### 8. 💵 Tính Lương & Phiếu Lương Điện Tử (Payroll & Payslip)
- **Tính lương tự động hàng loạt**: Tự động tổng hợp số ngày công thực tế từ chấm công, tự động khấu trừ BHXH (10.5%), thuế TNCN và phụ cấp/thưởng/phạt.
- Bảng lương chi tiết theo tháng và năm.
- **Phiếu lương điện tử (Payslip)**: Giao diện phiếu lương chi tiết từng khoản thu nhập và giảm trừ, số tiền thực lĩnh (Net) chuyển khoản.
- Xuất bảng lương ra file CSV.

### 9. ⚖️ Mức Lương Cơ Sở & Phụ Cấp (Salary Structure)
- Cấu hình mức lương cơ bản, các khoản phụ cấp trách nhiệm, hỗ trợ ăn trưa, thưởng và kỷ luật.

### 10. 🎁 Khen Thưởng & Kỷ Luật (Bonus & Penalty)
- Ghi nhận quyết định khen thưởng thành tích và xử phạt vi phạm nội quy.
- Tự động cộng/trừ vào kỳ tính lương tương ứng của nhân viên.

### 11. 📈 Báo Cáo Thống Kê & Trích Xuất File (Reports)
- Báo cáo phân tích quỹ lương tháng: Tổng số người, tổng Gross, tổng Net, lương cao nhất/thấp nhất, mức trung bình.
- Báo cáo tỷ lệ nghỉ phép theo khoảng thời gian tùy chọn với biểu đồ tỷ lệ duyệt.
- Tải về các tệp CSV báo cáo và JSON tổng quan.

### 12. 🛡️ Phân Quyền & Tài Khoản Hệ Thống (Roles & Permissions)
- Dành cho Quản trị viên (Admin):
  - Quản lý tài khoản đăng nhập.
  - Tạo mới và xóa vai trò bảo mật.
  - Gán vai trò (Admin, Leader, User) cho người dùng.
  - Cấp Claim / Policy (`FULL.ACCESS`, `ASSET.VIEW`, `ASSET.CREATE`, `ASSET.EDIT`, `ASSET.DELETE`).

### 13. 👤 Hồ Sơ Tự Phục Vụ (Employee Self-Service)
- Xem thông tin cá nhân, cập nhật ảnh đại diện (Avatar), tra cứu nhanh quỹ phép năm, chấm công cá nhân và phiếu lương hàng tháng.

---

## 🛠️ Cài Đặt & Chạy Ứng Dụng

### Yêu Cầu
- Node.js >= 20 (khuyên dùng Node 22 hoặc Node 24)
- .NET 8 SDK (để chạy backend `QLNS.API`)

### 1. Chạy Backend API (`QLNS.API`)
```sh
cd "d:\Blazor study\QLNS\QLNS.API"
dotnet run --project QLNS.API
```
API sẽ chạy tại cổng mặc định `http://localhost:5034` (Swagger tại `http://localhost:5034/swagger`).

### 2. Chạy Frontend (`QLNS.FE`)
```sh
cd "d:\Blazor study\QLNS\QLNS.FE"
npm install
npm run dev
```
Truy cập giao diện tại: `http://localhost:5173`

### 3. Tài Khoản Đăng Nhập Mẫu
- **Admin**:
  - Email: `admin@gmail.com`
  - Mật khẩu: `Admin@1234`
  - (Đã tích hợp sẵn nút "Điền mẫu" tại trang Đăng nhập)
