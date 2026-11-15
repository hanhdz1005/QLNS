# HƯỚNG DẪN CÀI ĐẶT VÀ SỬ DỤNG HỆ THỐNG QUẢN LÝ NHÂN SỰ (QLNS - HRMS PRO)

## PHẦN 1: CÁCH CÀI ĐẶT MÔI TRƯỜNG ĐỂ CHẠY ĐƯỢC DỰ ÁN

### 1. Yêu cầu công cụ cần cài đặt trước trên máy
- Cài đặt bộ công cụ .NET 8.0 SDK trở lên (sử dụng Visual Studio 2022 hoặc VS Code để chạy Backend).
- Cài đặt hệ quản trị cơ sở dữ liệu Microsoft SQL Server (bản Developer, Express hoặc LocalDB) kèm SQL Server Management Studio (SSMS).
- Cài đặt Node.js phiên bản 20 LTS hoặc 22 LTS trở lên (tự động đi kèm trình quản lý gói npm để chạy Frontend).
- Máy tính có kết nối Webcam (hoặc Camera sẵn có trên laptop) và trình duyệt Chrome/Edge để sử dụng chức năng quét khuôn mặt chấm công thời gian thực.

### 2. Các bước cài đặt và khởi chạy Backend (QLNS.API)
- Bước 1 (Cấu hình chuỗi kết nối SQL Server): Mở file `QLNS/QLNS.API/QLNS.API/appsettings.json`, tìm tới mục `ConnectionStrings.DefaultConnection` và đổi tên `Server=...` cho đúng với tên SQL Server trên máy của bạn (ví dụ: `Server=localhost;Database=QLNS_DB;Trusted_Connection=True;TrustServerCertificate=True;`).
- Bước 2 (Khởi tạo cơ sở dữ liệu bằng Migration): Mở Terminal hoặc PowerShell tại thư mục `QLNS/QLNS.API` và chạy lệnh:
  `dotnet ef database update --project QLNS.Infrastructure --startup-project QLNS.API`
- Bước 3 (Chạy ứng dụng Backend): Di chuyển vào thư mục `QLNS/QLNS.API/QLNS.API` và chạy lệnh:
  `dotnet run`
- Cơ chế tự động khởi tạo khi chạy Backend: Ngay khi lệnh `dotnet run` được thực thi, hệ thống sẽ tự động chạy `IdentitySeed.EnsureExtendedSchemaAsync` và `IdentitySeed.SeedRoleAsync` để tự động cập nhật thêm bảng đơn làm thêm giờ (`OVERTIME_REQUESTS`), các cột mở rộng của bảng chấm công (`RegularHours`, `OvertimeHours`, `ScanCount`, `FaceEmbedding`) và tự động tạo sẵn 6 tài khoản mẫu tương ứng với 6 tầng Role.

### 3. Các bước cài đặt và khởi chạy Frontend (QLNS.FE)
- Bước 1 (Cài đặt thư viện): Mở một cửa sổ Terminal mới, di chuyển vào thư mục `QLNS/QLNS.FE` và chạy lệnh:
  `npm install`
- Bước 2 (Kiểm tra kết nối API): Kiểm tra địa chỉ cổng Backend trong file `src/api/axiosClient.ts` (hoặc biến môi trường `VITE_API_BASE_URL`) để đảm bảo khớp với cổng mà Backend đang chạy.
- Bước 3 (Khởi chạy giao diện web): Chạy lệnh:
  `npm run dev`
- Sau khi chạy lệnh trên, mở trình duyệt truy cập vào địa chỉ hiển thị trên Terminal (mặc định là `http://localhost:5173`). Nếu muốn kiểm tra kiểu dữ liệu TypeScript hoặc đóng gói bản Production thì chạy `npm run type-check` và `npm run build-only`.

---

