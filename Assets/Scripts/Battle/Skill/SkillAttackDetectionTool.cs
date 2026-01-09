using UnityEngine;

public static class SkillAttackDetectionTool
{
    private static Collider[] detectionResluts = new Collider[20];
    public static Collider[] ShapeDetection(Transform modelTransform, AttackDetectionDataBase data, AttackDetectionType attackDetectionType, LayerMask layerMask)
    {
        switch (attackDetectionType)
        {
            case AttackDetectionType.Box:
                return BoxDetection(modelTransform, (AttackBoxDetectionData)data, layerMask);
            case AttackDetectionType.Sphere:
                return SphereDetection(modelTransform, (AttackSphereDetectionData)data, layerMask);
            case AttackDetectionType.Fan:
                return FanDetection(modelTransform, (AttackFanDetectionData)data, layerMask);
        }
        return null;
    }

    public static Collider[] BoxDetection(Transform modelTransform, AttackBoxDetectionData data, LayerMask layerMask)
    {
        CleanDetectionResluts();
        Physics.OverlapBoxNonAlloc(modelTransform.TransformPoint(data.Position), data.Scale / 2, detectionResluts, modelTransform.rotation * Quaternion.Euler(data.Rotation), layerMask);
        return detectionResluts;
    }
    public static Collider[] SphereDetection(Transform modelTransform, AttackSphereDetectionData data, LayerMask layerMask)
    {
        CleanDetectionResluts();
        Physics.OverlapSphereNonAlloc(modelTransform.TransformPoint(data.Position), data.Radius, detectionResluts, layerMask);
        return detectionResluts;

    }
    public static Collider[] FanDetection(Transform modelTransform, AttackFanDetectionData data, LayerMask layerMask)
    {
        CleanDetectionResluts();
        Vector3 size = new Vector3();
        size.x = data.Radius * 2;
        size.z = size.x;
        size.y = data.Height;
        Vector3 fanPosition = modelTransform.TransformPoint(data.Position);
        Physics.OverlapBoxNonAlloc(fanPosition, size / 2, detectionResluts, modelTransform.rotation * Quaternion.Euler(data.Rotation), layerMask);

        // 过滤无效检测
        Vector3 fanForward = modelTransform.rotation * Quaternion.Euler(data.Rotation) * Vector3.forward;
        for (int i = 0; i < detectionResluts.Length; i++)
        {
            if (detectionResluts[i] == null) break;
            // 过滤内半径内的、外半径外的
            Vector3 point = detectionResluts[i].ClosestPoint(modelTransform.position);
            float distance = Vector3.Distance(point, modelTransform.position);
            bool remove = distance < data.InsideRadius || distance > data.Radius;
            // 过滤不在角度范围内的
            if (!remove)
            {
                Vector3 dir = point - fanPosition;
                float angle = Vector3.Angle(fanForward, dir);
                remove = angle > data.Angle * 0.5f;
            }
            if (remove)
            {
                detectionResluts[i] = null;
            }
        }
        return detectionResluts;
    }

    private static void CleanDetectionResluts()
    {
        for (int i = 0; i < detectionResluts.Length; i++)
        {
            detectionResluts[i] = null;
        }
    }
}