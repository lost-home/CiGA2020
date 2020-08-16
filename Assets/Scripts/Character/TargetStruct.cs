using UnityEngine;


namespace Characters {
	
	public readonly struct TargetStruct : ITarget {
		
		public readonly Collider2D collider;
		public readonly Transform transform;
		public readonly GameObject character;
		
		Collider2D ITarget.collider => collider;
		
		Transform ITarget.transform => transform;

		public TargetStruct ( GameObject character ) {
			this.character = character;
			collider       = character.GetComponentInChildren<Collider2D>();
			transform      = character.transform;
		}
	}
}