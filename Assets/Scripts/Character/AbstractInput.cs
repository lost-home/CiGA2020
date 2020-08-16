using UnityEngine;


namespace Characters {
	
	public abstract class AbstractInput : MonoBehaviour {
		
		public enum ButtonState {
			/// <summary>
			/// The button is released.
			/// </summary>
			Released,
			/// <summary>
			/// The button is pressed.
			/// </summary>
			Pressed
		}
		
		public virtual float GetHorizontal () {
			return 0f;
		}

		public virtual float GetVertical () {
			return 0f;
		}

		public virtual bool GetButton ( string actionName ) {
			return false;
		}

		public virtual bool GetButtonDown ( string actionName ) {
			return false;
		}

		public virtual bool GetButtonUp ( string actionName ) {
			return false;
		}
		
		public virtual void SetButtonState ( string       actionName, ButtonState buttonState,
		                                     ButtonState? previousButtonState = null ) {
		}

		public virtual void SetAxisValue ( string actionName, float axisValue, float? previousAxisValue = null ) {

		}
	}
}