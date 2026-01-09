using UnityEngine;

public class SkillAttackDetectionEvent
{
#if UNITY_EDITOR
    public string TrackName = "攻击检测轨道";
#endif
    public int FrameIndex = 0;
    public int DurationFrame = 10;
    public AttackDetectionDataBase AttackDetectionData;
    public AttackHitConfig AttackHitConfig = new AttackHitConfig();
    public AttackDetectionType GetAttackDetectionType()
    {
        if (AttackDetectionData == null) return AttackDetectionType.None;
        if (AttackDetectionData is AttackWeaponDetectionData) return AttackDetectionType.Weapon;
        if (AttackDetectionData is AttackBoxDetectionData) return AttackDetectionType.Box;
        if (AttackDetectionData is AttackSphereDetectionData) return AttackDetectionType.Sphere;
        if (AttackDetectionData is AttackFanDetectionData) return AttackDetectionType.Fan;
        return AttackDetectionType.None;
    }

#if UNITY_EDITOR
    public AttackDetectionType AttackDetectionType
    {
        get
        {
            return GetAttackDetectionType();
        }
        set
        {
            if (value != AttackDetectionType) // 类型发生了变化
            {
                switch (value)
                {
                    case AttackDetectionType.None:
                        AttackDetectionData = null;
                        break;
                    case AttackDetectionType.Weapon:
                        AttackDetectionData = new AttackWeaponDetectionData();
                        break;
                    case AttackDetectionType.Box:
                        AttackDetectionData = new AttackBoxDetectionData();
                        break;
                    case AttackDetectionType.Sphere:
                        AttackDetectionData = new AttackSphereDetectionData();
                        break;
                    case AttackDetectionType.Fan:
                        AttackDetectionData = new AttackFanDetectionData();
                        break;

                }
            }
        }

    }
#endif
}
#region 检测
// 攻击检测类型
public enum AttackDetectionType
{
    None, Weapon, Box, Sphere, Fan
}

// 攻击检测基类
public abstract class AttackDetectionDataBase
{ }

// 武器检测
public class AttackWeaponDetectionData : AttackDetectionDataBase
{
    public string weaponName;
}

// 形状检测
public abstract class AttackShapeDetectionDataBase : AttackDetectionDataBase
{
    public Vector3 Position;
}

// 盒型检测
public class AttackBoxDetectionData : AttackShapeDetectionDataBase
{
    public Vector3 Rotation;
    public Vector3 Scale = Vector3.one;
}

// 球形检测
public class AttackSphereDetectionData : AttackShapeDetectionDataBase
{
    public float Radius = 1;
}

// 扇形检测
public class AttackFanDetectionData : AttackShapeDetectionDataBase
{
    public Vector3 Rotation;
    public float InsideRadius = 1;
    public float Radius = 3;
    public float Height = 0.5f;
    public float Angle = 90;
}
#endregion
#region 命中
public class AttackHitConfig
{
    public float AttackMultiply;
    public Vector3 RepelStrength; // 0,0,2
    public float RepelTime; // 1秒 位移了两米
    public GameObject HitEffectPrefab;
    public AudioClip HitAudioClip;
}
#endregion