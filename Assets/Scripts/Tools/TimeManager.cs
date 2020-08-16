using System.Collections.Generic;


/// <summary>
/// 全局时间回调管理器，时间层引用CupheadTime
/// </summary>
public class TimeManager : SingletonBehaviour< TimeManager > {
	public static float DefaultTime { get; private set; }
	public static float PlayerTime  { get; private set; }
	public static float EnemyTime   { get; private set; }
	public static float UITime      { get; private set; }

	protected override void Awake () {
		Persistent = true;

		base.Awake ();
	}

	private void Update () {
		DefaultTime += CupheadTime.Delta [ CupheadTime.Layer.Default ];
		PlayerTime  += CupheadTime.Delta [ CupheadTime.Layer.Player ];
		EnemyTime   += CupheadTime.Delta [ CupheadTime.Layer.Enemy ];
		UITime      += CupheadTime.Delta [ CupheadTime.Layer.UI ];
	}

#if UNITY_EDITOR
	void OnGUI () {
		//var pTimer = GetPromiseTimer(CupheadTime.Layer.Player);

		//GUI.Label(new Rect(0, 100f, 300, 100), $"{pTimer.curFrame} {pTimer.curTime}");

		//GUI.Label(new Rect(0, 120f, 300, 100), $"Waiting List Count:{pTimer.waiting.Count}");

		//int i = 0;
		//foreach (var promise in pTimer.waiting)
		//{
		//    GUI.Label(new Rect(0, 140f + i * 20f, 300, 100), $"{promise.pendingPromise.Id} {promise.timeStarted}");
		//    i++;
		//}
	}
#endif
}