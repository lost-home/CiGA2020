using System.Collections.Generic;
using BeauRoutine;
using Characters;
using UnityEngine;


public class GameRule : AbstractMonoBehaviour {

	public const string VAR_AvailableExpressPackages = "AvailableExpressPackages";
	public const string VAR_PlacedExpressPackages = "PlacedExpressPackages";
	public const string VAR_Player = "Player";
	public const string VAR_PlayerHand = "PlayerHand";
	public const string VAR_ExpressPackageGenerator = "ExpressPackageGenerator";
	public const string VAR_FillPiter = "FillPiter";
	public const string VAR_EscapePoint = "EscapePoint";

	public BlackboardComponent worldBlackboard;
	public GameObject player;
	public Transform  expressPackageGenerator;
	public Transform  fillPit;
	public Transform  escapePoint;
	public GameObject[] cpuActors;

	protected override void Awake () {
		base.Awake ();
		timeLayer = CupheadTime.Layer.Default;

		Routine.Settings.DebugMode = false;
		
		// 设置世界变量
		worldBlackboard.Set ( VAR_AvailableExpressPackages, new List<GameObject>() );
		worldBlackboard.Set ( VAR_PlacedExpressPackages, new List<GameObject>() );
		worldBlackboard.Set ( VAR_Player, player.transform );
		worldBlackboard.Set ( VAR_PlayerHand, player.GetComponentInChildren<Hand>() );
		worldBlackboard.Set ( VAR_ExpressPackageGenerator, expressPackageGenerator );
		worldBlackboard.Set ( VAR_FillPiter, fillPit );
		worldBlackboard.Set ( VAR_EscapePoint, escapePoint );

		// 监听游戏事件
		EventKit.Subscribe< GameObject > ( GlobalSymbol.EVT_ExpressPackageGenerated, onExpressPackageGenerated );
		EventKit.Subscribe< GameObject > ( GlobalSymbol.EVT_ExpressPackagePlaced, onExpressPackagePlaced );
		EventKit.Subscribe< GameObject > ( GlobalSymbol.EVT_ExpressPackageDestroyed, onExpressPackageDestroyed );
	}
	
	private void OnGUI () {
		// var av = worldBlackboard.Get< List< GameObject > > ( VAR_AvailableExpressPackages );
		// var i = 0;
		// foreach ( var pack in av ) {
		// 	GUI.Label ( new Rect ( 0, 100 + i * 20, 400, 100 ), $"package：{(Vector2)pack.transform.position}" );
		// 	i++;
		// }
	}

	private void onExpressPackageGenerated ( GameObject pack ) {
		var av = worldBlackboard.Get< List< GameObject > > ( VAR_AvailableExpressPackages );
		av.Add ( pack );
		
		buildCPU ();
	}
	
	private void onExpressPackagePlaced ( GameObject pack ) {
		var av = worldBlackboard.Get< List< GameObject > > ( VAR_AvailableExpressPackages );
		var pl = worldBlackboard.Get< List< GameObject > > ( VAR_PlacedExpressPackages );
		av.Remove ( pack );
		pl.Add ( pack );

		buildCPU ();
	}
	
	private void onExpressPackageDestroyed ( GameObject pack ) {
		var av = worldBlackboard.Get< List< GameObject > > ( VAR_AvailableExpressPackages );
		var pl = worldBlackboard.Get< List< GameObject > > ( VAR_PlacedExpressPackages );
		av.Remove ( pack );
		pl.Remove ( pack );
	}

	private void buildCPU () {
		Object.Instantiate ( MathUtils.RandomArrayElement ( cpuActors ), escapePoint.position, Quaternion.identity );
	}
}