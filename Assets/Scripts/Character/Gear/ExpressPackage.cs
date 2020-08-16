using UnityEngine;


namespace Characters {
	
	public class ExpressPackage : Item {

		private Hand _hand;

		private void OnDestroy () {
			EventKit.Broadcast ( GlobalSymbol.EVT_ExpressPackageDestroyed, gameObject );
		}
		
		protected override void OnLoot ( GameObject character ) {
			Hand hand = character.GetComponentInChildren< Hand > ();
			if ( !hand.Equip ( this ) ) {
				return;
			}

			owner = character;
			_hand = hand;
			state = State.Equipped;
		}

		public override void Use () {
			_hand.Unequip ();
		}

		protected override void OnEquipped () {
			base.OnEquipped ();
			// _abilityAttacher.Initialize ( base.owner );
			// _abilityAttacher.StartAttach ();
		}

		protected override void OnDropped () {
			base.OnDropped ();
			// _abilityAttacher.StopAttach ();
		}
	}
}