## PHẦN 2: DANH SÁCH 6 TÀI KHOẢN PHÂN QUYỀN MẪU (MẬT KHẨU CHUNG: `Admin@1234`)

- Tầng 1 - Quản trị viên hệ thống (`Admin`): Đăng nhập bằng email `admin@gmail.com` và mật khẩu `Admin@1234`. Tài khoản này có toàn quyền quản trị hệ thống, cài đặt chính sách công ty, gán Role cho các tài khoản và truy cập toàn bộ chức năng.
- Tầng 2 - Trưởng phòng Nhân sự (`HRManager`): Đăng nhập bằng email `hrmanager@gmail.com` và mật khẩu `Admin@1234`. Tài khoản này quản lý toàn bộ nhân sự, phòng ban, chức vụ, hợp đồng, thưởng/phạt, cài đặt chính sách lương OT và giờ làm việc, duyệt đơn nghỉ phép, duyệt đơn OT, xem vòng đời nhân viên toàn công ty và xem bảng lương.
- Tầng 3 - Chuyên viên Nhân sự (`HRStaff`): Đăng nhập bằng email `hrstaff@gmail.com` và mật khẩu `Admin@1234`. Tài khoản này thực hiện các nghiệp vụ quản lý hồ sơ nhân sự, phòng ban, chức vụ, hợp đồng, thưởng/phạt, theo dõi bảng chấm công và xem vòng đời làm việc toàn công ty.
- Tầng 4 - Kế toán tiền lương (`Accountant`): Đăng nhập bằng email `accountant@gmail.com` và mật khẩu `Admin@1234`. Tài khoản này chuyên trách quản lý cấu trúc lương, chạy tính lương tự động hàng tháng (bao gồm tính lương OT, bảo hiểm, thuế TNCN), quản lý thưởng/phạt và xuất báo cáo quỹ lương.
- Tầng 5 - Trưởng bộ phận (`DepartmentManager`): Đăng nhập bằng email `manager@gmail.com` và mật khẩu `Admin@1234`. Tài khoản này có quyền phê duyệt hoặc từ chối đơn xin nghỉ phép và đơn đăng ký làm thêm giờ (OT) của nhân viên, đồng thời xem danh sách nhân sự, bảng chấm công và vòng đời nhân viên.
- Tầng 6 - Nhân viên (`Employee`): Đăng nhập bằng email `employee@gmail.com` và mật khẩu `Admin@1234`. Tài khoản này dùng để quét khuôn mặt chấm công, xem lịch sử chấm công cá nhân, gửi đơn xin nghỉ phép, gửi đơn đăng ký giờ làm thêm (OT), xem phiếu lương cá nhân và xem vòng đời làm việc của chính mình.

---

## PHẦN 3: HƯỚNG DẪN SỬ DỤNG ĐẦY ĐỦ CÁC CHỨC NĂNG TRONG HỆ THỐNG

### 1. Đăng nhập, Quét khuôn mặt lần đầu, Hồ sơ cá nhân và Đổi mật khẩu (`/login`, `/profile`)
- Tại trang đăng nhập (`/login`), người dùng nhập email và mật khẩu hoặc bấm vào các nút chọn nhanh tài khoản mẫu của từng Role bên dưới form để đăng nhập.
- Ngay sau khi đăng nhập thành công, nếu tài khoản chưa từng đăng ký khuôn mặt (`HasRegisteredFace = false`), hệ thống sẽ tự động hiển thị cửa sổ bắt buộc Quét khuôn mặt lần đầu. Người dùng chỉ cần nhìn thẳng vào khung elip trên camera khoảng 1 giây (không cần bấm nút chụp), thanh tiến trình sẽ tự động chạy từ 0% lên 100% và lưu mẫu khuôn mặt 128 chiều vào cơ sở dữ liệu để nhận diện cho các lần chấm công sau.
- Khi cần cập nhật lại mẫu khuôn mặt mới hoặc đổi mật khẩu tài khoản, người dùng truy cập trang Hồ sơ cá nhân (`/profile`) rồi bấm nút Cập nhật khuôn mặt hoặc Đổi mật khẩu.
- Trên góc phải thanh Header, người dùng có thể bấm nút chuyển đổi ngôn ngữ hiển thị giữa Tiếng Việt (`VI`) và Tiếng Anh (`EN`).

