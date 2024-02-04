using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// An implementation of a PhysicsComponent for 2D physics.
    /// </summary>
    public sealed class PhysicsComponent2D : PhysicsComponent
    {
        /// <summary>
        /// Gets the Rigidbody associated with this object.
        /// </summary>
        public Rigidbody2D Rigidbody { get; private set; } = null;

        /// <summary>
        /// Gets an array with all the colliders associated with this object.
        /// </summary>
        public Collider2D[] Colliders { get; private set; } = null;

        ContactPoint2D[] contactsBuffer = new ContactPoint2D[10];
        RaycastHit2D[] hitsBuffer = new RaycastHit2D[10];
        Collider2D[] overlapsBuffer = new Collider2D[10];

        protected override void Awake()
        {
            base.Awake();
            Colliders = GetComponentsInChildren<Collider2D>();
        }

        protected override void Start()
        {
            base.Start();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (ignoreCollisionMessages)
                return;

            if (!other.isTrigger)
                return;

            bool found = false;
            float fixedTime = Time.fixedTime;

            // Check if the trigger is contained inside the list
            for (int i = 0; i < Triggers.Count && !found; i++)
            {
                if (Triggers[i] != other)
                    continue;

                found = true;
                var trigger = Triggers[i];
                trigger.Update(fixedTime);
                Triggers[i] = trigger;
            }

            if (!found)
                Triggers.Add(new Trigger(other, Time.fixedTime));
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (ignoreCollisionMessages)
                return;

            for (int i = Triggers.Count - 1; i >= 0; i--)
            {
                if (Triggers[i].collider2D == other)
                {
                    Triggers.RemoveAt(i);
                    break;
                }
            }
        }



        void OnCollisionEnter2D(Collision2D collision)
        {
            if (ignoreCollisionMessages)
                return;


            int bufferHits = collision.GetContacts(contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint2D contact = contactsBuffer[i];

                Contact outputContact = new Contact(true, contact, collision);

                Contacts.Add(outputContact);
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (ignoreCollisionMessages)
                return;


            int bufferHits = collision.GetContacts(contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint2D contact = contactsBuffer[i];

                Contact outputContact = new Contact(false, contact, collision);

                Contacts.Add(outputContact);
            }
        }


        protected override LayerMask GetCollisionLayerMask()
        {
            int objectLayer = gameObject.layer;
            LayerMask output = 0;

            for (int i = 0; i < 32; i++)
            {
                bool exist = !Physics2D.GetIgnoreLayerCollision(i, objectLayer);

                if (exist)
                    output.value |= 1 << i;
            }

            return output;
        }

        public override void IgnoreCollision(in HitInfo hitInfo, bool ignore)
        {
            if (hitInfo.collider2D == null)
                return;

            for (int i = 0; i < Colliders.Length; i++)
                Physics2D.IgnoreCollision(Colliders[i], hitInfo.collider2D, ignore);
        }

        public override void IgnoreCollision(Transform otherTransform, bool ignore)
        {
            if (otherTransform == null)
                return;

            bool found = otherTransform.TryGetComponent(out Collider2D collider);
            if (!found)
                return;

            for (int i = 0; i < Colliders.Length; i++)
                Physics2D.IgnoreCollision(Colliders[i], collider, ignore);
        }

        public void IgnoreCollision(Collider2D collider, bool ignore)
        {
            if (collider == null)
                return;

            for (int i = 0; i < Colliders.Length; i++)
                Physics2D.IgnoreCollision(Colliders[i], collider, ignore);
        }

        public void IgnoreCollision(Collider2D collider, bool ignore, int layerMask)
        {
            if (collider == null)
                return;

            if (!CustomUtilities.BelongsToLayerMask(collider.gameObject.layer, layerMask))
                return;

            for (int i = 0; i < Colliders.Length; i++)
                Physics2D.IgnoreCollision(Colliders[i], collider, ignore);
        }
        

        public override void IgnoreLayerCollision(int targetLayer, bool ignore)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, targetLayer, ignore);

            if (ignore)
                CollisionLayerMask &= ~(1 << targetLayer);
            else
                CollisionLayerMask |= (1 << targetLayer);
        }

        public override void IgnoreLayerMaskCollision(LayerMask layerMask, bool ignore)
        {

            int layerMaskValue = layerMask.value;
            int currentLayer = 1;

            for (int i = 0; i < 32; i++)
            {
                bool exist = (layerMaskValue & currentLayer) > 0;

                if (exist)
                    IgnoreLayerCollision(i, ignore);

                currentLayer <<= 1;
            }

            if (ignore)
                CollisionLayerMask &= ~(layerMask.value);
            else
                CollisionLayerMask |= (layerMask.value);
        }

        public override bool SimpleRaycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = !filter.ignoreTriggers;

            hitsBuffer[0] = Physics2D.Raycast(
                origin,
                Vector3.Normalize(castDisplacement),
                castDisplacement.magnitude,
                filter.collisionLayerMask
            );

            Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

            bool hit = hitsBuffer[0].collider != null;

            if (hit)
                GetClosestHit(out hitInfo, 1, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hitInfo.hit;
        }

        public override int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, in HitInfoFilter filter)
        {

            bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = !filter.ignoreTriggers;

            var hits = Physics2D.RaycastNonAlloc(
                origin,
                Vector3.Normalize(castDisplacement),
                hitsBuffer,
                castDisplacement.magnitude,
                filter.collisionLayerMask
            );

            Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        public override int CapsuleCast(out HitInfo hitInfo, Vector3 bottom, Vector3 top, float radius, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            Vector3 bottomToTop = top - bottom;
            Vector3 center = bottom + CustomUtilities.Multiply(bottomToTop, 0.5f);

            Vector2 size;
            size.x = 2f * radius;
            size.y = bottomToTop.magnitude + size.x;

            float castAngle = Vector2.SignedAngle(bottomToTop, Vector2.up);

            bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = !filter.ignoreTriggers;

            var hits = Physics2D.CapsuleCastNonAlloc(
                center,
                size,
                CapsuleDirection2D.Vertical,
                castAngle,
                Vector3.Normalize(castDisplacement),
                hitsBuffer,
                castDisplacement.magnitude,
                filter.collisionLayerMask
            );

            Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }


        public override int SphereCast(out HitInfo hitInfo, Vector3 center, float radius, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = !filter.ignoreTriggers;

            var hits = Physics2D.CircleCastNonAlloc(
                center,
                radius,
                Vector3.Normalize(castDisplacement),
                hitsBuffer,
                castDisplacement.magnitude,
                filter.collisionLayerMask
            );

            Physics2D.queriesHitTriggers = previousQueriesHitTriggers;


            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        public override int BoxCast(out HitInfo hitInfo, Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, in HitInfoFilter filter)
        {
            var hits = BoxCast(center, size, castDisplacement, orientation, in filter);

            if (hits != 0)
                GetClosestHit(out hitInfo, hits, castDisplacement, in filter);
            else
                hitInfo = new HitInfo();

            return hits;
        }

        public override int BoxCast(Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, in HitInfoFilter filter)
        {
            bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = !filter.ignoreTriggers;

            var hits = Physics2D.BoxCastNonAlloc(
                center,
                size,
                orientation.eulerAngles.z,
                Vector3.Normalize(castDisplacement),
                hitsBuffer,
                castDisplacement.magnitude,
                filter.collisionLayerMask
            );

            Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

            UpdateHitsBuffer(hits, castDisplacement);

            return hits;
        }

        // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        public override bool OverlapSphere(Vector3 center, float radius, in HitInfoFilter filter)
        {
            bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = !filter.ignoreTriggers;

            var overlaps = Physics2D.OverlapCircleNonAlloc(
                center,
                radius,
                overlapsBuffer,
                filter.collisionLayerMask
            );

            Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

            var filteredOverlaps = FilterOverlaps(overlaps, filter.ignorePhysicsLayerMask);
            return filteredOverlaps != 0;
        }

        public override bool OverlapCapsule(Vector3 bottom, Vector3 top, float radius, in HitInfoFilter filter)
        {
            Vector3 bottomToTop = top - bottom;
            Vector3 center = bottom + 0.5f * bottomToTop;
            Vector2 size = new Vector2(2f * radius, bottomToTop.magnitude + 2f * radius);

            float castAngle = Vector2.SignedAngle(bottomToTop, Vector2.up);

            bool previousQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = !filter.ignoreTriggers;

            var overlaps = Physics2D.OverlapCapsuleNonAlloc(
                center,
                size,
                CapsuleDirection2D.Vertical,
                castAngle,
                overlapsBuffer,
                filter.collisionLayerMask
            );

            Physics2D.queriesHitTriggers = previousQueriesHitTriggers;

            var filteredOverlaps = FilterOverlaps(overlaps, filter.ignorePhysicsLayerMask);

            return filteredOverlaps != 0;
        }

        // ---------------------------------------------------------------------------------------------------------------------------------

        protected override int FilterOverlaps(int overlaps, LayerMask ignoredLayerMask)
        {
            int filteredOverlaps = overlaps;
            for (int i = 0; i < overlaps; i++)
            {
                Collider2D hitCollider = overlapsBuffer[i];

                if (hitCollider.transform.IsChildOf(transform))
                {
                    filteredOverlaps--;
                    continue;
                }

                IgnoreCollision(hitCollider, true, ignoredLayerMask);                
            }

            return filteredOverlaps;
        }
        
        void UpdateHitsBuffer(int hits, Vector3 castDisplacement)
        {
            for (int i = 0; i < hits; i++)
            {
                RaycastHit2D raycastHit = hitsBuffer[i];
                float raycastHitDistance = raycastHit.distance;
                Collider2D raycastHitCollider = raycastHit.collider;
                int raycastHitLayer = raycastHit.transform.gameObject.layer;

                HitsBuffer[i] = new HitInfo(ref raycastHit, Vector3.Normalize(castDisplacement));
            }
        }


        protected override void GetClosestHit(out HitInfo hitInfo, int hits, Vector3 castDisplacement, in HitInfoFilter filter)
        {
            RaycastHit2D closestRaycastHit = new RaycastHit2D();
            closestRaycastHit.distance = Mathf.Infinity;

            bool hit = false;

            for (int i = 0; i < hits; i++)
            {
                RaycastHit2D raycastHit = hitsBuffer[i];
                float raycastHitDistance = raycastHit.distance;
                Collider2D raycastHitCollider = raycastHit.collider;
                int raycastHitLayer = raycastHit.transform.gameObject.layer;

                HitsBuffer[i] = new HitInfo(ref raycastHit, Vector3.Normalize(castDisplacement));

                if (raycastHitDistance == 0)
                    continue;

                bool continueSelf = false;
                for (int j = 0; j < Colliders.Length; j++)
                {
                    Collider2D thisCollider = Colliders[j];

                    if (raycastHitCollider == thisCollider)
                        continueSelf = true;

                    if (CustomUtilities.BelongsToLayerMask(raycastHitLayer, filter.ignorePhysicsLayerMask))
                        Physics2D.IgnoreCollision(thisCollider, raycastHitCollider, true);
                }

                if (continueSelf)
                    continue;

                if (raycastHitDistance < filter.minimumDistance || raycastHitDistance > filter.maximumDistance)
                    continue;

                if (filter.ignoreRigidbodies && raycastHitCollider.attachedRigidbody != null)
                    continue;

                hit = true;

                if (raycastHitDistance < closestRaycastHit.distance)
                    closestRaycastHit = raycastHit;

            }

            if (hit)
                hitInfo = new HitInfo(ref closestRaycastHit, Vector3.Normalize(castDisplacement));
            else
                hitInfo = new HitInfo();

        }


    }

}
