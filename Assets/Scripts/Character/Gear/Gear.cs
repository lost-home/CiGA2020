using System;
using UnityEngine;


namespace Characters {
	
	public abstract class Gear : MonoBehaviour {
		
		public enum Type {
			Weapon,
			Item
		}

		public enum State {
			Dropped,
			Equipped
		}

		public bool canBeDropped = true;
		
		[SerializeField] private DroppedGear _dropped;
		[SerializeField] private GameObject _equipped;

		public Vector2 localOffsetInHand = new Vector2 ( 0, 0.16f );	

		private State _state;

		public abstract Type type { get; }
		
		public DroppedGear dropped => _dropped;
		public GameObject equipped => _equipped;
		
		public Sprite icon => dropped.spriteRenderer.sprite;

		public virtual Sprite thumbnail => icon;

		public State state {
			get { return _state; }
			set {
				if ( _state != value ) {
					_state = value;
					switch ( _state ) {
						case State.Dropped:
							OnDropped ();
							break;
						case State.Equipped:
							OnEquipped ();
							break;
					}
				}
			}
		}

		public GameObject owner { get; protected set; }

		public event Action onDropped;
		public event Action onEquipped;

		protected virtual void Awake () {
			if ( _dropped != null ) {
				_dropped.onLoot += OnLoot;
			}

			OnDropped ();
		}

		public virtual void Initialize () {
			_dropped?.Initialize ( this );
		}

		public virtual void Use () {}

		protected abstract void OnLoot ( GameObject character );

		protected virtual void OnDropped () {
			transform.parent     = null;
			transform.localScale = Vector3.one;
			if ( _equipped != null ) {
				_equipped.SetActive ( value: false );
			}

			if ( _dropped != null ) {
				_dropped.gameObject.SetActive ( value: true );
			}

			onDropped?.Invoke ();
		}

		protected virtual void OnEquipped () {
			if ( _dropped != null ) {
				_dropped.gameObject.SetActive ( value: false );
			}

			if ( _equipped != null ) {
				_equipped.SetActive ( value: true );
			}

			onEquipped?.Invoke ();
		}
	}
}