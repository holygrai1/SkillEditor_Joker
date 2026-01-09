using JKFrame;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShortcutSkillSlot : UI_SkillSlotBase
{
    [SerializeField] private Image cdMask;
    private int slotIndex;  // 第几个格子
    public int skillIndex { get; private set; }
    public void Init(int slotIndex)
    {
        this.slotIndex = slotIndex;
        UpadteCDTime(0);
        Init();
    }

    public void Show(int skillIndex, SkillConfig skillConfig)
    {
        this.skillIndex = skillIndex;
        this.skillConfig = skillConfig;
        Show(skillConfig);
        InputManager.Instance.BindSkillKeyCode(slotIndex, skillIndex);
    }

    public void ChangeSkill(int newSkillIndex)
    {
        UISystem.GetWindow<UI_GameSceneMainWindow>().ChangeShortcutSkill(slotIndex, newSkillIndex);
    }

    protected override void OnDragToNewSlot(UI_SkillSlotBase other)
    {
        // 非同类则丢弃，也就是变成空格子
        if (other is not UI_ShortcutSkillSlot)
        {
            ChangeSkill(-1);
            return;
        }
        // 同类则交换
        if (other is UI_ShortcutSkillSlot)
        {
            UI_ShortcutSkillSlot otherShortcutSkillSlot = (UI_ShortcutSkillSlot)other;

            int tempSkillIndex = this.skillIndex;
            // 自己变成对方的技能
            this.ChangeSkill(otherShortcutSkillSlot.skillIndex);
            // 对方变成当前旧的技能
            otherShortcutSkillSlot.ChangeSkill(tempSkillIndex);
        }
    }

    public void UpadteCDTime(float fillAmount)
    {
        cdMask.fillAmount = fillAmount;
    }
    public void UpdateCDTimeAndMaskColor(float fillAmount, Color color)
    {
        UpadteCDTime(fillAmount);
        cdMask.color = color;
    }

    public void UpdateSkillReleaseState(bool canRelease)
    {
        iconImage.color = canRelease ? Color.white : Color.gray;
    }

    public void UpdateIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }
}
