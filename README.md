# MorgueManager.Web

> [!WARNING]
> **THÔNG BÁO TỪ QUẢN TRỊ VIÊN (LEADER)**
> Hiện tại tỷ lệ đóng góp (Commit) của các thành viên đang bị mất cân bằng nghiêm trọng. Đề nghị các thành viên đẩy nhanh tiến độ làm việc để hoàn thành các tính năng và kéo lại % đóng góp của mình (Mục tiêu: mỗi thành viên cần đạt ít nhất 15-20 commits).

### 📊 Bảng thống kê đóng góp (Tự động cập nhật)
<!-- START_STATS -->
| Thành viên | Số lượng Commits | Tỷ lệ (%) | Trạng thái |
|---|---|---|---|
| **BDTG (Leader)** | 173 | ~66.5% | 🟢 Vượt chỉ tiêu |
| **newiexk-cyber** | 33 | ~12.7% | 🔴 Cần cố gắng hơn |
| **Simpson-31ev3n** | 29 | ~11.2% | 🔴 Cần cố gắng hơn |
| **khanhphamvn222** | 25 | ~9.6% | 🔴 Cần cố gắng hơn |

*(Tổng số commits dự án: 260)*
<!-- END_STATS -->

---

## 💡 Về dự án MorgueManager
**MorgueManager** là một hệ thống phần mềm chuyên dụng quản lý nhà xác (Morgue) dành cho các bệnh viện và cơ sở y tế. Hệ thống giúp số hóa toàn bộ quy trình tiếp nhận, lưu trữ, giám sát và bàn giao thi thể, đảm bảo tính minh bạch, chính xác và tuân thủ các quy định y tế/pháp lý nghiêm ngặt.

### 🚀 Công nghệ sử dụng (Tech Stack)
* **Kiến trúc Frontend:** Blazor WebAssembly Standalone (.NET 10). Toàn bộ ứng dụng chạy trực tiếp trên trình duyệt của người dùng (Client-Side Rendering) giúp giảm tải hoàn toàn cho Server, tốc độ chuyển trang tức thời không độ trễ.
* **Giao diện (UI/UX):** Thiết kế độc bản, chuẩn Premium với Tailwind CSS. Tích hợp thư viện GSAP và Lenis để xử lý các hiệu ứng hoạt ảnh vi mô (micro-animations), hỗ trợ Dark/Light Mode và giao diện Responsive 100% trên thiết bị di động.
* **Backend & Cơ sở dữ liệu:** Hệ thống sử dụng Supabase đóng vai trò Backend-as-a-Service (BaaS) với lõi là PostgreSQL. Giao tiếp dữ liệu thông qua RESTful API chuẩn mực.
* **Xác thực bảo mật (Authentication):** Tích hợp Supabase Auth với giao thức OAuth 2.0 (Google Sign-In), đảm bảo định danh nhân sự chặt chẽ, an toàn.
* **Tự động hóa Triển khai (CI/CD):** Xây dựng luồng DevOps tự động với GitHub Actions, tự động Build và Deploy lên hạ tầng Cloudflare Pages chỉ trong 1-2 phút sau mỗi lần đẩy code.

### ✨ Các tính năng cốt lõi đã hoàn thiện
1. **Số hóa Toàn diện Hồ sơ Pháp lý & Y tế:**
   * Quản lý không chỉ thông tin cá nhân cơ bản mà còn theo dõi các trường dữ liệu pháp y/y tế chuyên sâu: Mã giấy chứng tử, Phân loại nhóm bệnh lây nhiễm (A, B, C), Trạng thái thi thể vô danh (Unidentified).
   * Minh bạch hóa quy trình quản lý tư trang, biên bản giao nhận và tài liệu đính kèm (PDF, Hình ảnh).
2. **Sơ đồ Tủ lưu trữ Trực quan (Visual Storage Map):**
   * Thay vì hiển thị danh sách nhàm chán, hệ thống vẽ sơ đồ các ngăn lạnh/đông sâu (-16°C) với trạng thái thời gian thực (Trống, Đang sử dụng, Bảo trì).
3. **Hệ thống Logic Cảnh báo Thời gian (Auto-Retention Warning):**
   * "Bộ não" của hệ thống được lập trình sát với quy định của Luật Y tế:
     * *Nhóm lây nhiễm (Ví dụ: Covid-19 nhóm A):* Cảnh báo đếm ngược khẩn cấp màu đỏ tối đa 24 giờ.
     * *Nhóm vô danh/pháp y:* Cảnh báo tự động mốc 30 ngày chờ thân nhân.
     * *Nhóm thông thường:* Cảnh báo hạn 7 ngày.
   * Giúp bệnh viện và quản lý không bao giờ vi phạm pháp luật quy định về thời gian lưu trữ xác.

### 🔮 Hướng phát triển tiếp theo (Future Works)
1. **Tích hợp phần cứng IoT theo thời gian thực (IoT Sensor Integration):**
   * Sử dụng vi điều khiển (như ESP32) kết nối với các cảm biến đo nhiệt độ/độ ẩm gắn trực tiếp vào các ngăn tủ xác lạnh truyền dữ liệu qua MQTT.
   * Tự động phát còi báo động và gọi API gửi tin nhắn khẩn cấp qua SMS/Zalo/Telegram cho nhân viên trực nếu tủ mất điện hoặc nhiệt độ vượt ngưỡng an toàn.
2. **Ứng dụng AI & Thị giác Máy tính (Computer Vision):**
   * **Nhận diện khuôn mặt (Face Recognition):** Tích hợp AI quét và đối chiếu đặc điểm khuôn mặt cho các ca thi thể vô danh, hỗ trợ lực lượng chức năng.
   * **Quét QR/Barcode chống nhầm lẫn:** Ứng dụng quét mã vạch trên vòng định danh thi thể ở khâu Bàn giao, ngăn chặn tuyệt đối rủi ro giao nhầm thi thể cho gia đình.
3. **Mở rộng Hệ sinh thái (Native Mobile App):**
   * Phát triển thêm ứng dụng di động (sử dụng React Native hoặc .NET MAUI) dành riêng cho nhân viên kỹ thuật và bác sĩ pháp y. Giúp họ thao tác, chụp ảnh hiện trạng thi thể và upload thẳng lên hệ thống ngay dưới hầm nhà xác bằng điện thoại di động.
4. **Ứng dụng Blockchain vào Hồ sơ Pháp lý (Forensic Blockchain):**
   * Băm (Hash) các bản ghi quan trọng (kết quả khám nghiệm, thời gian tiếp nhận/bàn giao) và đưa lên chuỗi khối (Blockchain) để tạo ra tính chất "bất biến" (Immutable) tuyệt đối. Đảm bảo hồ sơ pháp y không bao giờ bị hacker hay quản trị viên can thiệp sửa đổi trái phép.

---

### Kế hoạch phân công hiện tại
Các bạn vui lòng kiểm tra trực tiếp tab **[Issues](https://github.com/BDTG/MorgueManager.Web/issues)** để nhận các đầu mục công việc và tiến hành fix lỗi. Đã có 4 issues quan trọng được gán tên đích danh.