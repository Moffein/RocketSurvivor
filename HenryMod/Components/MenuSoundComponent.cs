using UnityEngine;
using RoR2;
using System.Collections;

namespace RocketSurvivor.Components
{
    public class MenuSoundComponent : MonoBehaviour
    {

        private void OnEnable()
        {
            base.StartCoroutine(this.MenuSound());
        }

        private IEnumerator MenuSound()
        {
            yield return new WaitForSeconds(8f / 30f);
            Util.PlaySound("Play_Moffein_RocketSurvivor_Shift_Rearm", base.gameObject);
            yield return new WaitForSeconds(1.5f - 8f / 30f);
            Util.PlaySound("Play_MULT_shift_hit", base.gameObject);
            yield return null;
        }
    }
}
