using Characters.Movements;


namespace Nez.AI.GOAP {
	
	public class CheckCharacterController2DState : AICondition {
		
		public bool above;
		public bool below;

		private CharacterController2D controller;

		public override void Awake () {
			controller = gameObject.GetComponentInChildren< CharacterController2D > ();
		}

		public override bool OnCheck () {
			if ( controller == null ) {
				return false;
			}

			return controller.collisionState.above == above &&
			       controller.collisionState.below == below;
		}
	}
}