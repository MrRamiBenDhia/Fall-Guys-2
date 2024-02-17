using UnityEngine;

namespace Ilumisoft.HealthSystem
{
    /// <summary>
    /// Abstract base class for event listeners of a health component.
    /// </summary>
    public abstract class HealthEventListener : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the health component we are listening to")]
        private HealthComponent health;

        public HealthComponent Health { get => health; set => health = value; }

        void Reset()
        {
            // Automatically try to assign the health component when the component is created or gets reset
            if (Health == null)
            {
                Health = GetComponentInParent<HealthComponent>();
            }
        }
    }
}