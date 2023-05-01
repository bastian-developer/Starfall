using Powers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class Player : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;

        [Header("Padding")] 
        [SerializeField] private float paddingLeft;
        [SerializeField] private float paddingRight;
        [SerializeField] private float paddingTop;
        [SerializeField] private float paddingBottom;

        private PlayerAnimator _playerAnimator;
        private Shooter _shooter;
        private Vector2 _rawInput;
        private Vector2 _minBounds;
        private Vector2 _maxBounds;
        private Camera _mainCamera;
        private Mouse _mouse;

        private ShieldManager _shieldManager;
        private BombManager _bombManager;

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }

        private void Awake()
        {
            _shooter = GetComponent<Shooter>();
            var animator = GetComponent<Animator>();
            _playerAnimator = new PlayerAnimator(animator);
            _shieldManager = FindObjectOfType<ShieldManager>();
            _bombManager = FindObjectOfType<BombManager>();

            _mainCamera = Camera.main;
            _mouse = Mouse.current;

            // Set Input Action Callbacks
            var shieldAction = playerInput.actions["Shield"];
            shieldAction.performed += StartShieldCallback;
            shieldAction.canceled += StopShieldCallback;
        }

        private void Update()
        {
            Move();
            RotateTowardsMousePosition();
        }

        private void Start()
        {
            InitBounds();
        }

        // Initialize bounds that prevent player from reaching borders of the screen
        private void InitBounds()
        {
            _minBounds = _mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
            _maxBounds = _mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        }
        
        // Get player movement
        private void OnMove(InputValue value)
        {
            _rawInput = value.Get<Vector2>();
        }

        private void OnFire(InputValue value)
        {
            _shooter.isFiring = value.isPressed;
        }

        private void OnSecondaryFire(InputValue value)
        {
            _shooter.isFiringSecondary = value.isPressed;

        }
        
        private void OnBomb(InputValue value)
        {
            _bombManager.Explode();
        }

        // Called when player presses space, ask to shield manager to activate one of the shields
        private void StartShieldCallback(InputAction.CallbackContext context)
        {
            _shieldManager.StartShield();
        }

        // Called when player releases space, ask to shield manager to disable current shield
        private void StopShieldCallback(InputAction.CallbackContext context)
        {
            _shieldManager.StopShield();
        }

        // Transform player character position to player input
        private void Move()
        {
            var delta = _rawInput * (moveSpeed * Time.deltaTime);
            _playerAnimator.AnimatePlayer(_rawInput);
            var position = transform.position;
            var newPosition = new Vector2
            {
                x = Mathf.Clamp(position.x + delta.x, _minBounds.x + paddingLeft, _maxBounds.x - paddingRight),
                y = Mathf.Clamp(position.y + delta.y, _minBounds.y + paddingBottom, _maxBounds.y - paddingTop)
            };
            position = newPosition;
            transform.position = position;
        }

        // Get mouse position, calculate angle and rotate player sprite to fire accordingly
        private void RotateTowardsMousePosition()
        {
            var mousePosition = _mainCamera.ScreenToWorldPoint(_mouse.position.ReadValue());
            transform.rotation = Quaternion.Slerp(transform.rotation, _shooter.Vector3ToQuaternion(mousePosition),
                Time.deltaTime * rotationSpeed);
        }
    }
}