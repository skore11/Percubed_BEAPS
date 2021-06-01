using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    public class Flex_Adhesion : MonoBehaviour
    {
        //public bool ahere;

        private GameObject other_collider;

        private FlexActor other_actor;

        private FlexSoftActor m_actor;

        private Vector4[] other_particles;

        private Vector3[] other_velocities;

        private Vector4[] m_particles;

        private Vector3[] m_velocities;

        private bool adhere;

        public void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
            // local caches used during the update of particle positions based on the reference animation:
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_particles.Length];
        }

        // Update is called once per frame
        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
            //Register a contact from Store_Actors script between intearcting actors and there constituent particles:
            //Probably dont need to check for collision every frame but every 5 or 10,
            //Will need an IEnumnerator and Coroutine.
            //If a collision with another flex actor's particles exists, set adhere to true:
            //if (adhere || other_collider != null)

            if (!adhere)
            {
                check_Distance(this.gameObject);
            }
            //check_Distance(this.gameObject);
            if (adhere)
                {
                other_actor = other_collider.GetComponent<FlexActor>();
                other_particles = new Vector4[other_actor.indexCount];
                other_velocities = new Vector3[other_actor.indexCount];
                _particleData.GetParticles(other_actor.indices[0], other_actor.indexCount, other_particles);
                _particleData.GetVelocities(other_actor.indices[0], other_actor.indexCount, other_velocities);
                //Find the nearest the particles to this actor's particles and set velocity to 
                //that particle
                //Set stuck adhering/sticking particles
                for (int pId = 0; pId < m_particles.Length; pId++)
                {
                    for (int pId2 = 0; pId2 < other_particles.Length; pId2++)
                    {
                        Vector3 a = new Vector3(m_particles[pId].x, m_particles[pId].y, m_particles[pId].z);
                        Vector3 b = new Vector3(other_particles[pId2].x, other_particles[pId2].y, other_particles[pId2].z);
                        if (Vector3.Distance(a, b) < 1.0f)
                        {
                            //m_velocities[pId] = other_velocities[pId2];
                            print("found a particle: " + b);
                            _particleData.SetVelocity(m_actor.indices[pId2], other_velocities[pId2]);
                        }
                    }
                }
                //_particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);           
                //adhere = false;
                
            }
            else
            {
                print("nothing to collide");
            }


            //if adhere is turned off, clear adhering particles
        }

        void check_Distance(GameObject a)
        {
            Transform[] all_hits = FindObjectsOfType<Transform>();
            foreach (var hit in all_hits)
            {
                if (Vector3.Distance(a.transform.position, hit.position) <= 5.0f)
                {
                    print("collided with object: " + hit.name);
                    adhere = true;
                    other_collider = hit.gameObject;
                }
            }

        }

        //void OnCollisionEnter(Collision collision)
        //{
        //    adhere = true;
        //    other_collider = collision.collider.gameObject;
        //}


    }
}
