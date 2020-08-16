using System.Collections.Generic;
using Characters;
using RPGStatSystem;
using UnityEngine;


namespace Nez.AI.GOAP {
	
	public class AnyAvailableExpressPackageInRange : AICondition {

		public bool includePlayerHand = false;
		public string KEY_RANGE_X;
		public string KEY_RANGE_Y;
		
		private RPGStatCollection statCollection;
		private BlackboardComponent blackboard;

		public override void Awake () {
			statCollection = gameObject.GetComponentInChildren<RPGStatCollection>();
			blackboard = GameObject.Find ( "GameRule" ).GetComponentInChildren<BlackboardComponent>();			
		}

		public override bool OnCheck () {
			
			var packages   = blackboard.Get< List<GameObject> > ( GameRule.VAR_AvailableExpressPackages );
			var playerHand = blackboard.Get< Hand > ( GameRule.VAR_PlayerHand );
			foreach ( var package in packages ) {
				if ( !includePlayerHand && package != playerHand.equipped ) {
					if ( checkDistance ( package.transform ) ) {
						return true;
					}
				}
				if ( includePlayerHand && package == playerHand.equipped ) {
					if ( checkDistance ( package.transform ) ) {
						return true;
					}
				}
			}
			
			return false;
		}

		private bool checkDistance ( Transform a ) {
			Vector3 centerOffset = Vector3.zero;
			if ( statCollection.ContainStat ( GlobalSymbol.CENTER_OFFSET_X ) ) {
				centerOffset.x = statCollection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_X );
			}

			if ( statCollection.ContainStat ( GlobalSymbol.CENTER_OFFSET_Y ) ) {
				centerOffset.y = statCollection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_Y );
			}
			bool xOK = string.IsNullOrEmpty ( KEY_RANGE_X ) || Mathf.Abs ( a.position.x - ( transform.position.x + centerOffset.x ) ) <
				statCollection.GetStatValue ( KEY_RANGE_X );
			bool yOK = string.IsNullOrEmpty ( KEY_RANGE_Y ) || Mathf.Abs ( a.position.y - ( transform.position.y + centerOffset.y ) ) <
				statCollection.GetStatValue ( KEY_RANGE_Y );

			return xOK && yOK;
		}
	}
}