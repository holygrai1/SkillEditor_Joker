using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GenarteFanMesh(float insideRadius, float radius, float height, float angle)
    {
        Mesh fanMesh = new Mesh();
        Vector3 centerPos = Vector3.zero;
        Vector3 direction = Vector3.forward;
        Vector3 rightDir = Quaternion.AngleAxis(angle / 2, Vector3.up) * direction;
        float deltaAngle = 2.5f;
        int rects = (int)(angle / deltaAngle);
        int lines = rects + 1;
        Vector3[] vertexs = new Vector3[2 * lines * 2];
        int[] triangles = new int[rects * 6 * 4 + 6 * 12];

        // 底面
        for (int i = 0; i < lines; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(-deltaAngle * i, Vector3.up) * rightDir;
            Vector3 minPos = centerPos + dir * insideRadius;
            Vector3 maxPos = centerPos + dir * radius;

            vertexs[i * 2] = minPos;
            vertexs[i * 2 + 1] = maxPos;

            // 处理三角形，因为lines比rect多1
            // 1 2 0
            // 1 3 2
            if (i < lines - 1)
            {
                triangles[i * 6 + 0] = i * 2 + 1;
                triangles[i * 6 + 1] = i * 2 + 2;
                triangles[i * 6 + 2] = i * 2 + 0;
                triangles[i * 6 + 3] = i * 2 + 1;
                triangles[i * 6 + 4] = i * 2 + 3;
                triangles[i * 6 + 5] = i * 2 + 2;
            }
        }

        // 顶面
        for (int i = lines; i < 2 * lines; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(-deltaAngle * (i - lines), Vector3.up) * rightDir;
            Vector3 minPos = centerPos + dir * insideRadius;
            Vector3 maxPos = centerPos + dir * radius;

            minPos.y += height;
            maxPos.y += height;
            vertexs[i * 2] = minPos;
            vertexs[i * 2 + 1] = maxPos;

            // 处理三角形，因为lines比rect多1
            // 0 2 1
            // 2 3 1
            if (i < 2 * lines - 1)
            {
                triangles[i * 6 + 0] = i * 2 + 0;
                triangles[i * 6 + 1] = i * 2 + 2;
                triangles[i * 6 + 2] = i * 2 + 1;
                triangles[i * 6 + 3] = i * 2 + 2;
                triangles[i * 6 + 4] = i * 2 + 3;
                triangles[i * 6 + 5] = i * 2 + 1;
            }
        }

        // 右面
        // 0 2 1
        // 2 3 1
        int start = 2 * lines - 1;
        triangles[start * 6 + 0] = 0;
        triangles[start * 6 + 1] = 2 * lines;
        triangles[start * 6 + 2] = 1;
        triangles[start * 6 + 3] = 2 * lines;
        triangles[start * 6 + 4] = 2 * lines + 1;
        triangles[start * 6 + 5] = 1;

        // 左面
        // 1 2 0
        // 1 3 2
        triangles[start * 6 + 6] = (lines - 1) * 2 + 1;
        triangles[start * 6 + 7] = (2 * lines - 1) * 2;
        triangles[start * 6 + 8] = (lines - 1) * 2;
        triangles[start * 6 + 9] = (lines - 1) * 2 + 1;
        triangles[start * 6 + 10] = (2 * lines - 1) * 2 + 1;
        triangles[start * 6 + 11] = (2 * lines - 1) * 2;

        // 后面
        // 0 2 1
        // 2 3 1
        start += 2;
        for (int i = 0; i < rects; i++)
        {
            int index = start + i;
            triangles[index * 6 + 0] = i * 2 + 0;
            triangles[index * 6 + 1] = i * 2 + 2;
            triangles[index * 6 + 2] = 2 * lines + i * 2;
            triangles[index * 6 + 3] = i * 2 + 2;
            triangles[index * 6 + 4] = 2 * lines + i * 2 + 2;
            triangles[index * 6 + 5] = 2 * lines + i * 2;
        }

        // 前面
        // 1 2 0
        // 1 3 2
        start += rects;
        for (int i = 0; i < rects; i++)
        {
            int index = start + i;
            triangles[index * 6 + 0] = 2 * lines + i * 2 + 1;
            triangles[index * 6 + 1] = i * 2 + 1 + 2;
            triangles[index * 6 + 2] = i * 2 + 1;
            triangles[index * 6 + 3] = 2 * lines + i * 2 + 1;
            triangles[index * 6 + 4] = 2 * lines + i * 2 + 1 + 2;
            triangles[index * 6 + 5] = i * 2 + 1 + 2;
        }


        fanMesh.vertices = vertexs;
        fanMesh.triangles = triangles;
        fanMesh.RecalculateNormals();
        return fanMesh;
    }
}
