using PhysicsUtils;
using UnityEngine;
using UnityEngine.Serialization;


namespace Characters.Movements {
	
	public class CharacterController2D : MonoBehaviour {
		
		[HideInInspector] public bool ignorePlatform;
		[HideInInspector] public bool ignoreAbovePlatform = true;

		private const float _extraRayLength = 0.001f;

		[Range ( 0.001f, 0.3f )] private float _skinWidth = 0.0001f; // 0.03

		[FormerlySerializedAs ( "platformMask" )]
		public LayerMask terrainMask = 0;
		public LayerMask triggerMask = 0;
		public LayerMask oneWayPlatformMask = 0;

		public float jumpingThreshold = 0.07f;

		[Range ( 2f, 20f )] public int horizontalRays = 8;
		[Range ( 2f, 20f )] public int verticalRays = 4;

		[SerializeField] protected BoxCollider2D _boxCollider;
		[SerializeField] protected Rigidbody2D _rigidBody;

		public readonly CharacterController2D.CollisionState collisionState = new CharacterController2D.CollisionState ();

		[HideInInspector] public Vector3 velocity;
		private BoxSequenceNonAllocCaster _boxCaster;
		private Bounds _bounds;

		public bool isGrounded {
			get { return this.collisionState.below && this.velocity.y <= 0.001f; }
		}

		public bool onTerrain {
			get {
				return this.collisionState.below &&
				       this.terrainMask.Contains ( this.collisionState.lastStandingCollider.gameObject.layer );
			}
		}

		public bool onPlatform {
			get {
				return this.collisionState.below &&
				       this.oneWayPlatformMask.Contains ( this.collisionState.lastStandingCollider.gameObject.layer );
			}
		}

		private void Awake () {
			Bounds bounds = this._boxCollider.bounds;
			bounds.center   -= base.transform.position;
			this._boxCaster =  new BoxSequenceNonAllocCaster ( 1, this.horizontalRays, this.verticalRays );
			this._boxCaster.SetOriginsFromBounds ( bounds );
		}

		public void UpdateBounds () {
			Bounds bounds = this._boxCollider.bounds;
			bounds.center -= base.transform.position;
			if ( this._bounds == bounds ) {
				return;
			}

			if ( this._boxCaster == null ) {
				return;
			}

			this._boxCaster.origin = base.transform.position;
			this.UpdateTopCasterPosition ( bounds );
			this.UpdateBottomCasterPosition ( bounds );
			this.UpdateLeftCasterPosition ( bounds );
			this.UpdateRightCasterPosition ( bounds );
			this._boxCaster.SetOriginsFromBounds ( this._bounds );
		}

		private void UpdateTopCasterPosition ( Bounds bounds ) {
			Vector2                    mostLeftTop  = bounds.GetMostLeftTop ();
			Vector2                    mostRightTop = bounds.GetMostRightTop ();
			LineSequenceNonAllocCaster topRaycaster = this._boxCaster.topRaycaster;
			float                      num          = mostLeftTop.y;
			if ( topRaycaster.start.y < mostLeftTop.y - this._skinWidth ) {
				topRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
				topRaycaster.CastToLine ( mostLeftTop, mostRightTop );
				for ( int i = 0; i < topRaycaster.nonAllocCasters.Count; i++ ) {
					ReadonlyBoundedList< RaycastHit2D > results = topRaycaster.nonAllocCasters[ i ].results;
					if ( results.Count != 0 ) {
						num = Mathf.Min ( num, results[ 0 ].point.y - this._boxCaster.origin.y );
					}
				}

				num -= this._skinWidth;
				if ( num < this._bounds.max.y ) {
					return;
				}
			}

			Vector3 max = this._bounds.max;
			max.y            = num;
			this._bounds.max = max;
		}

