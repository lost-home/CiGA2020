using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraShakeController))]
class CameraShakeControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        if (GUILayout.Button("测试一下"))
        {
            (target as CameraShakeController).TestShake();
        }
    }
}
