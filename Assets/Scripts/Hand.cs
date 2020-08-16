using System;
using UnityEngine;


namespace Characters {
	
	public class Hand : MonoBehaviour {
		
		[SerializeField] [GetComponent] private GameObject _owner;
		[SerializeField] [GetComponent] private PlayerInput _input;

		private Gear _gear;
		public Gear equipped => _gear;
		
		public event Action<Gear> onEquipped;
		public event Action<Gear> onUnequipped;

		public void Use () {
			if ( _gear != null ) {
				_gear.Use ();
			}
		}

		public bool Equip ( Gear gear ) {
			if ( _gear != null ) {
				return false;
			}

			_gear = gear;
			_gear.transform.SetParent ( transform );
			_gear.transform.localPosition = _gear.localOffsetInHand;
			_gear.transform.localScale = Vector3.one;
			onEquipped?.Invoke ( gear );
			return true;
		}

		public bool Unequip () {
			if ( _gear == null ) {
				return false;
			}

			_gear.state = Gear.State.Dropped;
			onUnequipped?.Invoke ( _gear );			
			_gear = null;
			return true;
		}
	}
}