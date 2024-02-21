using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;
namespace RadianceSkin;
/// <summary>
/// Gif 播放类
/// </summary>
public class PlayGifAction : MonoBehaviour
{
    public UnityEngine.UI.Image Im;
    public string gifName = "";
    public GameObject[] Ims;
    public UnityEngine.Color color;
    [SerializeField]
    public float fps = 5f;
    public List<Texture2D> tex2DList = new List<Texture2D>();
    private float time;
    Bitmap mybitmp;
    void Start()
    {
        gameObject.AddComponent<Renderer>();
    }
    void Update()
    {
        if (tex2DList.Count > 0)
        {
            time += Time.deltaTime;
            int index = (int)(time * fps) % tex2DList.Count;
             gameObject.GetComponent<Renderer>().material.mainTexture = tex2DList[index];
            gameObject.GetComponent<Renderer>().material.color = color;


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

    //byte[] 转换 Bitmap
    public static Bitmap BytesToBitmap(byte[] Bytes)
    {
        MemoryStream stream = null;
        try
        {
            stream = new MemoryStream(Bytes);
            return new Bitmap(stream);
        }
        catch (ArgumentNullException ex)
        {
            throw ex;
        }
        catch (ArgumentException ex)
        {
            throw ex;
        }
        finally
        {
            stream.Close();
        }
    } 
}