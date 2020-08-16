using UnityEngine;


public class ExpressPackageGen : InteractiveObject {

	public GameObject prefab;
	public Vector2 genOffset;
	
	
	public override void InteractWith ( GameObject character ) {
		ClosePopup ();
		
		var pack = Object.Instantiate ( prefab, transform.position + ( Vector3 )genOffset, Quaternion.identity );
		
		EventKit.Broadcast ( GlobalSymbol.EVT_ExpressPackageGenerated, pack );
	}
}