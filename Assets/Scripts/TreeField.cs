using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class TreeField : MonoBehaviour
{
    public Material material;

    [Header("Где лес (мировые X/Z)")]
    public Vector2 areaMin = new Vector2(-90f, -100f);

    public Vector2 areaMax = new Vector2(90f, 80f);

    public Rect clearing = new Rect(-30f, -40f, 64f, 56f);

    public float edgeSoftness = 12f;

    public float groundY;

    [Header("Деревья")]
    public int count = 450;

    public int seed = 3;

    public Vector2 heightRange = new Vector2(7f, 16f);

    [Range(0f, 10f)]
    public float maxTilt = 3f;

    private readonly Mesh[] variants = new Mesh[3];

    private readonly List<Matrix4x4[]>[] batches = new List<Matrix4x4[]>[3]
    {
        new List<Matrix4x4[]>(),
        new List<Matrix4x4[]>(),
        new List<Matrix4x4[]>()
    };

    private RenderParams rp;

    private bool dirty = true;

    private void OnEnable()
    {
        dirty = true;
    }

    private void OnDisable()
    {
        for (int i = 0; i < variants.Length; i++)
        {
            if (variants[i] == null)
            {
                continue;
            }
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(variants[i]);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(variants[i]);
            }
            variants[i] = null;
        }
    }

    private void OnValidate()
    {
        dirty = true;
    }

    private void Build()
    {
        dirty = false;
        for (int i = 0; i < variants.Length; i++)
        {
            if (variants[i] == null)
            {
                variants[i] = BuildSpruce(101 + i * 17, i == 2);
            }
        }
        System.Random random = new System.Random(seed);
        List<Matrix4x4>[] array = new List<Matrix4x4>[3]
        {
            new List<Matrix4x4>(),
            new List<Matrix4x4>(),
            new List<Matrix4x4>()
        };
        int num = 0;
        int num2 = 0;
        while (num < count && num2 < count * 20)
        {
            num2++;
            float x = Mathf.Lerp(areaMin.x, areaMax.x, (float)random.NextDouble());
            float z = Mathf.Lerp(areaMin.y, areaMax.y, (float)random.NextDouble());
            float num3 = DistanceOutside(clearing, x, z);
            if (!(num3 <= 0f) && !(random.NextDouble() > (double)Mathf.Clamp01(num3 / edgeSoftness) * 0.9 + 0.1))
            {
                float num4 = Mathf.Lerp(heightRange.x, heightRange.y, (float)random.NextDouble());
                Quaternion q = Quaternion.Euler(((float)random.NextDouble() * 2f - 1f) * maxTilt, (float)random.NextDouble() * 360f, ((float)random.NextDouble() * 2f - 1f) * maxTilt);
                int num5 = random.Next(variants.Length);
                array[num5].Add(Matrix4x4.TRS(new Vector3(x, groundY, z), q, Vector3.one * num4));
                num++;
            }
        }
        for (int j = 0; j < variants.Length; j++)
        {
            batches[j].Clear();
            List<Matrix4x4> list = array[j];
            for (int k = 0; k < list.Count; k += 1000)
            {
                int num6 = Mathf.Min(1000, list.Count - k);
                Matrix4x4[] array2 = new Matrix4x4[num6];
                list.CopyTo(k, array2, 0, num6);
                batches[j].Add(array2);
            }
        }
        if (material != null)
        {
            material.enableInstancing = true;
            rp = new RenderParams(material)
            {
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = true,
                worldBounds = new Bounds(new Vector3((areaMin.x + areaMax.x) / 2f, groundY + heightRange.y / 2f, (areaMin.y + areaMax.y) / 2f), new Vector3(areaMax.x - areaMin.x + 20f, heightRange.y * 2f, areaMax.y - areaMin.y + 20f))
            };
        }
    }

    private static float DistanceOutside(Rect r, float x, float z)
    {
        float num = Mathf.Max(r.xMin - x, 0f, x - r.xMax);
        float num2 = Mathf.Max(r.yMin - z, 0f, z - r.yMax);
        return Mathf.Sqrt(num * num + num2 * num2);
    }

    private void Update()
    {
        if (material == null)
        {
            return;
        }
        if (dirty || variants[0] == null)
        {
            Build();
        }
        for (int i = 0; i < variants.Length; i++)
        {
            foreach (Matrix4x4[] item in batches[i])
            {
                Graphics.RenderMeshInstanced(in rp, variants[i], 0, item);
            }
        }
    }

    private static Mesh BuildSpruce(int seed, bool deadBottom)
    {
        System.Random rnd = new System.Random(seed);
        List<Vector3> list = new List<Vector3>();
        List<Vector3> list2 = new List<Vector3>();
        List<Vector2> list3 = new List<Vector2>();
        List<Vector2> list4 = new List<Vector2>();
        List<int> list5 = new List<int>();
        int num = list.Count;
        for (int i = 0; i <= 1; i++)
        {
            float num2 = ((i == 0) ? 0f : 0.97f);
            float num3 = ((i == 0) ? 0.017f : 0.003f);
            for (int j = 0; j <= 8; j++)
            {
                float f = (float)j / 8f * MathF.PI * 2f;
                Vector3 vector = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
                list.Add(vector * num3 + Vector3.up * num2);
                list2.Add(vector);
                list3.Add(new Vector2(0.25f * (float)j / 8f, num2));
                list4.Add(Vector2.zero);
            }
        }
        for (int k = 0; k < 8; k++)
        {
            int num4 = num + k;
            int num5 = num4 + 1;
            int num6 = num4 + 8 + 1;
            int num7 = num6 + 1;
            list5.AddRange(new int[6] { num4, num6, num5, num5, num6, num7 });
        }
        int num8 = 18 + rnd.Next(4);
        for (int l = 0; l < num8; l++)
        {
            float num9 = (float)l / (float)(num8 - 1);
            if (!deadBottom || !(num9 < 0.18f))
            {
                float y = Mathf.Lerp(0.1f, 0.94f, num9) + (R() - 0.5f) * 0.015f;
                float num10 = Mathf.Lerp(0.3f, 0.05f, Mathf.Pow(num9, 0.85f)) * Mathf.Lerp(0.85f, 1.15f, R());
                int num11 = Mathf.RoundToInt(Mathf.Lerp(9f, 5f, num9));
                float droop = Mathf.Lerp(0.45f, 0.18f, num9);
                float num12 = R() * MathF.PI * 2f;
                for (int m = 0; m < num11; m++)
                {
                    float f2 = num12 + (float)m / (float)num11 * MathF.PI * 2f + (R() - 0.5f) * 0.4f;
                    Vector3 dir = new Vector3(Mathf.Cos(f2), 0f, Mathf.Sin(f2));
                    Vector3 vector2 = new Vector3(0f - dir.z, 0f, dir.x);
                    float num13 = num10 * Mathf.Lerp(0.85f, 1.1f, R());
                    float num14 = num13 * 0.7f;
                    Vector3 normalized = (vector2 * Mathf.Cos(0.45f) + Vector3.up * Mathf.Sin(0.45f)).normalized;
                    Vector3 normalized2 = (Vector3.up * 0.9f + vector2 * 0.35f).normalized;
                    Card(list, list2, list3, list4, list5, new Vector3(0f, y, 0f), dir, num13, droop, normalized, num14);
                    Card(list, list2, list3, list4, list5, new Vector3(0f, y, 0f), dir, num13, droop, normalized2, num14 * 0.8f);
                }
            }
        }
        Leader(list, list2, list3, list4, list5, Vector3.right);
        Leader(list, list2, list3, list4, list5, Vector3.forward);
        Mesh mesh = new Mesh();
        mesh.name = "Spruce";
        mesh.hideFlags = HideFlags.DontSave;
        mesh.SetVertices(list);
        mesh.SetNormals(list2);
        mesh.SetUVs(0, list3);
        mesh.SetUVs(1, list4);
        mesh.SetTriangles(list5, 0);
        mesh.RecalculateBounds();
        return mesh;
        float R()
        {
            return (float)rnd.NextDouble();
        }
    }

    private static void Card(List<Vector3> v, List<Vector3> n, List<Vector2> uv, List<Vector2> uv2, List<int> tri, Vector3 root, Vector3 dir, float len, float droop, Vector3 side, float width)
    {
        int num = v.Count;
        Vector3 normalized = (Vector3.up + dir * 0.6f).normalized;
        for (int i = 0; i <= 3; i++)
        {
            float num2 = (float)i / 3f;
            Vector3 vector = root + dir * len * num2 + Vector3.down * droop * len * Mathf.Pow(num2, 1.5f) + Vector3.up * 0.05f * len * num2 * (1f - num2);
            v.Add(vector - side * width * 0.5f);
            n.Add(normalized);
            uv.Add(new Vector2(0.25f + 0.75f * num2, 0f));
            uv2.Add(new Vector2(num2, 0f));
            v.Add(vector + side * width * 0.5f);
            n.Add(normalized);
            uv.Add(new Vector2(0.25f + 0.75f * num2, 1f));
            uv2.Add(new Vector2(num2, 0f));
        }
        for (int j = 0; j < 3; j++)
        {
            int num3 = num + j * 2;
            tri.AddRange(new int[6]
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

    private static void Leader(List<Vector3> v, List<Vector3> n, List<Vector2> uv, List<Vector2> uv2, List<int> tri, Vector3 side)
    {
        int num = v.Count;
        float num2 = 0.86f;
        float num3 = 1f;
        float num4 = 0.035f;
        for (int i = 0; i <= 1; i++)
        {
            float y = ((i == 0) ? num2 : num3);
            float num5 = i;
            v.Add(new Vector3(0f, y, 0f) - side * num4);
            n.Add(Vector3.up);
            uv.Add(new Vector2(0.25f + 0.75f * num5, 0f));
            uv2.Add(new Vector2(num5 * 0.5f, 0f));
            v.Add(new Vector3(0f, y, 0f) + side * num4);
            n.Add(Vector3.up);
            uv.Add(new Vector2(0.25f + 0.75f * num5, 1f));
            uv2.Add(new Vector2(num5 * 0.5f, 0f));
        }
        tri.AddRange(new int[6]
        {
            num,
            num + 2,
            num + 1,
            num + 1,
            num + 2,
            num + 3
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.8f, 0.3f, 0.6f);
        Gizmos.DrawWireCube(new Vector3((areaMin.x + areaMax.x) / 2f, groundY, (areaMin.y + areaMax.y) / 2f), new Vector3(areaMax.x - areaMin.x, 0.1f, areaMax.y - areaMin.y));
        Gizmos.color = new Color(1f, 0.8f, 0.3f, 0.8f);
        Gizmos.DrawWireCube(new Vector3(clearing.center.x, groundY, clearing.center.y), new Vector3(clearing.width, 0.1f, clearing.height));
    }
}
