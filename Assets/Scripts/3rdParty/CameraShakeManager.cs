using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Attach CameraShakeManager to your primary Camera GameObject.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraShakeManager : MonoBehaviour
{

    /// <summary>
    /// Internal list of active camera shake components.
    /// </summary>
    private List<CameraShake> m_activeShakes = new List<CameraShake>();

    /// <summary>
    /// Singleton reference.
    /// </summary>
    private static CameraShakeManager m_instance;
    public static CameraShakeManager Instance
    {
        get { return m_instance; }
    }

	private static Camera m_camera;
    /// <summary>
    /// Convenience getter for the camera.
    /// </summary>
    public static Camera Camera
    {
        get { return m_camera; }
        set { 
			m_camera = value;
			m_instance = AffirmShakeManager ( value );
		}
    }

    /// <summary>
    /// Initialize singleton.
    /// </summary>
    void Awake()
    {
        m_instance = this;
		m_camera = GetComponent < Camera >();
	}

    /// <summary>
    /// Unity recommends most camera logic run in late update, to ensure the camera is most up to date this frame.
    /// </summary>
    void LateUpdate()
    {
		if ( Camera == null ) {
			return;
		}

        Matrix4x4 shakeMatrix = Matrix4x4.identity;

        // For each active shake
        foreach (var shake in m_activeShakes.Reverse<CameraShake>())
        {

            if (shake == null)
            {
                m_activeShakes.Remove(shake);
                continue;
            }

            // Concatenate its shake matrix
            shakeMatrix *= shake.ComputeMatrix();

            // If done, remove
            if (shake.IsDone())
            {
                m_activeShakes.Remove(shake);
                Destroy(shake.gameObject);
                Camera.ResetWorldToCameraMatrix();
            }
        }

        // Camera always looks down the negative z-axis
        shakeMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));

        // Update camera matrix
        if (m_activeShakes.Count > 0)
        {
            Camera.worldToCameraMatrix = shakeMatrix * transform.worldToLocalMatrix;
        }
    }

    /// <summary>
    /// Start a camera shake.
    /// </summary>
    /// <param name="name">The resource name of the shake to play.</param>
    /// <returns>A reference to the camera shake object.</returns>
    public CameraShake Play(string name)
    {
        var cs = Instantiate(Resources.Load<GameObject>(name), transform) as GameObject;
        if (cs != null)
        {
            m_activeShakes.Add(cs.GetComponent<CameraShake>());
        }
        return cs.GetComponent<CameraShake>();
    }

    /// <summary>
    /// 向管理器添加一个相机震动特效实例
    /// </summary>
    /// <param name="ins"></param>
    public void AddShake(CameraShake ins)
    {
		if ( ins != null ) {
			m_activeShakes.Add ( ins );
		}
	}

	/// <summary>
	/// 执行一个屏震事件
	/// </summary>
	/// <param name="shakeEvt"></param>
	public void AddShake( string shakeEvent ) {
		// CameraShake.ShakeType shakeType;
		// CameraShake.NoiseType noiseType;
		// Vector3 moveExt;
		// float speed;
		// float duration;
		//
		// ParseUtil.ParseShakeEvent ( shakeEvent, out shakeType, out noiseType, out moveExt, out speed, out duration );
		//
		// var shakeInstance = new GameObject ( shakeEvent ).AddComponent<CameraShake> ();
		//
		// shakeInstance.Shake = shakeType;
		// shakeInstance.Noise = noiseType;
		// shakeInstance.MoveExtents = moveExt;
		// shakeInstance.RotateExtents = Vector3.zero;
		// shakeInstance.Speed = speed;
		// shakeInstance.Duration = duration;
		//
		// AddShake ( shakeInstance );
	}

    /// <summary>
    /// Stop a camera shake.
    /// </summary>
    /// <param name="shake">The camera shake to stop.</param>
    /// <param name="immediate">True to stop immediately this frame, false to ramp down.</param>
    public void Stop(CameraShake shake, bool immediate = false)
    {
        if (shake == null) return;

        shake.Finish(immediate);
    }

    /// <summary>
    /// Stop all active camera shakes.
    /// </summary>
    /// <param name="immediate">True to stop immediately this frame, false to ramp down.</param>
    public void StopAll(bool immediate = false)
    {
        foreach (var shake in m_activeShakes)
        {
            Stop(shake, immediate);
        }
    }

	private static CameraShakeManager AffirmShakeManager ( Camera camera ) {

		Debug.Assert ( camera != null );

		var ins = camera.GetComponent < CameraShakeManager >();
		if ( ins == null ) {
			return camera.gameObject.AddComponent < CameraShakeManager >();
		}

		//Debug.Log ( $"CameraShakeManager::AffirmShakeManager() 实例 {Instance.transform.GetPath()}" );
		return ins;
	}

	/// <summary>
	/// 拷贝一个组件到目标物体上
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="original"></param>
	/// <param name="destination"></param>
	/// <returns>克隆组件引用</returns>
	public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}