using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootlegCopyTransformsConstraint : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform shovel;

    [SerializeField]
    private Transform shovelHand;

    [SerializeField]
    private string animatorParameter;

    bool holdingShovel = true;

    public void Reparent () {

    }
}
