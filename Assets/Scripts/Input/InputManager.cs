using JKFrame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMono<InputManager>
{
    [Serializable]
    public class Key
    {
        public KeyCode keyCode;
        public bool isCache;
        public float cacheTime;

        private float lastInputTime;
        public bool vaild;
        public bool GetState()
        {
            if (!isCache) return Input.GetKey(keyCode);
            return Input.GetKey(keyCode) || (Time.time - lastInputTime) < cacheTime;
        }

        public void Update()
        {
            if (!isCache) return;
            if (Input.GetKey(keyCode))
            {
                lastInputTime = Time.time;
            }
            vaild = GetState();
        }

        public void ResetCacheTimer()
        {
            lastInputTime = float.MinValue; // 一个不可能成立的时间，肯定无法通过缓存胖丁
        }
    }
    [Serializable]
    public class MouseKey
    {
        public int mouseButtonID;
        public bool isCache;
        public float cacheTime;

        private float lastInputTime;
        public bool vaild;
        public bool GetState()
        {
            if (!isCache) return Input.GetMouseButton(mouseButtonID);
            return Input.GetMouseButton(mouseButtonID) || (Time.time - lastInputTime) < cacheTime;
        }

        public void Update()
        {
            if (!isCache) return;
            if (Input.GetMouseButton(mouseButtonID))
            {
                lastInputTime = Time.time;
            }
            vaild = GetState();
        }
        public void ResetCacheTimer()
        {
            lastInputTime = float.MinValue; // 一个不可能成立的时间，肯定无法通过缓存胖丁
        }
    }

    [SerializeField] private Key[] skillKeys;
    [SerializeField] private MouseKey standAttackKey;
    [SerializeField] private GameObject cineMachine;

    // Key:角色配置中技能索引,而不是目前角色已学技能列表中的索引
    private Dictionary<int, Key> skillKeyCodeDic = new Dictionary<int, Key>();
    private bool characterControl;

    public bool CharacterControl
    {
        get => characterControl;
        set
        {
            characterControl = value;
            Cursor.lockState = characterControl ? CursorLockMode.Locked : CursorLockMode.None;
            cineMachine.SetActive(characterControl);
        }
    }

    public void Init(bool characterControl)
    {
        this.CharacterControl = characterControl;
    }

    private void Update()
    {
        standAttackKey.Update();
        for (int i = 0; i < skillKeys.Length; i++)
        {
            skillKeys[i].Update();
        }
    }

    // 将keyCodeIndex绑定到skillIndex
    public void BindSkillKeyCode(int keyCodeIndex, int skillIndex)
    {
        skillKeyCodeDic[skillIndex] = skillKeys[keyCodeIndex];
    }

    public Key GetSkillKey(int skillIndex)
    {
        return skillKeys[skillIndex];
    }
    public bool GetSkillKeyState(int skillIndex)
    {
        if (skillKeyCodeDic.TryGetValue(skillIndex, out Key skillKey))
        {
            return characterControl && skillKey.GetState();
        }
        return false;
    }
    public bool GetStandAttackState()
    {
        return characterControl && standAttackKey.GetState();
    }

    public void ResetStandAttackCacheTimer()
    {
        standAttackKey.ResetCacheTimer();
    }

    public void ResetSkillKeyCodeCacheTimer(int skillIndex)
    {
        if (skillKeyCodeDic.TryGetValue(skillIndex, out Key key))
        {
            key.ResetCacheTimer();
        }
    }


    public Vector2 GetMoveInput()
    {
        if (characterControl)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector2 dir = new Vector2(h, v);
            if (dir.magnitude > 1)
            {
                dir.Normalize();
            }
            return dir;
        }
        else
        {
            return Vector2.zero;
        }
    }

}