using UnityEngine;
using RPGStatSystem;

namespace Nez.AI.GOAP {
	
	public class CheckTargetInRange : AICondition {

		public string KEY_Target;
		public string KEY_RANGE_X;
		public string KEY_RANGE_Y;

		private RPGStatCollection   statCollection;
		private BlackboardComponent blackboard;
		private Transform           target;

		public override void Awake () {
			statCollection = gameObject.GetComponentInChildren<RPGStatCollection>();
			blackboard     = GameObject.Find ( "GameRule" ).GetComponentInChildren<BlackboardComponent>();
		}

		public override bool OnCheck () {

			target = blackboard.Get< Transform > ( KEY_Target );

			if ( target == null ) {
				Debug.Log ( $"CheckTargetInRange: {KEY_Target} is null." );
				return false;
			}

			Vector3 centerOffset = Vector3.zero;
			if ( statCollection.ContainStat ( GlobalSymbol.CENTER_OFFSET_X ) ) {
				centerOffset.x = statCollection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_X );
			}

			if ( statCollection.ContainStat ( GlobalSymbol.CENTER_OFFSET_Y ) ) {
				centerOffset.y = statCollection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_Y );
			}

			bool xOK = string.IsNullOrEmpty ( KEY_RANGE_X ) || Mathf.Abs ( target.position.x - ( transform.position.x + centerOffset.x ) ) <
				statCollection.GetStatValue ( KEY_RANGE_X );
			bool yOK = string.IsNullOrEmpty ( KEY_RANGE_Y ) || Mathf.Abs ( target.position.y - ( transform.position.y + centerOffset.y ) ) <
				statCollection.GetStatValue ( KEY_RANGE_Y );
			
			// Debug.Log ( $"CheckTargetInRange: xOK {target} => {transform} {target.position.x} - {( transform.position.x + centerOffset.x )} = {Mathf.Abs ( target.position.x - ( transform.position.x + centerOffset.x ) )} < {statCollection.GetStatValue ( KEY_RANGE_X )} {xOK} yOK {KEY_RANGE_Y} {yOK}" );

			return xOK && yOK;
		}
	}
}