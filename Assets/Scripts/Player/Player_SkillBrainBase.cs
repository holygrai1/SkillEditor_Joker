using System.Collections.Generic;

public abstract class Player_SkillBrainBase : SkillBrainBase
{
    protected Player_Controller player;
    public virtual void Init(Player_Controller player, SkillLearnedDatas learnedDatas)
    {
        base.Init(player);
        this.player = player;
        // 基于所学技能去初始化skillBehaviours，后续是需要学习加进来的
        skillBehaviours = new List<SkillBehaviourBase>(learnedDatas.skillLearnedDataDic.Dictionary.Count);
        List<SkillConfig> skillConfigs = PlayerManager.Instance.GetAllSkillConfig();
        foreach (KeyValuePair<int, SkillLearnedData> item in learnedDatas.skillLearnedDataDic.Dictionary)
        {
            AddSkill(player, skillConfigs, item.Key, item.Value);
        }
    }

    public override bool CheckCost(SkillCostType costType, float cost)
    {
        switch (costType)
        {
            case SkillCostType.MP:
                return player.CharacterProperties.currentMP >= -cost;
        }
        return false;
    }

    public override void ApplyCost(SkillCostType costType, float cost)
    {
        switch (costType)
        {
            case SkillCostType.MP:
                player.CharacterProperties.AddMP(cost);
                break;
        }
    }
}
