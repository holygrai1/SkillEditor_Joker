using Codice.CM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class SkillAttackDetectionEventInspector : SkillEventDataInspectorBase<AttackDetectionTrackItem, AttackDetectionTrack>
{
    public override void OnDraw()
    {
        DrawDetection();
        DrawHitConfig();
    }

    #region 检测部分
    private IntegerField detectionDurationFrameField;
    private List<string> detectionChoiceList;
    private void DrawDetection()
    {
        detectionDurationFrameField = new IntegerField("持续帧数");
        detectionDurationFrameField.value = trackItem.SkillAttackDetectionEvent.DurationFrame;
        detectionDurationFrameField.RegisterValueChangedCallback(DurationFrameFieldValueChanged);
        root.Add(detectionDurationFrameField);

        detectionChoiceList = new List<string>(Enum.GetNames(typeof(AttackDetectionType)));
        DropdownField detectionDropDownField = new DropdownField("检测类型", detectionChoiceList, (int)trackItem.SkillAttackDetectionEvent.AttackDetectionType);
        detectionDropDownField.RegisterValueChangedCallback(OnDetectionDropDownFieldValueChanged);
        root.Add(detectionDropDownField);
        // 根据检测类型进行绘制
        switch (trackItem.SkillAttackDetectionEvent.AttackDetectionType)
        {
            case AttackDetectionType.Weapon:
                AttackWeaponDetectionData weaponDetectionData = (AttackWeaponDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
                DropdownField weaponDetectionDropDownField = new DropdownField("武器选择");
                if (SkillEditorWindow.Instance.PreviewCharacterObj != null)
                {
                    Skill_Player skill_Player = SkillEditorWindow.Instance.PreviewCharacterObj.GetComponent<Skill_Player>();
                    weaponDetectionDropDownField.choices = skill_Player.WeaponDic.Keys.ToList();
                }

                if (!string.IsNullOrEmpty(weaponDetectionData.weaponName))
                {
                    weaponDetectionDropDownField.value = weaponDetectionData.weaponName;
                }
                weaponDetectionDropDownField.RegisterValueChangedCallback(WeaponDetectionDropDownFieldValueChanged);
                root.Add(weaponDetectionDropDownField);
                break;
            case AttackDetectionType.Box:
                AttackBoxDetectionData boxDetectionData = (AttackBoxDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
                Vector3Field boxDetectionPositionField = new Vector3Field("坐标");
                Vector3Field boxDetectionRotationField = new Vector3Field("旋转");
                Vector3Field boxDetectionScaleField = new Vector3Field("缩放");
                boxDetectionPositionField.value = boxDetectionData.Position;
                boxDetectionRotationField.value = boxDetectionData.Rotation;
                boxDetectionScaleField.value = boxDetectionData.Scale;
                boxDetectionPositionField.RegisterValueChangedCallback(ShapeDetectionPositionFieldValueChanged);
                boxDetectionRotationField.RegisterValueChangedCallback(BoxDetectionRotationFieldValueChanged);
                boxDetectionScaleField.RegisterValueChangedCallback(BoxDetectionScaleFieldValueChanged);
                root.Add(boxDetectionPositionField);
                root.Add(boxDetectionRotationField);
                root.Add(boxDetectionScaleField);
                break;
            case AttackDetectionType.Sphere:
                AttackSphereDetectionData sphereDetectionData = (AttackSphereDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
                Vector3Field sphereDetectionPositionField = new Vector3Field("坐标");
                sphereDetectionPositionField.RegisterValueChangedCallback(ShapeDetectionPositionFieldValueChanged);

                FloatField sphereRadiusField = new FloatField("半径");
                sphereRadiusField.value = sphereDetectionData.Radius;
                sphereRadiusField.RegisterValueChangedCallback(SphereRadiusFieldValueChanged);

                root.Add(sphereDetectionPositionField);
                root.Add(sphereRadiusField);
                break;
            case AttackDetectionType.Fan:
                AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;

                Vector3Field fanDetectionPositionField = new Vector3Field("坐标");
                Vector3Field fanDetectionRotationField = new Vector3Field("旋转");
                FloatField fanInsideRadiusFiled = new FloatField("内半径");
                FloatField fanRadiusField = new FloatField("外半径");
                FloatField fanHeightField = new FloatField("高度");
                FloatField fanAangle = new FloatField("角度");

                fanDetectionPositionField.value = fanDetectionData.Position;
                fanDetectionRotationField.value = fanDetectionData.Rotation;
                fanInsideRadiusFiled.value = fanDetectionData.InsideRadius;
                fanRadiusField.value = fanDetectionData.Radius;
                fanHeightField.value = fanDetectionData.Height;
                fanAangle.value = fanDetectionData.Angle;

                fanDetectionPositionField.RegisterValueChangedCallback(ShapeDetectionPositionFieldValueChanged);
                fanDetectionRotationField.RegisterValueChangedCallback(FanDetectionRotationFieldValueChanged);
                fanInsideRadiusFiled.RegisterValueChangedCallback(FanInsideRadiusFieldValueChanged);
                fanRadiusField.RegisterValueChangedCallback(FanRadiusFieldValueChanged);
                fanHeightField.RegisterValueChangedCallback(FanHeightFieldValueChanged);
                fanAangle.RegisterValueChangedCallback(FanAngleFieldValueChanged);
                root.Add(fanDetectionPositionField);
                root.Add(fanDetectionRotationField);
                root.Add(fanInsideRadiusFiled);
                root.Add(fanRadiusField);
                root.Add(fanHeightField);
                root.Add(fanAangle);
                break;
        }

        // 设置持续帧数至选中帧
        Button setFrameButton = new Button(SetDetectionDurationFrameButton);
        setFrameButton.text = "设置持续帧数至选中帧";
        root.Add(setFrameButton);
    }

    private void WeaponDetectionDropDownFieldValueChanged(ChangeEvent<string> evt)
    {
        AttackWeaponDetectionData detectionData = (AttackWeaponDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.weaponName = evt.newValue;
    }

    private void DurationFrameFieldValueChanged(ChangeEvent<int> evt)
    {
        AttackDetectionTrackItem attackDetectionTrackItem = trackItem;
        attackDetectionTrackItem.SkillAttackDetectionEvent.DurationFrame = evt.newValue;
        trackItem.ResetView();
    }

    private void SetDetectionDurationFrameButton()
    {
        detectionDurationFrameField.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.FrameIndex;
    }


    private void OnDetectionDropDownFieldValueChanged(ChangeEvent<string> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackDetectionType = (AttackDetectionType)detectionChoiceList.IndexOf(evt.newValue);
        SkillEditorWindow.Instance.Show();
    }
    private void ShapeDetectionPositionFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackShapeDetectionDataBase shapeDetectionData = (AttackShapeDetectionDataBase)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        shapeDetectionData.Position = evt.newValue;
    }
    private void BoxDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackBoxDetectionData detectionData = (AttackBoxDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Rotation = evt.newValue;
    }

    private void BoxDetectionScaleFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackBoxDetectionData detectionData = (AttackBoxDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Scale = evt.newValue;
    }

    private void SphereRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackSphereDetectionData detectionData = (AttackSphereDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Radius = evt.newValue;
    }

    private void FanDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackFanDetectionData detectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Rotation = evt.newValue;
    }

    private void FanInsideRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData detectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.InsideRadius = evt.newValue;
        if (detectionData.Radius <= detectionData.InsideRadius)
        {
            detectionData.InsideRadius = detectionData.Radius - 0.01f;
            SkillEditorWindow.Instance.Show();
        }
    }

    private void FanRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData detectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Radius = evt.newValue;
        if (detectionData.Radius <= detectionData.InsideRadius)
        {
            detectionData.InsideRadius = detectionData.Radius - 0.01f;
            SkillEditorWindow.Instance.Show();
        }
    }

    private void FanHeightFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData detectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Height = evt.newValue;
        if (detectionData.Height <= 0)
        {
            detectionData.Height = 0.01f;
            SkillEditorWindow.Instance.Show();
        }
    }

    private void FanAngleFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData detectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Angle = evt.newValue;
        if (detectionData.Angle < 0)
        {
            detectionData.Angle = 0.1f;
            SkillEditorWindow.Instance.Show();
        }
        else if (detectionData.Angle > 360)
        {
            detectionData.Angle = 360;
            SkillEditorWindow.Instance.Show();
        }
    }
    #endregion

    #region 命中部分
    private void DrawHitConfig()
    {
        root.Add(new Label());
        FloatField attackMultiplyFied = new FloatField("攻击力系数");
        attackMultiplyFied.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.AttackMultiply;
        attackMultiplyFied.RegisterValueChangedCallback(OnAttackMultiplyFieldValueChaned);
        root.Add(attackMultiplyFied);

        Vector3Field repelStrengthFied = new Vector3Field("击退强度");
        repelStrengthFied.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelStrength;
        repelStrengthFied.RegisterValueChangedCallback(OnRepelStrengthFieldValueChanged);
        root.Add(repelStrengthFied);

        FloatField repelTimeFied = new FloatField("击退时间");
        repelTimeFied.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelTime;
        repelTimeFied.RegisterValueChangedCallback(OnRepelTimeFieldValueChaned);
        root.Add(repelTimeFied);

        ObjectField hitEffectPrefabField = new ObjectField("命中特效");
        hitEffectPrefabField.objectType = typeof(GameObject);
        hitEffectPrefabField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitEffectPrefab;
        hitEffectPrefabField.RegisterValueChangedCallback(OnHitEffectPrefabFieldValueChaned);
        root.Add(hitEffectPrefabField);

        ObjectField hitAudioClipField = new ObjectField("命中音效");
        hitAudioClipField.objectType = typeof(AudioClip);
        hitAudioClipField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitAudioClip;
        hitAudioClipField.RegisterValueChangedCallback(OnHitAudioClipFieldValueChaned);
        root.Add(hitAudioClipField);
    }

    private void OnAttackMultiplyFieldValueChaned(ChangeEvent<float> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.AttackMultiply = evt.newValue;
    }
    private void OnRepelTimeFieldValueChaned(ChangeEvent<float> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelTime = evt.newValue;
    }

    private void OnRepelStrengthFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelStrength = evt.newValue;
    }
    private void OnHitEffectPrefabFieldValueChaned(ChangeEvent<UnityEngine.Object> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitEffectPrefab = (GameObject)evt.newValue;
    }

    private void OnHitAudioClipFieldValueChaned(ChangeEvent<UnityEngine.Object> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitAudioClip = (AudioClip)evt.newValue;
    }
    #endregion


}
