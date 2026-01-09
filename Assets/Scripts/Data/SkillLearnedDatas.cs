using System;

[Serializable]
public class SkillLearnedDatas
{
    // Key:对应技能配置中的索引
    public Serialized_Dic<int, SkillLearnedData> skillLearnedDataDic = new Serialized_Dic<int, SkillLearnedData>();
    public int skillPoints;
}


[Serializable]
public class SkillLearnedData
{
    public int lv;
}