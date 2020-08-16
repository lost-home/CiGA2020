using System;
using System.Collections.Generic;
using PhysicsUtils;
using RPGStatSystem;
using UnityEngine;


namespace Characters.Movements {
	
	public class Movement : MonoBehaviour {
		
		public readonly Sum< int > airJumpCount = new Sum< int > ( 1 );
		public readonly TrueOnlyLogicalSumList ignoreGravity = new TrueOnlyLogicalSumList ( false );

		[NonSerialized] public int currentAirJumpCount;
		[NonSerialized] public bool binded;
		[NonSerialized] public TrueOnlyLogicalSumList blocked = new TrueOnlyLogicalSumList ( false );
		[NonSerialized] public Vector2 move;
		[NonSerialized] public Vector2 force;
		[NonSerialized] public bool moveBackward;

		private const float _almostZero = 0.0001f;

		// [SerializeField]
		// private Character _character;
		[SerializeField]
		private RPGStatCollection _statCollection;

		[SerializeField] private Movement.Config _config;
		[SerializeField] [GetComponent] private CharacterController2D _controller;

		private Vector2 _moved;
		private Vector2 _velocity;

		private bool _jump;

		private Transform _moveAndForceTransform;
		private Transform _smashTransform;

		private static readonly NonAllocCaster _belowCaster = new NonAllocCaster ( 15 );

		public readonly Stack< Movement.Config > configs = new Stack< Movement.Config > ();

		public Push push;

		public event Action onGrounded;
		public event Action onFall;
		public event Movement.onJumpDelegate onJump;
		public event Action< Vector2 > onMoved;

		private float speed {
			get {
				return _statCollection.GetStatValue ( "MoveSpeed" );
			}
		}

		private float knockbackMultiplier {
			get {
				return 0;
				// return (float)this._character.stat.GetFinal(Stat.Kind.KnockbackResistance);
			}
		}

		public Movement.Config config {
			get { return configs.Peek (); }
		}

		public Vector2 lastDirection { get; private set; }

		public CharacterController2D controller {
			get { return _controller; }
		}

		// public Character owner
		// {
		// 	get
		// 	{
		// 		return _character;
		// 	}
		// }

		public Vector2 moved {
			get { return _moved; }
		}

		public Vector2 velocity {
			get { return _velocity; }
		}

		public float verticalVelocity {
			get { return _velocity.y; }
			set { _velocity.y = value; }
		}

		public bool isGrounded { get; private set; }

		protected virtual void Awake () {
			push = new Push ( controller.gameObject );
			configs.Push ( _config );
			_controller = GetComponent< CharacterController2D > ();
			_controller.collisionState.aboveCollisionDetector.OnEnter += delegate ( RaycastHit2D hit ) {
				OnControllerCollide ( hit, Movement.CollisionDirection.Above );
			};
			_controller.collisionState.belowCollisionDetector.OnEnter += delegate ( RaycastHit2D hit ) {
				OnControllerCollide ( hit, Movement.CollisionDirection.Below );
			};
			_controller.collisionState.leftCollisionDetector.OnEnter += delegate ( RaycastHit2D hit ) {
				OnControllerCollide ( hit, Movement.CollisionDirection.Left );
			};
			_controller.collisionState.rightCollisionDetector.OnEnter += delegate ( RaycastHit2D hit ) {
				OnControllerCollide ( hit, Movement.CollisionDirection.Right );
			};
			// Singleton<Service>.Instance.levelManager.onMapLoadedAndFadedIn += FindClosestBelowGround;
			currentAirJumpCount = 0;
		}

		private void OnDestroy () {
			// if (Service.quitting)
			// {
			// 	return;
			// }
			// Singleton<Service>.Instance.levelManager.onMapLoadedAndFadedIn -= this.FindClosestBelowGround;
		}

		private void FindClosestBelowGround () {
			RaycastHit2D hit = Physics2D.Raycast ( transform.position, Vector2.down, float.PositiveInfinity, _controller.terrainMask );
			if ( hit ) {
				controller.collisionState.lastStandingCollider = hit.collider;
			}
		}

		private void Start () {
			if ( config.snapToGround ) {
				_controller.Move ( new Vector2 ( 0f, -50f ) );
			}
		}

		private void OnControllerCollide ( RaycastHit2D raycastHit, Movement.CollisionDirection direction ) {
			// if ( this.push.smash && !this.push.expired ) {
			// 	this.push.CollideWith ( raycastHit, direction );
			// }
		}

