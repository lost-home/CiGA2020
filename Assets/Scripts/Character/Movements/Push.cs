using UnityEngine;


namespace Characters.Movements {
	
	public sealed class Push {
		
		public delegate void OnSmashEndDelegate ( Push push, GameObject from, GameObject to, SmashEndType endType,
		                                          RaycastHit2D? raycastHit, Movement.CollisionDirection direction );

		public enum SmashEndType {
			Expire,
			Collide,
			Cancel
		}

		private readonly GameObject _owner;
		private GameObject _from;
		private (Vector2, Vector2) _force;
		private (Curve, Curve) _curve;
		private float _time;
		private (float, float) _amountBefore;

		public bool ignoreOtherForce { get; private set; }
		public bool expireOnGround { get; private set; }
		public bool smash { get; private set; }
		public bool expired { get; private set; } = true;
		
		public Vector2 direction { get; private set; }
		public Vector2 totalForce { get; private set; }
		public float duration { get; private set; }

		public event OnSmashEndDelegate onEnd;

		internal Push ( GameObject owner ) {
			_owner = owner;
		}

		private void Intialize () {
			_time         = 0f;
			_amountBefore = ( 0f, 0f );
			direction     = ( _force.Item1 + _force.Item2 ).normalized;
			totalForce    = _force.Item1 * _curve.Item1.valueMultiplier + _force.Item2 * _curve.Item2.valueMultiplier;
			expired       = false;
			duration      = Mathf.Max ( _curve.Item1.duration, _curve.Item2.duration );
		}

		public void ApplySmash ( GameObject from, PushInfo info, OnSmashEndDelegate onEnd = null ) {
			_from            = from;
			_force           = info.Evaluate ( from.transform, new TargetStruct ( _owner ) );
			_curve           = ( info.curve1, info.curve2 );
			ignoreOtherForce = info.ignoreOtherForce;
			expireOnGround   = info.expireOnGround;
			this.onEnd       = onEnd;
			smash            = true;
			Intialize ();
		}

		public bool ApplyKnockback ( GameObject from, Vector2 force, Curve curve, bool ignoreOtherForce = false,
		                             bool      expireOnGround = false ) {
			if ( smash && !expired ) {
				return false;
			}

			_from                 = from;
			_force                = ( force, Vector2.zero );
			_curve                = ( curve, Curve.empty );
			this.ignoreOtherForce = ignoreOtherForce;
			this.expireOnGround   = expireOnGround;
			smash                 = false;
			Intialize ();
			return true;
		}

		public bool ApplyKnockback ( Transform from, PushInfo info ) {
			if ( smash && !expired ) {
				return false;
			}

			_from            = null;
			_force           = info.Evaluate ( from, new TargetStruct ( _owner ) );
			_curve           = ( info.curve1, info.curve2 );
			ignoreOtherForce = info.ignoreOtherForce;
			expireOnGround   = info.expireOnGround;
			smash            = false;
			Intialize ();
			return true;
		}

		// public bool ApplyKnockback ( Projectile from, PushInfo info ) {
		// 	if ( smash && !expired ) {
		// 		return false;
		// 	}
		//
		// 	_from            = from.owner;
		// 	_force           = info.Evaluate ( from, new TargetStruct ( _owner ) );
		// 	_curve           = ( info.curve1, info.curve2 );
		// 	ignoreOtherForce = info.ignoreOtherForce;
		// 	expireOnGround   = info.expireOnGround;
		// 	smash            = false;
		// 	Intialize ();
		// 	return true;
		// }

		public bool ApplyKnockback ( GameObject from, PushInfo info ) {
			if ( smash && !expired ) {
				return false;
			}

			_from            = from;
			_force           = info.Evaluate ( from.transform, new TargetStruct ( _owner ) );
			_curve           = ( info.curve1, info.curve2 );
			ignoreOtherForce = info.ignoreOtherForce;
			expireOnGround   = info.expireOnGround;
			smash            = false;
			Intialize ();
			return true;
		}

		internal void Update ( out Vector2 vector, float deltaTime ) {
			vector = Vector2.zero;
			if ( duration == 0f ) {
				vector  = _force.Item1 + _force.Item2;
				expired = true;
				onEnd?.Invoke ( this, _from, _owner, SmashEndType.Expire, null, Movement.CollisionDirection.None );
				return;
			}

			bool num  = UpdateForce ( ref vector, _force.Item1, _curve.Item1, ref _amountBefore.Item1 );
			bool flag = UpdateForce ( ref vector, _force.Item2, _curve.Item2, ref _amountBefore.Item2 );
			_time += deltaTime;
			if ( !( num || flag ) ) {
				expired = true;
				onEnd?.Invoke ( this, _from, _owner, SmashEndType.Expire, null, Movement.CollisionDirection.None );
			}

			bool UpdateForce ( ref Vector2 forceVector, Vector2 force, Curve curve, ref float amountBefore ) {
				if ( _time > curve.duration || curve.duration == 0f ) {
					return false;
				}

				float num2 = curve.Evaluate ( _time );
				forceVector  += force * ( num2 - amountBefore );
				amountBefore =  num2;
				return true;
			}
		}

		internal void CollideWith ( RaycastHit2D raycastHit, Movement.CollisionDirection direction ) {
			if ( !expired ) {
				expired = true;
				onEnd?.Invoke ( this, _from, _owner, SmashEndType.Collide, raycastHit, direction );
			}
		}

		internal void Expire () {
			expired = true;
		}
	}
}