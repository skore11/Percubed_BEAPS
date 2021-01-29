using System.Collections;
using System.Collections.Generic;
using NVIDIA.Flex;
using UnityEngine;
using UnityEngine.InputSystem;
using TreeSharpPlus;

namespace Percubed.Flex
{

    public class MeltEffectFlex : MonoBehaviour//, IStorable
    {
        //private const float MAX_MELT_FACTOR = 9.8f;
        //public float MAX_ShOCK_FACTOR = 20f;
        private FlexSoftActor m_actor;
        // caches for values copied in and out of the flex subsystem:
        private Vector4[] m_particles;
        private Vector3[] m_velocities;

        public bool melt;

        public void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
        }

        private void Start()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
            // local caches used during the update of particle positions based on the reference animation:
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_particles.Length];
        }

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.mKey.isPressed || melt)
                {
                    for (int pId = 0; pId < m_particles.Length; pId++)
                    {
                        //m_particles[pId].x += (Random.value - 0.5f) ;
                        //m_particles[pId].y += (Random.value - 0.5f) ;
                        //m_particles[pId].z += (Random.value - 0.5f) ;
                        //m_velocities[pId].x = (Random.value - 0.5f) * MAX_ShOCK_FACTOR * m_particles[pId].w;
                        m_velocities[pId].y = -1000f * m_particles[pId].w;
                        //m_velocities[pId].z = (Random.value - 0.5f) * MAX_ShOCK_FACTOR * m_particles[pId].w;
                    }
                }
                //_particleData.SetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
                _particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
            }
        }

    }
}
