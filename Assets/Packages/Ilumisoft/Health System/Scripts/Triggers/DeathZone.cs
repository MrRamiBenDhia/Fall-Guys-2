using UnityEngine;

namespace Ilumisoft.HealthSystem
{
    /// <summary>
    /// Triggers the death of GameObjects entering the zone.
    /// </summary>
    [AddComponentMenu("Health System/Triggers/Death Zone")]
    public class DeathZone : MonoBehaviour
    {
        [SerializeField]
        LayerMask layers = -1;

        private void OnTriggerEnter(Collider other)
        {
            // Cancel if collider not in layer mask
            if (ContainsLayer(layers, other.gameObject.layer))
            {
                return;
            }

            if (other.TryGetComponent<HealthComponent>(out var health) && health.IsAlive)
            {
                health.ApplyDamage(health.CurrentHealth);
            }
        }

        private void Reset()
        {
            // Log a warning if there is a collider component, but it is not set to trigger
            if (TryGetComponent<Collider>(out var collider))
            {
                if (!collider.isTrigger)
                {
                    Debug.LogWarning("Death Zone: The attached collider component is not a trigger. Please enable 'Is Trigger'.", this);
                }
            }
            else
            {
                // Automatically add a sphere trigger if no collider has already been added
                gameObject.AddComponent<SphereCollider>().isTrigger = true;
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
    }
}