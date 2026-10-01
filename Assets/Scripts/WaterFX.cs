using UnityEngine;

public class WaterFX : MonoBehaviour
{
    public static WaterFX I;

    public ParticleSystem runnels;

    public ParticleSystem drips;

    private void Awake()
    {
        I = this;
    }

    public void Runnel(Vector3 from, Vector3 to, float time)
    {
        if (!(runnels == null) && !(time <= 0.01f))
        {
            ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
            emitParams.position = from;
            emitParams.velocity = (to - from) / time;
            emitParams.startLifetime = time;
            emitParams.applyShapeToPosition = false;
            ParticleSystem.EmitParams emitParams2 = emitParams;
            runnels.Emit(emitParams2, 1);
        }
    }

    public void Drip(Vector3 at, float size)
    {
        if (!(drips == null))
        {
            ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
            emitParams.position = at;
            emitParams.velocity = Vector3.down * Random.Range(0.1f, 0.4f);
            emitParams.startSize = size;
            emitParams.applyShapeToPosition = false;
            ParticleSystem.EmitParams emitParams2 = emitParams;
            drips.Emit(emitParams2, 1);
        }
    }
}
