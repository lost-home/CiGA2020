using UnityEngine;
using Object = UnityEngine.Object;


public class DestroyOnTouch : MonoBehaviour {
	private void OnTriggerEnter2D ( Collider2D collision ) {
		Object.Destroy ( collision.gameObject );
	}

	private void OnCollisionEnter ( Collision collision ) {
		Object.Destroy ( collision.gameObject );
	}
}