using Satchel;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RadianceSkin
{
    internal class test : MonoBehaviour
    {
        string[] toBeFound = { "Legs" };//"Shot Charge", "Shot Charge 2", "GG_crowd", "GodSeeker crowd", "GG_Arena_Prefab", "Pt Feather Burst", "Radiance Roar","Dream Impact", "dream_particle_03 (3)","Halo" };
        private static readonly string _dllFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private void Start()
        {
            StartCoroutine(FindThat());
        }
        private IEnumerator FindThat()
        {
            yield return new WaitForSeconds(3f);
            Scene scene = SceneUtils.getCurrentScene();
            var golist = scene.GetAllGameObjects();
            foreach (var g in golist)
            {
                RadianceSkin.Instance.Log(g.name);
                if (g.name.Contains("Pt") || g.name.Contains("Shot Charge") || g.name.Contains("dream_particle_03"))
                {
                    try
                    {

                        TextureUtils.WriteTextureToFile(g.GetComponent<ParticleSystemRenderer>().materials[0].mainTexture, Path.Combine(_dllFolder, g.name + ".png"));
                    }
                    catch { Modding.Logger.Log("ERRORRRRRRRRR"); }
                }
                if (g.name.Contains("Radiance Roar") || g.name == "Dream Impact")
                {
                    try
                    {
                        Modding.Logger.Log("Radiance Roar" + g.GetComponent<MeshRenderer>().materials.Length);
                        TextureUtils.WriteTextureToFile(g.GetComponent<MeshRenderer>().materials[0].mainTexture, Path.Combine(_dllFolder, g.name + ".png"));
                    }
                    catch { Modding.Logger.Log("ERRORRRRRRRRR"); }
                }
                if (g.name.IsAny(toBeFound))
                {
                    //gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystem>().main. 
                    gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().materials[0].color = Color.black;
                    SetMaterialRenderingMode(gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().materials[0], RenderingMode.Cutout);
                    //col.color = new ParticleSystem.MinMaxGradient(Color.black);
                    
                    RadianceSkin.Instance.Log("");
                    RadianceSkin.Instance.Log("Find it");
                    var mos = g.GetComponents<Component>();
                    foreach (var mo in mos)
                    {
                        if (mo.GetType() == typeof(tk2dSprite))
                        {
                            RadianceSkin.Instance.Log(g.GetComponent<tk2dSprite>().Collection.materials.Length);
                            RadianceSkin.Instance.Log(g.GetComponent<tk2dSpriteAnimator>().CurrentFrame);
                            TextureUtils.WriteTextureToFile(g.GetComponent<tk2dSprite>().Collection.materials[0].mainTexture, Path.Combine(_dllFolder, mo.name + ".png"));
                        }
                        RadianceSkin.Instance.Log(mo);

                    }
                    RadianceSkin.Instance.Log("");

                }
            }

        }

        public enum RenderingMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent,
        }

        public static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }

        }
    }
    }