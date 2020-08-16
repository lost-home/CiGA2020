using UnityEngine;

namespace Nez.AI.GOAP {
	
	public class IsTargetHigherThanSelf : AICondition {
		
		private BlackboardComponent blackboard;
		private Transform target;

		public override void Awake () {
			blackboard = gameObject.GetComponentInChildren<BlackboardComponent>();
		}

		public override bool OnCheck () {

			target = blackboard.Get< Transform > ( "Target" );

			if ( target == null ) {
				return false;
			}
			
			return target.position.y > gameObject.transform.position.y;
		}
	}
}
