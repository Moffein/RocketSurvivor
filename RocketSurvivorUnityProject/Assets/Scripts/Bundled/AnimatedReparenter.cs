using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedReparenter : MonoBehaviour
{

    [SerializeField]
    private Transform reparentingChild;

    [SerializeField]
    private Transform[] parentTransforms;

    public void Reparent (int index = 0) {
        transform.SetParent(parentTransforms[index], false);
    }
}
