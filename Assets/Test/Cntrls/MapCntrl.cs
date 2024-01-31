using System;
using System.Collections;
using System.Collections.Generic;
using Plugins.SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace YandexMaps.Cntrls
{
    public class MapCntrl
    {
        public static MapLoader MLoader;
        public static double Longitude { set; get; }
        public static double Latitude { set; get; }
        public static double LongitudePlane { set; get; }
        public static double LatitudePlane { set; get; }
        public static int Size { set; get; }
        public static int Width { set; get; }
        public static int Height { set; get; }
        public static Texture GetTexture { get; private set; }
        public static Texture2D GetTexture2D { get; private set; }
     //   public static void LoadMap() => MLoader.StartCoroutine(DownloadMap());


        internal static IEnumerator DownloadMap(MeshRenderer renderer)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(YandexMap()))
            {
                yield return (object) uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError((object) uwr.error);
                }
                else
                {
                    Texture2D download = DownloadHandlerTexture.GetContent(uwr);
                    download.name = "YandexMap";
                    if (uwr.isDone)
                    {
                        GetTexture = (Texture) download;
                        GetTexture2D = download;
                        renderer.material.mainTexture = download;
                    }
                }
            }
        }

        public static Location InitMap(string Coordinates)
        {
            string[] ArLocate = Coordinates.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            Latitude = double.Parse(ArLocate[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
            Longitude = double.Parse(ArLocate[1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
            Width = 650;
            Height = 450;
            Size = 17;
            Size = Mathf.Clamp(Size, 0, 17);
            MLoader.StartCoroutine(GetPointMap());
            Location loc = new Location();
            loc.Latitude = Latitude;
            loc.Longitude = Longitude;
            return loc;
        }

        public static IEnumerator GetPointMap()
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(YandexPoint()))
            {
                yield return (object) uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError((object) uwr.error);
                }
                else
                {
                    JSONNode GeoData = JSON.Parse(uwr.downloadHandler.text);
                    MLoader._Location.Label =
                        GeoData["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["metaDataProperty"][
                            "GeocoderMetaData"]["text"];
                }
            }

            yield return null;
        }

        private static string Convert(double value) => value.ToString().Replace(',', '.');

        private static string YandexMap()
        {
            string str = "";
            str =
                $"https://static-maps.yandex.ru/1.x/?ll={Convert(LongitudePlane)},{Convert(LatitudePlane)}&size={Width},{Height}&z={Size}&l=map";
            return str;
        }

        private static string YandexPoint()
        {
            string str;

            str =
                $"https://geocode-maps.yandex.ru/1.x/?apikey=97f4b224-2fee-4a44-b504-cf6b72926cbb&geocode={Longitude},{Latitude}&format=json";
            return str;
        }
    }
}