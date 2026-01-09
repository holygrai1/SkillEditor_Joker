using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 头发配置
/// </summary>
[CreateAssetMenu(fileName = "ClothConfig_", menuName = "Config/ClothConfig")]
public class ClothConfig : CharacterPartConfigBase
{
    [LabelText("主色Index")] public int ColorIndex1;
    [LabelText("衣领色Index")] public int ColorIndex2;
}
