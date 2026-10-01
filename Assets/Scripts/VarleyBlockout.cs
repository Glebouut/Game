using System.Collections.Generic;
using UnityEngine;

public class VarleyBlockout : MonoBehaviour
{
    private struct UvRule
    {
        public float u;

        public float v;

        public bool worldHeight;
    }

    public float width = 32.4f;

    public float depth = 14.4f;

    public float height = 9f;

    private Material concrete;

    private Material plinth;

    private Material roof;

    private Material glassDark;

    private Material glassLit;

    private Material metal;

    private Material door;

    private Material sign;

    private readonly Dictionary<Material, UvRule> uvRules = new Dictionary<Material, UvRule>();

    [ContextMenu("Build")]
    private void Build()
    {
        Clear();
        LoadMaterials();
        float num = width / 2f;
        float num2 = depth / 2f;
        float num3 = 0f - num2;
        Box("Body", new Vector3(0f, height / 2f, 0f), new Vector3(width, height, depth), concrete);
        Box("Plinth", new Vector3(0f, 0.35f, 0f), new Vector3(width + 0.2f, 0.7f, depth + 0.2f), plinth);
        float num4 = 0.8f;
        float num5 = 0.25f;
        Box("Parapet_F", new Vector3(0f, height + num4 / 2f, num3 + num5 / 2f), new Vector3(width, num4, num5), plinth);
        Box("Parapet_B", new Vector3(0f, height + num4 / 2f, num2 - num5 / 2f), new Vector3(width, num4, num5), plinth);
        Box("Parapet_L", new Vector3(0f - num + num5 / 2f, height + num4 / 2f, 0f), new Vector3(num5, num4, depth), plinth);
        Box("Parapet_R", new Vector3(num - num5 / 2f, height + num4 / 2f, 0f), new Vector3(num5, num4, depth), plinth);
        float num6 = 0f - num;
        float num7 = 0f - num + 11.1f;
        float num8 = (num6 + num7) / 2f;
        Box("DrillHall_Top", new Vector3(num8, height + 1.2f, 0f), new Vector3(num7 - num6, 2.4f, depth), concrete);
        Box("DrillHall_Cap", new Vector3(num8, height + 2.5f, 0f), new Vector3(num7 - num6 + 0.3f, 0.2f, depth + 0.3f), plinth);
        float[] array = new float[3]
        {
            num8 - 3.2f,
            num8,
            num8 + 3.2f
        };
        foreach (float x in array)
        {
            Window("HallWindow", x, 5.4f, 1.1f, 5.2f, num3, glassDark);
        }
        array = new float[3] { -1.9f, 4.05f, 9.3f };
        foreach (float x2 in array)
        {
            Window("Win1", x2, 2.6f, 1.8f, 0.9f, num3, glassDark);
        }
        array = new float[6] { -3f, 0.3f, 4f, 7f, 10.6f, 14.2f };
        foreach (float x3 in array)
        {
            Window("Win2", x3, 6.4f, 2.2f, 1.4f, num3, glassDark);
        }
        float num9 = 12.9f;
        Box("DoorFrame", new Vector3(num9, 1.75f, num3 - 0.06f), new Vector3(3f, 3.5f, 0.14f), plinth);
        Box("Door_L", new Vector3(num9 - 0.66f, 1.6f, num3 - 0.1f), new Vector3(1.25f, 3.1f, 0.08f), door);
        Box("Door_R", new Vector3(num9 + 0.66f, 1.6f, num3 - 0.1f), new Vector3(1.25f, 3.1f, 0.08f), door);
        Box("Step", new Vector3(num9, 0.12f, num3 - 0.9f), new Vector3(4.2f, 0.24f, 1.6f), plinth);
        Box("Canopy", new Vector3(num9, 3.95f, num3 - 0.9f), new Vector3(4.6f, 0.18f, 1.9f), metal);
        array = new float[2] { -1f, 1f };
        foreach (float num10 in array)
        {
            Box("CanopyStrut", new Vector3(num9 + num10 * 2.1f, 3.6f, num3 - 1.6f), new Vector3(0.08f, 0.7f, 0.08f), metal);
        }
        Box("EntranceLamp", new Vector3(num9, 3.8f, num3 - 0.9f), new Vector3(0.5f, 0.12f, 0.3f), glassLit);
        AddLight("EntranceLight", new Vector3(num9, 3.7f, num3 - 1f), LightType.Spot, new Color(1f, 0.66f, 0.35f), 6f, 12f, Quaternion.Euler(90f, 0f, 0f), 95f);
        Window("BoothWindow", 15.25f, 1.7f, 1.3f, 1f, num3, glassLit);
        AddLight("BoothLight", new Vector3(15.25f, 1.6f, num3 + 0.8f), LightType.Point, new Color(1f, 0.84f, 0.63f), 2.2f, 5f, Quaternion.identity, 0f);
        Box("SignBoard", new Vector3(num8, 8.4f, num3 - 0.12f), new Vector3(7.5f, 1.1f, 0.12f), sign);
        array = new float[3]
        {
            0f - num + 0.3f,
            num7 + 0.2f,
            num - 0.3f
        };
        foreach (float x4 in array)
        {
            Cyl("Downpipe", new Vector3(x4, height / 2f, num3 - 0.18f), 0.12f, height, metal);
        }
        Box("Vent_A", new Vector3(3f, height + 0.6f, 2f), new Vector3(1.6f, 1.2f, 1.6f), metal);
        Box("Vent_B", new Vector3(9f, height + 0.45f, -1.5f), new Vector3(2.4f, 0.9f, 1.2f), metal);
        Derrick(new Vector3(num8, height + 2.6f, 0f), 2.6f, 0.9f, 11f);
    }

