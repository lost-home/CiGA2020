using UnityEngine;


public abstract class InteractiveObject : MonoBehaviour {
	
	protected static readonly int _activateHash = Animator.StringToHash ( "Activate" );
	protected static readonly int _deactivateHash = Animator.StringToHash ( "Deactivate" );

	public bool autoInteract;

	// [SerializeField] protected SoundInfo _activateSound;
	// [SerializeField] protected SoundInfo _deactivateSound;
	// [SerializeField] protected SoundInfo _interactSound;

	[SerializeField] protected GameObject _uiObject;
	[SerializeField] protected GameObject[] _uiObjects;
	[SerializeField] protected bool _activated = true;

	protected GameObject _character;

	public bool popupVisible => _character != null;

	public bool activated {
		get { return _activated; }
		private set { _activated = value; }
	}

	protected virtual void Awake () {
		ClosePopup ();
	}

	private void OnDisable () {
		_activated = false;
	}

	public void Activate () {
		_activated = true;
		OnActivate ();
	}

	public void Deactivate () {
		ClosePopup ();
		_activated = false;
		OnDeactivate ();
	}

	public virtual void OnActivate () {
		// PersistentSingleton< SoundManager >.Instance.PlaySound ( _activateSound, base.transform.position );
	}

	public virtual void OnDeactivate () {
		// PersistentSingleton< SoundManager >.Instance.PlaySound ( _deactivateSound, base.transform.position );
	}

	public abstract void InteractWith ( GameObject character );

	public virtual void OpenPopupBy ( GameObject character ) {
		_character = character;
		GameObject[] uiObjects = _uiObjects;
		foreach ( GameObject gameObject in uiObjects ) {
			if ( !( gameObject == null ) && !gameObject.activeSelf ) {
				gameObject.SetActive ( value: true );
			}
		}

		if ( !( _uiObject == null ) && !_uiObject.activeSelf && !autoInteract ) {
			_uiObject.SetActive ( value: true );
		}
	}

	public virtual void ClosePopup () {
		_character = null;
		GameObject[] uiObjects = _uiObjects;
		foreach ( GameObject gameObject in uiObjects ) {
			if ( gameObject != null ) {
				gameObject.SetActive ( value: false );
			}
		}

		if ( _uiObject != null ) {
			_uiObject.SetActive ( value: false );
		}
	}
}