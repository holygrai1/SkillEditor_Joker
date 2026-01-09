using JKFrame;
using System.Collections.Generic;
using UnityEngine;
[UIWindowData(nameof(UI_GameSceneMainWindow), true, nameof(UI_GameSceneMainWindow), 1)]
public class UI_GameSceneMainWindow : UI_WindowBase
{

    #region 技能快捷栏
    [SerializeField] private UI_ShortcutSkillSlot[] shortcutSkillSlots;
    public void Show(ShortcutSkillSlotData shortcutSkillSlotData)
    {
        ShowShortcutSkillSlots(shortcutSkillSlotData);
    }

    public bool TryGetShortcutSkillSlotIndex(int skillIndex, out UI_ShortcutSkillSlot slot)
    {
        bool contain = TryGetShortcutSkillSlotIndex(skillIndex, out int slotIndex);
        if (contain) slot = shortcutSkillSlots[slotIndex];
        else slot = null;
        return contain;
    }

    public bool TryGetShortcutSkillSlotIndex(int skillIndex, out int slotIndex)
    {
        for (int i = 0; i < shortcutSkillSlots.Length; i++)
        {
            if (shortcutSkillSlots[i].skillIndex == skillIndex)
            {
                slotIndex = i;
                return true;
            }
        }
        slotIndex = -1;
        return false;
    }

    public void ShowShortcutSkillSlots(ShortcutSkillSlotData shortcutSkillSlotData)
    {
        List<SkillConfig> skillConfigs = PlayerManager.Instance.GetAllSkillConfig();
        for (int i = 0; i < shortcutSkillSlotData.skillIDs.Length; i++)
        {
            SkillConfig skillConfig = null;
            int skillIndex = shortcutSkillSlotData.skillIDs[i];
            if (skillIndex != -1) skillConfig = skillConfigs[skillIndex];
            shortcutSkillSlots[i].Init(i);
            shortcutSkillSlots[i].Show(skillIndex, skillConfig);
        }
    }

    public void ChangeShortcutSkill(int slotIndex, int newSkillIndex)
    {
        SkillConfig skillConfig = null;
        if (newSkillIndex != -1)
        {
            skillConfig = PlayerManager.Instance.GetAllSkillConfig()[newSkillIndex];
        }
        shortcutSkillSlots[slotIndex].Show(newSkillIndex, skillConfig);
        DataManager.GameData.ShortcutSkillSlotData.skillIDs[slotIndex] = newSkillIndex;
    }
    #endregion
    #region Buff栏
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private Transform buffSlotRoot;
    private List<UI_BuffSlot> buffSlotList = new List<UI_BuffSlot>();
    public UI_BuffSlot AddBuff(BuffConfig buffConfig)
    {
        UI_BuffSlot slot = ProjectUtility.GetOrInstantiateGameObject(buffSlotPrefab, buffSlotRoot).GetComponent<UI_BuffSlot>();
        buffSlotList.Add(slot);
        slot.Init(buffConfig);
        return slot;
    }

    public void RemoveBuff(UI_BuffSlot buffSlot)
    {
        buffSlot.Destroy();
        buffSlotList.Remove(buffSlot);
    }
    #endregion

}
