using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnableMorph : MonoBehaviour
{
    public SkinnedMorphTargets sm_target;
    public ExposeBakedMesh morphFrom;
    public ExposeBakedMesh morphTo;

    public InputAction morphAction;

    public void Start()
    {
        morphAction.performed += context => ChangeMorph();
        morphAction.Enable();
    }

    void ChangeMorph()
    {
        // on key press, turn on/off morphtarget script after setting the from and to meshes:
        if (sm_target.morphEnabled)
        {
            sm_target.morphEnabled = false;
        }
        else
        {
            sm_target.SetupMorph(morphFrom.gameObject, morphTo.bakedMesh);
            sm_target.morphEnabled = true;
        }
    }
}