		private bool HandlePush ( float deltaTime ) {
			if ( _config.ignorePush ) {
				return false;
			}

			if ( push.expired ) {
				_controller.ignoreAbovePlatform = true;
				return false;
			}

			Vector2 vector;
			push.Update ( out vector, deltaTime );
			_controller.ignoreAbovePlatform = !push.smash;
			vector                          *= knockbackMultiplier;
			if ( push.ignoreOtherForce ) {
				_moved    = _controller.Move ( vector );
				_velocity = Vector2.zero;
				return true;
			}

			force += vector;
			return false;
		}

		private Vector2 HandleMove ( float deltaTime ) {
			Vector2 vector = Vector2.zero;
			if ( HandlePush ( deltaTime ) ) {
				return vector;
			}

			float num = blocked.value ? 0f : speed;
			vector = move * num;
			// _character.animationController.parameter.walk = (vector.x != 0f);
			// _character.animationController.parameter.movementSpeed = (moveBackward ? (-num) : num) * 0.25f;
			if ( !config.lockLookingDirection ) {
				if ( moveBackward ) {
					if ( move.x > 0f ) {
						// _character.lookingDirection = Character.LookingDirection.Left;
					}
					else if ( move.x < 0f ) {
						// _character.lookingDirection = Character.LookingDirection.Right;
					}
				}
				else if ( move.x > 0f ) {
					// _character.lookingDirection = Character.LookingDirection.Right;
				}
				else if ( move.x < 0f ) {
					// _character.lookingDirection = Character.LookingDirection.Left;
				}
			}

			if ( _controller.isGrounded && _velocity.y < 0f ) {
				_velocity.y = 0f;
			}

			switch ( config.type ) {
				case Movement.Config.Type.Walking:
					_velocity.x = vector.x;
					AddGravity ( deltaTime );
					break;
				case Movement.Config.Type.Flying:
					_velocity = vector;
					break;
				case Movement.Config.Type.AcceleratingFlying:
					_velocity *= 1f - config.friction * deltaTime;
					_velocity += vector * config.acceleration * deltaTime;
					AddGravity ( deltaTime );
					break;
				case Movement.Config.Type.AcceleratingWalking:
					_velocity.x = _velocity.x * ( 1f - config.friction * deltaTime );
					_velocity.x = _velocity.x + vector.x * config.acceleration * deltaTime;
					if ( Mathf.Abs ( _velocity.x ) > num ) {
						_velocity.x = num * Mathf.Sign ( _velocity.x );
					}

					AddGravity ( deltaTime );
					break;
			}

			vector    = _velocity * deltaTime + force;
			_moved    = _controller.Move ( vector );
			_velocity = _moved - force;
			if ( vector.x > 0f != _velocity.x > 0f ) {
				_velocity.x = 0f;
			}

			if ( vector.y > 0f != _velocity.y > 0f ) {
				_velocity.y = 0f;
			}

			// this._character.animationController.parameter.ySpeed = this._velocity.y;
			if ( deltaTime > 0f ) {
				_velocity /= deltaTime;
				if ( config.type == Movement.Config.Type.AcceleratingFlying && _velocity.sqrMagnitude > num * num ) {
					_velocity = _velocity.normalized * num;
				}

				_controller.velocity = _velocity;
			}

			onMoved?.Invoke ( _moved );

			return vector;
		}

		private void AddGravity ( float deltaTime ) {
			if ( !ignoreGravity.value && !config.ignoreGravity ) {
				_velocity.y = _velocity.y + config.gravity * deltaTime;
			}

			if ( _velocity.y < -config.maxFallSpeed ) {
				_velocity.y = -config.maxFallSpeed;
			}
		}

