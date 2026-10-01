using UnityEngine;

public class BannerCloth : MonoBehaviour
{
    [Header("Крепление")]
    [Range(0f, 1f)]
    public float pinnedTop = 0.72f;

    [Range(0f, 1f)]
    public float pinnedBottom = 0.35f;

    public bool tornOnRight = true;

    [Header("Ветер (в мировых осях)")]
    public Vector3 wind = new Vector3(4f, 0f, -2f);

    public float gust = 9f;

    public float gustSpeed = 0.7f;

    private Cloth cloth;

    [ContextMenu("Make Cloth")]
    private void MakeCloth()
    {
        MeshFilter component = GetComponent<MeshFilter>();
        MeshRenderer component2 = GetComponent<MeshRenderer>();
        if (component == null || component2 == null)
        {
            Debug.LogWarning("BannerCloth: тут уже ткань или нет меша.");
            return;
        }
        Mesh sharedMesh = component.sharedMesh;
        Material[] sharedMaterials = component2.sharedMaterials;
        Object.DestroyImmediate(component2);
        Object.DestroyImmediate(component);
        SkinnedMeshRenderer skinnedMeshRenderer = base.gameObject.AddComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.sharedMesh = sharedMesh;
        skinnedMeshRenderer.sharedMaterials = sharedMaterials;
        skinnedMeshRenderer.updateWhenOffscreen = true;
        Cloth obj = base.gameObject.AddComponent<Cloth>();
        obj.stretchingStiffness = 0.95f;
        obj.bendingStiffness = 0.25f;
        obj.damping = 0.06f;
        obj.useGravity = true;
        obj.worldVelocityScale = 0f;
        Debug.Log("BannerCloth: готово, жми Play.");
    }

    private void Start()
    {
        cloth = GetComponent<Cloth>();
        if (cloth == null)
        {
            return;
        }
        Vector3[] vertices = GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices;
        float num = float.MaxValue;
        float num2 = float.MinValue;
        float num3 = float.MaxValue;
        float num4 = float.MinValue;
        Vector3[] array = vertices;
        for (int i = 0; i < array.Length; i++)
        {
            Vector3 vector = array[i];
            num = Mathf.Min(num, vector.x);
            num2 = Mathf.Max(num2, vector.x);
            num3 = Mathf.Min(num3, vector.y);
            num4 = Mathf.Max(num4, vector.y);
        }
        ClothSkinningCoefficient[] array2 = new ClothSkinningCoefficient[cloth.vertices.Length];
        for (int j = 0; j < array2.Length && j < vertices.Length; j++)
        {
            float num5 = (vertices[j].x - num) / (num2 - num);
            if (!tornOnRight)
            {
                num5 = 1f - num5;
            }
            float num6 = (num4 - vertices[j].y) / (num4 - num3);
            bool flag = (num6 < 0.02f && num5 < pinnedTop) || num5 < 0.02f || (num6 > 0.98f && num5 < pinnedBottom);
            array2[j].maxDistance = (flag ? 0f : 10f);
            array2[j].collisionSphereDistance = float.MaxValue;
        }
        cloth.coefficients = array2;
    }

    private void Update()
    {
        if (!(cloth == null))
        {
            float num = Time.time * gustSpeed;
            float num2 = Mathf.PerlinNoise(num, 0.37f);
            float num3 = Mathf.Pow(Mathf.PerlinNoise(num * 2.3f, 5.1f), 3f);
            cloth.externalAcceleration = wind * (0.4f + num2);
            cloth.randomAcceleration = new Vector3(1f, 0.4f, 1f) * gust * (0.3f + num3 * 2f);
        }
    }
}
