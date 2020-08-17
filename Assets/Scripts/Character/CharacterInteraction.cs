using System.Collections.Generic;
using UnityEngine;


namespace Characters {
	
	public class CharacterInteraction : MonoBehaviour {
		
		private const float _interactInterval = 0.2f;

		[SerializeField] [GetComponent] private GameObject _character;
		[SerializeField] [GetComponent] private PlayerInput _input;

		private readonly List< InteractiveObject > _interactiveObjects = new List< InteractiveObject > ();
		private float _lastInteractedTime;
		private InteractiveObject _objectToInteract;

		private void OnTriggerEnter2D ( Collider2D collision ) {
			InteractiveObject component = collision.GetComponent< InteractiveObject > ();
			if ( component != null ) {
				_interactiveObjects.Add ( component );
			}
			else {
				// collision.GetComponent< IPickupable > ()?.PickedUpBy ( _character );
			}
		}

		private void OnTriggerExit2D ( Collider2D collision ) {
			InteractiveObject component = collision.GetComponent< InteractiveObject > ();
			if ( component != null ) {
				_interactiveObjects.Remove ( component );
			}
		}

		private void Update () {
			for ( int num = _interactiveObjects.Count - 1; num >= 0; num-- ) {
				InteractiveObject interactiveObject = _interactiveObjects[ num ];
				if ( interactiveObject == null || !interactiveObject.isActiveAndEnabled ) {
					_interactiveObjects.RemoveAt ( num );
				}
			}

			if ( _interactiveObjects.Count == 0 || PlayerInput.blocked.value ) {
				if ( _objectToInteract != null ) {
					_objectToInteract.ClosePopup ();
				}

				_objectToInteract = null;
				return;
			}

			_interactiveObjects.Sort ( ( InteractiveObject i1, InteractiveObject i2 ) =>
				Mathf.Abs ( base.transform.position.x - i1.transform.position.x )
				     .CompareTo ( Mathf.Abs ( base.transform.position.x - i2.transform.position.x ) ) );
			
			for ( int j = 0; j < _interactiveObjects.Count; j++ ) {
				InteractiveObject interactiveObject2 = _interactiveObjects[ j ];
				if ( !interactiveObject2.activated ) {
					continue;
				}

				if ( interactiveObject2.autoInteract ) {
					interactiveObject2.InteractWith ( _character );
				}

				if ( _objectToInteract != interactiveObject2 ) {
					if ( _objectToInteract != null ) {
						_objectToInteract.ClosePopup ();
					}

					interactiveObject2.OpenPopupBy ( _character );
					_objectToInteract = interactiveObject2;
				}

				break;
			}
		}

		public void Interact () {
			if ( !( _objectToInteract == null ) && _objectToInteract.activated &&
			     !( _lastInteractedTime + 0.2f > Time.realtimeSinceStartup ) ) {
				_objectToInteract.InteractWith ( _character );
				_lastInteractedTime = Time.realtimeSinceStartup;
			}
		}
	}
}