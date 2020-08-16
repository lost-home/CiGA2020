using UnityEngine;


namespace Characters {
	
	public class Target : MonoBehaviour, ITarget {
		
		[SerializeField] [GetComponent] private Collider2D _collider;

		public Collider2D collider => _collider;
	}
}