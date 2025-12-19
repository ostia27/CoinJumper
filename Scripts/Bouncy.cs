using System;
using UnityEngine;

namespace CoinJumper.Scripts
{
    public class Bouncy : MonoBehaviour
    {
        private const float MinSpeed = 0f;
        private const float MaxSpeed = 1000f;
        public GameObject audioObjectPrefab;
        private readonly LayerMask _collisionMask = -1;

        private int _bounceCount = 0;

        private Rigidbody _rb;
        private const float SkinWidth = 0.01f;
        public Action<int> OnBounce;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.sleepThreshold = 0f;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void FixedUpdate()
        {
            float speed = _rb.velocity.magnitude;
            if (speed < 0.01f) return;

            if (speed > MaxSpeed)
            {
                speed = MaxSpeed;
                _rb.velocity = _rb.velocity.normalized * speed;
            }

            Vector3 direction = _rb.velocity.normalized;
            float distanceThisFrame = speed * Time.fixedDeltaTime;

            RaycastHit[] hits = _rb.SweepTestAll(direction, distanceThisFrame, QueryTriggerInteraction.Ignore);

            if (hits == null || hits.Length == 0) return;

            Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            foreach (var t in hits)
            {
                if (t.transform == transform) continue;
                if (((1 << t.collider.gameObject.layer) & _collisionMask) == 0) continue;

                float distanceToMove = Mathf.Max(0, t.distance - SkinWidth);
                _rb.position += direction * distanceToMove;

                Vector3 normal = t.normal;
                Vector3 reflectedDir = Vector3.Reflect(direction, normal).normalized;

                float calculatedSpeed = speed * Plugin.BounceMultiplierValue;
                float newSpeed = Mathf.Clamp(calculatedSpeed, MinSpeed, MaxSpeed);
                
                _rb.velocity = reflectedDir * newSpeed;

                if (audioObjectPrefab != null)
                {
                    Instantiate(audioObjectPrefab, t.point, Quaternion.LookRotation(normal));
                }

                float remainingDistance = distanceThisFrame - t.distance;
                if (remainingDistance > 0)
                {
                    _rb.position += reflectedDir * remainingDistance;
                }

                _bounceCount++;
                OnBounce.Invoke(_bounceCount);
                break;
            }
        }
    }
}