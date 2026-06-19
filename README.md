# MorgueManager.Web

> [!WARNING]
> **THÔNG BÁO TỪ QUẢN TRỊ VIÊN (LEADER)**
> Hiện tại tỷ lệ đóng góp (Commit) của các thành viên đang bị mất cân bằng nghiêm trọng. Đề nghị các thành viên đẩy nhanh tiến độ làm việc để hoàn thành các tính năng và kéo lại % đóng góp của mình (Mục tiêu: mỗi thành viên cần đạt ít nhất 15-20 commits).

### 📊 Bảng thống kê đóng góp (Tự động cập nhật)
<!-- START_STATS -->
| Thành viên | Số lượng Commits | Tỷ lệ (%) | Trạng thái |
|---|---|---|---|
| **BDTG (Leader)** | 128 | ~71.9% | 🟢 Vượt chỉ tiêu |
| **Simpson-31ev3n** | 18 | ~10.1% | 🔴 Cần cố gắng hơn |
| **khanhphamvn222** | 17 | ~9.6% | 🔴 Cần cố gắng hơn |
| **newiexk-cyber** | 15 | ~8.4% | 🔴 Cần cố gắng hơn |

*(Tổng số commits dự án: 178)*
<!-- END_STATS -->

---

## 💡 Về dự án MorgueManager
**MorgueManager** là một hệ thống phần mềm chuyên dụng quản lý nhà xác (Morgue) dành cho các bệnh viện và cơ sở y tế. Hệ thống giúp số hóa toàn bộ quy trình tiếp nhận, lưu trữ, giám sát và bàn giao thi thể, đảm bảo tính minh bạch, chính xác và tuân thủ các quy định y tế/pháp lý nghiêm ngặt.

### 🚀 Công nghệ sử dụng (Tech Stack)
* **Frontend:** Blazor WebAssembly (.NET 10), Tailwind CSS, Vanilla JS (GSAP Animations)
* **Backend / Database:** Supabase (PostgreSQL, RESTful API)
* **Authentication:** Supabase Auth (Google OAuth 2.0)
* **Deployment (CI/CD):** GitHub Actions -> Cloudflare Pages

### ✨ Các tính năng cốt lõi đã hoàn thiện
1. **Quản lý Hồ sơ Pháp lý & Y tế:**
   * Số hóa giấy chứng tử, biên bản giao nhận, quản lý tài liệu đính kèm (PDF, Hình ảnh).
   * Phân loại thi thể: Vô danh, Có danh tính, Lưu trữ pháp y.
   * Quản lý tư trang của người đã khuất minh bạch.
2. **Kiểm soát & Giám sát Lưu trữ:**
   * Sơ đồ trực quan các ngăn đông lạnh (Mô phỏng nhiệt độ thực tế 2-4°C hoặc đông sâu -16°C).
   * Hệ thống tự động tính toán và **cảnh báo hạn lưu giữ** dựa trên quy định pháp luật:
     * *Nhóm lây nhiễm (Nhóm A):* Cảnh báo đếm ngược tối đa 24 giờ.
     * *Nhóm vô danh:* Cảnh báo hạn 30 ngày chờ thân nhân.
     * *Nhóm thông thường:* Cảnh báo hạn 7 ngày.
3. **Bảo mật & Trải nghiệm (UI/UX):**
   * Đăng nhập an toàn qua tài khoản Google.
   * Giao diện Dark/Light mode cao cấp, tốc độ tải trang siêu nhanh nhờ kiến trúc WebAssembly Standalone.

### 🔮 Hướng phát triển tiếp theo (Future Works)
1. **Tích hợp IoT thực tế (Real-time IoT):** Kết nối trực tiếp với các cảm biến nhiệt độ/độ ẩm phần cứng tại nhà xác. Tự động gửi SMS/Zalo/Email cho quản lý khi tủ lạnh mất điện hoặc nhiệt độ vượt ngưỡng an toàn.
2. **AI & Computer Vision:** Tích hợp nhận diện khuôn mặt (Face Recognition) hoặc quét mã vạch/QR code thông minh để dán lên vòng định danh thi thể, ngăn chặn tuyệt đối rủi ro nhầm lẫn khi bàn giao.
3. **Ứng dụng Mobile:** Phát triển phiên bản Mobile App (MAUI) dành riêng cho nhân viên kỹ thuật dưới nhà xác thao tác nhanh trên điện thoại thay vì dùng máy tính.
4. **Blockchain:** Lưu vết toàn bộ lịch sử (Audit Log) bằng công nghệ chuỗi khối để đảm bảo hồ sơ pháp y không bao giờ bị can thiệp hay sửa đổi trái phép.

---

### Kế hoạch phân công hiện tại
Các bạn vui lòng kiểm tra trực tiếp tab **[Issues](https://github.com/BDTG/MorgueManager.Web/issues)** để nhận các đầu mục công việc và tiến hành fix lỗi. Đã có 4 issues quan trọng được gán tên đích danh.