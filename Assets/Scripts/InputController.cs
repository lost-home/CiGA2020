using Rewired;
using UnityEngine;

public class InputController : MonoBehaviour {
	
	public float getHorizontal () {
		return ReInput.players.GetPlayer ( 0 ).GetAxisRaw ( "Horizontal" );
	}
	
	public bool getAction ( string actionName ) {
		return ReInput.players.GetPlayer ( 0 ).GetButton ( actionName );
	}
	
	public bool getActionDown ( string actionName ) {
		return ReInput.players.GetPlayer ( 0 ).GetButtonDown ( actionName );
	}
	
	public bool getActionUp ( string actionName ) {
		return ReInput.players.GetPlayer ( 0 ).GetButtonUp ( actionName );
	}
	
}