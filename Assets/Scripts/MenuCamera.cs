using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCamera : MonoBehaviour
{
    [Header("Дыхание")]
    public float driftPosition = 0.06f;

    public float driftRotation = 0.5f;

    public float driftSpeed = 0.12f;

    [Header("За мышкой")]
    public float mouseYaw = 1.5f;

    public float mousePitch = 0.8f;

    public float mouseSmooth = 2.5f;

    private Vector3 basePos;

    private Quaternion baseRot;

    private Vector2 mouse;

    private void Start()
    {
        basePos = base.transform.localPosition;
        baseRot = base.transform.localRotation;
    }

    private void LateUpdate()
    {
        float num = Time.time * driftSpeed;
        Vector3 vector = new Vector3(Mathf.PerlinNoise(num, 0.1f) - 0.5f, Mathf.PerlinNoise(0.3f, num) - 0.5f, Mathf.PerlinNoise(num + 5f, 7f) - 0.5f) * 2f * driftPosition;
        Vector3 vector2 = new Vector3(Mathf.PerlinNoise(num + 11f, 3f) - 0.5f, Mathf.PerlinNoise(9f, num + 2f) - 0.5f, Mathf.PerlinNoise(num + 21f, 13f) - 0.5f) * 2f * driftRotation;
        Vector2 b = Vector2.zero;
        Vector2 vector3 = Vector2.zero;
        bool flag = false;
        if (Mouse.current != null)
        {
            vector3 = Mouse.current.position.ReadValue();
            flag = true;
        }
        if (flag)
        {
            b = new Vector2(vector3.x / (float)Screen.width - 0.5f, vector3.y / (float)Screen.height - 0.5f) * 2f;
            b = Vector2.ClampMagnitude(b, 1.5f);
        }
        mouse = Vector2.Lerp(mouse, b, 1f - Mathf.Exp((0f - Time.deltaTime) * mouseSmooth));
        base.transform.localPosition = basePos + vector;
        base.transform.localRotation = baseRot * Quaternion.Euler((0f - mouse.y) * mousePitch + vector2.x, mouse.x * mouseYaw + vector2.y, vector2.z * 0.5f);
    }
}