		private void UpdateBottomCasterPosition ( Bounds bounds ) {
			Vector2                    mostLeftBottom  = bounds.GetMostLeftBottom ();
			Vector2                    mostRightBottom = bounds.GetMostRightBottom ();
			LineSequenceNonAllocCaster bottomRaycaster = this._boxCaster.bottomRaycaster;
			float                      num             = mostLeftBottom.y;
			if ( bottomRaycaster.start.y > mostLeftBottom.y + this._skinWidth ) {
				bottomRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
				bottomRaycaster.CastToLine ( mostLeftBottom, mostRightBottom );
				for ( int i = 0; i < bottomRaycaster.nonAllocCasters.Count; i++ ) {
					ReadonlyBoundedList< RaycastHit2D > results = bottomRaycaster.nonAllocCasters[ i ].results;
					if ( results.Count != 0 ) {
						num = Mathf.Max ( num, results[ 0 ].point.y - this._boxCaster.origin.y );
					}
				}

				num += this._skinWidth;
				if ( num > this._bounds.min.y ) {
					return;
				}
			}

			Vector3 min = this._bounds.min;
			min.y            = num;
			this._bounds.min = min;
		}

		private void UpdateLeftCasterPosition ( Bounds bounds ) {
			Vector2                    mostLeftTop    = bounds.GetMostLeftTop ();
			Vector2                    mostLeftBottom = bounds.GetMostLeftBottom ();
			LineSequenceNonAllocCaster leftRaycaster  = this._boxCaster.leftRaycaster;
			float                      num            = mostLeftTop.x;
			if ( leftRaycaster.start.x > mostLeftTop.x + this._skinWidth ) {
				leftRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
				leftRaycaster.CastToLine ( mostLeftTop, mostLeftBottom );
				for ( int i = 0; i < leftRaycaster.nonAllocCasters.Count; i++ ) {
					ReadonlyBoundedList< RaycastHit2D > results = leftRaycaster.nonAllocCasters[ i ].results;
					if ( results.Count != 0 ) {
						num = Mathf.Max ( num, results[ 0 ].point.x - this._boxCaster.origin.x );
					}
				}

				num += this._skinWidth;
				if ( num > this._bounds.min.x ) {
					return;
				}
			}

			Vector3 min = this._bounds.min;
			min.x            = num;
			this._bounds.min = min;
		}

		private void UpdateRightCasterPosition ( Bounds bounds ) {
			Vector2                    mostRightTop    = bounds.GetMostRightTop ();
			Vector2                    mostRightBottom = bounds.GetMostRightBottom ();
			LineSequenceNonAllocCaster rightRaycaster  = this._boxCaster.rightRaycaster;
			float                      num             = mostRightTop.x;
			if ( rightRaycaster.start.x < mostRightTop.x - this._skinWidth ) {
				rightRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
				rightRaycaster.CastToLine ( mostRightTop, mostRightBottom );
				for ( int i = 0; i < rightRaycaster.nonAllocCasters.Count; i++ ) {
					ReadonlyBoundedList< RaycastHit2D > results = rightRaycaster.nonAllocCasters[ i ].results;
					if ( results.Count != 0 ) {
						num = Mathf.Min ( num, results[ 0 ].point.x - this._boxCaster.origin.x );
					}
				}

				num -= this._skinWidth;
				if ( num < this._bounds.max.x ) {
					return;
				}
			}

			Vector3 max = this._bounds.max;
			max.x            = num;
			this._bounds.max = max;
		}

		public Vector2 Move ( Vector2 deltaMovement ) {
			Vector3 position = base.transform.position;
			this.Move ( ref position, ref deltaMovement );
			position.x              += deltaMovement.x;
			position.y              += deltaMovement.y;
			base.transform.position =  position;
			return deltaMovement;
		}

		private bool TeleportUponGround ( Vector2 direction, float distance, bool recursive ) {
			if ( recursive ) {
				Vector2 a = base.transform.position;
				while ( distance > 0f ) {
					if ( this.TeleportUponGround ( a + direction * distance, 4f ) ) {
						return true;
					}

					distance -= 1f;
				}
			}
			else {
				this.TeleportUponGround ( base.transform.position + ( Vector3 )direction * distance, 4f );
			}

			return false;
		}

		public bool TeleportUponGround ( Vector2 destination, float distance = 4f ) {
			RaycastHit2D hit = Physics2D.Raycast ( destination, Vector2.down, distance,
				this.terrainMask | this.oneWayPlatformMask );
			if ( hit ) {
				destination   =  hit.point;
				destination.y += this._skinWidth * 2f;
				return this.Teleport ( destination );
			}

			return false;
		}

