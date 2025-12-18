using System;
using UnityEngine;

namespace CoinJumper.Scripts
{
    public class Bouncy : MonoBehaviour
    {
        [SerializeField] private float minSpeed = 0f;
        [SerializeField] private float maxSpeed = 1000f;
        [SerializeField] public GameObject audioObjectPrefab;

        private Rigidbody _rb;
        private Vector3 _previousVelocity;
        
        public int BounceCount { get; private set; }
        public event Action OnBounce;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void FixedUpdate()
        {
            _previousVelocity = _rb.velocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            BounceCount++;
            
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(_previousVelocity.normalized, normal);

            float speed = _previousVelocity.magnitude;
            float newSpeed = Mathf.Clamp(speed * Plugin.BounceMultiplierValue, minSpeed, maxSpeed);

            _rb.velocity = reflectedDirection * newSpeed;

            if (audioObjectPrefab != null && Plugin.SoundOnValue)
            {
                Instantiate(audioObjectPrefab, collision.contacts[0].point, Quaternion.identity);
            }
            
            OnBounce?.Invoke();
        }
    }
}