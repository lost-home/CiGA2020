namespace Nez.AI.GOAP {
	
	public class CheckEnemyTimeLayerInterval : AICondition {
		
		public string        key;
		public CompareMethod checkType = CompareMethod.GreaterOrEqualTo;
		public float          value;

		private BlackboardComponent blackboard;

		public override void Awake () {
			blackboard = gameObject.GetComponentInChildren< BlackboardComponent > ();
		}

		public override bool OnCheck () {
			return OperationTools.Compare ( TimeManager.EnemyTime - blackboard.Get < float > ( key ), value,
				checkType, 0 );
		}
	}
}