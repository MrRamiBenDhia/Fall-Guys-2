using UnityEngine;
using UnityEngine.Events;

namespace Ilumisoft.HealthSystem
{
    /// <summary>
    /// Invokes a list of callbacks when the health of a GameObject becomes empty.
    /// </summary>
    [AddComponentMenu("Health System/Events/On Health Empty")]
    public class HealthEmptyListener : HealthEventListener
    {
        [Tooltip("List of callbacks being invoked when the event gets invoked")]
        public UnityEvent Callbacks = new();
     
        private void OnEnable()
        {
            if (Health != null)
            {
                Health.OnHealthEmpty += Callbacks.Invoke;
            }
        }

        private void OnDisable()
        {
            if (Health != null)
            {
                Health.OnHealthEmpty -= Callbacks.Invoke;
            }
        }
    }
}