using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private Dictionary<string, string> localizedText;
    private string currentLanguage = "en"; // Mặc định là tiếng Anh

    private void Awake()
    {
        // Đảm bảo chỉ có 1 Manager duy nhất trong suốt game (Singleton)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguage(currentLanguage);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm tải ngôn ngữ từ file JSON
    public void LoadLanguage(string langCode)
    {
        currentLanguage = langCode;
        string filePath = Path.Combine(Application.streamingAssetsPath, langCode + ".json");

        if (File.Exists(filePath))
        {
            string jsonText = File.ReadAllText(filePath);
            
            // Chuyển đổi JSON thành Dictionary thông qua một class trung gian
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(jsonText);
            
            localizedText = new Dictionary<string, string>();
            foreach (var item in loadedData.items)
            {
                localizedText.Add(item.key, item.value);
            }

            Debug.Log($"Đã chuyển sang ngôn ngữ: {langCode}");
            
            // Thông báo cho tất cả các Text trong game cập nhật lại giao diện
            EventManager.TriggerLanguageChanged();
        }
        else
        {
            Debug.LogError($"Không tìm thấy file ngôn ngữ tại: {filePath}");
        }
    }

    // Hàm lấy chuỗi văn bản dịch dựa vào Key
    public string GetText(string key, params object[] args)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
        {
            string rawText = localizedText[key];
            // Nếu có tham số truyền vào (ví dụ: Điểm số: {0}) thì format chuỗi
            return args.Length > 0 ? string.Format(rawText, args) : rawText;
        }
        return $"[{key}]"; // Trả về dạng lỗi nếu không tìm thấy key
    }
}

// Các class phụ trợ để Unity có thể Deserialize từ JSON
[System.Serializable]
public class LocalizationData
{
    public List<LocalizationItem> items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}