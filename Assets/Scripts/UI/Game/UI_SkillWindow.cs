using JKFrame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[UIWindowData(nameof(UI_SkillWindow), true, nameof(UI_SkillWindow), 1)]
public class UI_SkillWindow : UI_WindowBase
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform releaseSkillSlotRoot;
    [SerializeField] private Transform passiveSkillSlotRoot;
    [SerializeField] private GameObject slotPrefab;
    private const int slotCount = 12;
    private List<UI_SkillWindow_Slot> releaseSkillSlots = new List<UI_SkillWindow_Slot>(slotCount);
    private List<UI_SkillWindow_Slot> passiveSkillSlots = new List<UI_SkillWindow_Slot>(slotCount);
    public override void Init()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        for (int i = 0; i < slotCount; i++)
        {
            UI_SkillWindow_Slot slot1 = GameObject.Instantiate(slotPrefab, releaseSkillSlotRoot).GetComponent<UI_SkillWindow_Slot>();
            UI_SkillWindow_Slot slot2 = GameObject.Instantiate(slotPrefab, passiveSkillSlotRoot).GetComponent<UI_SkillWindow_Slot>();
            slot1.Init();
            slot2.Init();
            releaseSkillSlots.Add(slot1);
            passiveSkillSlots.Add(slot2);
        }
    }
    public override void OnShow()
    {
        InputManager.Instance.CharacterControl = false;
    }
    public override void OnClose()
    {
        if (UI_SkillSlotBase.currentEnterSlot != null)
        {
            UI_SkillSlotBase.currentEnterSlot.OnPointerExit(null);
        }

        InputManager.Instance.CharacterControl = true;
    }
    private void CloseButtonClick()
    {
        UISystem.Close<UI_SkillWindow>();
    }

    public void Show(SkillLearnedDatas skillLearnedDatas)
    {
        int releaseSkillIndex = 0;
        int passiveSkillIndex = 0;
        List<SkillConfig> skillConfigs = PlayerManager.Instance.GetAllSkillConfig();
        foreach (KeyValuePair<int, SkillLearnedData> item in skillLearnedDatas.skillLearnedDataDic.Dictionary)
        {
            SkillConfig skillConfig = skillConfigs[item.Key];
            if (skillConfig.canRelease) // 主动技能
            {
                releaseSkillSlots[releaseSkillIndex].Show(item.Value, item.Key, skillConfig, true);
                releaseSkillIndex += 1;
            }
            else
            {
                passiveSkillSlots[passiveSkillIndex].Show(item.Value, item.Key, skillConfig, false);
                passiveSkillIndex += 1;
            }
        }

        for (int i = releaseSkillIndex; i < slotCount; i++)
        {
            releaseSkillSlots[i].Show(null, -1, null, false);
        }


        for (int i = passiveSkillIndex; i < slotCount; i++)
        {
            passiveSkillSlots[i].Show(null, -1, null, false);
        }
    }
}
