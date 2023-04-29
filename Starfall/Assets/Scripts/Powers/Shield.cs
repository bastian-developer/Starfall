using System.Collections;
using UnityEngine;

namespace Powers
{
    // This class represents a shield that can protect the player from enemy projectiles.
    public class Shield : MonoBehaviour
    {
        // The player and laser prefab to be used.
        [Header("Setup")] [SerializeField] private GameObject player;
        [SerializeField] private GameObject laserPrefab;

        // The speed at which the shield rotates and follows the player.
        [Header("Movement")] [SerializeField] private float rotationSpeed;
        [SerializeField] private float followSpeed;

        // The energy required to activate the shield, the energy cost per second, and the delay between energy consumption.
        [Header("Energy")] [SerializeField] private int energyActivationCost;
        [SerializeField] private int energyCostOverTime;
        [SerializeField] private float energyConsumptionDelay;

        private AudioPlayer _audioPlayer;
        private Coroutine _consumeEnergyCoroutine;
        private Energy _playerEnergy;
        private bool _shieldSwitch;
        
        // The rotation of the player.
        private Quaternion PlayerRotation => player.transform.rotation;

        // Properties to access the energy activation cost, energy consumption delay, and energy cost per second.
        public int EnergyActivationCost => energyActivationCost;

        public float EnergyConsumptionDelay => energyConsumptionDelay;

        public int EnergyCostOverTime => energyCostOverTime;

        private void Awake()
        {
            // Set the initial position of the shield to be the same as the player.
            transform.position = player.gameObject.transform.position;

            // Find the AudioPlayer component in the scene.
            _audioPlayer = FindObjectOfType<AudioPlayer>();
            
            //Find Energy component to pay energy.
            _playerEnergy = FindObjectOfType<Energy>();
        }

        private void Update()
        {
            FollowPlayer();
        }

        private void FollowPlayer()
        {
            //Move the shield towards the player and rotate it.
            if (!player) return;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, followSpeed);
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // If the shield collides with a laser and the player is present,
            // destroy the laser and spawn a new laser instance.
            if (!other.CompareTag("Laser") || !player) return;
            _audioPlayer.PlayShieldClip();
            Destroy(other.gameObject);
            var laserInstance = Instantiate(laserPrefab, transform.position, PlayerRotation);
    
            // Set the velocity of the laser instance to the speed of the player's
            // projectiles and destroy it after a set lifetime.
            if (laserInstance.TryGetComponent(out Rigidbody2D laserRigidbody))
            {
                var projectileSpeed = player.GetComponent<Shooter>().GetProjectileSpeed();
                laserRigidbody.velocity = player.transform.up * projectileSpeed;
                var projectileLifetime = player.GetComponent<Shooter>().GetProjectileLifetime();
                Destroy(laserInstance, projectileLifetime);
            }
        }
        
        //Verifies if the player should consume energy in shielding actions
        public void ManageEnergyConsumption(bool isShielded)
        {
            switch (isShielded)
            {
                case true when _consumeEnergyCoroutine == null:
                    _consumeEnergyCoroutine = StartCoroutine(RemoveEnergyOverTimeShield(isShielded));
                    break;
                case false when _consumeEnergyCoroutine != null:
                    StopCoroutine(_consumeEnergyCoroutine);
                    _consumeEnergyCoroutine = null;
                    break;
            }
        }

        //Calls energy class to reduce current energy quantity
        private IEnumerator RemoveEnergyOverTimeShield(bool isShielded)
        {
            while (isShielded)
            {
                yield return new WaitForSeconds(energyConsumptionDelay);
                _playerEnergy.PayEnergyCost(energyCostOverTime, "Shielding");
            }
        }
    }
}