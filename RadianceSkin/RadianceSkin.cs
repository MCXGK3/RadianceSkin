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
using UnityEngine.SceneManagement;
using HutongGames.PlayMaker.Actions;
using WavLib;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using MonoMod.Cil;
using GlobalEnums;

namespace RadianceSkin
{
    public class RadianceSkin : Mod, ITogglableMod, IMenuMod, IGlobalSettings<Setting>
    {
        internal static RadianceSkin Instance;
        private static readonly string _dllFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private string _skinFolder = Path.Combine(_dllFolder, "Skins");
        public Setting set = new();
        const string hornetScene = "Deepnest_East_Hornet";
        const string wind = "_Scenery/blizzard_particles";
        public GameObject windg=null;
        const string snow = "pre_blizzard_particles";
        public GameObject snowg=null;
        List<GameObject> customSprites= new List<GameObject>();
        public bool usewatcher=false;
        FileSystemWatcher watcher=null;
        public int watcherID=0;
        public GameObject reChangeColor;
        public int ColorCount=0;

        public override List<ValueTuple<string, string>> GetPreloadNames()
        {
            return new List<ValueTuple<string, string>>
            {

                new ValueTuple<string, string>(hornetScene, wind),
                new ValueTuple<string, string>(hornetScene, snow),
            };
        }

        public class ImagePosition
        {
            public float x=0f;
            public float y=0f;
            public float z=0f;
            public float a=1f;
        }
        public class BackImage
        {
            public enum ImageType
            {
                PNG,
                GIF
            }
            public string name;
            public ImageType type;
            public List<ImagePosition> positions=new();
            [NonSerialized]
            public GameObject gameObject;
        }
        public class Skin
        {
            public string name;
            public int id;
            public Texture2D skin0 = null;
            public Texture2D skin1 = null;
            public Texture2D skin2 = null;
            public Texture2D plats = null;
            public Texture2D shadeLord = null;
            public Texture2D dreamEffect = null;
            public Texture2D statue = null;
            public Texture2D feather = null;
            public Texture2D halo = null;
            public Texture2D cloud= null;
            public AudioClip music = null;
            public Dictionary<string, string> replace = new();
            public LocalSetting local = new LocalSetting();
            public List<BackImage> backimageSettings = new List<BackImage>();
            public Dictionary<string,Sprite> images=new Dictionary<string,Sprite>();
            public Dictionary<string, List<Texture2D>> gifs = new();
        }
        public class OriSkin
        {
            public Texture skin0 = null;
            public Texture skin1 = null;
            public Texture skin2 = null;
            public Texture plats = null;
            public Texture shadeLord = null;
            public Texture dreamEffect = null;
            public Sprite statue = null;
            public Texture feather = null;
            public Sprite halo = null;
            public Sprite cloud = null;
            public AudioClip music = null;
            public bool shotYes = false;
            public ParticleSystem.MinMaxGradient shotColor = null;
        }
        public Dictionary<string, Skin> skins = new();
        public OriSkin oriSkin = new();


        
        public List<string> skinNames = new List<string>();

        public bool skinused = false;
        public bool statueused = false;
        public bool found = false;

        private int i = 0;

        public bool ToggleButtonInsideMenu => true;

        public RadianceSkin() : base("RadianceSkin")
        {
            Instance = this;
        }

