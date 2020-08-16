using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using Characters.Movements;
using Nez.AI.GOAP;
using UnityEngine;


namespace Characters.AI {
	
	public class AILogicExecuter : MonoBehaviour {
		
		public AIScenarioAgentComponent agent;
		public BlackboardComponent      blackboard;
		public AbstractInput            cputInput;
		public Movement                 movement;
		public Hand                     hand;

		private BlackboardComponent worldBlackboard;
		private Routine currentTask;

		private void Start () {
			worldBlackboard = GameObject.Find ( "GameRule" ).GetComponentInChildren< BlackboardComponent > ();
			
			blackboard.Set ( "StealCD", TimeManager.EnemyTime );
			
			agent.agent.plan ();
		}
		
		private void OnGUI () {
			var target = worldBlackboard.Get< Transform > ( GameRule.VAR_Player );
			GUI.Label ( new Rect ( 0, 20, 400, 100 ), $"player distance：{Mathf.Abs ( target.transform.position.x - transform.position.x )}, {Mathf.Abs ( target.transform.position.y - transform.position.y )}" );
			target = worldBlackboard.Get< Transform > ( GameRule.VAR_ExpressPackageGenerator );
			GUI.Label ( new Rect ( 0, 40, 400, 100 ), $"Generator distance：{Mathf.Abs ( target.transform.position.x - transform.position.x )}, {Mathf.Abs ( target.transform.position.y - transform.position.y )}" );
			target = worldBlackboard.Get< Transform > ( GameRule.VAR_FillPiter );
			GUI.Label ( new Rect ( 0, 60, 400, 100 ), $"Piter distance：{Mathf.Abs ( target.transform.position.x - transform.position.x )}, {Mathf.Abs ( target.transform.position.y - transform.position.y )}" );
			target = worldBlackboard.Get< Transform > ( GameRule.VAR_EscapePoint );
			GUI.Label ( new Rect ( 0, 80, 400, 100 ), $"EscapePoint distance：{Mathf.Abs ( target.transform.position.x - transform.position.x )}, {Mathf.Abs ( target.transform.position.y - transform.position.y )}" );
		}

		private void Update () {
			if ( currentTask.Exists () ) {
				return;
			}

			if ( agent.enabled ) {
				
				agent.agent.plan ();
				if ( agent.agent.hasActionPlan () ) {
					currentTask = DoAction ( agent.agent.actions.Peek ().name );
				}
				else {
					agent.agent.plan ();
				}
			}
		}

		private Routine DoAction ( string action ) {
			switch ( action ) {
				case "GetPackageFromGenerator":
					return Routine.Start ( this,
						Sequence.Create (
							CMoveTo ( worldBlackboard.Get< Transform > ( GameRule.VAR_ExpressPackageGenerator ) ),
							CPressInteract (),
							CPickupAvailableExpressPackage ()
						)
					);
				case "MoveToAvailableExpressPackage":
					return Routine.Start ( this, CMoveTo ( findAvailableExpressPackage ()?.transform ) );
				case "MoveToGeneratorPoint":
					return Routine.Start ( this, CMoveTo ( worldBlackboard.Get< Transform > ( GameRule.VAR_ExpressPackageGenerator ) ) );
				case "MoveToPlayer":
					return Routine.Start ( this, CMoveTo ( worldBlackboard.Get< Transform > ( GameRule.VAR_Player ) ) );
				case "PickupPackage":
				case "DropPackage":
				case "LaunchGenerator":
					return Routine.Start ( this, CPressInteract () );
				
				case "StealPackage":
					return Routine.Start ( this, CStealPackage () );
				
				case "Jump":
					return Routine.Start ( this, CJump () );
				
				case "Escape":
					return Routine.Start ( CMoveTo ( worldBlackboard.Get< Transform > ( GameRule.VAR_EscapePoint ) ) )
					              .OnComplete ( () => Object.Destroy ( gameObject ) );
			}
			
			Debug.LogError ( $"DoAction: {action} not defined!" );
			return default;
		}

		private bool handHasExpressPackage () {
			return hand.equipped != null;
		}
		
		private Hand getPlayerHand () {
			return worldBlackboard.Get< Hand > ( GameRule.VAR_PlayerHand );
		}

		private GameObject findAvailableExpressPackage () {
			var packages = worldBlackboard.Get< List<GameObject> > ( GameRule.VAR_AvailableExpressPackages );
			var playerHand = getPlayerHand ();
			foreach ( var package in packages ) {
				// if ( package != playerHand.equipped ) {
				// 	return package;
				// }
				return package;
			}
			return null;
		}

		IEnumerator CMoveTo ( Transform target, float okDistance = 0.05f ) {
			if ( target == null ) {
				yield return Routine.Command.Stop;
			}
			
			var distance = target.position.x - transform.position.x;

			while ( Mathf.Abs ( distance ) > okDistance ) {
				if ( distance < 0 ) {
					cputInput.SetAxisValue ( GlobalSymbol.INPUTACTION_MoveHorizontal, -1f );
				}
				else {
					cputInput.SetAxisValue ( GlobalSymbol.INPUTACTION_MoveHorizontal, 1f );
				}

				if ( movement.controller.collisionState.left || movement.controller.collisionState.right ) {
					Debug.Log ( $"CMoveTo遇到障碍 {movement.controller.collisionState.left} {movement.controller.collisionState.right}" );
					cputInput.SetAxisValue ( GlobalSymbol.INPUTACTION_MoveHorizontal, 0f );
					yield return Routine.Command.Stop;
				}

				yield return null;
				
				if ( target == null ) {
					yield return Routine.Command.Stop;
					yield break;
				}
				
				distance = target.position.x - transform.position.x;
			}
			
			cputInput.SetAxisValue ( GlobalSymbol.INPUTACTION_MoveHorizontal, 0f );
		}
		
		IEnumerator CPressInteract () {
			cputInput.SetButtonState ( GlobalSymbol.INPUTACTION_Collect, AbstractInput.ButtonState.Pressed );
			yield return Routine.WaitForLateUpdate ();
			cputInput.SetButtonState ( GlobalSymbol.INPUTACTION_Collect, AbstractInput.ButtonState.Released );
		}
		
		IEnumerator CJump () {
			cputInput.SetButtonState ( GlobalSymbol.INPUTACTION_Jump, AbstractInput.ButtonState.Pressed );
			yield return null;
			cputInput.SetButtonState ( GlobalSymbol.INPUTACTION_Jump, AbstractInput.ButtonState.Released );
		}
		
		IEnumerator CPickupAvailableExpressPackage () {
			var package = findAvailableExpressPackage ();
			if ( package == null ) {
				yield return Routine.Command.Stop;
			}

			yield return Routine.Start ( this, CMoveTo ( package.transform ) );
			yield return Routine.Start ( this, CPressInteract () );
		}
		
		IEnumerator CStealPackage () {
			getPlayerHand ().Unequip ();
			blackboard.Set ( "StealCD", TimeManager.EnemyTime );
			yield return Routine.Command.Stop;
		}
	}
}