		protected virtual void LateUpdate () {
			Movement.Config config = this.config;
			if ( config.type == Movement.Config.Type.Static ) {
				// this._character.animationController.parameter.grounded = true;
				// this._character.animationController.parameter.walk = false;
				// this._character.animationController.parameter.ySpeed = 0f;
				return;
			}

			_controller.UpdateBounds ();
			bool isGrounded = this.isGrounded;
			_moved = Vector2.zero;
			Vector2 ptr = this.HandleMove ( Time.deltaTime );
			// Vector2 ptr = this.HandleMove(this._character.chronometer.animation.DeltaTime());
			if ( config.type == Movement.Config.Type.Flying ||
			     config.type == Movement.Config.Type.AcceleratingFlying ) {
				// this._character.animationController.parameter.grounded = true;
			}
			else {
				// this._character.animationController.parameter.grounded = this._controller.isGrounded;
			}

			force = Vector2.zero;
			if ( !config.keepMove ) {
				move = Vector2.zero;
			}

			if ( ptr.y <= 0f && this._controller.collisionState.below ) {
				this.isGrounded = true;
				if ( !isGrounded ) {
					onGrounded?.Invoke ();
					currentAirJumpCount = 0;
					// if ( this._velocity.y <= 0f && !this.push.expired && this.push.expireOnGround ) {
					// 	this.push.Expire ();
					// 	return;
					// }
				}
			}
			else {
				if ( this.isGrounded ) {
					onFall?.Invoke ();
				}

				this.isGrounded = false;
			}
		}

		public void Move ( Vector2 normalizedDirection ) {
			if ( config.keepMove ) {
				if ( normalizedDirection == Vector2.zero ) {
					return;
				}

				if ( normalizedDirection.x > 0f ) {
					normalizedDirection.x = 1f;
				}

				if ( normalizedDirection.x < 0f ) {
					normalizedDirection.x = -1f;
				}
			}

			move          = normalizedDirection;
			lastDirection = move;
		}

		public void Move ( float angle ) {
			move.x        = Mathf.Cos ( angle );
			move.y        = Mathf.Sin ( angle );
			lastDirection = move;
		}

		public void MoveTo ( Vector2 position ) {
			Move ( new Vector2 ( position.x - base.transform.position.x, position.y - base.transform.position.y ).normalized );
		}

		public void Jump ( float jumpHeight ) {
			if ( jumpHeight > 1.401298E-45f ) {
				_velocity.y = Mathf.Sqrt ( 2f * jumpHeight * -config.gravity );
			}
			
			onJump?.Invoke ( isGrounded ? Movement.JumpType.GroundJump : Movement.JumpType.AirJump, jumpHeight );
		}

		public void JumpDown () {
			bool ignorePlatform = _controller.ignorePlatform;
			_controller.ignorePlatform = true;
			_controller.Move ( new Vector3 ( 0f, -0.1f, 0f ) );
			_controller.ignorePlatform = ignorePlatform;

			onJump?.Invoke ( Movement.JumpType.DownJump, 0f );
		}

		public bool TryBelowRayCast ( LayerMask mask, out RaycastHit2D point, float distance ) {
			Movement._belowCaster.contactFilter.SetLayerMask ( mask );
			// Movement._belowCaster.RayCast(this.owner.transform.position, Vector2.down, distance);
			Movement._belowCaster.RayCast ( transform.position, Vector2.down, distance );
			ReadonlyBoundedList< RaycastHit2D > results = Movement._belowCaster.results;
			point = default ( RaycastHit2D );
			if ( results.Count < 0 ) {
				return false;
			}

			int index = 0;
			float num = results[ 0 ].distance;
			for ( int i = 1; i < results.Count; i++ ) {
				float distance2 = results[ i ].distance;
				if ( distance2 < num ) {
					num   = distance2;
					index = i;
				}
			}

			point = results[ index ];
			return true;
		}

		[Serializable]
		public class Config {
			[SerializeField] internal Movement.Config.Type type = Movement.Config.Type.Walking;
			[SerializeField] internal bool keepMove;
			[SerializeField] internal bool snapToGround;
			[SerializeField] internal bool lockLookingDirection;
			[SerializeField] internal float gravity = -40f;
			[SerializeField] internal float maxFallSpeed = 25f;
			[SerializeField] internal float acceleration = 2f;
			[SerializeField] internal float friction = 0.95f;
			[SerializeField] internal bool ignoreGravity;
			[SerializeField] internal bool ignorePush;

			internal enum Type {
				Static,
				Walking,
				Flying,
				AcceleratingFlying,
				AcceleratingWalking
			}
		}

		public enum CollisionDirection {
			None,
			Above,
			Below,
			Left,
			Right
		}

		public enum JumpType {
			GroundJump,
			AirJump,
			DownJump
		}

		public delegate void onJumpDelegate ( Movement.JumpType jumpType, float jumpHeight );
	}
}