        public override string GetVersion()
        {
            return "0.0.0.11";
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");
            Instance = this;
            On.PlayMakerFSM.OnEnable += ReplaceSkin;
            ModHooks.LanguageGetHook += Changelanguage;
            ModHooks.ObjectPoolSpawnHook += RemoveOrbLight;
            On.SpriteFlash.flashFocusHeal += FlashLight;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += FindStatue;
            reChangeColor = new();
            reChangeColor.AddComponent<test>();
            UObject.DontDestroyOnLoad(reChangeColor);
            GetWindAndSnow(preloadedObjects);
            skins.Clear();


            skinNames.Clear();
            skinNames.Add("关闭");
            if (Directory.Exists(_skinFolder))
            {
                string[] skinFolderNames = Directory.GetDirectories(_skinFolder);
                foreach (var skinFolderName in skinFolderNames)
                {
                    string skinName = skinFolderName.Split('\\')[skinFolderName.Split('\\').Length - 1];
                    skinNames.Add(skinName);
                    skins.Add(skinName, new Skin() { name = skinName });
                    var skin = skins[skinName];
                    var dic = skins[skinName].replace;
                    skin.id = skinNames.IndexOf(skinName);

                    string[] assetsNames = Directory.GetFiles(skinFolderName);

                    foreach (var assestName in assetsNames)
                    {
                        string tempname = assestName.Split('\\')[assestName.Split('\\').Length - 1];
                        Log(tempname);
                        if (!assestName.EndsWith(".png") && !assestName.EndsWith(".txt") && !assestName.EndsWith(".wav")&&!assestName.EndsWith(".gif"))
                        {
                            continue;
                        }
                        tempname = tempname.Split('.')[0];
                        if (assestName.EndsWith(".txt"))
                        {
                            foreach (var str in File.ReadAllLines(assestName))
                            {
                                try
                                {
                                    string key = str.Split('=')[0];
                                    string value = str.Split('=')[1];
                                    value = value.Replace("\\n", "\n");
                                    if (!dic.ContainsKey(key))
                                    {
                                        dic.Add(key, value);
                                    }
                                }
                                catch { }
                            }

                        }
                        if (assestName.EndsWith(".png"))
                        {
                            switch (tempname)
                            {
                                case "Radiance0":
                                    {
                                        skin.skin0 = new(1, 1);
                                        skin.skin0.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin1.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v1 = true;*/
                                        break;
                                    }
                                case "Radiance1":
                                    {
                                        skin.skin1 = new(1, 1);
                                        skin.skin1.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin2.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v2 = true;*/
                                        break;
                                    }
                                case "Radiance2":
                                    {
                                        skin.skin2 = new(1, 1);
                                        skin.skin2.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin3.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v3 = true;*/
                                        break;
                                    }
                                case "Plats":
                                    {
                                        skin.plats = new(1, 1);
                                        skin.plats.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin4.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v4 = true;*/
                                        break;
                                    }
                                case "ShadeLord":
                                    {
                                        skin.shadeLord = new(1, 1);
                                        skin.shadeLord.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin5.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v5 = true;*/
                                        break;
                                    }
                                case "Feather":
                                    {
                                        skin.feather = new(1, 1);
                                        skin.feather.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin6.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v6 = true;*/
                                        break;
                                    }
                                case "DreamEffect":
                                    {
                                        skin.dreamEffect = new(1, 1);
                                        skin.dreamEffect.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin7.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v7 = true;*/
                                        break;
                                    }
                                case "Statue":
                                    {
                                        skin.statue = new(1, 1);
                                        skin.statue.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin8.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v8 = true;*/
                                        break;
                                    }
                                case "Halo":
                                    {
                                        skin.halo = new(1, 1);
                                        skin.halo.LoadImage(File.ReadAllBytes(assestName), true);
                                        /*skinList[i].skin9.LoadImage(File.ReadAllBytes(assestName), true);
                                        skinList[i].v9 = true;*/
                                        break;
                                    }
                                case "Cloud":
                                    {
                                        skin.cloud=new(1, 1);
                                        skin.cloud.LoadImage(File.ReadAllBytes(assestName), true);
                                        break;
                                    }
                                default:
                                    skin.backimageSettings.Add(new BackImage() { name = tempname,type=BackImage.ImageType.PNG});
                                    skin.images.Add(tempname, MakeSprite(TextureUtils.LoadTextureFromFile(assestName), 64f));
                                    break;
                            }
                        }
                        if (assestName.EndsWith(".gif"))
                        {
                            
                            
                            /*skin.backimageSettings.Add(new BackImage() { name = tempname, type = BackImage.ImageType.GIF });
                            Log("BEGIN");
                            Log(assestName.ToCharArray());
                            System.Drawing.Image image = System.Drawing.Image.FromFile(assestName);
                            Log(image);
                            Log("IMAGE");
                            skin.gifs.Add(tempname, MyGifSet(System.Drawing.Image.FromFile(assestName)));*/
                        }
                        if (assestName.EndsWith(".wav"))
                        {
                            try
                            {
                                skin.music = LoadAudioClip(assestName, tempname);

                                Log("MUSIC OK");
                            }
                            catch (Exception e) { Log(e); }
                        }
                        Log(skinName+" "+ tempname + "is loaded");
                    }
                    Log(skin.backimageSettings.Count);
                    LoadLocal(skin.id);
                    LoadBackImage(skin.id);
                }



            }
            else
            {
                Directory.CreateDirectory(_skinFolder);
            }

            //处理异常情况
            if (set.skinID >= skinNames.Count) set.skinID = 0;
            if (!skins.ContainsKey(skinNames[set.skinID]))
            {
                set.skinID = 0;
            }

            Log("Initialized");
        }


