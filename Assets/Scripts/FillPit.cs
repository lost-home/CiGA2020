using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;


public class FillPit : MonoBehaviour {

	
	public int fillCountPerColumn = 4;
	public Vector2 startFillPosition = new Vector2 ( 0.09f, -0.82f );
	public Vector2 expressPackageSize = new Vector2 ( 0.18f, 0.18f );
	public GameObject mapL;
	public GameObject mapR;
	public GameObject player;

	private Dictionary< int, ExpressPackage[] > depot;
	private float timer;

	private void Awake () {
		depot = new Dictionary< int , ExpressPackage[] >();
		depot.Add ( 0, new ExpressPackage [ fillCountPerColumn ] );
	}
	
	private void OnTriggerStay2D ( Collider2D collision ) {
		
		ExpressPackage pack = collision.GetComponent< ExpressPackage > ();
		if ( pack != null && pack.state == Gear.State.Dropped ) {
			FillWithExpressPackage ( pack );
		}
	}

	public void ExpandPit () {
		
		// if ( depot.Count >= expandCount * 2 + 1 ) {
		// 	return;
		// }
		
		if ( depot.Count != 0 ) {
			int l = depot.OrderBy ( i => i.Key ).First ().Key - 1;
			int r = depot.OrderBy ( i => i.Key ).Last ().Key + 1;
			depot.Add ( l, new ExpressPackage [ fillCountPerColumn ] );
			depot.Add ( r, new ExpressPackage [ fillCountPerColumn ] );
			
			mapL.transform.Translate ( new Vector3 ( -expressPackageSize.x, 0 ) );
			mapR.transform.Translate ( new Vector3 ( +expressPackageSize.x, 0 ) );
			player.transform.Translate ( new Vector3 ( -expressPackageSize.x, 0 ) );
			
			CameraShakeManager.Instance.AddShake ( "camera_shake 2 0 0.2,0.2 10 0.5" );
		}
		else {
			depot.Add ( 0, new ExpressPackage [ fillCountPerColumn ] );
		}
		
		SortDepot ();
	}

	public void SortDepot () {
		depot.OrderBy ( i => i.Key );
	}

	public void FillWithExpressPackage ( ExpressPackage pack ) {
		pack.dropMovement.enabled = false;
		
		// 自下而上寻找空点
		int row = 0;
		int column = 0;
		bool found = false;
		while ( row < fillCountPerColumn ) {
			foreach ( var kv in depot ) {
				if ( kv.Value[ row ] == null ) {
					found = true;
					column = kv.Key;
					kv.Value[ row ] = pack;
					goto Result;
				}
			}
			row++;
		}

Result:
		if ( found ) {
			pack.transform.position = new Vector3 ( startFillPosition.x + column * expressPackageSize.x, startFillPosition.y + row * expressPackageSize.y );
			
			EventKit.Broadcast ( GlobalSymbol.EVT_ExpressPackagePlaced, pack.gameObject );
			
			CameraShakeManager.Instance.AddShake ( "camera_shake 2 0 0.1,0.1 10 0.25" );
		}
		else {
			// 满了
		}
	}

	public bool IsFull () {
		int  row    = 0;
		bool found  = false;
		while ( row < fillCountPerColumn ) {
			foreach ( var kv in depot ) {
				if ( kv.Value[ row ] == null ) {
					return true;
				}
			}
			row++;
		}

		return false;
	}
}