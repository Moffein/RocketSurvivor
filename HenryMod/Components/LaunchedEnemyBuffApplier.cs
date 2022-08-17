using UnityEngine;
using RoR2;

namespace RocketSurvivor.Components
{
    public class LaunchedEnemyBuffApplier : MonoBehaviour
    {
        public CharacterBody body;
        public CharacterMotor characterMotor;
        private float stopwatch;

        public static float initialGracePeriod = 0.3f;
        public static float flyingDebuffDuration = 5f;

        public void Awake()
        {
            stopwatch = 0f;

            if (!body)
            {
                body = base.GetComponent<CharacterBody>();
            }

            if (body && !body.HasBuff(Buffs.AirshotVulnerableDebuff))
            {

                if (!characterMotor)
                {
                    characterMotor = body.characterMotor;
                }

                body.AddBuff(Buffs.AirshotVulnerableDebuff);
            }
            else
            {
                Destroy(this);
            }
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;

            if (characterMotor)
            {
                if (characterMotor.isGrounded)
                {
                    if (stopwatch >= initialGracePeriod)
                    {
                        Destroy(this);
                        return;
                    }
                }
                else
                {
                    if (characterMotor.isFlying || (body && (body.isFlying)))
                    {
                        if (stopwatch >= flyingDebuffDuration)
                        {
                            Destroy(this);
                            return;
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
            if (body)
            {
                body.RemoveBuff(Buffs.AirshotVulnerableDebuff);
            }
        }
    }
}
