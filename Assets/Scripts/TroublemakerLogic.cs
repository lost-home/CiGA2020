using Characters;
using Characters.Movements;
using RPGStatSystem;
using UnityEngine;
using RSG;


public class TroublemakerLogic : MonoBehaviour {
	
	public const string STATE_Idle = "Idle";
	public const string STATE_Run = "Run";
	public const string STATE_Jump = "Jump";
	public const string STATE_Throw = "Throw";
	public const string STATE_Fall  = "Fall";

	// public tk2dSpriteAnimator animator;
	public RPGStatCollection statCollection;
	public BlackboardComponent blackboard;
	public Animator animator;
	public Movement movement;
	public AbstractInput playerInput;
	public Hand hand;
	public FaceDirectionComponent faceDirectionComponent;
	
	
	private AbstractState stateMachine;
	private float timer;
	
	private void Awake () {
		stateMachine = new StateMachineBuilder ()
			.State ( STATE_Idle )
				.Enter ( state => {
					 animator.Play ( hand.equipped ? getEquippedAnimation ( stateMachine.CurrentStateStr ) : "idle" );
				} )
				.Condition ( () => statCollection.GetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL ) != 0, state => stateMachine.ChangeState ( STATE_Run ) )
				.Condition ( () => playerInput.GetButtonDown ( GlobalSymbol.INPUTACTION_Jump ), state => stateMachine.ChangeState ( STATE_Jump ) )
				.Condition ( () => !movement.isGrounded, state => stateMachine.ChangeState ( STATE_Fall ) )
				.Update ( ( state, dt ) => {
				
				} )
				.End ()
			.State ( STATE_Run )
				.Enter ( state => {
					animator.Play ( hand.equipped ? getEquippedAnimation ( stateMachine.CurrentStateStr ) : "run" );
				} )
				.Condition ( () => statCollection.GetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL ) == 0, state => stateMachine.ChangeState ( STATE_Idle ) )
				.Condition ( () => playerInput.GetButtonDown ( GlobalSymbol.INPUTACTION_Jump ), state => stateMachine.ChangeState ( STATE_Jump ) )
				.Update ( ( state, dt ) => {
					movement.Move ( new Vector2 ( statCollection.GetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL ), 0 ) );
					faceDirectionComponent.SetFaceDirection ( statCollection.GetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL ) );
				} )
				.End ()
			.State ( STATE_Jump )
				.Enter ( state => {
					animator.Play ( hand.equipped ? getEquippedAnimation ( stateMachine.CurrentStateStr ) : "jump" );
					movement.Jump ( statCollection.GetStatValue ( GlobalSymbol.JUMP_HEIGHT ) );
				} )
				.Exit ( state => {
					
				} )
				.Condition ( () => movement.isGrounded, state => stateMachine.ChangeState ( STATE_Idle ) )
				.Update ( ( state, dt ) => {
					movement.Move ( new Vector2 ( statCollection.GetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL ), 0 ) );
				} )
				.End ()
			.State ( STATE_Fall )
				.Enter ( state => {
					animator.Play ( hand.equipped ? getEquippedAnimation ( stateMachine.CurrentStateStr ) : "jump" );
				} )
				.Condition ( () => movement.isGrounded, state => stateMachine.ChangeState ( STATE_Idle ) )
				.Update ( ( state, dt ) => {
					movement.Move ( new Vector2 ( statCollection.GetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL ), 0 ) );
					faceDirectionComponent.SetFaceDirection ( statCollection.GetStatValue ( GlobalSymbol.INPUTVAR_MOVE_HORIZONTAL ) );
				} )
				.End ()
			.Build () as RSG.AbstractState;
		
		// movement.onMoved += onMovementMove;
		hand.onEquipped += onHandGearEquipped;
		hand.onUnequipped += onHandGearUnequipped;
		
		stateMachine.PushState ( STATE_Idle );
	}

	private void Update () {
		stateMachine.Update ( CupheadTime.GlobalDelta );
	}

	private void onHandGearEquipped ( Gear gear ) {
		animator.Play ( getEquippedAnimation ( stateMachine.CurrentStateStr ) );
	}
	
	private void onHandGearUnequipped ( Gear gear ) {
		switch ( stateMachine.CurrentStateStr ) {
			case STATE_Idle:
				animator.Play ( "idle" );
				break;
			case STATE_Run:
				animator.Play ( "run" );
				break;
			case STATE_Jump:
			case STATE_Fall:
				animator.Play ( "jump" );
				break;
		}
	}

	private string getEquippedAnimation ( string stateName ) {
		switch ( stateMachine.CurrentStateStr ) {
			case STATE_Idle:
				return "holdidle";
			case STATE_Run:
				return "holdrun";
			case STATE_Jump:
			case STATE_Fall:
				return "holdjump";
			default:
				return "null";
		}
	}

}