### 2. Trang Tổng quan hệ thống - Dashboard (`/dashboard`)
- Khi vào trang `/dashboard`, hệ thống hiển thị các thẻ chỉ số KPI tổng quan gồm: tổng số nhân sự, số nhân sự đang làm việc, tổng số phòng ban, số đơn từ đang chờ duyệt, tổng lượt chấm công trong tháng, tổng quỹ lương thực chi và mức lương trung bình.
- Phần giữa trang hiển thị biểu đồ phân bổ số lượng và tỷ trọng nhân sự theo từng phòng ban trong công ty.
- Phần Cảnh báo nhân sự thông minh tự động liệt kê các hợp đồng lao động sắp hết hạn trong 30 ngày tới, danh sách nhân viên sắp kết thúc thử việc và danh sách nhân viên có sinh nhật trong tháng.

### 3. Cài đặt Chính sách Công ty, Cài đặt Lương OT, Hệ số OT và Giờ làm việc (`/settings`)
- Truy cập menu Cài đặt (`/settings`) (chỉ `Admin` và `HRManager` có quyền chỉnh sửa và bấm Lưu cài đặt; các Role khác có thể xem chính sách đang áp dụng). Trang này gồm 3 tab cấu hình chính sách nghiệp vụ:
- Tại Tab 1 (Chính sách Làm thêm giờ & Cài đặt Lương OT):
  + Chọn hình thức đãi ngộ OT mặc định của công ty trong 3 chế độ: Quy đổi thành ngày Nghỉ bù (`CompensatoryLeave`), Ghi nhận cống hiến / KPI (`RecognitionOnly`), hoặc Tính thành Tiền lương OT (`Paid`).
  + Bật hoặc tắt công tắc "Cho phép nhân viên tự chọn hình thức đãi ngộ OT khi gửi đơn": Nếu tắt công tắc này, khi nhân viên tạo đơn đăng ký OT thì chế độ đãi ngộ sẽ bị khóa cố định theo chế độ mặc định của công ty; nếu bật thì nhân viên được phép chọn nhận Tiền lương OT, Nghỉ bù hoặc Ghi nhận KPI.
  + Bật hoặc tắt công tắc "Cho phép người phê duyệt điều chỉnh hình thức đãi ngộ OT khi duyệt đơn": Khi bật công tắc này, Trưởng bộ phận (`DepartmentManager`), `HRManager` hoặc `Admin` có thể chuyển đổi đơn OT của nhân viên sang Tính tiền lương OT (hoặc Nghỉ bù) ngay tại bước duyệt đơn.
  + Cài đặt các thông số định mức và hệ số nhân lương OT (`Multiplier`): Thiết lập giới hạn số giờ OT tối đa trong 1 tháng (mặc định 40 giờ), số giờ OT để quy đổi thành 1 ngày nghỉ bù (mặc định 8 giờ), Hệ số nhân lương OT ngày thường (`Weekday Multiplier`, mặc định 1.5x tức 150%), Hệ số nhân lương OT ngày cuối tuần (`Weekend Multiplier`, mặc định 2.0x tức 200%), và Hệ số nhân lương OT ngày Lễ/Tết (`Holiday Multiplier`, mặc định 3.0x tức 300%).
