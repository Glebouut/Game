using UnityEngine;

[RequireComponent(typeof(Cloth))]
public class BannerSound : MonoBehaviour
{
    public AudioClip rustleLoop;

    public AudioClip[] flaps;

    public AudioClip[] slaps;

    [Header("Громкость и чувствительность")]
    [Range(0f, 1f)]
    public float volume = 0.8f;

    public float rustleGain = 0.4f;

    public float flapThreshold = 1.5f;

    public float flapSpike = 1.8f;

    public float wallZ = 0.05f;

    public float slapSpeed = 0.8f;

    public float cooldown = 0.18f;

    private Cloth cloth;

    private AudioSource loop;

    private AudioSource shots;

    private Vector3[] prev;

    private float smooth;

    private float nextFlap;

    private float nextSlap;

    private void Start()
    {
        cloth = GetComponent<Cloth>();
        loop = MakeSource(looping: true);
        shots = MakeSource(looping: false);
        if (rustleLoop != null)
        {
            loop.clip = rustleLoop;
            loop.volume = 0f;
            loop.Play();
        }
    }

    private AudioSource MakeSource(bool looping)
    {
        AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
        audioSource.loop = looping;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 3f;
        audioSource.maxDistance = 45f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        return audioSource;
    }

    private void Update()
    {
        Vector3[] vertices = cloth.vertices;
        if (vertices == null || vertices.Length == 0)
        {
            return;
        }
        if (prev == null || prev.Length != vertices.Length)
        {
            prev = (Vector3[])vertices.Clone();
            return;
        }
        float num = Mathf.Max(Time.deltaTime, 0.0001f);
        float num2 = 0f;
        int num3 = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vector = vertices[i] - prev[i];
            num2 += vector.magnitude;
            if (vertices[i].z > wallZ && vector.z / num > slapSpeed)
            {
                num3++;
            }
            prev[i] = vertices[i];
        }
        float num4 = num2 / (float)vertices.Length / num;
        smooth = Mathf.Lerp(smooth, num4, 1f - Mathf.Exp((0f - num) * 5f));
        loop.volume = Mathf.Clamp01(smooth * rustleGain) * volume;
        loop.pitch = 0.9f + Mathf.Clamp01(smooth * 0.3f) * 0.25f;
        if (flaps != null && flaps.Length != 0 && Time.time > nextFlap && num4 > flapThreshold && num4 > smooth * flapSpike)
        {
            Play(flaps, Mathf.Clamp01(num4 / (flapThreshold * 3f)));
            nextFlap = Time.time + cooldown;
        }
        if (slaps != null && slaps.Length != 0 && Time.time > nextSlap && num3 > vertices.Length / 20)
        {
            Play(slaps, 0.9f);
            nextSlap = Time.time + cooldown * 2f;
        }
    }

    private void Play(AudioClip[] set, float strength)
    {
        shots.pitch = Random.Range(0.85f, 1.15f);
        shots.PlayOneShot(set[Random.Range(0, set.Length)], Mathf.Lerp(0.4f, 1f, strength) * volume);
    }
}
