Chào bạn, dự án luận văn của bạn rất thú vị và có ý tưởng vô cùng sáng tạo khi kết hợp kiến thức mạng máy tính khô khan vào một tựa game 3D giải đố mang tính hợp tác.

Dưới đây là bản nháp cho file `README.md` hoặc phần Description trên GitHub repo của bạn. Nó được tổng hợp và cấu trúc lại dựa trên tài liệu GDD và Báo cáo tiến độ bạn đã cung cấp để mang lại tính chuyên nghiệp nhất:

---

# Networking Simulation 3D Multiplayer Game

## 📖 Giới thiệu chung

**Networking Simulation 3D Multiplayer Game** là một dự án trò chơi 3D đa người chơi kết hợp giải đố và mô phỏng mạng lưới truyền thông. Trong trò chơi này, người chơi sẽ nhận các nhiệm vụ (câu đố) yêu cầu thiết lập một mạng lưới truyền tải thông tin đầy đủ và chính xác. Trò chơi đề cao tính hợp tác, nơi các người chơi có thể cùng nhau kết hợp các kỹ năng và công cụ để vượt qua thử thách dễ dàng hơn.

Dự án được phát triển bởi sinh viên **Nguyễn Trịnh Trọng Chiến (B2204924)** - chuyên ngành Mạng máy tính và Truyền thông dữ liệu K48.

## 🎮 Gameplay Cốt lõi

* 
**Core Gameloop:** Bắt đầu màn chơi, người chơi nhận nhiệm vụ giải quyết một vấn đề (câu đố) trong thời gian giới hạn. Người chơi sử dụng các công cụ được cung cấp để tạo mạng lưới truyền thông và phải đối mặt, ngăn chặn các mối đe dọa (threats) có ý đồ phá hoại mạng lưới.


* 
**Hệ thống Công cụ Mô phỏng:** Trò chơi cung cấp các công cụ mô phỏng lại các thiết bị mạng thực tế như router, switch, access point, repeater, modem, firewall, hub, bridge và card mạng. Môi trường truyền dẫn mô phỏng cáp mạng và wifi, nhưng được biến tấu hình hài theo bối cảnh game (ví dụ: bối cảnh trung cổ sử dụng dây thừng để truyền tin phép thuật).


* 
**Kỹ năng & Tiến trình:** Độ khó của game tăng dần theo tuyến tính. Người chơi thu thập các kỹ năng qua từng màn chơi cố định và sử dụng chúng để kết hợp với công cụ giải quyết các câu đố sau này.



## 🛠️ Công nghệ & Kiến trúc Mạng

Dự án được thiết kế song song giữa Client-side (xử lý logic) và Backend-side (lưu trữ dữ liệu).

* 
**Giải pháp mạng:** Trò chơi tích hợp **Photon Fusion**.


* 
**Kiến trúc mạng:** Sử dụng **Shared Mode**.


* Mỗi máy khách (Client) là một Network Object và tự quản lý logic của nó. Quyền kiểm soát (State Authority) được cấp ở cấp độ vật thể.


* 
**Photon Cloud** đóng vai trò là "Room State Keeper", chuyên lưu trữ snapshot dữ liệu của phòng (ai đang ở đâu, trạng thái vật thể) thay vì chạy logic game. Các Client nhận dữ liệu từ Cloud để render các Proxy object mà không trực tiếp can thiệp logic của người khác.




* 
**Lý do lựa chọn Shared Mode:** Phù hợp vì trò chơi giải đố không yêu cầu tốc độ xử lý quá nhanh, không cần kiểm tra gian lận khắt khe, tương thích với nhiều thiết bị và có nguồn tài liệu học tập dồi dào.



## 📅 Lộ trình Phát triển

Dự án được áp dụng framework **Scrum** với quỹ thời gian **15 tuần**, chia làm 7 Sprints (mỗi Sprint 2 tuần). Lộ trình đảm bảo sản phẩm liên tục được test và nhận feedback.

| Giai đoạn | Mục tiêu (Sprint Goal) |
| --- | --- |
| **Sprint 1 (Tuần 1-2)** | Xây dựng Core Gameplay (Offline): Nhân vật di chuyển mượt mà, cơ chế cốt lõi chạy trên Client.

 |
| **Sprint 2 (Tuần 3-4)** | Tích hợp Photon Fusion & Network Foundation: Tạo kết nối Lobby, Room và Join game.

 |
| **Sprint 3 (Tuần 5-6)** | Đồng bộ hóa (Synchronization): Đồng bộ hoạt ảnh (Animation) và di chuyển giữa các người chơi.

 |
| **Sprint 4 (Tuần 7-8)** | Xử lý Logic Multiplayer (Core Loop): Đồng bộ va chạm, nhặt đồ, tính toán sát thương/điểm số.

 |
| **Sprint 5 (Tuần 9-10)** | Vertical Slice: Hoàn thiện một ván đấu hoàn chỉnh với điều kiện Thắng/Thua.

 |
| **Sprint 6 (Tuần 11-12)** | Mở rộng nội dung: Thêm Map, UI/UX (Menu, Settings, Scoreboard) và âm thanh.

 |
| **Sprint 7 (Tuần 13-14)** | Tối ưu hóa & Bug Fixing: Xử lý Host migration, rớt mạng, tối ưu hiệu năng.

 |
| **Release (Tuần 15)** | Phát hành phiên bản PC/WebGL lên nền tảng **itch.io**.

 |

---
