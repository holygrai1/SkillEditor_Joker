using JKFrame;
using UnityEngine;
public class GameSceneManager : SingletonMono<GameSceneManager>
{
    #region 测试逻辑
    public bool IsTest;
    public bool IsCreateArchive;
    #endregion
    private void Start()
    {
        #region 测试逻辑
        if (IsTest)
        {
            if (IsCreateArchive)
            {
                DataManager.CreateArchive();
            }
            else
            {
                DataManager.LoadCurrentArchive();
            }
        }
        #endregion
        // 初始化角色
        PlayerManager.Instance.Init(DataManager.GameData);
    }

    private void OnDestroy()
    {
        // TODO:模拟游戏退出时的存档
        DataManager.SaveGameData();
    }
}