- Tại Tab 2 (Giờ làm việc hành chính & Chấm công): Cho phép cài đặt khung giờ làm việc ca sáng (mặc định 08:00 đến 12:00), khung giờ ca chiều (mặc định 13:00 đến 17:30), mốc giờ ân hạn đi muộn buổi sáng (mặc định 08:15), số ngày công chuẩn trong tháng (mặc định 22 hoặc 26 ngày) và tùy chọn có làm việc ngày Thứ Bảy hay không.
- Tại Tab 3 (Chính sách Tính lương & Nghỉ phép): Cho phép cài đặt số ngày phép năm mặc định cấp cho nhân viên (mặc định 12 ngày/năm) và bật/tắt mặc định 3 tùy chọn khi chạy bảng lương gồm: Tự động cộng tiền lương OT vào bảng lương, Tự động trừ Bảo hiểm bắt buộc (10.5%) và Tự động trừ Thuế TNCN.

### 4. Chấm công Khuôn mặt Real-Time Tự động (`/attendance`)
- Truy cập menu Chấm công (`/attendance`), tại khung Quét Khuôn Mặt Real-Time bấm nút Bật Camera.
- Cách quét tự động: Người dùng chỉ cần đứng trước camera và đưa khuôn mặt vào vòng elip định vị (hoàn toàn không cần bấm nút chụp). Khi giữ mặt ổn định khoảng 1 giây để thanh tiến trình khóa từ 0% lên 100%, hệ thống sẽ tự động gửi dữ liệu khuôn mặt lên Server đối sánh và hiển thị ngay kết quả chấm công ở khung bên phải (gồm họ tên, mã nhân viên, độ khớp %, giờ vào đầu, giờ ra cuối, lượt quét thứ mấy trong ngày, số giờ công hành chính và số giờ OT đã duyệt), sau đó tự động mở khóa sau 4 giây để lượt tiếp theo quét.
- Cách xử lý khi nhân viên quét nhiều lần trong 1 ngày:
  + Lần quét đầu tiên trong ngày được lưu làm Giờ vào đầu tiên (`CheckIn`). Nếu quét sau mốc ân hạn 08:15, hệ thống tự động ghi nhận trạng thái Đi muộn (`Late`).
  + Từ lần quét thứ 2, thứ 3 trở đi trong cùng một ngày, hệ thống vẫn giữ nguyên Giờ vào đầu tiên (`CheckIn`), chỉ cập nhật thời điểm quét mới nhất làm Giờ ra cuối cùng (`CheckOut`) và cộng tăng số lần quét trong ngày (`#ScanCount`).
  + Cách tính giờ công hành chính (`RegularHours`): Hệ thống chỉ tính phần thời gian giao nhau giữa khoảng `[Lần quét đầu, Lần quét cuối]` với khung giờ hành chính chuẩn `08:00 – 12:00` và `13:00 – 17:30` (tự động trừ giờ nghỉ trưa và giới hạn tối đa 8.0 giờ hành chính/ngày).
  + Cách cộng giờ công OT (`OvertimeHours`): Hệ thống kiểm tra xem ngày hôm đó nhân viên có đơn đăng ký OT đã được phê duyệt (`Approved`) hay không; nếu có thì hệ thống chỉ cộng thêm đúng bằng số giờ OT đã đăng ký vào cột giờ OT của ngày hôm đó.
- Ở phần bảng dữ liệu phía dưới trang `/attendance`, nhóm quản lý (`Admin`, `HRManager`, `HRStaff`) có thể lọc, tìm kiếm, xem nhật ký chấm công toàn công ty hoặc thêm/sửa chấm công thủ công khi cần; nhân viên thường xem bảng lịch sử chấm công cá nhân của mình.