		public bool Teleport ( Vector2 destination, float maxRetryDistance ) {
			return this.Teleport ( destination,
				( MMMaths.Vector3ToVector2 ( base.transform.position ) - destination ).normalized, maxRetryDistance );
		}

		public bool Teleport ( Vector2 destination, Vector2 direction, float maxRetryDistance ) {
			int num = 0;
			while ( ( float )num <= maxRetryDistance ) {
				if ( this.Teleport ( destination + direction * ( float )num ) ) {
					return true;
				}

				num++;
			}

			return false;
		}

		public bool Teleport ( Vector2 destination ) {
			Bounds bounds = this._boxCollider.bounds;
			bounds.center = new Vector2 ( destination.x, destination.y + ( bounds.center.y - bounds.min.y ) );
			NonAllocOverlapper.shared.contactFilter.SetLayerMask ( this.terrainMask | this.oneWayPlatformMask );
			if ( NonAllocOverlapper.shared.OverlapBox ( bounds.center, bounds.size, 0f ).results.Count == 0 ) {
				base.transform.position = destination;
				return true;
			}

			return false;
		}

		private void Move ( ref Vector3 origin, ref Vector2 deltaMovement ) {
			int  num = 0;
			bool flag;
			do {
				flag = false;
				num++;
				if ( !this.CastLeft ( ref origin, ref deltaMovement ) ) {
					origin.x += 0.1f * ( float )num;
					flag      =  true;
				}

				if ( !this.CastRight ( ref origin, ref deltaMovement ) ) {
					origin.x -= 0.1f * ( float )num;
					flag      =  true;
				}

				if ( !this.CastUp ( ref origin, ref deltaMovement ) ) {
					origin.y -= 0.1f * ( float )num;
					flag      =  true;
				}

				if ( !this.CastDown ( ref origin, ref deltaMovement ) ) {
					origin.y += 0.1f * ( float )num;
					flag      =  true;
				}
			} while ( flag );
		}

		private bool CastRight ( ref Vector3 origin, ref Vector2 deltaMovement ) {
			float distance = Mathf.Abs ( deltaMovement.x ) + this._skinWidth + 0.001f;
			float num      = deltaMovement.x;
			this._boxCaster.rightRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
			this._boxCaster.rightRaycaster.caster.origin   = origin;
			this._boxCaster.rightRaycaster.caster.distance = distance;
			this._boxCaster.rightRaycaster.Cast ();
			using ( this.collisionState.rightCollisionDetector.scope ) {
				for ( int i = 0; i < this._boxCaster.rightRaycaster.nonAllocCasters.Count; i++ ) {
					NonAllocCaster nonAllocCaster = this._boxCaster.rightRaycaster.nonAllocCasters[ i ];
					if ( nonAllocCaster.results.Count != 0 ) {
						RaycastHit2D hit = nonAllocCaster.results[ 0 ];
						if ( hit ) {
							if ( hit.distance == 0f ) {
								return false;
							}

							num = Mathf.Min ( num, hit.distance - this._skinWidth );
							this.collisionState.rightCollisionDetector.Add ( hit );
						}
					}
				}
			}

			deltaMovement.x = num;
			return true;
		}

		private bool CastLeft ( ref Vector3 origin, ref Vector2 deltaMovement ) {
			float distance = Mathf.Abs ( deltaMovement.x ) + this._skinWidth + 0.001f;
			float x        = deltaMovement.x;
			this._boxCaster.leftRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
			this._boxCaster.leftRaycaster.caster.origin   = origin;
			this._boxCaster.leftRaycaster.caster.distance = distance;
			this._boxCaster.leftRaycaster.Cast ();
			using ( this.collisionState.leftCollisionDetector.scope ) {
				for ( int i = 0; i < this._boxCaster.leftRaycaster.nonAllocCasters.Count; i++ ) {
					NonAllocCaster nonAllocCaster = this._boxCaster.leftRaycaster.nonAllocCasters[ i ];
					if ( nonAllocCaster.results.Count != 0 ) {
						RaycastHit2D hit = nonAllocCaster.results[ 0 ];
						if ( hit ) {
							if ( hit.distance == 0f ) {
								return false;
							}

							x = Mathf.Max ( deltaMovement.x, -hit.distance + this._skinWidth );
							this.collisionState.leftCollisionDetector.Add ( hit );
						}
					}
				}
			}

			deltaMovement.x = x;
			return true;
		}

