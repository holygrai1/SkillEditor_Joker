using JKFrame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[UIWindowData(nameof(UI_SkillLearnWindow), false, nameof(UI_SkillLearnWindow), 1)]
public class UI_SkillLearnWindow : UI_WindowBase
{
    private class ItemInfo
    {
        public int skillIndex;
        public SkillConfig skillConfig;
        public SkillLearnedData skillLearnedData;
        public UI_SkillLearnWindow_Item item;
    }

    [SerializeField] private Transform itemRoot;
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private Button backButton;
    [SerializeField] private Button learnButton;
    [SerializeField] private Text skillTotalPointText;
    [SerializeField] private Text skillDescription;
    [SerializeField] private Text skillCDTimerText;
    [SerializeField] private Text skillPointText;
    [SerializeField] private Text skillAttackValueText;

    private List<UI_SkillLearnWindow_Item> itemList;
    private SkillLearnedDatas skillLearnedDatas;
    public override void Init()
    {
        backButton.onClick.AddListener(BackButtonClick);
        learnButton.onClick.AddListener(LearnButtonClick);
    }

    public void Init(SkillLearnedDatas skillLearnedDatas)
    {
        this.skillLearnedDatas = skillLearnedDatas;
        // 拿到全部技能配置
        List<SkillConfig> skillConfigList = PlayerManager.Instance.GetAllSkillConfig();
        itemList = new List<UI_SkillLearnWindow_Item>(skillConfigList.Count);
        for (int i = 0; i < skillConfigList.Count; i++)
        {
            UI_SkillLearnWindow_Item item = CreateItem();
            skillLearnedDatas.skillLearnedDataDic.Dictionary.TryGetValue(i, out SkillLearnedData skillLearnedData);
            item.Init(skillConfigList[i], skillLearnedData);
            ItemInfo itemInfo = new ItemInfo { skillIndex = i, skillLearnedData = skillLearnedData, skillConfig = skillConfigList[i], item = item };
            item.OnClickDown(OnSelectItem, itemInfo);
            itemList.Add(item);
            if (i == 0) OnSelectItem(null, itemInfo); // 默认选择第0个
        }
        // 更新技能点
        UpadteSkillTotalPoint(skillLearnedDatas.skillPoints);
    }

    private ItemInfo seletecdItemInfo;
    private void OnSelectItem(PointerEventData data, ItemInfo newItemInfo)
    {
        if (seletecdItemInfo == newItemInfo) return;
        newItemInfo.item.Select();
        if (seletecdItemInfo != null) seletecdItemInfo.item.UnSelect();
        seletecdItemInfo = newItemInfo;
        UpdateRightPanel(newItemInfo);
    }

    private void UpdateRightPanel(ItemInfo itemInfo)
    {
        skillDescription.text = itemInfo.skillConfig.skillDescription;
        skillPointText.text = $"升级所需技能点：{itemInfo.skillConfig.skillPoint}";
        int lv = itemInfo.skillLearnedData == null ? 1 : itemInfo.skillLearnedData.lv;
        skillCDTimerText.text = $"冷却时间：{itemInfo.skillConfig.GetCDTimeByLV(lv)}/{itemInfo.skillConfig.baseCDTime}";
        if (itemInfo.skillConfig.baseAttackValue == 0)
        {
            skillAttackValueText.gameObject.SetActive(false);
        }
        else
        {
            skillAttackValueText.gameObject.SetActive(true);
            skillAttackValueText.text = $"攻击力：{itemInfo.skillConfig.GetAttackValueByLV(lv)}/{itemInfo.skillConfig.baseAttackValue}";
        }

        // 如果满级禁止学习
        if (itemInfo.skillLearnedData != null && itemInfo.skillLearnedData.lv == itemInfo.skillConfig.maxLV)
        {
            learnButton.interactable = false;
        }
        // 技能点不够禁止学习
        else if (skillLearnedDatas.skillPoints < itemInfo.skillConfig.skillPoint)
        {
            learnButton.interactable = false;
        }
        else learnButton.interactable = true;
    }


    private UI_SkillLearnWindow_Item CreateItem()
    {
        return GameObject.Instantiate(itemPrefab, itemRoot).GetComponent<UI_SkillLearnWindow_Item>();
    }

    public override void OnShow()
    {
        InputManager.Instance.CharacterControl = false;
    }
    public override void OnClose()
    {
        InputManager.Instance.CharacterControl = true;
    }

    private void BackButtonClick()
    {
        UISystem.Close<UI_SkillLearnWindow>();
    }

    private void LearnButtonClick()
    {
        if (!skillLearnedDatas.skillLearnedDataDic.Dictionary.TryGetValue(seletecdItemInfo.skillIndex, out SkillLearnedData skillLearnedData))
        {
            skillLearnedData = new SkillLearnedData();
            skillLearnedData.lv = 1;
            seletecdItemInfo.skillLearnedData = skillLearnedData;
            skillLearnedDatas.skillLearnedDataDic.Dictionary.Add(seletecdItemInfo.skillIndex, skillLearnedData);
            // 新学习的技能需要通知玩家
            PlayerManager.Instance.AddSkill(seletecdItemInfo.skillIndex, skillLearnedData);
        }
        else
        {
            skillLearnedData.lv += 1;
        }
        skillLearnedDatas.skillPoints -= seletecdItemInfo.skillConfig.skillPoint; // 扣除技能点
        UpadteSkillTotalPoint(skillLearnedDatas.skillPoints);
        seletecdItemInfo.item.Init(seletecdItemInfo.skillConfig, skillLearnedData);
        UpdateRightPanel(seletecdItemInfo);
    }

    private void UpadteSkillTotalPoint(int count)
    {
        skillTotalPointText.text = count.ToString();
    }
}
