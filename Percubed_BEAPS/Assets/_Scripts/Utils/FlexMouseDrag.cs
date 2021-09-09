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
    //[RequireComponent(typeof(FlexActor))]
    public class FlexMouseDrag : MonoBehaviour
    {
        FlexSoftActor m_actor;

        private Vector4[] m_particles; // local cache of positions

        private Vector3[] m_velocities; //local cache of velocities

        public int m_mouseParticle = -1; // made public to allow use in subclass! --strank

        private float m_mouseMass = 0;

        private float m_mouseT = 0.0f;

        public Vector3 m_mousePos = new Vector3(); // made public to allow use in subclass! --strank

        float particleRadius;

        public bool mouse_particle;

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

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);


            if (Input.GetMouseButtonDown(0))
            {
                print("mousedown");
                //Vector3 worldPos = Camera.ScreenToWorldPoint(mouse.position);

                StartCoroutine(PickParticle());
               
            }


            if (Input.GetMouseButton(0) && m_mouseParticle != -1)
            {
                //    Flex.GetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);
                //    Flex.GetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Debug.DrawLine(ray.origin, ray.direction, Color.red);
                m_mousePos = ray.origin + ray.direction * m_mouseT;
                //m_mousePos = this.transform.InverseTransformPoint(m_mousePos);
                Vector3 pos = m_particles[m_mouseParticle];
                //pos = this.transform.TransformPoint(pos);

                Vector3 p = Vector3.Lerp(pos, m_mousePos, 0.8f);
                //Debug.DrawLine(pos, ray.origin, Color.blue);
                Vector3 delta = p - pos;
                //print(p);
                m_particles[m_mouseParticle] = p;

                //m_actor.asset.FixedParticle(m_mouseParticle, true);
                m_velocities[m_mouseParticle] = delta/Time.deltaTime;
                //_particleData.SetVelocity(m_mouseParticle, new Vector3 (m_particles[m_mouseParticle].x, m_particles[m_mouseParticle].y, m_particles[m_mouseParticle].z) );

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

            _particleData.SetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            _particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);

        }

        IEnumerator PickParticle()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            print("ray origin" + ray.origin);
            print("ray direction" + ray.direction);
            m_mouseParticle = PickedParticle(ray.origin, ray.direction, m_particles, m_particles.Length, particleRadius * 0.8f, ref m_mouseT);

            if (m_mouseParticle != -1)
            {
                Debug.Log("picked: " + m_mouseParticle);

                m_mousePos = ray.origin + ray.direction * m_mouseT;
                //m_mousePos = this.transform.TransformPoint(m_mousePos);
                print("mouse picked particle pos:" + " x: " + m_mousePos.x + " y: " + m_mousePos.y + " z: " + m_mousePos.z);
                m_mouseMass = m_particles[m_mouseParticle].w;
                //print("mouse particle mass:" + m_mouseMass);
                //m_actor.asset.FixedParticle(m_mouseParticle, true);
                m_particles[m_mouseParticle].w = 0.0f;

            }

            yield return new WaitForSeconds(1.0f);
        }

        // finds the closest particle to a view ray
        int PickedParticle(Vector3 origin, Vector3 dir, Vector4[] particles, int n, float radius, ref float t)
        {
            print("trying to pick particle");
            float maxDistSq = radius;
            float minT = float.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < n; ++i)
            {

                Vector3 p = new Vector3(particles[i].x, particles[i].y, particles[i].z);
                Vector3 delta = p - origin;

                //float deltaSq = delta.sqrMagnitude;
                //if (deltaSq < minT)
                //{
                //    minT = deltaSq;
                //    minIndex = i;
                //}

                float tt = Vector3.Dot(delta, dir);

                //Vector3 cross = Vector3.Cross(delta, dir);

                //float cr = cross.sqrMagnitude;

                //if (cr < maxDistSq )
                //{
                //    //minT = tt;
                //    minIndex = i;
                //}

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
