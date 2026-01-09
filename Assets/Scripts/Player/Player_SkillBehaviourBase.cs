public abstract class Player_SkillBehaviourBase : SkillBehaviourBase
{
    protected Player_Controller player;
    public override void Init(ICharacter owner, SkillConfig skillConfig, SkillBrainBase skillBrain, Skill_Player skill_Player, SkillLearnedData learnedData, int skillIndex)
    {
        base.Init(owner, skillConfig, skillBrain, skill_Player, learnedData, skillIndex);
        player = (Player_Controller)owner;
    }
}