		private bool CastUp ( ref Vector3 origin, ref Vector2 deltaMovement ) {
			float distance = Mathf.Abs ( deltaMovement.y ) + this._skinWidth + 0.001f;
			if ( this.ignoreAbovePlatform ) {
				this._boxCaster.topRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
			}
			else {
				this._boxCaster.topRaycaster.caster.contactFilter.SetLayerMask (
					this.terrainMask | this.oneWayPlatformMask );
			}

			this._boxCaster.topRaycaster.caster.origin   = origin;
			this._boxCaster.topRaycaster.caster.distance = distance;
			this._boxCaster.topRaycaster.Cast ();
			using ( this.collisionState.aboveCollisionDetector.scope ) {
				for ( int i = 0; i < this._boxCaster.topRaycaster.nonAllocCasters.Count; i++ ) {
					NonAllocCaster nonAllocCaster = this._boxCaster.topRaycaster.nonAllocCasters[ i ];
					if ( nonAllocCaster.results.Count != 0 ) {
						RaycastHit2D hit = nonAllocCaster.results[ 0 ];
						if ( hit ) {
							if ( hit.distance == 0f ) {
								return false;
							}

							deltaMovement.y = Mathf.Min ( deltaMovement.y, hit.distance - this._skinWidth );
							this.collisionState.aboveCollisionDetector.Add ( hit );
						}
					}
				}
			}

			return true;
		}

		private bool CastDown ( ref Vector3 origin, ref Vector2 deltaMovement ) {
			float distance = Mathf.Abs ( deltaMovement.y ) + this._skinWidth + 0.001f;
			if ( this.ignorePlatform ) {
				this._boxCaster.bottomRaycaster.caster.contactFilter.SetLayerMask ( this.terrainMask );
			}
			else {
				this._boxCaster.bottomRaycaster.caster.contactFilter.SetLayerMask (
					this.terrainMask | this.oneWayPlatformMask );
			}

			this._boxCaster.bottomRaycaster.caster.origin   = origin;
			this._boxCaster.bottomRaycaster.caster.distance = distance;
			this._boxCaster.bottomRaycaster.Cast ();
			using ( this.collisionState.belowCollisionDetector.scope ) {
				for ( int i = 0; i < this._boxCaster.bottomRaycaster.nonAllocCasters.Count; i++ ) {
					NonAllocCaster nonAllocCaster = this._boxCaster.bottomRaycaster.nonAllocCasters[ i ];
					if ( nonAllocCaster.results.Count != 0 ) {
						RaycastHit2D hit = nonAllocCaster.results[ 0 ];
						if ( hit ) {
							if ( hit.distance == 0f ) {
								return false;
							}

							deltaMovement.y = Mathf.Max ( deltaMovement.y, -hit.distance + this._skinWidth );
							this.collisionState.lastStandingCollider = hit.collider;
							this.collisionState.belowCollisionDetector.Add ( hit );
						}
					}
				}
			}

			return true;
		}

		public class CollisionState {
			internal readonly ManualCollisionDetector aboveCollisionDetector = new ManualCollisionDetector ();

			internal readonly ManualCollisionDetector belowCollisionDetector = new ManualCollisionDetector ();

			internal readonly ManualCollisionDetector leftCollisionDetector = new ManualCollisionDetector ();

			internal readonly ManualCollisionDetector rightCollisionDetector = new ManualCollisionDetector ();

			public bool above {
				get { return this.aboveCollisionDetector.colliding; }
			}

			public bool below {
				get { return this.belowCollisionDetector.colliding; }
			}

			public bool right {
				get { return this.rightCollisionDetector.colliding; }
			}

			public bool left {
				get { return this.leftCollisionDetector.colliding; }
			}

			public bool horizontal {
				get { return this.right || this.left; }
			}

			public bool vertical {
				get { return this.right || this.left; }
			}

			public bool any {
				get { return this.below || this.right || this.left || this.above; }
			}

			public Collider2D lastStandingCollider { get; internal set; }

			internal CollisionState () {}
		}
	}
}