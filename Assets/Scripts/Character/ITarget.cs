using UnityEngine;


namespace Characters {
	
	public interface ITarget {
		
		Collider2D collider { get; }

		Transform transform { get; }
		
	}
}