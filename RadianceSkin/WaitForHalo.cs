using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RadianceSkin
{
    internal class WaitForHalo:MonoBehaviour
    {
        GameObject Halo;
        bool found;
        private void Update()
        {
            if (found) return;
            Halo = GameObject.Find("Halo");
            if( Halo != null ) { found = true; Halo.GetComponent<SpriteRenderer>().sprite = RadianceSkin.MakeSprite(RadianceSkin.Instance.skinList[RadianceSkin.Instance.set.skinID].skin9, Halo.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit); }
        }
    }
}