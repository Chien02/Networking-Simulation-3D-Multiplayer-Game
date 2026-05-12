# Networking Simulation 3D Multiplayer Game

## 📖 Giới thiệu chung

**Networking Simulation 3D Multiplayer Game** là một dự án trò chơi 3D đa người chơi kết hợp giải đố và mô phỏng mạng lưới truyền thông. Trong trò chơi này, người chơi sẽ nhận các nhiệm vụ (câu đố) yêu cầu thiết lập một mạng lưới truyền tải thông tin đầy đủ và chính xác. Trò chơi đề cao tính hợp tác, nơi các người chơi có thể cùng nhau kết hợp các kỹ năng và công cụ để vượt qua thử thách dễ dàng hơn.

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



---
