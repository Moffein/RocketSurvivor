using UnityEngine;

public class AnimationPlaySmokeParticle : MonoBehaviour {
    [SerializeField]
    private Transform smokeBone;

    [SerializeField]
    private ParticleSystem smokeParticleSystem;

    public void Smoke() {
        //uncomment this to disable
        //smokeParticleSystem.Play();
    }
}
