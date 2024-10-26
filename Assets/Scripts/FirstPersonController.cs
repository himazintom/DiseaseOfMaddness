using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Move speed in the River")]
		public float RiverMoveSpeed = 4.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The height the player can jump in the River")]
		public float RiverJumpHeight = 0.6f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;
		[Tooltip("Head Check")]
		public bool HeadHit;
		[Tooltip("Head Check Object")]
		public GameObject HeadCheckObject;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;
		[Tooltip("Slide in slope")]
		public float groundAngle = 0;
		public Vector3 groundNormal = Vector3.up;
		private Vector3 lastGroundNormal = Vector3.zero;
		private Vector3 lastHitPoint= new Vector3(Mathf.Infinity, 0, 0);
		private Vector3 slipMoveVector3;

		[Header("Player Sound")]
		public AudioSource audioSource;
		private float shoesSoundTime=0f;
		private bool shoesLeft=false;
		public float NeedShoesSoundTime=1f;
		public AudioClip WalkingGroundClipLeft;
		public AudioClip WalkingGroundClipRight;
		public AudioClip EnterRiverClip;
		public AudioClip ExitRiverClip;
		public AudioClip BitedClip;
		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;
		private bool waterCheck=false;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;
		public GameManager gameManager;
		public bool NowAction;

	
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			if(!NowAction)Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
			if(waterCheck==true){
				targetSpeed=RiverMoveSpeed;
			}

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero){
				targetSpeed = 0.0f;
				shoesSoundTime=0f;
			}else{
				if(Grounded==true && waterCheck==false){//WalkingSound
					shoesSoundTime+=Time.deltaTime;
					float needtime = _input.sprint ? NeedShoesSoundTime*MoveSpeed/SprintSpeed : NeedShoesSoundTime;
					if(shoesSoundTime>needtime){
						if(shoesLeft==true){
							audioSource.PlayOneShot(WalkingGroundClipLeft);
							shoesLeft=false;
						}else{
							audioSource.PlayOneShot(WalkingGroundClipRight);
							shoesLeft=true;
						}
						shoesSoundTime=0f;
					}
				}
			}
			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}
			slipMoveVector3=new Vector3(0.0f, _verticalVelocity, 0.0f);
			//Debug.Log("_controller.slopeLimit="+_controller.slopeLimit+"  groundAngle="+groundAngle);
			//Debug.Log("saa="+(Grounded && _controller.slopeLimit<groundAngle && groundAngle<=90f));
			if(Grounded && _controller.slopeLimit<groundAngle){
				var xz=Mathf.Sqrt(Mathf.Pow(groundNormal.x,2)+Mathf.Pow(groundNormal.z,2));
				var ty=xz;
				var tx=-groundNormal.y*groundNormal.x;
				var tz=-groundNormal.y*groundNormal.z;
				slipMoveVector3 = new Vector3(tx,ty,tz)*_verticalVelocity;
				//Debug.Log("slipVector3="+slipMoveVector3);
			}
			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + slipMoveVector3 * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				float tempJumpHeight= waterCheck ? RiverJumpHeight : JumpHeight;
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)//落下の初速
				{
					_verticalVelocity = -2f;
				}

				// // Jump
				// if (_input.jump && _jumpTimeoutDelta <= 0.0f && _controller.slopeLimit>groundAngle)
				// {
				// 	// the square root of H * -2 * G = how much velocity needed to reach desired height
				// 	_verticalVelocity = Mathf.Sqrt(tempJumpHeight * -2f * Gravity);
				// }

				// // jump timeout
				// if (_jumpTimeoutDelta >= 0.0f)
				// {
				// 	_jumpTimeoutDelta -= Time.deltaTime;
				// }
				

				if(_controller.slopeLimit<groundAngle)//地面についてても滑ってたら加速させる
				{
					//Debug.Log("verticalVelocity="+_verticalVelocity+"Ground="+Grounded);
					_verticalVelocity += Gravity * Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
				
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)//もし、超超上昇中でなければ常に落下させる。
			{
				//Debug.Log("verticalVelocity="+_verticalVelocity+"Ground="+Grounded);
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
		void OnTriggerEnter(Collider collider){
			if(collider.gameObject.tag=="River"){
				audioSource.PlayOneShot(EnterRiverClip);
				waterCheck=true;
			}
		}
		void OnTriggerExit(Collider collider){
			if(collider.gameObject.tag=="River"){
				audioSource.PlayOneShot(ExitRiverClip);
				if(Grounded)waterCheck=false;
			}
		}
		private void OnCollisionEnter(Collision other) {
			if(other.gameObject.tag=="Ozisan"){

				gameManager.Gameover();
			}
		}
		void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (hit.normal.y > 0 && hit.moveDirection.y < 0)//地面に触れたら~
			{
				if ((hit.point - lastHitPoint).sqrMagnitude > 0.001f || lastGroundNormal == Vector3.zero)//触れた地面の場所（距離）が離れていたら　また　最後に触れた地面がx,z面に平行であったら
				{
					groundNormal = hit.normal;
				}
				else
				{
					groundNormal = lastGroundNormal;
				}

				lastHitPoint = hit.point;
			}
			// 現在の接地面の角度を取得
			groundAngle = Vector3.Angle(hit.normal, Vector3.up);
			//Debug.Log("groundAngle="+groundAngle+"  groundNormal="+groundNormal);
		}

	}
}