### 5. Quản lý Nghỉ phép, Đăng ký làm thêm giờ (OT) và Duyệt đơn OT (`/leaves`)
- Cách sử dụng phần Nghỉ phép và Quỹ phép năm:
  + Nhân viên xem các thẻ số dư phép năm ở đầu trang `/leaves`, bấm nút Tạo đơn nghỉ phép, chọn loại nghỉ phép (Phép năm, Nghỉ ốm, Nghỉ việc riêng, Nghỉ không lương...), chọn ngày bắt đầu, ngày kết thúc, nhập lý do rồi bấm Gửi đơn.
  + Trưởng bộ phận (`DepartmentManager`), `HRManager` hoặc `Admin` chuyển sang tab Duyệt đơn nghỉ phép để bấm Duyệt hoặc Từ chối đơn xin nghỉ. Ngoài ra, HR/Admin có thể bấm nút Cấu hình quỹ phép để cấp mới hoặc chỉnh sửa tổng số ngày phép năm cho từng nhân viên.
- Cách sử dụng phần Đăng ký làm thêm giờ (OT) dành cho Nhân viên:
  + Kéo xuống khu vực Quản lý Làm thêm giờ (Overtime - OT) trong trang `/leaves` và bấm nút Đăng ký OT.
  + Chọn Ngày làm thêm giờ và Loại ngày OT (Ngày thường, Cuối tuần, hoặc Ngày Lễ/Tết), hoặc bấm vào các nút chọn nhanh ca OT mẫu (Tối 2h từ 17:30-19:30, Tối 3h từ 18:00-21:00, Nửa ngày cuối tuần 4h, Cả ngày cuối tuần 8h).
  + Nhập Giờ bắt đầu, Giờ kết thúc và Số giờ OT đăng ký (`RegisteredHours`): Hệ thống tự tính số giờ gợi ý theo khung giờ hoặc cho phép nhập chính xác số giờ OT đăng ký (từ 0.5h đến 16h).
  + Chọn Hình thức đãi ngộ OT (Tính tiền lương OT, Nghỉ bù, hoặc Ghi nhận KPI nếu công ty mở quyền tự chọn), kiểm tra dòng Xem trước quyền lợi quy đổi (hiển thị rõ số giờ quy đổi nhân với hệ số lương OT `1.5x / 2.0x / 3.0x` hoặc số ngày nghỉ bù), nhập lý do tăng ca và bấm Gửi đơn đăng ký OT.
- Cách sử dụng phần Phê duyệt và Điều chỉnh đơn OT dành cho `DepartmentManager`, `HRManager`, `Admin`:
  + Chuyển sang tab Đơn OT chờ duyệt trong khu vực Overtime và bấm vào đơn cần xử lý để mở cửa sổ Xem xét & Phê duyệt đơn OT.
  + Trên cửa sổ phê duyệt, hệ thống hiển thị sẵn thông tin đơn đăng ký kèm dòng đối chiếu "Chấm công khuôn mặt thực tế" (`CheckIn -> CheckOut`) của nhân viên trong ngày đó để người quản lý kiểm tra xem nhân viên có thực sự ở lại làm việc hay không.
  + Người quản lý có thể giữ nguyên hoặc chỉnh lại Số giờ OT được duyệt, Loại ngày OT và Hình thức đãi ngộ OT (ví dụ chốt trả bằng Tiền lương OT hoặc Nghỉ bù), nhập ghi chú phê duyệt rồi bấm nút Phê duyệt OT (hoặc Từ chối).
  + Ngay khi đơn OT được phê duyệt, hệ thống lập tức cộng đúng số giờ OT đã duyệt vào bản ghi chấm công của nhân viên trong ngày đó để phục vụ tính lương cuối tháng.

### 6. Theo dõi Vòng đời làm việc của nhân viên tự động cập nhật theo tháng (`/lifecycle` và `/lifecycle/:id`)
- Truy cập menu Vòng đời nhân sự (`/lifecycle`):
  + Đối với tài khoản nhân viên thường (`Employee`, `Accountant`), trang `/lifecycle` chỉ hiển thị duy nhất vòng đời làm việc, chỉ số và biểu đồ của chính cá nhân người đang đăng nhập.
  + Đối với tài khoản cấp quản lý (`Admin`, `HRManager`, `HRStaff`, `DepartmentManager`), trang `/lifecycle` hiển thị danh sách tổng quan vòng đời của tất cả nhân viên trong công ty (có tìm kiếm theo tên/mã và lọc theo phòng ban). Khi bấm vào một dòng nhân viên hoặc bấm nút Xem chi tiết, hệ thống sẽ chuyển sang trang chi tiết `/lifecycle/:id` để hiển thị toàn bộ vòng đời làm việc của nhân viên đó.
