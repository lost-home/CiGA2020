
public static class GlobalSymbol {

	#region 战斗属性相关
	public const string HP                    = "HP";
	public const string ENERGY                = "Energy";
	public const string ENERGY_RECOVERY_SPEED = "EnergyRecoverySpeed";
	public const string ATK                   = "Atk";
	public const string FLAG_INVINCIBLE       = "Invincible";
	public const string FLAG_DEFENSE          = "Defense";
	public const string DEFENSE_DIR           = "DefenseDirection"; // 0: 左右， 1: 世界方向右， -1:世界方向左
	public const string FLAG_SUPER_DEFENSE    = "SuperDefense";
	public const string STAMINA               = "Stamina";
	public const string FLAG_TARGET_LOCKED    = "TargetLocked";    // 黑板目标锁定标志
	public const string FLAG_BLOCK_FIRE       = "BlockFire";       // 火属性无效标志
	public const string FLAG_BLOCK_ICE        = "BlockIce";        // 冰属性无效标志
	public const string FLAG_BLOCK_POISON     = "BlockPoison";     // 毒属性无效标志
	public const string FLAG_BLOCK_STUN       = "BlockStun";       // 眩晕无效标志
	public const string FLAG_BLOCK_DEEPFREEZE = "BlockDeepFreeze"; // 冰封无效标志
	public const string FLAG_BLOCK_TAUNT      = "BlockTaunt";      // 嘲讽无效标志
	public const string FLAG_BLOCK_CHAOS      = "BlockChaos";      // 混乱无效标志
	public const string FLAG_BLOCK_DEVOUR     = "BlockDevour";     // 吞噬无效标志
	#endregion

	#region 2D平台控制属性相关
	public const string MOVE_SPEED = "MoveSpeed";
	public const string MOVE_SPEED_AIR = "MoveSpeed_Air";
	public const string MOVE_SPEED_X_DROP = "MoveSpeedX_Drop";
	public const string ROLL_SPEED = "RollSpeed";
	public const string JUMP_MAX_SEGMENTS = "JumpMaxSegments";
	public const string GRAVITY = "Gravity";
	public const string JUMP_HEIGHT = "JumpHeight";
	public const string MAX_JUMP_HEIGHT = "MaxJumpHeight";
	public const string MIN_JUMP_HEIGHT = "MinJumpHeight";
	#endregion

	#region 2D平台Layer相关
	public const string LAYER_PLATFORM = "terrain";
	public const string LAYER_ONEWAY_PLATFORM = "platform";
	#endregion

	#region 输入变量

	public const string INPUTVAR_MOVE_HORIZONTAL = "InputVar_MoveHorizontal";
	public const string INPUTVAR_MOVE_VERTICAL = "InputVar_MoveVertical";
	
	public const string INPUTACTION_Collect = "Collect";
	public const string INPUTACTION_Grab = "Grab";
	public const string INPUTACTION_Jump = "Jump";
	public const string INPUTACTION_MoveHorizontal = "MoveHorizontal";
	public const string INPUTACTION_MoveVertical = "MoveVertical";

	#endregion
	
	#region 游戏事件
	public const string EVT_ExpressPackageGenerated = "ExpressPackageGenerated";
	public const string EVT_ExpressPackagePlaced = "ExpressPackagePlaced";
	public const string EVT_ExpressPackageDestroyed = "ExpressPackageDestroyed";
	public const string EVT_Player = "ExpressPackagePlaced";

	#endregion

	#region 范围相关
	public const string LOOK_RANGE_X = "LookRangeX";
	public const string LOOK_RANGE_Y = "LookRangeY";
	public const string CHASE_RANGE_X = "ChaseRangeX";
	public const string CHASE_RANGE_Y = "ChaseRangeY";
	public const string MELEE_ATTACK_RANGE_X = "MeleeAttackRangeX";
	public const string MELEE_ATTACK_RANGE_Y = "MeleeAttackRangeY";
	public const string REMOTE_ATTACK_RANGE_X = "RemoteAttackRangeX";
	public const string REMOTE_ATTACK_RANGE_Y = "RemoteAttackRangeY";
	public const string JUMP_ATTACK_RANGE = "JumpAttackRange";
	public const string SAFE_RANGE_X = "SafeRangeX";
	public const string SAFE_RANGE_Y = "SafeRangeY";
	public const string PATROL_DISTANCE = "PatrolDistance";
	public const string CENTER_OFFSET_X = "CenterOffsetX";
	public const string CENTER_OFFSET_Y = "CenterOffsetY";
	#endregion
	
}
