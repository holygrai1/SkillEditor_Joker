using JKFrame;
using UnityEngine;
using UnityEngine.UI;

[UIWindowData(nameof(UI_SkillInfoPopupWindow), true, nameof(UI_SkillInfoPopupWindow), 1)]
public class UI_SkillInfoPopupWindow : UI_WindowBase
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    private RectTransform rectTransform => (RectTransform)transform;
    public void Show(Vector3 slotWorldPosition, float topOffset, SkillConfig skillConfig)
    {
        // 位置的计算
        transform.position = slotWorldPosition;   // 直接等于格子的世界坐标
        Vector2 windowSize = rectTransform.sizeDelta;
        Vector3 uiPos = rectTransform.anchoredPosition;
        uiPos.y += topOffset; // 偏移多少个屏幕像素
        Vector2 canvasSize = GameManager.canvasSize;
        Vector2 widhtRange = new Vector2(canvasSize.x / -2 + windowSize.x / 2, canvasSize.x / 2 - windowSize.x / 2);
        Vector2 heightRange = new Vector2(canvasSize.y / -2, canvasSize.y / 2 - windowSize.y / 2);
        uiPos.x = Mathf.Clamp(uiPos.x, widhtRange.x, widhtRange.y);
        uiPos.y = Mathf.Clamp(uiPos.y, heightRange.x, heightRange.y);
        rectTransform.anchoredPosition = uiPos;
        // 显示技能信息
        iconImage.sprite = skillConfig.skillIcon;
        nameText.text = skillConfig.skillName;
        descriptionText.text = skillConfig.skillDescription;
    }
}
