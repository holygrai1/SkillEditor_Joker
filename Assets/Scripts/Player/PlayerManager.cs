using JKFrame;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonMono<PlayerManager>
{
    [SerializeField] private Player_Controller player;
    public CharacterConfig characterConfig { get; private set; }
    public void Init(GameData gameData)
    {
        // 根据不同的职业，获取不同的角色配置
        characterConfig = ResSystem.LoadAsset<CharacterConfig>(gameData.ProfessionType.ToString() + "Config");
        player.Init(characterConfig, gameData);
        InputManager.Instance.Init(true);
        UISystem.Show<UI_GameSceneMainWindow>().Show(gameData.ShortcutSkillSlotData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && UISystem.GetWindow<UI_SkillLearnWindow>() == null)
        {
            UISystem.Show<UI_SkillLearnWindow>().Init(DataManager.GameData.SkillLearnedDatas);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            UI_SkillWindow skillWindow = UISystem.GetWindow<UI_SkillWindow>();
            if (skillWindow == null || !skillWindow.gameObject.activeInHierarchy)
            {
                UISystem.Show<UI_SkillWindow>().Show(DataManager.GameData.SkillLearnedDatas);
            }
            else
            {
                UISystem.Close<UI_SkillWindow>();
            }
        }
    }

    public List<SkillConfig> GetAllSkillConfig()
    {
        return characterConfig.skillCofnigList;
    }

    public void AddSkill(int skillIndex, SkillLearnedData skillLearnedData)
    {
        player.SkillBrain.AddSkill(player, GetAllSkillConfig(), skillIndex, skillLearnedData);
    }
}
