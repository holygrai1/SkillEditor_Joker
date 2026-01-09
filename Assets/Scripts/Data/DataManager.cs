using JKFrame;
using UnityEngine;
/// <summary>
/// 数据管理器
/// </summary>
public static class DataManager
{
    static DataManager()
    {
        LoadSaveData();
    }

    /// <summary>
    /// 是否有存档
    /// </summary>
    public static bool HaveArchive { get; private set; }

    private static void LoadSaveData()
    {
        SaveItem saveItem = SaveSystem.GetSaveItem(0);
        HaveArchive = saveItem != null;
    }

    /// <summary>
    /// 创建新存档
    /// </summary>
    public static void CreateArchive()
    {
        if (HaveArchive)
        {
            // 删除全部存档
            SaveSystem.DeleteAllSaveItem();
        }

        // 创建一个存档
        SaveSystem.CreateSaveItem();

        // 初始化角色外观数据
        InitGameData();
        SaveGameData();
    }

    /// <summary>
    /// 加载当前存档
    /// </summary>
    public static void LoadCurrentArchive()
    {
        GameData = SaveSystem.LoadObject<GameData>();
    }

    #region 玩家数据
    public static GameData GameData { get; private set; }
    public static void InitGameData()
    {
        GameData = new GameData();
        GameData.CustomPartDataDic = new Serialized_Dic<int, CustomCharacterPartData>();
        GameData.CustomPartDataDic.Dictionary.Add((int)CharacterPartType.Face, new CustomCharacterPartData()
        {
            Index = 1,
            Size = 1,
            Height = 0
        });
        GameData.CustomPartDataDic.Dictionary.Add((int)CharacterPartType.Hair, new CustomCharacterPartData()
        {
            Index = 1,
            Color1 = Color.white.ConverToSerializationColor()
        }); ;
        GameData.CustomPartDataDic.Dictionary.Add((int)CharacterPartType.Cloth, new CustomCharacterPartData()
        {
            Index = 1,
            Color1 = Color.white.ConverToSerializationColor(),
            Color2 = Color.black.ConverToSerializationColor()
        });
        GameData.SkillLearnedDatas = new SkillLearnedDatas
        {
            skillPoints = 1000
        };
        GameData.SkillLearnedDatas.skillLearnedDataDic.Dictionary.Add(0, new SkillLearnedData { lv = 1 });
        //GameData.SkillLearnedDatas.skillLearnedDataDic.Dictionary.Add(1, new SkillLearnedData { lv = 1 });
        //GameData.SkillLearnedDatas.skillLearnedDataDic.Dictionary.Add(2, new SkillLearnedData { lv = 2 });
        GameData.ShortcutSkillSlotData = new ShortcutSkillSlotData();
        GameData.ShortcutSkillSlotData.skillIDs = new int[6] { -1, -1, -1, -1, -1, -1 };
    }
    public static void SaveGameData()
    {
        SaveSystem.SaveObject(GameData);
    }
    #endregion

}
