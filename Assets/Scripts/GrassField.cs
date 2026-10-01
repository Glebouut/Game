using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class GrassField : MonoBehaviour
{
    public Material material;

    [Header("Где растёт (мировые X/Z)")]
    public Vector2 areaMin = new Vector2(-45f, -50f);

    public Vector2 areaMax = new Vector2(45f, 25f);

    public float groundY;

    public List<Rect> exclude = new List<Rect>
    {
        new Rect(-16.8f, -7.7f, 33.6f, 15.4f),
        new Rect(10.2f, -10.4f, 5.4f, 3.3f)
    };

    [Header("Сколько и какая")]
    public int count = 25000;

    public int seed = 7;

    public float patchScale = 0.08f;

    [Range(0f, 1f)]
    public float patchThreshold = 0.42f;

    public Vector2 scaleRange = new Vector2(0.7f, 1.35f);

    public float clumpHeight = 0.5f;

    public float clumpWidth = 0.6f;

    private Mesh mesh;

    private readonly List<Matrix4x4[]> batches = new List<Matrix4x4[]>();

    private RenderParams rp;

    private bool dirty = true;

    private void OnEnable()
    {
        dirty = true;
    }

    private void OnDisable()
    {
        DestroyMesh();
    }

    private void OnValidate()
    {
        dirty = true;
    }

    private void DestroyMesh()
    {
        if (mesh == null)
        {
            return;
        }
        if (Application.isPlaying)
        {
            UnityEngine.Object.Destroy(mesh);
        }
        else
        {
            UnityEngine.Object.DestroyImmediate(mesh);
        }
        mesh = null;
    }

    private void Build()
    {
        dirty = false;
        // Размеры пучка могли поменяться в инспекторе — пересобираем меш.
        DestroyMesh();
        mesh = BuildClump(clumpWidth, clumpHeight);
        System.Random random = new System.Random(seed);
        List<Matrix4x4> list = new List<Matrix4x4>(count);
        int num = 0;
        while (list.Count < count && num < count * 8)
        {
            num++;
            float num2 = Mathf.Lerp(areaMin.x, areaMax.x, (float)random.NextDouble());
            float num3 = Mathf.Lerp(areaMin.y, areaMax.y, (float)random.NextDouble());
            if (!Excluded(num2, num3))
            {
                float num4 = Mathf.PerlinNoise(num2 * patchScale + (float)seed, num3 * patchScale - (float)seed);
                if (!(num4 < patchThreshold) || !(random.NextDouble() > 0.12))
                {
                    float num5 = Mathf.Lerp(scaleRange.x, scaleRange.y, (float)random.NextDouble()) * Mathf.Lerp(0.8f, 1.15f, num4);
                    Quaternion q = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f);
                    list.Add(Matrix4x4.TRS(new Vector3(num2, groundY, num3), q, new Vector3(num5, num5, num5)));
                }
            }
        }
        batches.Clear();
        for (int i = 0; i < list.Count; i += 1000)
        {
            int num6 = Mathf.Min(1000, list.Count - i);
            Matrix4x4[] array = new Matrix4x4[num6];
            list.CopyTo(i, array, 0, num6);
            batches.Add(array);
        }
        if (material != null)
        {
            material.enableInstancing = true;
            rp = new RenderParams(material)
            {
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = true,
                worldBounds = new Bounds(new Vector3((areaMin.x + areaMax.x) / 2f, groundY, (areaMin.y + areaMax.y) / 2f), new Vector3(areaMax.x - areaMin.x + 4f, clumpHeight * 4f, areaMax.y - areaMin.y + 4f))
            };
        }
    }

    private bool Excluded(float x, float z)
    {
        foreach (Rect item in exclude)
        {
            if (item.Contains(new Vector2(x, z)))
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (material == null)
        {
            return;
        }
        if (dirty || mesh == null)
        {
            Build();
        }
        foreach (Matrix4x4[] batch in batches)
        {
            Graphics.RenderMeshInstanced(in rp, mesh, 0, batch);
        }
    }

    private static Mesh BuildClump(float w, float h)
    {
        List<Vector3> list = new List<Vector3>();
        List<Vector2> list2 = new List<Vector2>();
        List<Vector3> list3 = new List<Vector3>();
        List<int> list4 = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            float f = (float)i * MathF.PI / 3f;
            Vector3 vector = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * (w / 2f);
            int num = list.Count;
            for (int j = 0; j <= 3; j++)
            {
                float num2 = (float)j / 3f;
                Vector3 vector2 = Vector3.up * (h * num2);
                list.Add(-vector + vector2);
                list2.Add(new Vector2(0f, num2));
                list3.Add(Vector3.up);
                list.Add(vector + vector2);
                list2.Add(new Vector2(1f, num2));
                list3.Add(Vector3.up);
            }
            for (int k = 0; k < 3; k++)
            {
                int num3 = num + k * 2;
                list4.AddRange(new int[6]
                {
                    num3,
                    num3 + 2,
                    num3 + 1,
                    num3 + 1,
                    num3 + 2,
                    num3 + 3
                });
            }
        }
        Mesh obj = new Mesh();
        obj.name = "GrassClump";
        obj.hideFlags = HideFlags.DontSave;
        obj.SetVertices(list);
        obj.SetUVs(0, list2);
        obj.SetNormals(list3);
        obj.SetTriangles(list4, 0);
        obj.RecalculateBounds();
        return obj;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.9f, 0.4f, 0.6f);
        Gizmos.DrawWireCube(new Vector3((areaMin.x + areaMax.x) / 2f, groundY, (areaMin.y + areaMax.y) / 2f), new Vector3(areaMax.x - areaMin.x, 0.1f, areaMax.y - areaMin.y));
        Gizmos.color = new Color(1f, 0.4f, 0.3f, 0.6f);
        foreach (Rect item in exclude)
        {
            Gizmos.DrawWireCube(new Vector3(item.center.x, groundY, item.center.y), new Vector3(item.width, 0.1f, item.height));
        }
    }
}
