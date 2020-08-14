using UnityEngine;
using RSG;


public class Test : MonoBehaviour {
	
	public const string STATE_Idle = "Idle";
	public const string STATE_Run = "Run";

	// public tk2dSpriteAnimator animator;
	public Animator animator;
	public float workTime = 2f;
	public float restTime = 1f;
	
	private AbstractState stateMachine;
	private float timer;
	
	private void Awake () {
		stateMachine = new StateMachineBuilder ()
			.State ( STATE_Idle )
				.Enter ( state => {
					 animator.Play ( "idle" );
					 timer = 0f;
				} )
				.Condition ( () => timer >= workTime, state => stateMachine.ChangeState ( STATE_Run ) )
				.Update ( ( state, dt ) => {
					timer += CupheadTime.GlobalDelta;
				} )
				.End ()
			.State ( STATE_Run )
				.Enter ( state => {
					animator.Play ( MathUtils.RandomBool () ? "rest" : "rest_2" );
					timer = 0f;
				} )
				.Condition ( () => timer >= restTime, state => stateMachine.ChangeState ( STATE_Idle ) )
				.Update ( ( state, dt ) => {
					timer += CupheadTime.GlobalDelta;
				} )
				.End ()
			.Build () as RSG.AbstractState;

		stateMachine.PushState ( STATE_Idle );
	}

	private void Update () {
		stateMachine.Update ( CupheadTime.GlobalDelta );
	}
}