        private GameObject RemoveOrbLight(GameObject @object)
        {
            if(@object.name.Contains("Radiant Orb"))
            {
                if (set.skinID != 0) { if (!skins[skinNames[set.skinID]].local.orblight) { @object.FindGameObjectInChildren("Fader").GetComponent<SpriteRenderer>().enabled = false; } else { @object.FindGameObjectInChildren("Fader").GetComponent<SpriteRenderer>().enabled = true; } }
                else { @object.FindGameObjectInChildren("Fader").GetComponent<SpriteRenderer>().enabled = true;  }
            }
            return @object;
        }


        private void GetWindAndSnow(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            windg = GameObject.Instantiate(preloadedObjects[hornetScene][wind]);
            snowg = GameObject.Instantiate(preloadedObjects[hornetScene][snow]);
            UObject.DontDestroyOnLoad(windg);
            UObject.DontDestroyOnLoad(snowg);
            windg.SetActive(false);
            snowg.SetActive(false);
        }

        private string Changelanguage(string key, string sheetTitle, string orig)
        {
            if (set.skinID != 0 && skins.ContainsKey(skinNames[set.skinID]))
            {
                var dic = skins[skinNames[set.skinID]].replace;
                if (dic.ContainsKey(orig))
                {
                    return dic[orig];
                }
            }
            return orig;
        }

        private void FindStatue(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GG_Workshop")
            {
                var gs = arg1.GetAllGameObjects();
                int i = 0;
                Sprite s = null;
                GameObject statue = null;
                foreach (var g in gs)
                {
                    if (g.name == "GG_statues_0006_5 (1)" && g.GetComponent<SpriteRenderer>().sprite.name == "GG_statues_0014_13")
                    {
                        s = g.GetComponent<SpriteRenderer>().sprite;
                        statue = g;
                    }
                }
                if (oriSkin.statue == null)
                {
                    oriSkin.statue = s;
                }
                if (set.skinID != 0 && skins[skinNames[set.skinID]].statue != null)
                {
                    statue.GetComponent<SpriteRenderer>().sprite = MakeSprite(skins[skinNames[set.skinID]].statue, s.pixelsPerUnit);
                    Log("PPU   " + s.pixelsPerUnit);
                    statue.GetComponent<SpriteRenderer>().sprite.name = "GG_statues_0014_13";
                    Log("OK");

                }
                else
                {
                    statue.GetComponent<SpriteRenderer>().sprite = oriSkin.statue;
                }
            }

            if (arg0.name == "GG_Radiance")
            {
                if(windg.activeSelf)windg.LocateMyFSM("Control").SendEvent("BLIZZARD END");
                windg.SetActive(false);
                snowg.SetActive(false);
                if (usewatcher)
                {
                    if (watcher != null) { watcher.EnableRaisingEvents = false; watcher.Dispose(); watcher = null; }
                }
            }
            
        }

