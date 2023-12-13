using HKMenu;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Satchel;
using UObject = UnityEngine.Object;
using System.Linq;
using Steamworks;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using HutongGames.PlayMaker.Actions;
using System.Security.Policy;
using Satchel.Futils;
using System.Runtime.CompilerServices;

namespace RadianceSkin
{
    public class RadianceSkin : Mod,ITogglableMod,IMenuMod,IGlobalSettings<Setting>
    {
        internal static RadianceSkin Instance;
        private static readonly string _dllFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private string _skinFolder = Path.Combine(_dllFolder, "Skins");
        public Setting set=new();

        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}
        public class radianceSkin
        {
            public string name;
            //public string author;
            public bool v1 = false;
            public bool v2 = false;
            public bool v3 = false;
            public bool v4 = false;
            public bool v5 = false;
            public bool v6 = false;
            public bool v7 = false;
            public bool v8 = false;
            public bool v9 = false;
            public Texture2D skin1 = new(1, 1);
            public Texture2D skin2 = new(1, 1);
            public Texture2D skin3 = new(1, 1);
            public Texture2D skin4 = new(1, 1);
            public Texture2D skin5 = new(1, 1);
            public Texture2D skin6 = new(1, 1);
            public Texture2D skin7 = new(1, 1);
            public Texture2D skin8 = new(1, 1);
            public Texture2D skin9 = new(1, 1);
        }
        public List<radianceSkin> skinList = new List<radianceSkin>();
        public List<string> skinNames = new List<string>();
        string[] dreamdotstring = { "Pt StunOutBurst", 
                                                    "Pt StunOutRise", 
                                                        "Pt Tele Out", 
                                                   "Pt AscendRise", 
                                                    "Eye Leak Pt",
                                                    "Eye Final Pt", 
                                                    "Eye Break Pt", 
                                                "Death Stream Pt", 
                                                    "Death Pt",
                                                    "Attack Pt",
                                                    "Shot Charge 2",
                                                    "Shot Charge",
                                                     "Ghost Hit Pt",
                                                      "dream_particle_03 (3)"};
        List<Texture> texs = new List<Texture>();
        Texture dream;
        Sprite spr;
        SpriteRenderer Halo;
        Sprite storeHalo;
        public bool skinused=false;
        public bool statueused=false;
        public bool found = false;
        Material plats;
        Material hand;
        Material feather;
        //Material Halo;
        List<Material> dreamdot=new();
        Material statue;

        private int i = 0;

        public bool ToggleButtonInsideMenu => true;

        public RadianceSkin() : base("RadianceSkin")
        {
           Instance = this;
        }

        public override string GetVersion()
        {
            return "m.x.0.4";
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            On.PlayMakerFSM.OnEnable += ReplaceSkin;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += FindStatue;
            skinNames.Add("关闭");
            skinList.Add(new());
            if (Directory.Exists(_skinFolder))
            {
                string[]nas=Directory.GetDirectories(_skinFolder);
                foreach(var ns in nas)
                {
                    string[] names = Directory.GetFiles(ns);
                    string n = ns.Split('\\')[ns.Split('\\').Length - 1];
                    foreach (var name in names)
                    {
                        string tempname = name.Split('\\')[name.Split('\\').Length - 1];
                        Log(tempname);
                        if (!name.EndsWith(".png"))
                        {
                            continue;
                        }
                        tempname = tempname.Split('.')[0];
                        //string skinname = tempname.Split('-')[0];
                       //string skinid = tempname.Split('-')[1];
                        if (!n.IsAny(skinNames.ToArray()))
                        {
                            skinNames.Add(n);
                            i = skinNames.Count - 1;
                            skinList.Add(new radianceSkin { name = n});
                        }
                        else
                        {
                            i = skinNames.IndexOf(n);
                        }
                        switch (tempname)
                        {
                            case "Radiance0":
                                {
                                    skinList[i].skin1.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v1 = true;
                                    break;
                                }
                            case "Radiance1":
                                {
                                    skinList[i].skin2.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v2 = true;
                                    break;
                                }
                            case "Radiance2":
                                {
                                    skinList[i].skin3.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v3 = true;
                                    break;
                                }
                            case "Plats":
                                {
                                    skinList[i].skin4.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v4 = true;
                                    break;
                                }
                            case "ShadeLord":
                                {
                                    skinList[i].skin5.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v5 = true;
                                    break;
                                }
                            case "Feather":
                                {
                                    skinList[i].skin6.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v6 = true;
                                    break;
                                }
                            case "DreamEffect":
                                {
                                    skinList[i].skin7.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v7 = true;
                                    break;
                                }
                            case "Statue":
                                {
                                    skinList[i].skin8.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v8 = true;
                                    break;
                                }
                            case "Halo":
                                {
                                    skinList[i].skin9.LoadImage(File.ReadAllBytes(name), true);
                                    skinList[i].v9 = true;
                                    break;
                                }
                            default:
                                break;
                        }
                        Log(tempname + "is loaded");
                    }
                }

                

            }
            else
            {
                Directory.CreateDirectory(_skinFolder);
            }

            Log("Initialized");
        }

