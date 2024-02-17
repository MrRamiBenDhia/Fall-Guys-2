using UnityEngine;
using UnityEngine.Events;

namespace Ilumisoft.HealthSystem
{
    /// <summary>
    /// Invokes a list of callbacks when the health of a GameObject gets changed.
    /// </summary>
    [AddComponentMenu("Health System/Events/On Health Changed")]
    public class HealthChangedListener : HealthEventListener
    {
        [Tooltip("List of callbacks being invoked when the event gets invoked")]
        public UnityEvent<float> Callbacks = new();

        private void OnEnable()
        {
            if (Health != null)
            {
                Health.OnHealthChanged += Callbacks.Invoke;
            }
        }

        private void OnDisable()
        {
            if (Health != null)
            {
                Health.OnHealthChanged -= Callbacks.Invoke;
            }
        }
    }
}