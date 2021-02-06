using UnityEngine;
using NVIDIA.Flex;
using System;
using System.Collections;

namespace Percubed.Flex
{
    /**
     * Lock particles in the flex object based on any Colliders on the same GameObject.
     * Note that FixedParticles of the flex asset are persistently stored!
     * So they will survive a restart, so we do need to reset them initially,
     * and then set/reset when the Colliders change or are activated/deactivated (by calling ReLock).
     */
    [RequireComponent(typeof(FlexActor))]
    [RequireComponent(typeof(BoxCollider))]
    public class FlexCollidersLock : MonoBehaviour
    {
        FlexSoftActor m_actor;

        private Vector4[] m_particles; // local cache

        private BoxCollider myCol;
        // cache collider values to be able to detect changes:
        private Vector3 myCol_center;
        private Vector3 myCol_size;
        private bool myCol_enabled;
        private const float CHECK_INTERVAL = 0.5f; // seconds

        void Awake()
        {
            myCol = GetComponent<BoxCollider>();
            m_actor = GetComponent<FlexSoftActor>();
        }

        void Start()
        {
            myCol_center = myCol.center;
            myCol_size = myCol.size;
            myCol_enabled = myCol.enabled;
            m_particles = new Vector4[m_actor.indexCount];
            m_actor.onFlexUpdate += OnFlexUpdate;
            StartCoroutine(CheckForColliderChange());
        }

        /**
         * Call this method after moving/adding/enabling/disabling colliders, to trigger re-locking.
         */
        void ReLock()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
        }

        /**
         * This will only run once by unregistering itself (until ReLock is called).
         */
        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            m_actor.onFlexUpdate -= OnFlexUpdate; // only run once!
            // rather then calling rebuild on the asset (which would trigger a recreate of the whole actor instance)
            // change the fixed particles directly. See also FlexActor.MoveFixedParticles
            var prev_Fixed = m_actor.asset.fixedParticles;
            m_actor.asset.ClearFixedParticles();
            //Debug.Log("Pre-loop. cleared particles, but kept copy of length: " + prev_Fixed.Length);
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            // find all particles that are inside one of the colliders, and add it to fixedParticles:
            Collider[] to_be_locked_colls = GetComponents<Collider>();
            float particleRadius = m_actor.asset.particleSpacing; // not sure if this is the radius or the diameter
            for (int i = 0; i < m_particles.Length; i++)
            {
                bool previouslyLocked = Array.IndexOf<int>(prev_Fixed, i) > -1;
                bool fixIt = false;
                var particle = m_particles[i];
                Collider[] overlapped_colls = Physics.OverlapSphere(particle, particleRadius);
                foreach (Collider ovrlp_c in overlapped_colls)
                {
                    if (Array.IndexOf<Collider>(to_be_locked_colls, ovrlp_c) > -1) {
                        fixIt = true;
                        break; // no need to check other colliders for this particle now
                    }
                }
                if (fixIt && !previouslyLocked)
                {
                    //Debug.Log("Changing particle to fixed: " + particle);
                    m_actor.asset.FixedParticle(i, true);
                    particle.w = 0f;
                    m_particles[i] = particle;
                }
                if (!fixIt && (previouslyLocked || particle.w == 0f)) // reset weight even if particle was not locked!
                {
                    //Debug.Log("Changing particle back to not fixed: " + particle);
                    particle.w = 1f / m_actor.massScale;
                    m_particles[i] = particle;
                }
            }
            _particleData.SetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
        }

        // check for a change in the local BoxCollider every so often
        private IEnumerator CheckForColliderChange()
        {
            while (Application.isPlaying)
            {
                yield return new WaitForSeconds(CHECK_INTERVAL);
                if (myCol.center != myCol_center || myCol.size != myCol_size || myCol.enabled != myCol_enabled)
                {
                    myCol_center = myCol.center;
                    myCol_size = myCol.size;
                    myCol_enabled = myCol.enabled;
                    ReLock();
                }
            }
        }
    }
}