        private void FindStatue(Scene arg0, Scene arg1)
        {
            Log(arg0.name + "         " + arg1.name);
            if (arg1.name == "GG_Workshop") 
            {
                    var gs = arg1.GetAllGameObjects();
                    int i = 0;
                    Sprite s=null;
                GameObject statue=null;
                    foreach (var g in gs)
                    {
                        if (g.name == "GG_statues_0006_5 (1)"&&g.GetComponent<SpriteRenderer>().sprite.name== "GG_statues_0014_13")
                        {
                                s=g.GetComponent<SpriteRenderer>().sprite;
                        //TextureUtils.WriteTextureToFile(s.texture,Path.Combine(_dllFolder,s.name+".png"));
                            statue = g;
                        }
                    }
            if (skinList[set.skinID].v8)
            {
                if (!statueused)
                {
                        spr = s;
                }
               
                statue.GetComponent<SpriteRenderer>().sprite = MakeSprite(skinList[set.skinID].skin8, s.pixelsPerUnit);
                    Log("OK");
                  
            }
                else
                {
                    if (statueused)
                    {
                        statue.GetComponent<SpriteRenderer>().sprite = spr;
                    }
                }
        }
        }

            private void ReplaceSkin(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            if(self.FsmName=="Control"&&self.gameObject.name=="Boss Control")
            {
                
                try
                {
                    var mat = self.FsmVariables.FindFsmGameObject("Radiance").Value.GetComponent<tk2dSprite>().Collection.materials;
                    if (skinList[set.skinID].v9) self.gameObject.AddComponent<WaitForHalo>();
                    if (!skinused && set.skinID != 0)
                    {
                        texs.Add(mat[0].mainTexture);
                        texs.Add(mat[1].mainTexture);
                        texs.Add(mat[2].mainTexture);
                        if (skinList[set.skinID].v1) { mat[0].mainTexture = skinList[set.skinID].skin1; Log("1 OK"); }
                        if (skinList[set.skinID].v2) { mat[1].mainTexture = skinList[set.skinID].skin2; Log("2 OK"); }
                        if (skinList[set.skinID].v3) { mat[2].mainTexture = skinList[set.skinID].skin3; Log("3 OK"); }
                    }


                    else
                    {
                        if (skinused&&set.skinID==0) { mat[0].mainTexture = texs[0]; mat[1].mainTexture = texs[1]; mat[2].mainTexture = texs[2]; }
                        if(skinused&&set.skinID!=0) {
                            if (skinList[set.skinID].v1) { mat[0].mainTexture = skinList[set.skinID].skin1; Log("1 OK"); }
                            if (skinList[set.skinID].v2) { mat[1].mainTexture = skinList[set.skinID].skin2; Log("2 OK"); }
                            if (skinList[set.skinID].v3) { mat[2].mainTexture = skinList[set.skinID].skin3; Log("3 OK"); }
                        }
                    }
                }
                catch (Exception e) { Log("NOT ROAR"); }
                //TextureUtils.WriteTextureToFile(self.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture, Path.Combine(_dllFolder, "roar.png"));
                //self.gameObject.GetComponent<MeshRenderer>().materials[0].mainTexture = skinList[set.skinID].skin1;
                //self.gameObject.GetComponent<MeshRenderer>().materials[1].mainTexture = skinList[set.skinID].skin2;
                //self.gameObject.GetComponent<MeshRenderer>().materials[2].mainTexture = skinList[set.skinID].skin3;

            }
            /*if(self.FsmName=="Orb Control"&&self.gameObject.name=="Radiant Orb")
            {
                    if (skinList[set.skinID].v7)
                    {
                    TextureUtils.WriteTextureToFile(self.FsmVariables.FindFsmGameObject("Particle System").Value.GetComponent<ParticleSystemRenderer>().materials[0].mainTexture, Path.Combine(_dllFolder, "orb.png"));
                    Log("Orb OK");
                    //self.FsmVariables.FindFsmGameObject("Particle System").Value.GetComponent<ParticleSystemRenderer>().sharedMaterial
                }
            }*/
            /*if (self.FsmName == "Orb Control" && self.gameObject.name == "Radiant Orb")
            {
                if (set.skinID != 0)
                {
                    if (skinList[set.skinID].v7)
                    {
                        TextureUtils.WriteTextureToFile(self.FsmVariables.FindFsmGameObject("Impact Particles").Value.GetComponent<ParticleSystemRenderer>().materials[0].mainTexture, Path.Combine(_dllFolder, "orb.png"));
                    }
                    else
                    {

                    }
                }
            }*/
            if (self.FsmName=="Control"&&self.gameObject.name=="Absolute Radiance")
            {
                //self.gameObject.AddComponent<test>();
                /*foreach (var s in self.gameObject.GetComponents<Component>())
                {
                    Log(s);
                }*/
                if (skinList[set.skinID].v7)
                {
                    //Log(self.gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().materials[1].mainTexture);
                    //TextureUtils.WriteTextureToFile(self.gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().materials[1].mainTexture, Path.Combine(_dllFolder, "ghosthit.png"));
                   if(!skinused) dream = self.gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().sharedMaterials[0].mainTexture;
                    self.gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().sharedMaterials[0].mainTexture = skinList[set.skinID].skin7;
                    var main = self.gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystem>().main;
                     main.startColor  = new ParticleSystem.MinMaxGradient(Color.white);

                    //self.gameObject.GetComponent<EnemyHitEffectsGhost>().slashEffectGhost1.GetComponent<ParticleSystemRenderer>().materials[0].mainTexture = skinList[set.skinID].skin7;
                    //self.gameObject.GetComponent<EnemyHitEffectsGhost>().slashEffectGhost2.GetComponent<ParticleSystemRenderer>().materials[0].mainTexture = skinList[set.skinID].skin7;

                }
                else
                {
                    if (skinused)
                    {
                       //TextureUtils.WriteTextureToFile(texs[6], Path.Combine(_dllFolder, "tex6.png"));
                        self.gameObject.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().sharedMaterials[0].mainTexture = texs[6];
                    }

                }
                if (set.Animation)
                {
                    self.GetAction<GGCheckIfBossSequence>("Tendrils 2", 0).falseEvent = self.GetAction<GGCheckIfBossSequence>("Tendrils 2", 0).trueEvent;
                    if (!BossSequenceController.IsInSequence)
                    {
                        self.ChangeTransition("Final Explode", "FINISHED", "Return to workshop");
                    }
                    /*self.RemoveAction("Return to workshop", 0);
                    self.RemoveAction("Return to workshop", 0);*/
                }
                
                FindAll();

                /*Log(plats);
                if (plats != null)
                {*/
                
                //Log(hand);
                //TextureUtils.WriteTextureToFile(sprite.Collection.materials[0].mainTexture, Path.Combine(_dllFolder, sprite.name + ".png"));
                    /*foreach(var sprite in sprites)
                    {
                        Log(sprite);
                        Log(sprite.Collection);
                        Log(sprite.Collection.Count);
                        Log(sprite.Collection.materials.Length);
                        Log(sprite.Collection.materials[0].mainTexture);
                    }

                    //var sprites=plats.GetComponent<tk2dSprite>();

                    TextureUtils.WriteTextureToFile(sprites[0].Collection.materials[0].mainTexture, Path.Combine(_dllFolder, sprites[0].name + ".png"));
                        
                    }
                    else { Log("Fail"); }*/
                   
                
                Material[] materials = self.gameObject.GetComponent<tk2dSprite>().Collection.materials;
                if (set.skinID != 0)
                {
                    if (!skinused)
                    {
                        skinused = true;
                        /*texs.Add(materials[0].mainTexture);
                        texs.Add(materials[1].mainTexture);
                        texs.Add(materials[2].mainTexture);*/
                        texs.Add(plats.mainTexture);
                        texs.Add(hand.mainTexture);
                        texs.Add(feather.mainTexture);
                        //texs.Add(dreamdot[0].mainTexture);
                        texs.Add(dream);
                        storeHalo = Halo.sprite;
                    }
                    /*if (skinList[set.skinID].v1) { materials[0].mainTexture = skinList[set.skinID].skin1; Log("1 OK"); }
                    if (skinList[set.skinID].v2) { materials[1].mainTexture = skinList[set.skinID].skin2; Log("2 OK"); }
                    if (skinList[set.skinID].v3) { materials[2].mainTexture = skinList[set.skinID].skin3; Log("3 OK"); }*/
                    if (skinList[set.skinID].v4) { plats.mainTexture = skinList[set.skinID].skin4; Log("4 OK"); }
                    if (skinList[set.skinID].v5) { hand.mainTexture = skinList[set.skinID].skin5; Log("5 OK"); }
                    if (skinList[set.skinID].v6) { feather.mainTexture = skinList[set.skinID].skin6; Log("6 OK"); }
                    if (skinList[set.skinID].v7) { foreach (var mat in dreamdot) { mat.mainTexture = skinList[set.skinID].skin7; Log("7 OK"); } }
                    //if (skinList[set.skinID].v7) { dreamdot[0].mainTexture = skinList[set.skinID].skin7;}
                    if (skinList[set.skinID].v9) { Halo.sprite = MakeSprite(skinList[set.skinID].skin9, Halo.sprite.pixelsPerUnit); }
                }
                else
                {
                    if (skinused)
                    {
                        materials[0].mainTexture = texs[0];
                        materials[1].mainTexture = texs[1];
                        materials[2].mainTexture = texs[2];
                        plats.mainTexture = texs[3];
                        hand.mainTexture = texs[4];
                        feather.mainTexture = texs[5];
                        foreach (var mat in dreamdot)
                        {
                            mat.mainTexture = texs[6];
                        }
                        Halo.sprite = storeHalo;
                        }
                }
            }

            orig(self);
        }

