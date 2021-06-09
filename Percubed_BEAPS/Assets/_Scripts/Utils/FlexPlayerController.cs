using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using NVIDIA.Flex;
using System;

namespace Percubed.Flex {

    public class FlexPlayerController : MonoBehaviour
    {
        public float speed = 1.0f;
        
        private Vector3 movement = Vector3.zero;

        private FlexSoftActor m_actor;

        private Vector4[] m_particles;

        private Vector3[] m_velocities;

        Vector3 displacement_vectors;

        private void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
        }

        private void Start()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_particles.Length];
        }

        public void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);

            if (Input.GetKey(KeyCode.W))
            {
                for (int i = 0; i < m_velocities.Length; i++)
                {
                    m_velocities[i] += Vector3.forward * m_particles[i].w * speed;
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                for (int i = 0; i < m_velocities.Length; i++)
                {
                    m_velocities[i] += Vector3.left * m_particles[i].w * speed;
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                for (int i = 0; i < m_velocities.Length; i++)
                {
                    m_velocities[i] += Vector3.back * m_particles[i].w * speed;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                for (int i = 0; i < m_velocities.Length; i++)
                {
                    m_velocities[i] += Vector3.right * m_particles[i].w * speed;
                }
            }
            if (Input.GetKey(KeyCode.Q))
            {
                for (int i = 0; i < m_velocities.Length; i++)
                {
                    m_velocities[i] += Vector3.up * m_particles[i].w * speed;
                }
            }
            if (Input.GetKey(KeyCode.E))
            {
                for (int i = 0; i < m_velocities.Length; i++)
                {
                    m_velocities[i] += Vector3.down * m_particles[i].w * speed;
                }
            }
            _particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
            //throw new NotImplementedException();
        }

        //void Update()
        //{
        //    // Replace with corresponding setup for new Input package:
        //    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html
        //    //            float moveHorizontal = Input.GetAxis("Horizontal");
        //    //            float moveVertical = Input.GetAxis("Vertical");
        //    //            float moveUpDown = Input.GetAxis("UpandDown");
        //    //            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
        //    if (movement != Vector3.zero)
        //    {
        //        this.transform.Translate(movement);
        //    }
        //}

        /// <summary>
        /// Callback for default defined move inputaction, i.e. a 2D move
        /// If we want a 3D move again, we need to add input device triggers
        /// to that move definition for up and down
        /// and turn it from Vector2 to a Vector3 action
        /// 
        /// The input system sends one started, one performed and one cancelled
        /// version of this event. On cancel the returned vector is zero.
        /// So we can just directly save the vector and transform in update.
        /// </summary>
        //public void MoveCB(InputAction.CallbackContext context)
        //{
        //    //Debug.Log("MoveCB context: " + context);
        //    Vector2 input = context.ReadValue<Vector2>();
        //    movement.x = speed * input.x;
        //    movement.z = speed * input.y;
        //}
    }
}