        private void ReplaceSkin(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            //在bosscontrol的起始阶段替换了辐光的三张主图和音乐
            if (self.FsmName == "Control" && self.gameObject.name == "Boss Control")
            {
                ColorCount = 0;
                //TODO替换场景颜色
                try
                {
                    var absoluteRadiance = self.FsmVariables.FindFsmGameObject("Radiance").Value;
                    var radianceRoar = self.FsmVariables.FindFsmGameObject("Radiance Roar").Value;
                    var mat = absoluteRadiance.GetComponent<tk2dSprite>().Collection.materials;
                    var halo = radianceRoar.FindGameObjectInChildren("Halo").GetComponent<SpriteRenderer>();
                    var feather = radianceRoar.FindGameObjectInChildren("Roar Feathers").GetComponent<ParticleSystemRenderer>().sharedMaterials[0];
                    var plats = GameObject.Find("Plat Sets").GetComponentsInChildren<tk2dSprite>()[0].Collection.materials[0];
                    var shadelord = self.gameObject.Find("Shade Lord Hand").GetComponent<tk2dSprite>().Collection.materials[0];
                    var dream = absoluteRadiance.GetComponent<EnemyHitEffectsGhost>().ghostHitPt.GetComponent<ParticleSystemRenderer>().sharedMaterials[0];

                    if (oriSkin.skin0 == null) oriSkin.skin0 = mat[0].mainTexture;
                    if (oriSkin.skin1 == null) oriSkin.skin1 = mat[1].mainTexture;
                    if (oriSkin.skin2 == null) oriSkin.skin2 = mat[2].mainTexture;
                    if (oriSkin.shadeLord == null) oriSkin.shadeLord = shadelord.mainTexture;
                    if (oriSkin.plats == null) oriSkin.plats = plats.mainTexture;
                    if (oriSkin.halo == null) oriSkin.halo = halo.sprite;
                    if (oriSkin.feather == null) oriSkin.feather = feather.mainTexture;
                    if (oriSkin.dreamEffect == null) oriSkin.dreamEffect = dream.mainTexture;

                    if (set.skinID != 0)
                    {
                        LoadLocal(set.skinID);
                        LoadBackImage(set.skinID);
                        
                        
                        var currentSkin = skins[skinNames[set.skinID]];
                        if (currentSkin.skin0 != null) mat[0].mainTexture = currentSkin.skin0;
                        else mat[0].mainTexture = oriSkin.skin0;
                        if (currentSkin.skin1 != null) mat[1].mainTexture = currentSkin.skin1;
                        else mat[1].mainTexture = oriSkin.skin1;
                        if (currentSkin.skin2 != null) mat[2].mainTexture = currentSkin.skin2;
                        else mat[2].mainTexture = oriSkin.skin2;
                        if (currentSkin.plats != null) plats.mainTexture = currentSkin.plats;
                        else plats.mainTexture = oriSkin.plats;
                        if (currentSkin.feather != null) feather.mainTexture = currentSkin.feather;
                        else feather.mainTexture = oriSkin.feather;
                        if (currentSkin.shadeLord != null) shadelord.mainTexture = currentSkin.shadeLord;
                        else shadelord.mainTexture = oriSkin.shadeLord;
                        if (currentSkin.dreamEffect != null) dream.mainTexture = currentSkin.dreamEffect;
                        else dream.mainTexture = oriSkin.dreamEffect;
                        if (currentSkin.halo != null) halo.sprite = MakeSprite(currentSkin.halo, halo.sprite.pixelsPerUnit);
                        else halo.sprite = oriSkin.halo;

                        MusicCue radiance = self.GetAction<ApplyMusicCue>("Title Up", 3).musicCue.Value as MusicCue;
                        var chaninfo = ReflectionHelper.GetField<MusicCue, MusicCue.MusicChannelInfo[]>(radiance, "channelInfos")[0];
                        if (oriSkin.music == null)
                        {
                            oriSkin.music = chaninfo.Clip;
                            Log(oriSkin.music);
                        }
                        if (currentSkin.music != null)
                        {
                            ReflectionHelper.SetField(chaninfo, "clip", currentSkin.music);
                        }
                        else ReflectionHelper.SetField(chaninfo, "clip", oriSkin.music);
                    }
                    else
                    {
                        mat[0].mainTexture = oriSkin.skin0;
                        mat[1].mainTexture = oriSkin.skin1;
                        mat[2].mainTexture = oriSkin.skin2;
                        plats.mainTexture = oriSkin.plats;
                        feather.mainTexture = oriSkin.feather;
                        shadelord.mainTexture = oriSkin.shadeLord;
                        dream.mainTexture = oriSkin.dreamEffect;
                        halo.sprite = oriSkin.halo;



                        MusicCue radiance = self.GetAction<ApplyMusicCue>("Title Up", 3).musicCue.Value as MusicCue;
                        var chaninfo = ReflectionHelper.GetField<MusicCue, MusicCue.MusicChannelInfo[]>(radiance, "channelInfos")[0];
                        if (oriSkin.music == null)
                        {
                            oriSkin.music = chaninfo.Clip;
                        }
                        else
                        {
                            ReflectionHelper.SetField(chaninfo, "clip", oriSkin.music);
                        }
                    }

                    if (set.skinID != 0)
                    {
                        if (skins[skinNames[set.skinID]].local.blend)
                        {
                            feather.shader = Shader.Find("Sprites/Default");
                            dream.shader = Shader.Find("Sprites/Default");
                        }
                        else
                        {
                            feather.shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
                            dream.shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
                        }
                        if (!skins[skinNames[set.skinID]].local.orblight)
                        {
                            absoluteRadiance.LocateMyFSM("Attack Commands").GetAction<SpawnObjectFromGlobalPool>("Spawn Fireball", 1).gameObject.Value.FindGameObjectInChildren("Fader").SetActive(false);
                        }
                    }
                    else
                    {
                        feather.shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
                        dream.shader = Shader.Find("Legacy Shaders/Particles/Additive (Soft)");
                    }

                    if (set.skinID != 0)
                    {
                        if (skins[skinNames[set.skinID]].local.customBack)
                        {
                            ChangeColor();

                        }
                        if (skins[skinNames[set.skinID]].local.watcher)
                        {
                            usewatcher = true;
                            watcher = new FileSystemWatcher(Path.Combine(_skinFolder, skinNames[set.skinID]), "*.json");
                            watcherID = set.skinID;
                            watcher.Changed += ImageChange;

                            watcher.EnableRaisingEvents = true;
                        }
                        else
                        {
                            usewatcher = false;
                        }
                        if (skins[skinNames[set.skinID]].local.addImage)
                        {
                            AddBackImages();
                        }
                    }
                   
                    
                      




                }
                catch (Exception e) { Log(e); Log("NOT ROAR"); }

            }

            //在radiance的起始阶段替换了其他小图
            if (self.FsmName == "Control" && self.gameObject.name == "Absolute Radiance")
            {

                try
                {
                    //snowg.LocateMyFSM("Control").SetState("State 2");
                    //windg.LocateMyFSM("Control").SendEvent("BLIZZARD START");
                    var halo = self.gameObject.FindGameObjectInChildren("Halo").GetComponent<SpriteRenderer>();
                    if (set.skinID != 0)
                    {
                        if (skins[skinNames[set.skinID]].halo != null)
                            halo.sprite = MakeSprite(skins[skinNames[set.skinID]].halo, halo.sprite.pixelsPerUnit);
                        else halo.sprite = oriSkin.halo;
                    }
                    else halo.sprite = oriSkin.halo;
                    
                }
                catch (Exception e) { Log(e); }

                try
                {
                    var shotCharge = self.gameObject.FindGameObjectInChildren("Shot Charge").GetComponent<ParticleSystem>();
                    var shotMain = shotCharge.main;
                    if (oriSkin.shotYes == false) oriSkin.shotColor = shotMain.startColor;
                    if (set.skinID != 0) {
                        UnityEngine.Color color = new();
                        ColorUtility.TryParseHtmlString(skins[skinNames[set.skinID]].local.shotColor, out color);

                        if (skins[skinNames[set.skinID]].local.shotCharge) shotMain.startColor = new ParticleSystem.MinMaxGradient(color);
                        else shotMain.startColor = oriSkin.shotColor;
                    }
                    else { shotMain.startColor = oriSkin.shotColor; }
                }
                catch (Exception e) { Log(e); }


                try
                {
                    if (set.Animation)
                    {
                        self.GetAction<GGCheckIfBossSequence>("Tendrils 2", 0).falseEvent = self.GetAction<GGCheckIfBossSequence>("Tendrils 2", 0).trueEvent;
                        if (!BossSequenceController.IsInSequence)
                        {
                            self.ChangeTransition("Final Explode", "FINISHED", "Return to workshop");
                        }
                    }


                }
                catch (Exception e) { Log(e); }
            }

            orig(self);
        }