        private void FindAll()
        {
            Scene scene = SceneUtils.getCurrentScene();
            var golist = scene.GetAllGameObjects();
           
            foreach (var g in golist) { 
                if(g.name == "Plat Sets") { plats = g.GetComponentsInChildren<tk2dSprite>()[0].Collection.materials[0]; }
                if(g.name == "Shade Lord Hand"){ hand = g.GetComponent<tk2dSprite>().Collection.materials[0]; }
                if( g.name == "Pt Feather Burst") { feather = g.GetComponent<ParticleSystemRenderer>().materials[0]; }
                if(g.name.IsAny(dreamdotstring)) { 
                    Log(g.GetComponent<ParticleSystemRenderer>().materials.Length);
                    dreamdot.Add(g.GetComponent<ParticleSystemRenderer>().sharedMaterials[0]);
                    if (skinList[set.skinID].v7)
                    {
                        var main = g.GetComponent<ParticleSystem>().main;
                        main.startColor = Color.white;
                    }
                    //main.cullingMode
                }
                if (g.name == "Halo") { Halo = g.GetComponent<SpriteRenderer>();/*TextureUtils.WriteTextureToFile(Halo.sprite.texture, Path.Combine(_dllFolder, "Halo.png"));*/ }
            }
            
        }

        public void Unload()
        {
            On.PlayMakerFSM.OnEnable -= ReplaceSkin;
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> menus = new();
            if(toggleButtonEntry.HasValue) menus.Add(toggleButtonEntry.Value);
            menus.Add(
                new IMenuMod.MenuEntry
                {
                    Name = "辐光皮肤",
                    Description = "共有" + skinNames.Count + "个",
                    Values = skinNames.ToArray(),

                    Loader = () => set.skinID,
                    Saver = i => set.skinID = i
                }
                ) ;
            menus.Add(
                new IMenuMod.MenuEntry
                {
                    Name = "五门动画",
                    Description = "开启则在手办屋出现五门动画，但将不会出现亮标动画",
                    Values = new string[]
                {
                    Language.Language.Get("MOH_ON", "MainMenu"),
                    Language.Language.Get("MOH_OFF", "MainMenu"),
                },

                    Loader = () => set.Animation ? 0 : 1,
                    Saver = i => set.Animation = i == 0
                }
                 ) ;
            return menus;
        }
        public static Sprite MakeSprite(Texture2D tex, float ppu) =>
            Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
        public void OnLoadGlobal(Setting s)
        {
            set = s;
        }

        public Setting OnSaveGlobal()
        {
            return set;
        }
    }
}