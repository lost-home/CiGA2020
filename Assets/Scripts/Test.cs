using UnityEngine;
using RSG;


public class Test : MonoBehaviour {
	
	public const string STATE_Work = "Work";
	public const string STATE_Rest = "Rest";

	// public tk2dSpriteAnimator animator;
	public Animator animator;
	public float workTime = 2f;
	public float restTime = 1f;
	
	private AbstractState stateMachine;
	private float timer;
	
	private void Awake () {
		stateMachine = new StateMachineBuilder ()
			.State ( STATE_Work )
				.Enter ( state => {
					 animator.Play ( "work" );
					 timer = 0f;
				} )
				.Condition ( () => timer >= workTime, state => stateMachine.ChangeState ( STATE_Rest ) )
				.Update ( ( state, dt ) => {
					timer += CupheadTime.GlobalDelta;
				} )
				.End ()
			.State ( STATE_Rest )
				.Enter ( state => {
					animator.Play ( MathUtils.RandomBool () ? "rest" : "rest_2" );
					timer = 0f;
				} )
				.Condition ( () => timer >= restTime, state => stateMachine.ChangeState ( STATE_Work ) )
				.Update ( ( state, dt ) => {
					timer += CupheadTime.GlobalDelta;
				} )
				.End ()
			.Build () as RSG.AbstractState;

		stateMachine.PushState ( STATE_Work );
	}

	private void Update () {
		stateMachine.Update ( CupheadTime.GlobalDelta );
	}
}