        private void FlashLight(On.SpriteFlash.orig_flashFocusHeal orig, SpriteFlash self)
        {
            orig(self);
            if(self.gameObject.name=="Absolute Radiance"||self.gameObject.name=="Legs")
            {
                if(set.skinID!=0)
                ReflectionHelper.SetField<SpriteFlash, float>(self, "amount", skins[skinNames[set.skinID]].local.hitLight);
            }
        }

        private void ImageChange(object sender, FileSystemEventArgs e)
        {
            if (set.skinID != watcherID) { return; }
            Log("CHANGE");
            LoadBackImage(set.skinID,true);
            AddBackImages();
        }

        public void ChangeColor()
        {
            ColorCount++;
            Log("CHANGE");
            bool flag = false;
            if (SceneUtils.getCurrentScene().name!="GG_Radiance")
            { flag = true; }
            GameObject GG_Arena_Prefab = GameObject.Find("GG_Arena_Prefab");
            if (GG_Arena_Prefab != null)
            {
                
            }
            GameObject bg = GG_Arena_Prefab.FindGameObjectInChildren("BG");
            List<GameObject> lists = new();
            bg.FindAllChildren(lists);

            foreach (var g in lists)
            {
                UnityEngine.Color hazecolor;
                UnityEngine.Color lightcolor;
                ColorUtility.TryParseHtmlString(skins[skinNames[set.skinID]].local.hazeColor, out hazecolor);
                ColorUtility.TryParseHtmlString(skins[skinNames[set.skinID]].local.lightColor, out lightcolor);
                if (g.name.Contains("haze") )
                {
                    var te = g.GetComponent<test>();
                    if(te!= null) { flag = true; }
                    g.AddComponent<test>();
                    var render = g.GetComponent<SpriteRenderer>();
                    if (render != null) render.color = hazecolor;
                }
                else if (g.name.Contains("straight"))
                {
                    var te = g.GetComponent<test>();
                    if (te != null) { flag = true; }
                    g.AddComponent<test>();
                    var render = g.GetComponent<SpriteRenderer>();
                    if (render != null) render.color = lightcolor;
                }
                else if (g.name.Contains("GG_scenery_0004_17"))
                {
                    var render = g.GetComponent<SpriteRenderer>();
                    if (render != null)
                    {
                        if (oriSkin.cloud == null) oriSkin.cloud = render.sprite;
                        if (skins[skinNames[set.skinID]].cloud != null) { render.sprite = MakeSprite(skins[skinNames[set.skinID]].cloud, render.sprite.pixelsPerUnit); }
                        else render.sprite = oriSkin.cloud;
                    }
                    if (skins[skinNames[set.skinID]].local.removeCloud) { g.SetActive(false); } 
                }
                else if (g.name.Contains("BlurPlane") || g.name.Contains("GG_gods_ray")) ;
                else if(skins[skinNames[set.skinID]].local.removeOthers) g.SetActive(false);
            }
            string removeg = "gg_aerial";
            var go = GameObject.Find(removeg);
             if(go!=null)  go.SetActive(false);
            for (int i = 1; i < 5; i++)
            {
                var re = GameObject.Find(removeg + " (" + i + ")");
                if(re!=null) re.SetActive(false);
            }
            
            go = GameObject.Find("GG_pillar_top");
            if(go!=null)     go.SetActive(false);
            go = GameObject.Find("Godseeker Crowd");
            if(go!=null) go.SetActive(false);
            if (flag&&ColorCount<=5) reChangeColor.GetComponent<test>().StartCoroutine(WaitAndChange(0.2f));
        }