- Cách xem dữ liệu tự động cập nhật theo tháng:
  + Trên góc phải của khung thông tin nhân viên có bộ chọn Tháng và Năm. Khi đổi tháng hoặc năm, toàn bộ số tháng làm việc ở từng giai đoạn, các thẻ chỉ số KPI và 4 biểu đồ bên dưới sẽ tự động tính toán và cập nhật lại tính đến thời điểm cuối tháng đang chọn.
- Các thông tin hiển thị trong trang Vòng đời nhân viên:
  + Hiển thị 3 thẻ phân tách rõ thời gian Thực tập (`Internship`), thời gian Thử việc (`Probation`) và thời gian Làm chính thức (`Official`) của nhân viên trong công ty (tự động tính số tháng dựa trên lịch sử hợp đồng lao động và ngày vào làm).
  + Hiển thị 6 thẻ chỉ số KPI gồm: Tổng thâm niên công tác (tháng), Giờ công hành chính trong tháng, Giờ OT đã duyệt trong tháng, Tỷ lệ đi làm đúng giờ (%), Lương thực lĩnh (Net) trong tháng và Tổng số hợp đồng.
  + Hiển thị 4 biểu đồ tượng trưng SVG gồm: Biểu đồ thanh tỷ trọng thời gian 3 giai đoạn Thực tập - Thử việc - Chính thức, Biểu đồ đường xu hướng thu nhập và thâm niên 6 tháng gần nhất, Biểu đồ cột chồng so sánh Giờ công hành chính và Giờ OT đã duyệt trong 6 tháng, và Biểu đồ Donut tỷ lệ đi làm đúng giờ.
  + Hiển thị Trục thời gian sự nghiệp (Career Timeline) liệt kê lần lượt mốc gia nhập công ty, các lần ký hợp đồng và các quyết định khen thưởng, kỷ luật.

### 7. Quản lý Hồ sơ Nhân sự (`/employees`)
- Truy cập menu Nhân sự (`/employees`), sử dụng thanh tìm kiếm (theo tên, mã nhân viên, email, số điện thoại) và các bộ lọc theo phòng ban, chức vụ, trạng thái làm việc để tra cứu nhân viên, hoặc bấm nút Xuất CSV để tải danh sách nhân sự về máy.
- Bấm nút Thêm nhân viên (hoặc bấm biểu tượng Sửa / Xem chi tiết trên từng dòng) để mở hồ sơ nhân sự gồm 4 tab: Tab Thông tin chung (họ tên, mã NV, giới tính, ngày sinh, phòng ban, chức vụ, ngày vào làm, trạng thái), Tab CCCD & Thuế/BHXH (số CCCD, ngày cấp, nơi cấp, mã số thuế, số sổ BHXH), Tab Tài khoản ngân hàng (số tài khoản, ngân hàng, chi nhánh) và Tab Liên hệ khẩn cấp (tên người thân, mối quan hệ, số điện thoại).

### 8. Quản lý Phòng ban (`/departments`) và Chức vụ (`/positions`)
- Tại menu Phòng ban (`/departments`), người quản lý thực hiện tìm kiếm, thêm mới, cập nhật hoặc xóa phòng ban (nhập mã phòng ban, tên phòng ban và mô tả).
- Tại menu Chức vụ (`/positions`), người quản lý thêm mới, cập nhật hoặc xóa chức vụ kèm theo Mức lương cơ bản định mức (`BaseSalary`) áp dụng cho chức vụ đó. Hệ thống tự động chặn xóa nếu phòng ban hoặc chức vụ đang có nhân viên trực thuộc.

