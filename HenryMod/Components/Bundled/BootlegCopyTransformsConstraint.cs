using UnityEngine;

public class BootlegCopyTransformsConstraint : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform shovel;
    
    [SerializeField]
    private Transform shovelHand;
    
    bool holdingShovel = false;

    void FixedUpdate() {
        if (holdingShovel) {
            if (animator.GetFloat("ShovelParent") < 1) {
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

    public void HoldShovel() {
        holdingShovel = true;
    }
}
