using UnityEngine;


public class BlackboardComponent : MonoBehaviour {
	
	private Blackboard _blackboard = new Blackboard ();
	public  Blackboard blackboard => _blackboard;

	public T Get< T > ( string name ) {
		return _blackboard.Get< T > ( name );
	}

	public virtual void Set< T > ( string name, T value ) {
		_blackboard.Set< T > ( name, value );
	}
}