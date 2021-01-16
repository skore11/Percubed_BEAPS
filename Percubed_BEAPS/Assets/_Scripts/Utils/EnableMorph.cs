using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnableMorph : MonoBehaviour
{
    public SkinnedMorphTargets sm_target;
    public ExposeBakedMesh morphFrom;
    public ExposeBakedMesh morphTo;

    void FixedUpdate()
    {
        // on key press, enable morphtarget script after setting the from and to meshes:
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.mKey.isPressed)
            {
                this.enabled = false;
                // this OVERWRITES any settings made in the inspector!
                sm_target.originalMeshObjects = morphFrom.gameObject;
                sm_target.morphTargets = new SkinnedMorphTargets.MeshArray[1];
                sm_target.morphTargets[0] = new SkinnedMorphTargets.MeshArray();
                sm_target.morphTargets[0].name = "FLEXMorphTarget";
                sm_target.morphTargets[0].submeshes = new Mesh[1];
                sm_target.morphTargets[0].submeshes[0] = morphTo.bakedMesh;
                sm_target.enabled = true;
            }
        }
    }
}
