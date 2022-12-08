using RocketSurvivor.Components.Projectile;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RocketSurvivor.Components
{
    public class NetworkedBodyBlastJumpHandler : NetworkBehaviour
    {
        public CharacterBody characterBody;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }

        [ClientRpc]
        public void RpcBlastJump(Vector3 position, float radius, float force, float horizontalMultiplier, bool requireAirborne)
        {
            if (base.hasAuthority && characterBody && characterBody.characterMotor)
            {
                CharacterMotor cm = characterBody.characterMotor;
                if (cm)
                {
                    HealthComponent healthComponent = characterBody.healthComponent;
                    if (healthComponent && healthComponent.body && healthComponent.body.characterMotor && (!requireAirborne || !healthComponent.body.characterMotor.isGrounded))
                    {
                        Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
                        for (int i = 0; i < array.Length; i++)
                        {
                            HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                            if (hurtBox)
                            {
                                HealthComponent hc = hurtBox.healthComponent;
                                if (hc == healthComponent)
                                {
                                    Vector3 dist = characterBody.corePosition + BlastJumpComponent.bodyPositionOffset - position;

                                    Vector3 finalForce = dist.normalized * force;

                                    //Attempt to break your fall, should help with pogos
                                    if (cm.velocity.y < 0f)
                                    {
                                        cm.velocity.y = 0f;

                                        //Not much use to downwards launch, just results in accidental craters
                                        if (finalForce.y < 0f) finalForce.y = 0f;
                                    }

                                    finalForce.x *= horizontalMultiplier;
                                    finalForce.z *= horizontalMultiplier;

                                    cm.ApplyForce(finalForce, true, false);

                                    CmdAddRocketJumpBuff();

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command]
        public void CmdAddRocketJumpBuff()
        {
            if (characterBody && !characterBody.HasBuff(Buffs.RocketJumpSpeedBuff))
            {
                characterBody.AddBuff(Buffs.RocketJumpSpeedBuff);
            }
        }

        [Command]
        public void CmdRemoveRocketJumpBuff()
        {
            if (characterBody && characterBody.HasBuff(Buffs.RocketJumpSpeedBuff))
            {
                characterBody.RemoveBuff(Buffs.RocketJumpSpeedBuff);
            }
        }
    }
}
