using JKFrame;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill/SkillConfig", fileName = "SkillConfig")]
public class SkillConfig : ConfigBase
{
    public string skillName;
    public Sprite skillIcon;
    public Sprite[] skillIcons;
    public string skillDescription;
    public int skillPoint;
    public bool canRelease;
    public int maxLV;
    public float baseCDTime;
    public float baseAttackValue;

    public float cdTimeMultiplierPerLV;     // CD每等级会减少多少
    public float attackValueMultiplierPerLV;// 攻击力每等级会增加多少

    public Dictionary<SkillCostType, float> releaseCostDic = new Dictionary<SkillCostType, float>();
    public SkillClip[] Clips; // 全部的技能片段
    public SkillBehaviourBase Behaviour; // 技能的运行逻辑

    public float GetAttackValueByLV(int lv)
    {
        float value = baseAttackValue * ((lv - 1) * attackValueMultiplierPerLV + 1);
        return (float)System.Math.Round(value, 2);
    }
    public float GetCDTimeByLV(int lv)
    {
        float value = baseCDTime * (1 - (lv - 1) * cdTimeMultiplierPerLV);
        return (float)System.Math.Round(value, 2);
    }
}
