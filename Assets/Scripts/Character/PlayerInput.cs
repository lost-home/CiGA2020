using Characters;
using Rewired;
using RPGStatSystem;

public class PlayerInput : AbstractInput {
	
	public static readonly TrueOnlyLogicalSumList blocked = new TrueOnlyLogicalSumList();

	public int playerIndex = 0;
	public RPGStatCollection statCollection;
	public CharacterInteraction characterInteraction;
	public Hand hand;
	
	private void Update () {
		
		if ( blocked.value ) {
			statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL, 0f );
			statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_VERTICAL, 0f );
			return;
		}
		
		statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL, ReInput.players.GetPlayer ( playerIndex ).GetAxisRaw ( GlobalSymbol.INPUTACTION_MoveHorizontal ) );
		statCollection.SetStatValue ( GlobalSymbol.INPUTVAR_MOVE_VERTICAL, ReInput.players.GetPlayer ( playerIndex ).GetAxisRaw ( GlobalSymbol.INPUTACTION_MoveVertical ) );
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

	public override float GetHorizontal () {
		return ReInput.players.GetPlayer ( playerIndex ).GetAxisRaw ( GlobalSymbol.INPUTACTION_MoveHorizontal );
	}
	
	public override bool GetButton ( string actionName ) {
		return ReInput.players.GetPlayer ( playerIndex ).GetButton ( actionName );
	}
	
	public override bool GetButtonDown ( string actionName ) {
		return ReInput.players.GetPlayer ( playerIndex ).GetButtonDown ( actionName );
	}
	
	public override bool GetButtonUp ( string actionName ) {
		return ReInput.players.GetPlayer ( playerIndex ).GetButtonUp ( actionName );
	}
	
}