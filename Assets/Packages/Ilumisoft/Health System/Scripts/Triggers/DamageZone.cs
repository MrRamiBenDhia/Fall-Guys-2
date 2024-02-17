using System.Collections.Generic;
using UnityEngine;

namespace Ilumisoft.HealthSystem
{
    /// <summary>
    /// Constantly applies damage to all GameObjects within the zone
    /// </summary>
    public class DamageZone : MonoBehaviour
    {
        [SerializeField]
        LayerMask layers = -1;

        [SerializeField, Tooltip("The amount of damage being applied per second")]
        float damagePerSecond = 0;

        /// <summary>
        /// List of all targets, damage will be applied to
        /// </summary>
        List<HealthComponent> targets = new();

        private void OnTriggerEnter(Collider other)
        {
            // Cancel if collider is not in layer mask
            if (ContainsLayer(layers, other.gameObject.layer))
            {
                return;
            }

            if (other.TryGetComponent<HealthComponent>(out var health) && health.IsAlive)
            {
                if (targets.Contains(health))
                {
                    return;
                }

                targets.Add(health);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Cancel if collider is not in layer mask
            if (ContainsLayer(layers, other.gameObject.layer))
            {
                return;
            }

            if (other.TryGetComponent<HealthComponent>(out var health) && health.IsAlive)
            {
                targets.Remove(health);
            }
        }

        /// <summary>
        ///  Returns true if the given layer mask contains the given layer, false otherwise
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        bool ContainsLayer(LayerMask layerMask, int layer)
        {
            return (layers.value & 1 << layer) == 0;
        }

        private void Update()
        {
            if (targets.Count == 0)
            {
                return;
            }

            // Remove dead or null targets
            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (target == null || target.IsAlive == false)
                {
                    targets.RemoveAt(i);
                    i--;
                }
            }

            ApplyDamage();
        }

        /// <summary>
        /// Applies damage to all targets
        /// </summary>
        private void ApplyDamage()
        {
            float damage = damagePerSecond * Time.deltaTime;

            // Apply damage
            foreach (var target in targets)
            {
                target.ApplyDamage(damage);
            }
        }

        private void Reset()
        {
            // Log a warning if there is a collider component, but it is not set to trigger
            if (TryGetComponent<Collider>(out var collider))
            {
                if (!collider.isTrigger)
                {
                    Debug.LogWarning("Damage Zone: The attached collider component is not a trigger. Please enable 'Is Trigger'.", this);
                }
            }
            else
            {
                // Automatically add a sphere trigger if no collider has already been added
                gameObject.AddComponent<SphereCollider>().isTrigger = true;
            }
        }
    }
}