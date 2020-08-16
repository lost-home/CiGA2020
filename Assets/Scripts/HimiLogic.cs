using System;
using Nez.AI.FSM;
using UnityEngine;


public class HimiLogic : SimpleStateMachine<HimiLogic.State> {
	
	public enum State {
		Idle,
		Action
	}
	
	public Animator animator;
	public FillPit  fillPit;
	public int      expandCount    = 5;
	public float     expandInterval = 10f;
	
	private float timer;
	private int currentExpandCount;

	private void Awake () {
		initialState = State.Idle;
	}

	private void Idle_Enter () {
		animator.Play ( "idle" );
		timer = 0f;
	}
	
	private void Idle_Tick () {
		timer += Time.deltaTime;
		if ( timer >= expandInterval ) {
			currentState = State.Action;
		}
	}
	
	private void Action_Enter () {
		animator.Play ( "action" );
		timer = 0f;
	}
	
	private void Action_Tick () {
		timer += Time.deltaTime;
		if ( timer >= 2f ) {
			currentState = State.Idle;
			
			if ( currentExpandCount <= expandCount ) {
				fillPit.ExpandPit ();
				currentExpandCount++;
			}
			else {
				// GameOver
			}
		}
	}
}