        public IEnumerator WaitAndChange(float time)
        {
            yield return new WaitForSeconds(time);
            ChangeColor();
        }

        public void AddBackImages()
        {
            foreach(var g in customSprites)
            {
                if(g!=null) GameObject.Destroy(g);
            }
            customSprites.Clear();
            foreach (var g in skins[skinNames[set.skinID]].backimageSettings) 
            {
                foreach (var po in g.positions)
                {
                    GameObject temp = new GameObject(g.name);
                    customSprites.Add(temp);
                    Log(temp);
                    g.gameObject = temp;
                    temp.transform.position = new Vector3(po.x,po.y,po.z);
                    if (g.type == BackImage.ImageType.PNG)
                    {
                        var render = temp.GetAddComponent<SpriteRenderer>();
                        render.sprite = skins[skinNames[set.skinID]].images[g.name];
                        render.color = new UnityEngine.Color(render.color.r, render.color.g, render.color.b, po.a);
                    }
                    else if(g.type== BackImage.ImageType.GIF)
                    {
                        var gif=temp.GetAddComponent<PlayGifAction>();
                        gif.gifName = g.name;
                        gif.tex2DList= skins[skinNames[set.skinID]].gifs[g.name];
                        gif.color = new UnityEngine.Color(1, 1, 1, po.a);
                    }
                }
            }
            foreach (var g in skins[skinNames[set.skinID]].backimageSettings)
            {
                Log(g.gameObject);
            }
            if (skins[skinNames[set.skinID]].local.wind)
            {
                windg.SetActive(true);
                windg.transform.position = new Vector3(68, 23, 0);
                windg.LocateMyFSM("Control").SendEvent("BLIZZARD START");
            }
            if (skins[skinNames[set.skinID]].local.snow) snowg.SetActive(true); 
        }

