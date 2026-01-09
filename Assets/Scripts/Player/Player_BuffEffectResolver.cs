using UnityEngine;
public class Player_BuffEffectResolver : BuffEffectResolverBase
{
    [SerializeField] private Player_Controller player;
    public override void Resolve(Buff buff, BuffEffectDataBase effectData)
    {
        if (effectData is SimpleBuffEffectData)
        {
            SimpleBuffEffectData simpleBuffEffectData = (SimpleBuffEffectData)effectData;
            switch (simpleBuffEffectData.type)
            {
                case BuffEffectType.Hp:
                    Debug.Log($"由于{buff.config.name}Buff增加Hp:{simpleBuffEffectData.value * buff.layer}");
                    player.CharacterProperties.AddHP(simpleBuffEffectData.value);
                    break;
                case BuffEffectType.AttackValueMultiplier:
                    Debug.Log($"由于{buff.config.name}攻击力系数增加:{simpleBuffEffectData.value * buff.layer}");
                    player.CharacterProperties.attack.MultiplierBonus += simpleBuffEffectData.value;
                    break;
            }
        }
    }
}