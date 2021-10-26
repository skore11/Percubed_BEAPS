using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;
using Percubed.Flex;

//Raycast into the particles lattice, find the nearest shape center
//Highlight it on the screen
//Find all vertices nearest to each of the shapes selected using the neighborindex highlighter
public class MeshBonesSelect : MonoBehaviour
{
    //[SerializeField]
    Vector3 selectedBone;

    bool done = false;

    //public FlexAnimSoftSkin flexAnim;

    public FlexSoftActor m_softActor;

    private Vector4[] m_particles; // local cache of positions

    private Vector3[] m_velocities; //local cache of velocities

    Vector3[] shapeBones;

    Transform[] bones;

    int pickedParticle = -1;

    Vector3 pickParticleVector;

    float particleRadius;

    private float m_mouseT = 0.0f;

    void Awake()
    {
        m_softActor = GetComponent<FlexSoftActor>();
    }
    void Start()
    {
        m_particles = new Vector4[m_softActor.indexCount];
        m_velocities = new Vector3[m_softActor.indexCount];
        shapeBones = m_softActor.asset.shapeCenters;
        bones = this.GetComponent<SkinnedMeshRenderer>().bones;
        m_softActor.onFlexUpdate += OnFlexUpdate;
    }
    void OnFlexUpdate(FlexContainer.ParticleData _particleData)
    {
        _particleData.GetParticles(m_softActor.indices[0], m_softActor.indexCount, m_particles);
        _particleData.GetVelocities(m_softActor.indices[0], m_softActor.indexCount, m_velocities);

        if (Input.GetMouseButtonDown(0))
        {
            print("mousedown");
            //Vector3 worldPos = Camera.ScreenToWorldPoint(mouse.position);

            StartCoroutine(PickParticle());
            
            done = true;
            StartCoroutine(PickShapeCenter());

            
        }
        
        print("selectedBone's position every frame: " + selectedBone.x + "," + selectedBone.y + "," + selectedBone.z);
        
        if (Input.GetMouseButtonUp(0) && done)
        {
            GameObject sBone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            print("mouse up!");
            sBone.transform.position = selectedBone;
            sBone.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            if (pickedParticle != -1)
            {
                //pickedParticle = -1;
                //selectedBone = Vector3.zero;
                //pickParticleVector = Vector3.zero;
            }
            done = false;
        }
        //_particleData.SetParticles(m_softActor.indices[0], m_softActor.indexCount, m_particles);
        //_particleData.SetVelocities(m_softActor.indices[0], m_softActor.indexCount, m_velocities);
    }
        IEnumerator PickParticle()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            print("ray origin" + ray.origin);
            print("ray direction" + ray.direction);
            pickedParticle = PickedParticle(ray.origin, ray.direction, m_particles, m_particles.Length, particleRadius * 0.8f, ref m_mouseT);
            print("index of picked particle: " + pickedParticle);
            pickParticleVector.x = m_particles[pickedParticle].x;
            pickParticleVector.x = m_particles[pickedParticle].y;
            pickParticleVector.x = m_particles[pickedParticle].z;
            print("picked particle vector: " + pickParticleVector.x + "," + pickParticleVector.y + "," + pickParticleVector.z + ",");
            yield return new WaitForSeconds(1.0f);
        
        }

    IEnumerator PickShapeCenter()
    {
        print("shape pick , pikparticleVector " + pickParticleVector.x + " , " + pickParticleVector.y + " , " + pickParticleVector.z);
        if (pickParticleVector != Vector3.zero)
        {
            print("pick particle not zero");
            float minDist = Mathf.Infinity;
            for (int i = 0; i < bones.Length; ++i)
            {
                float dist = Vector3.Distance(pickParticleVector, bones[i].position);
                
                if (dist < minDist)
                {
                    print("index of selected bone: " + i);
                    minDist = dist;
                    //selectedBone = this.GetComponent<SkinnedMeshRenderer>().transform.InverseTransformPoint(m_softActor.transform.TransformPoint(shapeBones[i]));
                    selectedBone = bones[i].position;
                    print("selectedBone's position on mouse select: " + bones[i].transform.position.x + "," + bones[i].transform.position.y + "," + bones[i].transform.position.z);

                }
                else { continue; }
            }
            print("Closest shape center/soft body bone for this particle: x - " + selectedBone.x + " y - " + selectedBone.y + " z - " + selectedBone.z);
        }
        
        yield return new WaitForSeconds(2.0f);
    }

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

