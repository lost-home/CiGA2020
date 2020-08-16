using Characters;
using UnityEngine;


namespace Nez.AI.GOAP {
	
	public class IsHandHasExpressPackage : AICondition {
		
		private Hand hand;

		public override void Awake () {
			hand = gameObject.GetComponentInChildren<Hand>();
		}

		public override bool OnCheck () {
			return hand.equipped != null && hand.equipped.GetType () == typeof ( ExpressPackage );
		}
	}
	
	public class IsTargetHandHasExpressPackage : AICondition {

		public string KEY_Target;
		
		private BlackboardComponent blackboard;
		private Hand hand;

		public override void Awake () {
			blackboard = GameObject.Find ( "GameRule" ).GetComponentInChildren<BlackboardComponent>();			
		}

		public override bool OnCheck () {
			var target = blackboard.Get< Transform > ( KEY_Target );
			if ( target == null ) {
				return false;
			}
			else {
				hand = target.GetComponentInChildren<Hand>();
			}
			
			return hand != null && hand.equipped != null && hand.equipped.GetType () == typeof ( ExpressPackage );
		}
	}
}