using System;
using Characters.Movements;
using UnityEngine;


namespace Characters {
	
	public class DroppedGear : InteractiveObject {
		
		protected Vector2 _popupUIOffset = new Vector2 ( 2.5f, 1.5f );

		[SerializeField] private Movement _dropMovement;
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		private Gear _gear;
		private Vector3 _initialPosition;

		public SpriteRenderer spriteRenderer => _spriteRenderer;
		public Movement dropMovement => _dropMovement;

		public event Action< GameObject > onLoot;

		protected override void Awake () {
			base.Awake ();
			if ( _dropMovement == null ) {
				Activate ();
			}
			else {
				// _dropMovement.onGround += Activate;
			}

			_initialPosition = transform.localPosition;
		}

		public void Initialize ( Gear gear ) {
			_gear = gear;
		}

		public override void OpenPopupBy ( GameObject character ) {
			base.OpenPopupBy ( character );
			Vector3 position  = base.transform.position;
			Vector3 position2 = character.transform.position;
			position.x =  position2.x + ( ( position.x > position2.x ) ? _popupUIOffset.x : ( 0f - _popupUIOffset.x ) );
			position.y += _popupUIOffset.y;
			// Scene< GameBase >.instance.uiManager.gearPopupCanvas.gearPopup.Set ( _gear );
			// Scene< GameBase >.instance.uiManager.gearPopupCanvas.Open ( position );
		}

		public override void ClosePopup () {
			base.ClosePopup ();
			// Scene< GameBase >.instance.uiManager.gearPopupCanvas.Close ();
		}

		public override void InteractWith ( GameObject character ) {
			ClosePopup ();
			// PersistentSingleton< SoundManager >.Instance.PlaySound ( _interactSound, base.transform.position );
			transform.localPosition = _initialPosition;
			onLoot?.Invoke ( character );
		}
	}
}