### 9. Quản lý Hợp đồng lao động (`/contracts`)
- Truy cập menu Hợp đồng (`/contracts`) để tìm kiếm, lọc theo loại hợp đồng/trạng thái và quản lý toàn bộ hợp đồng của nhân viên.
- Bấm nút Thêm hợp đồng, chọn nhân viên, nhập số hợp đồng, chọn loại hợp đồng (`Thực tập - Internship`, `Thử việc - Probation`, `Có thời hạn - Fixed-Term`, hoặc `Vô thời hạn - Indefinite`), chọn ngày bắt đầu, ngày kết thúc, nhập mức lương cơ bản và phụ cấp rồi bấm Lưu. Các giai đoạn hợp đồng này sẽ tự động đồng bộ sang trang Vòng đời làm việc của nhân viên.

### 10. Quản lý Khen thưởng và Kỷ luật (`/bonus-penalty`)
- Truy cập menu Thưởng / Phạt (`/bonus-penalty`) để quản lý các quyết định khen thưởng hoặc kỷ luật cá nhân.
- Bấm nút Thêm mới, chọn nhân viên, chọn loại quyết định là Khen thưởng (`Bonus`) hoặc Kỷ luật (`Penalty`), nhập số tiền, chọn ngày quyết định, nhập lý do rồi bấm Lưu. Các khoản thưởng và phạt này sẽ tự động được cộng hoặc khấu trừ khi chạy tính lương tháng tương ứng.

### 11. Quản lý Cấu trúc Bảng lương thủ công (`/salaries`)
- Truy cập menu Cấu trúc lương (`/salaries`) (dành cho `Admin`, `HRManager`, `Accountant`).
- Tại đây kế toán có thể tìm kiếm các bản ghi lương theo tên nhân viên, bấm Thêm mới hoặc Chỉnh sửa trực tiếp từng dòng lương của một nhân viên cụ thể trong tháng (điều chỉnh phụ cấp, tiền thưởng, tiền phạt, ngày công thực tế, ngày nghỉ có lương, tiền lương OT, bảo hiểm, thuế) khi cần xử lý các trường hợp điều chỉnh lương cá biệt.

### 12. Tính lương tự động hàng tháng (kèm cấu hình tính lương OT) và Xem Phiếu lương (`/payroll`, `/my-payslip`)
- Cách chạy Tính lương tự động toàn công ty (`/payroll`) dành cho `Admin`, `HRManager`, `Accountant`:
  + Truy cập menu Bảng tính lương (`/payroll`) và bấm nút Tính lương tự động.
  + Trong cửa sổ cấu hình tham số tính lương, chọn Tháng, chọn Năm, nhập Số ngày công chuẩn trong tháng (ví dụ 22 hoặc 26 ngày) và tích chọn 5 tùy chọn tính toán:
    1. Tích chọn "Tính lương theo dữ liệu chấm công thực tế": Tự động tính lương cơ bản theo tỷ lệ tổng ngày công hành chính thực tế cộng ngày nghỉ phép có lương chia cho số ngày công chuẩn.
    2. Tích chọn "Bao gồm tiền lương làm thêm giờ (OT)": Khi bật tùy chọn này, hệ thống tự động lấy toàn bộ các đơn OT đã được phê duyệt (`Approved`) có hình thức đãi ngộ là Tính tiền lương (`Paid`) trong tháng của từng nhân viên, lấy đúng số giờ OT đã đăng ký (`RegisteredHours`) nhân với đơn giá 1 giờ lương cơ bản (`Lương cơ bản / (Số ngày công chuẩn * 8)`) và nhân với hệ số nhân OT đã cài đặt (`1.5x` ngày thường, `2.0x` cuối tuần, `3.0x` ngày lễ). Đồng thời hệ thống tự động tách phần chênh lệch lương làm thêm giờ miễn thuế TNCN theo đúng quy định pháp luật.
    3. Tích chọn "Khấu trừ Bảo hiểm bắt buộc (10.5%)": Tự động trích trừ 8% BHXH, 1.5% BHYT và 1% BHTN trên mức lương cơ bản.
    4. Tích chọn "Khấu trừ Thuế thu nhập cá nhân (TNCN)": Tự động áp dụng mức giảm trừ gia cảnh bản thân 11.000.000 VNĐ/tháng, trừ các khoản miễn thuế và bảo hiểm, sau đó tính thuế TNCN theo biểu lũy tiến từng phần 7 bậc từ 5% đến 35%.
    5. Tích chọn "Lưu kết quả vào Cơ sở dữ liệu": Lưu chính thức bảng lương của tháng đó vào CSDL (nếu bỏ tích thì hệ thống chỉ tính nháp để xem trước trên màn hình).
  + Sau khi bấm Chạy tính lương, trang `/payroll` sẽ hiển thị 4 thẻ tổng hợp (Tổng lương Gross, Tổng thực lĩnh Net, Tổng thuế TNCN, Tổng Bảo hiểm) và bảng lương chi tiết. Người dùng có thể bấm vào từng nhân viên để mở Phiếu lương chi tiết (`Payslip Modal`) hoặc bấm nút Xuất CSV.
