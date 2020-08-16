using System.Collections.Generic;
using Characters;
using UnityEngine;


namespace Nez.AI.GOAP {
	
	public class WorldHasAvailableExpressPackage : AICondition {

		public bool includePlayerHand = false;
		
		private BlackboardComponent blackboard;

		public override void Awake () {
			blackboard = GameObject.Find ( "GameRule" ).GetComponentInChildren<BlackboardComponent>();			
		}

		public override bool OnCheck () {
			
			var packages   = blackboard.Get< List<GameObject> > ( GameRule.VAR_AvailableExpressPackages );
			var playerHand = blackboard.Get< Hand > ( GameRule.VAR_PlayerHand );
			foreach ( var package in packages ) {
				if ( !includePlayerHand && package != playerHand.equipped ) {
					return true;
				}
				if ( includePlayerHand && package == playerHand.equipped ) {
					return true;
				}
			}
			
			return false;
		}
	}
}