        public static AudioClip LoadAudioClip(string path, string name)
        {
            var stream = new StreamReader(path);
            //var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var wavData = new WavData();
            wavData.Parse(stream.BaseStream);
            stream.Close();
            var samples = wavData.GetSamples();
            var clip = AudioClip.Create(name, samples.Length / wavData.FormatChunk.NumChannels, wavData.FormatChunk.NumChannels, (int)wavData.FormatChunk.SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public void Unload()
        {
            On.PlayMakerFSM.OnEnable -= ReplaceSkin;
            ModHooks.LanguageGetHook -= Changelanguage;
            ModHooks.ObjectPoolSpawnHook -= RemoveOrbLight;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= FindStatue;
            On.SpriteFlash.flashFocusHeal -= FlashLight;
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> menus = new();
            menus.Add(
                new IMenuMod.MenuEntry
                {
                    Name = "辐光皮肤",
                    Description = "共有" + skinNames.Count + "个",
                    Values = skinNames.ToArray(),

                    Loader = () => set.skinID,
                    Saver = i => set.skinID = i
                }
                );
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
                 );
            /*menus.Add(
                new IMenuMod.MenuEntry
                {
                    Name = "混合模式",
                    Description = "加法混合或常规混合",
                    Values = new string[]
                {
                    //Language.Language.Get("MOH_ON", "MainMenu"),
                    //Language.Language.Get("MOH_OFF", "MainMenu"),
                    "常规混合",
                    "加法混合"

                },

                    Loader = () => set.blend ? 0 : 1,
                    Saver = i => set.blend = i == 0
                }
                 );*/
            return menus;
        }
        public static Sprite MakeSprite(Texture2D tex, float ppu) =>
            Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);

        private byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // 将bitmap 以png格式保存到流中
                bitmap.Save(stream, ImageFormat.Png);
                // 创建一个字节数组，长度为流的长度
                byte[] data = new byte[stream.Length];
                // 重置指针
                stream.Seek(0, SeekOrigin.Begin);
                // 从流读取字节块存入data中
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

