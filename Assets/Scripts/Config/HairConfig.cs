using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ???????
/// </summary>
[CreateAssetMenu(fileName = "HairConfig_", menuName = "Config/HairConfig")]
public class HairConfig : CharacterPartConfigBase
{
    /// <summary>
    /// ???????,-1????ζ????Ч
    /// </summary>
    [LabelText("???Index")] public int ColorIndex;
    [LabelText("???????")] public Mesh Mesh2;
}
