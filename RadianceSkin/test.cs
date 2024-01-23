using MonoMod.RuntimeDetour;
using Satchel;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Modding;
using Steamworks;

namespace RadianceSkin
{
    internal class test : MonoBehaviour
    {
        SpriteRenderer render;
        private void Start()
        {
            render = GetComponent<SpriteRenderer>();
        }
        private void Update() {
            //Modding.Logger.Log(render.color);
        }
    }
    }