        public List<Texture2D> MyGifSet(System.Drawing.Image image)
        {
            List<Texture2D> tex = new List<Texture2D>();
            if (image != null)
            {
                FrameDimension frame = new FrameDimension(image.FrameDimensionsList[0]);
                int framCount = image.GetFrameCount(frame);//获取维度帧数
                for (int i = 0; i < framCount; ++i)
                {
                    image.SelectActiveFrame(frame, i);
                    Bitmap framBitmap = new Bitmap(image.Width, image.Height);
                    using (System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(framBitmap))
                    {
                        graphic.DrawImage(image, Point.Empty);
                    }
                    Texture2D frameTexture2D = new Texture2D(framBitmap.Width, framBitmap.Height, TextureFormat.ARGB32, true);
                    frameTexture2D.LoadImage(Bitmap2Byte(framBitmap));
                    tex.Add(frameTexture2D);
                }
            }
            return tex;
        }

 
        public void OnLoadGlobal(Setting s)
        {
            
            set = s;
        }

        public Setting OnSaveGlobal()
        {
            foreach (var skin in skins)
            {
               SaveLocal(skin.Value.id);
               SaveBackImages(skin.Value.id);
            }
            return set;
        }




        public void LoadLocal(int id)
        {
            string skinfolderSetting = Path.Combine(_dllFolder,"Skins", skinNames[id], "LocalSettings.json");
            LocalSetting localSetting;
            Log("LOAD");
            if (File.Exists(skinfolderSetting))
                {
                Log("READ");
                string content = File.ReadAllText(skinfolderSetting);
                    localSetting =JsonUtility.FromJson<LocalSetting>(content);
                var setting = JsonUtility.FromJson<OldLocalSetting>(content);
                if (setting.backColor != "#FFFFFFFF")
                {
                    localSetting.hazeColor = setting.backColor;
                    localSetting.lightColor = setting.backColor;
                    localSetting.addImage = setting.customBack;
                }
                }   
            else { 
                    
                    localSetting = new LocalSetting(); 
                    File.WriteAllText(skinfolderSetting,JsonUtility.ToJson(localSetting,true));
                Log("WRITE");
                }
             skins[skinNames[id]].local = localSetting;
            
        }

        public void SaveLocal(int id)
        {
            string skinfolderSetting = Path.Combine(_dllFolder, "Skins", skinNames[id], "LocalSettings.json");
            LocalSetting localSetting= skins[skinNames[id]].local;
            File.WriteAllText(skinfolderSetting, JsonUtility.ToJson(localSetting, true));
            Log("Write");

        }

        public void LoadBackImage(int id)
        {
            LoadBackImage(id, false);
        }
        public void LoadBackImage(int id,bool onlyRead)
        {
            string skinfolderSetting = Path.Combine(_dllFolder, "Skins", skinNames[id], "BackImages.json");
            Log("LOAD");
            if (File.Exists(skinfolderSetting))
            {
                Log("READ");
                try
                {
                    var list = JsonConvert.DeserializeObject<List<BackImage>>(File.ReadAllText(skinfolderSetting));
                    ListUnion(skins[skinNames[id]].backimageSettings, list);
                }
                catch { 
                    foreach(var i in skins[skinNames[id]].backimageSettings)
                    {
                        if (i.positions.Count == 0) i.positions.Add(new());
                    }
                }
                if(!onlyRead) SaveBackImages(id);

            }
            else
            {
                if (!onlyRead) SaveBackImages(id);
            }
        }
        public void SaveBackImages(int id)
        { 
            string skinfolderSetting = Path.Combine(_dllFolder, "Skins", skinNames[id], "BackImages.json");
            Log(JsonConvert.SerializeObject(skins[skinNames[id]].backimageSettings));
            string data = JsonConvert.SerializeObject(skins[skinNames[id]].backimageSettings,Formatting.Indented);
            File.WriteAllText(skinfolderSetting, data);
            Log("WRITE");
        }

        public void ListUnion(List<BackImage> list1,List<BackImage> list2)
        {

            foreach(BackImage item in list1)
            {
                foreach(BackImage item2 in list2)
                {
                    if(item.name== item2.name)
                    {
                        item.positions=item2.positions;
                        item.type=item2.type;
                        break;
                    }
                }
                if (item.positions.Count == 0)
                {
                    item.positions.Add(new());
                }
            }
        }
    }
}