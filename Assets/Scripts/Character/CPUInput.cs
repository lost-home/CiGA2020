using System.Collections.Generic;
using Characters;
using RPGStatSystem;
using UnityEngine;

public class CPUInput : AbstractInput {
	
	public class ButtonStateIm {
		public AbstractInput.ButtonState currentButtonState;
		public AbstractInput.ButtonState previousButtonState;
	}
	
	public class AxisStateIm {
		public float currentAxisValue;
		public float previousAxisValue;
	}
	
	public static readonly TrueOnlyLogicalSumList blocked = new TrueOnlyLogicalSumList();

	public RPGStatCollection    statCollection;
	public CharacterInteraction characterInteraction;
	public Hand                 hand;

	private Dictionary< string, ButtonStateIm > buttonStates = new Dictionary< string , ButtonStateIm >();
	private Dictionary< string, AxisStateIm  > axisStates = new Dictionary< string , AxisStateIm >();

	private void Awake () {
		buttonStates.Add ( GlobalSymbol.INPUTACTION_Collect, new ButtonStateIm () );
		buttonStates.Add ( GlobalSymbol.INPUTACTION_Grab, new ButtonStateIm () );
		buttonStates.Add ( GlobalSymbol.INPUTACTION_Jump, new ButtonStateIm () );
		axisStates.Add ( GlobalSymbol.INPUTACTION_MoveHorizontal, new AxisStateIm () );
		axisStates.Add ( GlobalSymbol.INPUTACTION_MoveVertical, new AxisStateIm () );
	}

	private void Update () {
		
		if ( blocked.value ) {
			statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL, 0f );
			statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_VERTICAL, 0f );
			return;
		}
		
		statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL, GetHorizontal () );
		statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_VERTICAL, GetVertical () );
		if ( GetButtonDown ( GlobalSymbol.INPUTACTION_Collect ) || GetButtonDown ( GlobalSymbol.INPUTACTION_Grab ) ) {
			if ( hand.equipped ) {
				hand.Use ();
				return;
			}
		}
		if ( GetButtonDown ( GlobalSymbol.INPUTACTION_Collect ) || GetButtonDown ( GlobalSymbol.INPUTACTION_Grab ) ) {
			characterInteraction.Interact ();
			return;
		}
	}

	private void LateUpdate () {
		foreach ( var kv in buttonStates ) {
			kv.Value.previousButtonState = kv.Value.currentButtonState;
		}
		foreach ( var kv in axisStates ) {
			kv.Value.previousAxisValue = kv.Value.currentAxisValue;
		}
	}

	public override float GetHorizontal () {
		return axisStates[ GlobalSymbol.INPUTACTION_MoveHorizontal ].currentAxisValue;
	}
	
	public override float GetVertical () {
		return axisStates[ GlobalSymbol.INPUTACTION_MoveVertical ].currentAxisValue;
	}
	
	public override bool GetButton ( string actionName ) {
		return buttonStates[ actionName ].currentButtonState == ButtonState.Pressed;
	}
	
	public override bool GetButtonDown ( string actionName ) {
		return buttonStates[ actionName ].currentButtonState == ButtonState.Pressed && buttonStates[ actionName ].previousButtonState != ButtonState.Pressed;
	}
	
	public override bool GetButtonUp ( string actionName ) {
		return buttonStates[ actionName ].currentButtonState == ButtonState.Released && buttonStates[ actionName ].previousButtonState == ButtonState.Pressed;
	}
	
	#region Set
	public override void SetButtonState ( string actionName, ButtonState buttonState, ButtonState? previousButtonState = null ) {
		buttonStates[ actionName ].currentButtonState = buttonState;
		if ( previousButtonState.HasValue ) {
			buttonStates[ actionName ].previousButtonState = previousButtonState.Value;
		}
	}
	
	public override void SetAxisValue ( string actionName, float axisValue, float? previousAxisValue = null ) {
		axisStates[ actionName ].currentAxisValue = axisValue;
		if ( previousAxisValue.HasValue ) {
			axisStates[ actionName ].previousAxisValue = previousAxisValue.Value;
		}
	}
	#endregion
	
}