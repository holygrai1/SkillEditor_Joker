using UnityEngine;
using UnityEngine.UI;

public class UI_SkillLearnWindow_Item : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text lvText;
    [SerializeField] private Text canReleaseText;
    [SerializeField] private Color normalCorlor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    public void Init(SkillConfig skillConfig, SkillLearnedData skillLearnedData)
    {
        canReleaseText.text = skillConfig.canRelease ? "主动" : "被动";
        skillNameText.text = skillConfig.skillName;
        skillIcon.sprite = skillConfig.skillIcon;

        if (skillLearnedData != null) lvText.text = "LV " + skillLearnedData.lv;
        else lvText.text = "LV 0";
    }
    public void Select()
    {
        bgImage.color = selectedColor;
    }

    public void UnSelect()
    {
        bgImage.color = normalCorlor;
    }
}