    [ContextMenu("Clear")]
    private void Clear()
    {
        for (int num = base.transform.childCount - 1; num >= 0; num--)
        {
            Object.DestroyImmediate(base.transform.GetChild(num).gameObject);
        }
    }

    private void Window(string name, float x, float y, float w, float h, float front, Material glass)
    {
        Box(name + "_Frame", new Vector3(x, y, front - 0.03f), new Vector3(w + 0.2f, h + 0.2f, 0.06f), plinth);
        Box(name, new Vector3(x, y, front - 0.07f), new Vector3(w, h, 0.04f), glass);
        Box(name + "_Sill", new Vector3(x, y - h / 2f - 0.12f, front - 0.14f), new Vector3(w + 0.3f, 0.08f, 0.22f), plinth);
    }

    private void Derrick(Vector3 baseCenter, float baseHalf, float topHalf, float h)
    {
        Transform transform = new GameObject("Derrick").transform;
        transform.SetParent(base.transform, worldPositionStays: false);
        Vector3[] array = Corners(baseCenter, baseHalf);
        Vector3[] array2 = Corners(baseCenter + Vector3.up * h, topHalf);
        for (int i = 0; i < 4; i++)
        {
            Beam("Leg", array[i], array2[i], 0.22f, transform);
        }
        int num = 5;
        for (int j = 1; j <= num; j++)
        {
            float t = (float)(j - 1) / (float)num;
            float t2 = (float)j / (float)num;
            for (int k = 0; k < 4; k++)
            {
                Vector3 a = Vector3.Lerp(array[k], array2[k], t2);
                Vector3 b = Vector3.Lerp(array[(k + 1) % 4], array2[(k + 1) % 4], t2);
                Beam("Ring", a, b, 0.1f, transform);
                Vector3 a2 = Vector3.Lerp(array[k], array2[k], t);
                Beam("Brace", a2, b, 0.07f, transform);
            }
        }
        Box("CrownBlock", baseCenter + Vector3.up * (h + 0.35f), new Vector3(topHalf * 2.4f, 0.7f, topHalf * 2.4f), metal).transform.SetParent(transform, worldPositionStays: true);
    }

    private static Vector3[] Corners(Vector3 c, float s)
    {
        return new Vector3[4]
        {
            c + new Vector3(0f - s, 0f, 0f - s),
            c + new Vector3(s, 0f, 0f - s),
            c + new Vector3(s, 0f, s),
            c + new Vector3(0f - s, 0f, s)
        };
    }

