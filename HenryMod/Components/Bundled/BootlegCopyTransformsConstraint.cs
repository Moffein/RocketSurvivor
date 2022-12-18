using UnityEngine;

public class BootlegCopyTransformsConstraint : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform shovel;
    
    [SerializeField]
    private Transform shovelHand;

    [SerializeField]
    private string animatorParameter;
    
    bool holdingShovel = false;

    void FixedUpdate() {
        if (holdingShovel) {
            if (animator.GetFloat(animatorParameter) < 1) {
                holdingShovel = false;
            }
        }
    }

    void LateUpdate() {
        if (holdingShovel) {
            shovel.position = shovelHand.position;
            shovel.rotation = shovelHand.rotation;
        }
    }

    public void Reparent() {
        holdingShovel = true;
    }
}
