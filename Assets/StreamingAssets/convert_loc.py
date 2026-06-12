import os
import json
import pandas as pd

def export_excel_to_unity_json(excel_path, output_dir):
    # 1. Đọc file Excel
    try:
        df = pd.read_excel(excel_path)
    except Exception as e:
        print(f"Lỗi khi đọc file Excel: {e}")
        return

    # Kiểm tra xem cột 'key' có tồn tại không
    if 'key' not in df.columns:
        print("Lỗi: Không tìm thấy cột 'key' trong file Excel!")
        return

    # Lấy danh sách các ngôn ngữ (bỏ qua cột 'key')
    languages = [col for col in df.columns if col != 'key']
    
    # Tạo thư mục đầu ra nếu chưa có
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)

    # 2. Lặp qua từng cột ngôn ngữ để xuất file JSON tương ứng
    for lang in languages:
        lang_data = {"items": []}
        
        for index, row in df.iterrows():
            # Bỏ qua nếu dòng đó bị trống key
            if pd.isna(row['key']):
                continue
                
            # Lấy giá trị dịch, nếu trống thì để chuỗi rỗng
            val = str(row[lang]) if not pd.isna(row[lang]) else ""
            
            # Cấu trúc đúng định dạng List<LocalizationItem> cho Unity
            item = {
                "key": str(row['key']).strip(),
                "value": val.strip()
            }
            lang_data["items"].append(item)
            
        # 3. Ghi dữ liệu ra file JSON (Đảm bảo mã hóa UTF-8 để không lỗi dấu tiếng Việt)
        output_file_path = os.path.join(output_dir, f"{lang}.json")
        with open(output_file_path, 'w', encoding='utf-8') as f:
            json.dump(lang_data, f, ensure_ascii=False, indent=2)
            
        print(f" Đã xuất file thành công: {output_file_path}")

# --- CẤU HÌNH ĐƯỜNG DẪN TẠI ĐÂY ---
EXCEL_FILE = "localization.xlsx"  # Tên file Excel của bạn
OUTPUT_DIRECTORY = "."  # Thư mục chứa các file JSON đầu ra

# Chạy hàm
export_excel_to_unity_json(EXCEL_FILE, OUTPUT_DIRECTORY)