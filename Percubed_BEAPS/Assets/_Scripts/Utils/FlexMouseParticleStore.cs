using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    public class FlexMouseParticleStore : MonoBehaviour
    {
        private FlexSoftActor m_actor;

        public FlexMouseDrag mouse_Particle;

        [HideInInspector]
        public bool picked;

        [HideInInspector]
        public int pMouseParticleID;

        [HideInInspector]
        public Vector3 pMouseParticlePos;

        private Vector4[] m_particles;

        private Vector3[] m_velocities;

        // Start is called before the first frame update
        public void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
        }

        private void Start()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_particles.Length];
        }

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            
            int tmp_part_id = -1;
            if (Input.GetMouseButtonUp(0))
            {
                if (mouse_Particle.m_mouseParticle != -1)
                {
                    // we got the end of a mouse drag with a particle selected,
                    // notify whoever needs to know!

                    picked = true;
                    pMouseParticleID = mouse_Particle.m_mouseParticle;
                    pMouseParticlePos = mouse_Particle.m_mousePos;

                    //TODO: move the following to a class that is appropriately named

                    //print(m_mouseParticle);
                    //print(m_mousePos);
                    //this.GetComponent<CreateBehavior>().behavior.dictionary.Add(m_mouseParticle, mousePos);
                    //TODO: check if the labeled behavior already contains a behavior and append to that behavior
                    //SerializableMap<int, Vector3> tempIVD = new SerializableMap<int, Vector3>();
                    //tempIVD.Add(m_mouseParticle, m_mousePos);
                    //this.GetComponent<CreateBehavior>().labeledBehavior.Add(this.GetComponent<CreateBehavior>().behaviorName.text, tempIVD);

                    // remember particle id, since we need to undo parent's setting it back to non-zero mass:
                    tmp_part_id = mouse_Particle.m_mouseParticle;
                }
            }
            //base.OnFlexUpdate(_particleData);
            if (tmp_part_id != -1)
            {
                //m_particles[tmp_part_id].w = 0.0f;
                m_actor.asset.FixedParticle(mouse_Particle.m_mouseParticle, true);
                m_velocities[tmp_part_id] = new Vector3();
            }
        }
    }
}
