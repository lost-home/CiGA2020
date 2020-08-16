using Characters.Movements;
using UnityEngine;


namespace Characters {
	
	public class Item : Gear {
		
		public override Type type => Type.Item;
		
		[SerializeField]
		private Movement _dropMovement;
		
		public Movement dropMovement => _dropMovement;
		
		private void OnDestroy () {
			
		}

		protected override void OnLoot ( GameObject character ) {
			Hand hand = character.GetComponent< Hand > ();
			if ( !hand.Equip ( this ) ) {
				return;
			}

			owner = character;
			state = State.Equipped;
		}

		protected override void OnEquipped () {
			base.OnEquipped ();
			dropMovement.enabled = false;
		}

		protected override void OnDropped () {
			base.OnDropped ();
			dropped.Activate ();
			dropMovement.enabled = true;
		}
	}
}