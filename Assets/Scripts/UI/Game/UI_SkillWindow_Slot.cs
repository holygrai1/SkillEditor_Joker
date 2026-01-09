using JKFrame;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillWindow_Slot : UI_SkillSlotBase
{
    [SerializeField] private Text lvText;
    private int skillIndex = -1;  // 对应的技能索引，-1代表其无效
    public void Show(SkillLearnedData learnedData, int skillIndex, SkillConfig skillConfig, bool canRelease)
    {
        base.Show(skillConfig);
        this.skillIndex = skillIndex;
        lvText.gameObject.SetActive(learnedData != null);
        if (learnedData != null)
        {
            lvText.text = $"LV{learnedData.lv}";
        }
    }
    protected override void OnDragToNewSlot(UI_SkillSlotBase other)
    {
        // 忽视同类
        if (other is UI_SkillWindow_Slot) return;
        // 拖拽到快捷栏
        if (other is UI_ShortcutSkillSlot)
        {
            // 避免在快捷栏中重复技能
            UI_GameSceneMainWindow mainWIndow = UISystem.GetWindow<UI_GameSceneMainWindow>();
            if (mainWIndow.TryGetShortcutSkillSlotIndex(skillIndex, out int slotIndex))
            {
                mainWIndow.ChangeShortcutSkill(slotIndex, -1);
            }
            ((UI_ShortcutSkillSlot)other).ChangeSkill(skillIndex);
        }
    }
}