- Cách xem Phiếu lương cá nhân (`/my-payslip`) dành cho mọi nhân viên:
  + Nhân viên truy cập menu Phiếu lương của tôi (`/my-payslip`), chọn tháng và năm cần tra cứu để xem chi tiết từng khoản cộng (lương ngày công, phụ cấp, số giờ OT đã duyệt, tiền lương OT, tiền thưởng) và từng khoản trừ (bảo hiểm 10.5%, thuế TNCN, tiền phạt) cùng tổng tiền thực nhận về tài khoản.

### 13. Báo cáo Thống kê và Xuất file CSV (`/reports`)
- Truy cập menu Báo cáo (`/reports`), người quản lý và kế toán có thể chuyển đổi giữa 2 tab báo cáo:
  + Tại Tab Báo cáo Quỹ lương: Chọn tháng và năm rồi bấm Lọc để xem thống kê tổng số nhân viên được trả lương, tổng quỹ lương cơ bản, tổng quỹ lương thực chi (Net), mức lương trung bình, mức lương cao nhất và thấp nhất trong tháng, hoặc bấm Xuất CSV để tải báo cáo lương.
  + Tại Tab Báo cáo Nghỉ phép: Chọn khoảng thời gian từ ngày đến ngày rồi bấm Lọc để xem thống kê tổng số đơn xin nghỉ phép, số đơn đang chờ duyệt, đã duyệt, từ chối, đã hủy kèm thanh biểu đồ tỷ lệ trực quan, hoặc bấm Xuất CSV để tải báo cáo nghỉ phép.
  + Bấm nút Xuất toàn bộ danh sách nhân sự CSV ở góc phải trên cùng để tải ngay file hồ sơ nhân sự toàn công ty.

### 14. Quản trị Phân quyền 6 tầng Role và Quyền hạn (`/roles`)
- Dành riêng cho tài khoản `Admin` tại menu Phân quyền (`/roles`):
  + Xem danh sách toàn bộ 6 cấp Role trong hệ thống (`Admin`, `HRManager`, `HRStaff`, `Accountant`, `DepartmentManager`, `Employee`), tạo thêm Role mới hoặc bấm vào từng Role để cấu hình các quyền chi tiết (`Role Claims`).
  + Xem danh sách toàn bộ tài khoản người dùng trong công ty, bấm nút Gán Role trên từng tài khoản để cấp thêm hoặc gỡ bỏ các cấp Role cho người dùng đó.
