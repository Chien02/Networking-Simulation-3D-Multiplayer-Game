using UnityEngine;
using TMPro; // Sử dụng TextMeshPro cho hiển thị đẹp hơn

public class LocalizedText : MonoBehaviour
{
    public string key; // Nhập key tương ứng trên Inspector (ví dụ: "welcome_msg")
    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Đăng ký sự kiện thay đổi ngôn ngữ
        EventManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        // Hủy đăng ký để tránh rò rỉ bộ nhớ
        EventManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (textComponent != null && LocalizationManager.Instance != null)
        {
            textComponent.text = LocalizationManager.Instance.GetText(key);
        }
    }
}