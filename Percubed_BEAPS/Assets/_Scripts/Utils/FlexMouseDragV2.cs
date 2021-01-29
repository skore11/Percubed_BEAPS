using System.Collections;
using System.Collections.Generic;
using NVIDIA.Flex;
using UnityEngine;
using UnityEngine.InputSystem;

//I have changed the input system to account for both the old and new input types, this is so we can use Input.mousePosition without worrying about 2D, 3D conversion
namespace Percubed.Flex

{
    /// <summary>
    /// Drag particles using mouse
    /// </summary>
    [RequireComponent(typeof(FlexActor))]
    public class FlexMouseDragV2 : MonoBehaviour
    {
        FlexSoftActor m_actor;

        private Vector4[] m_particles; // local cache of positions

        private Vector3[] m_velocities; //local cache of velocities

        public int m_mouseParticle = -1; // made public to allow use in subclass! --strank

        private float m_mouseMass = 0;

        private float m_mouseT = 0;

        public Vector3 m_mousePos = new Vector3(); // made public to allow use in subclass! --strank

        float particleRadius;

        void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
            particleRadius = m_actor.asset.particleSpacing;
        }

        void Start()
        {
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_actor.indexCount]; 
            m_actor.onFlexUpdate += OnFlexUpdate;
        }

        public void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            var mouse= Mouse.current;
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
            if (Input.GetMouseButtonDown(0))
            {
                //Vector3 worldPos = Camera.ScreenToWorldPoint(mouse.position);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                m_mouseParticle = PickParticle(ray.origin, ray.direction, m_particles, m_particles.Length, particleRadius * 0.8f, ref m_mouseT);

                if (m_mouseParticle != -1)
                {
                    Debug.Log("picked: " + m_mouseParticle);

                    m_mousePos = ray.origin + ray.direction * m_mouseT;
                    m_mouseMass = m_particles[m_mouseParticle].w;
                    //print("mouse particle mass:" + m_mouseMass);
                    //m_actor.asset.FixedParticle(m_mouseParticle, true);
                    m_particles[m_mouseParticle].w = 0.0f;

                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_mouseParticle != -1)
                {
                    m_particles[m_mouseParticle].w = m_mouseMass;
                    m_mouseParticle = -1;
                    //m_actor.asset.ClearFixedParticles();
                }
            }

            if (m_mouseParticle != -1)
            {
                //    Flex.GetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);
                //    Flex.GetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                m_mousePos = ray.origin + ray.direction * m_mouseT;

                Vector3 pos = new Vector3(m_particles[m_mouseParticle].x, m_particles[m_mouseParticle].y, m_particles[m_mouseParticle].z);
                Vector3 p = Vector3.Lerp(pos, m_mousePos, 0.8f);
                Vector3 delta = p - pos;
                //print(p);
                m_particles[m_mouseParticle].x = p.x;
                m_particles[m_mouseParticle].y = p.y;
                m_particles[m_mouseParticle].z = p.z;
                //m_actor.asset.FixedParticle(m_mouseParticle, true);
                m_velocities[m_mouseParticle]= Input.mousePosition * m_particles[m_mouseParticle].w;
            }
            _particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
        }


        // finds the closest particle to a view ray
        int PickParticle(Vector3 origin, Vector3 dir, Vector4[] particles, int n, float radius, ref float t)
        {
            float maxDistSq = radius * radius;
            float minT = float.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < n; ++i)
            {

                Vector3 p = new Vector3(particles[i].x, particles[i].y, particles[i].z);
                Vector3 delta = p - origin;
                float tt = Vector3.Dot(delta, dir);

                if (tt > 0.0f)
                {
                    Vector3 perp = delta - tt * dir;

                    float dSq = perp.sqrMagnitude;

                    if (dSq < maxDistSq && tt < minT)
                    {
                        minT = tt;
                        minIndex = i;
                    }
                }
            }

            t = minT;

            return minIndex;
        }
    }
}