    private void Beam(string name, Vector3 a, Vector3 b, float t, Transform parent)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.name = name;
        gameObject.transform.SetParent(parent, worldPositionStays: false);
        Vector3 vector = b - a;
        gameObject.transform.localPosition = (a + b) / 2f;
        gameObject.transform.localRotation = Quaternion.FromToRotation(Vector3.up, vector.normalized);
        gameObject.transform.localScale = new Vector3(t, vector.magnitude, t);
        gameObject.GetComponent<Renderer>().sharedMaterial = metal;
        FitUv(gameObject, metal);
    }

    private GameObject Box(string name, Vector3 c, Vector3 s, Material m)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.name = name;
        gameObject.transform.SetParent(base.transform, worldPositionStays: false);
        gameObject.transform.localPosition = c;
        gameObject.transform.localScale = s;
        gameObject.GetComponent<Renderer>().sharedMaterial = m;
        FitUv(gameObject, m);
        return gameObject;
    }

    private void Cyl(string name, Vector3 c, float r, float h, Material m)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obj.name = name;
        obj.transform.SetParent(base.transform, worldPositionStays: false);
        obj.transform.localPosition = c;
        obj.transform.localScale = new Vector3(r * 2f, h / 2f, r * 2f);
        obj.GetComponent<Renderer>().sharedMaterial = m;
    }

    private void AddLight(string name, Vector3 pos, LightType type, Color col, float intensity, float range, Quaternion rot, float angle)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(base.transform, worldPositionStays: false);
        obj.transform.localPosition = pos;
        obj.transform.localRotation = rot;
        Light light = obj.AddComponent<Light>();
        light.type = type;
        light.color = col;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.Soft;
        if (type == LightType.Spot)
        {
            light.spotAngle = angle;
        }
    }

    private void LoadMaterials()
    {
        concrete = Mat("Varley_Concrete", new Color(0.46f, 0.45f, 0.42f), 0.15f);
        plinth = Mat("Varley_ConcreteDark", new Color(0.28f, 0.27f, 0.26f), 0.2f);
        roof = Mat("Varley_Roof", new Color(0.2f, 0.2f, 0.21f), 0.1f);
        glassDark = Mat("Varley_GlassDark", new Color(0.05f, 0.06f, 0.07f), 0.9f);
        glassLit = Mat("Varley_GlassLit", new Color(0.9f, 0.75f, 0.5f), 0.8f, new Color(1f, 0.78f, 0.5f) * 2.2f);
        metal = Mat("Varley_Metal", new Color(0.24f, 0.25f, 0.26f), 0.45f);
        door = Mat("Varley_Door", new Color(0.3f, 0.34f, 0.3f), 0.35f);
        sign = Mat("Varley_Sign", new Color(0.62f, 0.58f, 0.48f), 0.2f);
        Tex(concrete, "varley_concrete", normal: true);
        Tex(plinth, "varley_concrete_dark", normal: true);
        Tex(metal, "varley_metal", normal: true);
        Tex(door, "varley_door", normal: true);
        Tex(sign, "varley_sign", normal: false);
        Tex(glassDark, "varley_glass_dark", normal: false);
        Tex(glassLit, "varley_glass_lit", normal: false, emissive: true);
        uvRules.Clear();
        uvRules[concrete] = new UvRule
        {
            u = 6f,
            v = 12f,
            worldHeight = true
        };
        uvRules[plinth] = new UvRule
        {
            u = 2f,
            v = 2f
        };
        uvRules[metal] = new UvRule
        {
            u = 2f,
            v = 2f
        };
    }

    private void Tex(Material m, string name, bool normal, bool emissive = false)
    {
    }

    private void FitUv(GameObject g, Material m)
    {
        if (m == null || !uvRules.TryGetValue(m, out var value))
        {
            return;
        }
        MeshFilter component = g.GetComponent<MeshFilter>();
        Mesh mesh = Object.Instantiate(component.sharedMesh);
        mesh.name = g.name + "_uv";
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] array = new Vector2[vertices.Length];
        Transform transform = g.transform;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vector = base.transform.InverseTransformPoint(transform.TransformPoint(vertices[i]));
            Vector3 vector2 = transform.TransformDirection(normals[i]);
            Vector3 vector3 = new Vector3(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y), Mathf.Abs(vector2.z));
            float num;
            float num2;
            if (vector3.y >= vector3.x && vector3.y >= vector3.z)
            {
                num = vector.x;
                num2 = vector.z;
            }
            else if (vector3.x >= vector3.z)
            {
                num = vector.z;
                num2 = vector.y;
            }
            else
            {
                num = vector.x;
                num2 = vector.y;
            }
            if (value.worldHeight && vector3.y < 0.5f)
            {
                num2 = vector.y;
            }
            array[i] = new Vector2(num / value.u, num2 / value.v);
        }
        mesh.uv = array;
        mesh.RecalculateTangents();
        component.sharedMesh = mesh;
    }

    private Material Mat(string name, Color c, float smooth, Color? emit = null)
    {
        return null;
    }
}
