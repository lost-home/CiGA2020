using UnityEngine;

namespace Nez.AI.GOAP {
	
	public class CheckGroundDistance : AICondition {
		
		public CompareMethod checkType = CompareMethod.LessOrEqualTo;
		public float         value;
		
		public override bool OnCheck () {
			
			var hit = Physics2D.Raycast (gameObject.transform.position, Vector2.down, float.MaxValue, LayerMask.GetMask("terrain"));

			if ( !hit ) {
				return false;
			}

			var distance = Mathf.Abs ( hit.point.y - gameObject.transform.position.y );
			
			return OperationTools.Compare ( distance, value, checkType, 0 );
		}
	}
}
