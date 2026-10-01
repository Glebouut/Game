using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WetSurface : MonoBehaviour
{
    private struct Flow
    {
        public float arrive;

        public int edge;

        public int seg;
    }

    [Header("Углы кромки, по порядку обхода")]
    public Transform[] corners;

    public bool closedLoop = true;

    [Header("Вода")]
    public float flowSpeed = 0.5f;

    public float dropVolume = 3f;

    public Vector2 dropSize = new Vector2(0.025f, 0.045f);

    public float segmentsPerMeter = 5f;

    [Header("Струйки по поверхности")]
    public bool showRunnels = true;

    [Range(0f, 1f)]
    public float runnelChance = 0.25f;

    private readonly List<ParticleCollisionEvent> hits = new List<ParticleCollisionEvent>();

    private readonly List<Flow> flows = new List<Flow>();

    private float[][] water;

    private int edgeCount;

    private bool Ready
    {
        get
        {
            if (corners != null && corners.Length >= 2)
            {
                return WaterFX.I != null;
            }
            return false;
        }
    }

    private int EdgeCount
    {
        get
        {
            if (corners.Length >= 2)
            {
                if (!closedLoop || corners.Length <= 2)
                {
                    return corners.Length - 1;
                }
                return corners.Length;
            }
            return 0;
        }
    }

    private void GetEdge(int i, out Vector3 a, out Vector3 b)
    {
        a = corners[i].position;
        b = corners[(i + 1) % corners.Length].position;
    }

    private void EnsureBins()
    {
        int num = EdgeCount;
        if (water == null || edgeCount != num)
        {
            edgeCount = num;
            water = new float[num][];
            for (int i = 0; i < num; i++)
            {
                GetEdge(i, out var a, out var b);
                int num2 = Mathf.Max(1, Mathf.RoundToInt(Vector3.Distance(a, b) * segmentsPerMeter));
                water[i] = new float[num2];
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!Ready)
        {
            return;
        }
        ParticleSystem component = other.GetComponent<ParticleSystem>();
        if (component == null)
        {
            return;
        }
        EnsureBins();
        int collisionEvents = component.GetCollisionEvents(base.gameObject, hits);
        for (int i = 0; i < collisionEvents; i++)
        {
            Vector3 intersection = hits[i].intersection;
            int num = 0;
            float num2 = 0f;
            float num3 = float.MaxValue;
            Vector3 to = intersection;
            for (int j = 0; j < edgeCount; j++)
            {
                GetEdge(j, out var a, out var b);
                Vector3 vector = b - a;
                float num4 = Mathf.Clamp01(Vector3.Dot(intersection - a, vector) / Mathf.Max(vector.sqrMagnitude, 0.0001f));
                Vector3 vector2 = a + vector * num4;
                float num5 = Vector3.Distance(intersection, vector2);
                if (num5 < num3)
                {
                    num3 = num5;
                    num = j;
                    num2 = num4;
                    to = vector2;
                }
            }
            float num6 = num3 / flowSpeed * Random.Range(0.8f, 1.3f);
            int num7 = water[num].Length;
            int seg = Mathf.Min(num7 - 1, (int)(num2 * (float)num7));
            flows.Add(new Flow
            {
                arrive = Time.time + num6,
                edge = num,
                seg = seg
            });
            if (showRunnels && num6 > 0.1f && Random.value < runnelChance)
            {
                WaterFX.I.Runnel(intersection, to, num6);
            }
        }
    }

    private void Update()
    {
        if (!Ready)
        {
            return;
        }
        EnsureBins();
        float time = Time.time;
        for (int num = flows.Count - 1; num >= 0; num--)
        {
            if (!(flows[num].arrive > time))
            {
                Flow flow = flows[num];
                if (flow.edge < water.Length && flow.seg < water[flow.edge].Length)
                {
                    water[flow.edge][flow.seg] += 1f;
                }
                flows[num] = flows[flows.Count - 1];
                flows.RemoveAt(flows.Count - 1);
            }
        }
        for (int i = 0; i < water.Length; i++)
        {
            GetEdge(i, out var a, out var b);
            float[] array = water[i];
            for (int j = 0; j < array.Length; j++)
            {
                if (!(array[j] < dropVolume))
                {
                    array[j] -= dropVolume;
                    float t = ((float)j + Random.value) / (float)array.Length;
                    WaterFX.I.Drip(Vector3.Lerp(a, b, t), Random.Range(dropSize.x, dropSize.y));
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (corners == null || corners.Length < 2)
        {
            return;
        }
        Gizmos.color = new Color(0.3f, 0.7f, 1f);
        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i] != null)
            {
                Gizmos.DrawSphere(corners[i].position, 0.05f);
            }
        }
        int num = EdgeCount;
        for (int j = 0; j < num; j++)
        {
            Transform transform = corners[j];
            Transform transform2 = corners[(j + 1) % corners.Length];
            if (transform != null && transform2 != null)
            {
                Gizmos.DrawLine(transform.position, transform2.position);
            }
        }
    }
}
