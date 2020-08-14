using UnityEngine;

[RequireComponent(typeof(CameraShakeManager))]
public class CameraShakeController : MonoBehaviour
{
    [Header("震动类型")]
    public CameraShake.ShakeType ShakeType = CameraShake.ShakeType.EaseOut;
    [Header("Noise")]
    public CameraShake.NoiseType NoiseType = CameraShake.NoiseType.Perlin;
    [Header("位移")]
    public Vector3 MoveExtents = new Vector3(0.1f, 0.1f, 0);

    public Vector3 RotateExtents;
    [Header("速度")]
    public float Speed = 10f;
    [Header("持续时间")]
    public float Duration = 2f;

    public void TestShake()
    {
        var shakeInstance = new GameObject("ShakeEffect").AddComponent<CameraShake>();

        shakeInstance.Shake = ShakeType;
        shakeInstance.Noise = NoiseType;
        shakeInstance.MoveExtents = MoveExtents;
        shakeInstance.RotateExtents = RotateExtents;
        shakeInstance.Speed = Speed;
        shakeInstance.Duration = Duration;

        CameraShakeManager.Instance.AddShake(shakeInstance);
    }
}
