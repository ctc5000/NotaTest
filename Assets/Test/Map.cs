// Decompiled with JetBrains decompiler
// Type: YandexMaps.Map
// Assembly: YandexMaps, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC6685D5-0A7A-4583-B81F-2C590B12B21A
// Assembly location: F:\BlackVr\NOTA\Assets\YandexMaps\Plugins\YandexMaps.dll
// XML documentation location: F:\BlackVr\NOTA\Assets\YandexMaps\Plugins\YandexMaps.xml

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace YandexMaps
{
  /// <summary>Базовый класс</summary>
  public static class Map
  {
    /// <summary>
    /// Добавление маркеров,которые требуется отобразить на карте. (Максимальное количество маркеров доступно 100)
    /// </summary>
    public static List<Vector2> SetMarker = new List<Vector2>();

    /// <summary>Долгота.</summary>
    public static float Longitude { set; get; }

    /// <summary>широта.</summary>
    public static float Latitude { set; get; }

    /// <summary>Уровень масштабирования карты от 0 до 17.</summary>
    public static int Size { set; get; }

    /// <summary>
    /// Ширина запрашиваемого изображения карты (в пикселах) доступный максимальный размер ширины 650.
    /// </summary>
    public static int Width { set; get; }

    /// <summary>
    /// Высота запрашиваемого изображения карты (в пикселах) доступный максимальный размер высоты 450.
    /// </summary>
    public static int Height { set; get; }

    /// <summary>Тип картов.</summary>
    public static Map.TypeMap SetTypeMap { set; get; }

    /// <summary>
    /// Тип слоёв. Прежде тем чтобы использовать слои, включите параметр EnabledLayer = true;
    /// </summary>
    public static Map.TypeMapLayer SetTypeMapLayer { set; get; }

    /// <summary>Включение/Выключение слоёв.</summary>
    public static bool EnabledLayer { set; get; }

    /// <summary>Получение текстуры</summary>
    /// <returns></returns>
    public static Texture GetTexture { get; private set; }

    /// <summary>Получение текстуры 2D</summary>
    /// <returns></returns>
    public static Texture2D GetTexture2D { get; private set; }

    /// <summary>
    /// Загружает карту один раз ( не рекомендуется запускать в методе Update(), для этого есть метод UpdateLoadMap() ).
    /// </summary>
    public static void LoadMap() => Object.FindObjectOfType<MonoBehaviour>().StartCoroutine(Map.DownloadMap());

    /// <summary>
    /// Загружает карту и обновляет каждый раз ( не рекомендуется запускать в методе Update() ).
    /// </summary>
    public static void UpdateLoadMap()
    {
      MonoBehaviour objectOfType = Object.FindObjectOfType<MonoBehaviour>();
      objectOfType.StopCoroutine(Map.DownloadMap(objectOfType));
      objectOfType.StartCoroutine(Map.DownloadMap(objectOfType));
    }

    /// <summary>
    /// Загружает карту и обновляет каждый раз в заданном тайм-аутом ( не рекомендуется запускать в методе Update() ).
    /// </summary>
    /// <param name="timeout">тайм-аут.</param>
    public static void UpdateLoadMap(float timeout)
    {
      MonoBehaviour objectOfType = Object.FindObjectOfType<MonoBehaviour>();
      objectOfType.StopCoroutine(Map.DownloadMap(timeout, objectOfType));
      objectOfType.StartCoroutine(Map.DownloadMap(timeout, objectOfType));
    }

    private static string Convert(List<Vector2> vectors)
    {
      string str1 = "";
      string str2 = "";
      for (int index = 0; index < vectors.Count; ++index)
      {
        str1 = vectors[index].x.ToString().Replace(',', '.');
        str2 = vectors[index].y.ToString().Replace(',', '.');
      }
      return str1 + "," + str2;
    }

    private static string[] ConvertArry(List<Vector2> vectors)
    {
      string[] strArray = new string[Map.SetMarker.Count];
      for (int index = 0; index < Map.SetMarker.Count; ++index)
        strArray[index] = Map.Convert(Map.SetMarker[index].x) + "," + Map.Convert(Map.SetMarker[index].y);
      return strArray;
    }

    private static string Convert(float value) => value.ToString().Replace(',', '.');

    private static string Convert(Vector2 vector) => vector.x.ToString().Replace(',', '.') + "," + vector.y.ToString().Replace(',', '.');

    private static string Yandex(bool layer)
    {
      string str;
      if (!layer)
        str = "https://static-maps.yandex.ru/1.x/?ll=" + Map.Convert(Map.Longitude) + "," + Map.Convert(Map.Latitude) + "&size=" + Map.Width.ToString() + "," + Map.Height.ToString() + "&z=" + Map.Size.ToString() + "&l=" + (object) Map.SetTypeMap + "&pt=" + string.Join("~", Map.ConvertArry(Map.SetMarker));
      else
        str = "https://static-maps.yandex.ru/1.x/?ll=" + Map.Convert(Map.Longitude) + "," + Map.Convert(Map.Latitude) + "&size=" + Map.Width.ToString() + "," + Map.Height.ToString() + "&z=" + Map.Size.ToString() + "&l=" + (object) Map.SetTypeMap + "," + (object) Map.SetTypeMapLayer + "&pt=" + string.Join("~", Map.ConvertArry(Map.SetMarker));
      return str;
    }

    private static IEnumerator DownloadMap()
    {
      Map.Width = Mathf.Clamp(Map.Width, 0, 650);
      Map.Height = Mathf.Clamp(Map.Width, 0, 450);
      Map.Size = Mathf.Clamp(Map.Size, 0, 17);
      if (Map.Width == 0 | Map.Height == 0)
      {
        Map.Width = 650;
        Map.Height = 450;
      }
      using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(Map.Yandex(Map.EnabledLayer)))
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
            Map.GetTexture = (Texture) download;
            Map.GetTexture2D = download;
          }
          download = (Texture2D) null;
        }
      }
    }

    private static IEnumerator DownloadMap(MonoBehaviour mono)
    {
      Map.Width = Mathf.Clamp(Map.Width, 0, 650);
      Map.Height = Mathf.Clamp(Map.Width, 0, 450);
      Map.Size = Mathf.Clamp(Map.Size, 0, 17);
      if (Map.Width == 0 | Map.Height == 0)
      {
        Map.Width = 650;
        Map.Height = 450;
      }
      using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(Map.Yandex(Map.EnabledLayer)))
      {
        yield return (object) uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
        {
          Debug.Log((object) uwr.error);
        }
        else
        {
          Texture2D download = DownloadHandlerTexture.GetContent(uwr);
          download.name = "YandexMap";
          if (uwr.isDone)
          {
            Map.GetTexture = (Texture) download;
            Map.GetTexture2D = download;
          }
          download = (Texture2D) null;
        }
      }
      yield return (object) new WaitForSeconds(0.075f);
      mono.StartCoroutine(Map.DownloadMap(mono));
    }

    private static IEnumerator DownloadMap(float timeout, MonoBehaviour mono)
    {
      Map.Width = Mathf.Clamp(Map.Width, 0, 650);
      Map.Height = Mathf.Clamp(Map.Width, 0, 450);
      Map.Size = Mathf.Clamp(Map.Size, 0, 17);
      if (Map.Width == 0 | Map.Height == 0)
      {
        Map.Width = 650;
        Map.Height = 450;
      }
      using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(Map.Yandex(Map.EnabledLayer)))
      {
        yield return (object) uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
        {
          Debug.Log((object) uwr.error);
        }
        else
        {
          Texture2D download = DownloadHandlerTexture.GetContent(uwr);
          download.name = "YandexMap";
          if (uwr.isDone)
          {
            Map.GetTexture = (Texture) download;
            Map.GetTexture2D = download;
          }
          download = (Texture2D) null;
        }
      }
      yield return (object) new WaitForSeconds(timeout);
      mono.StartCoroutine(Map.DownloadMap(timeout, mono));
    }

    /// <summary>Тип карты.</summary>
    public enum TypeMap
    {
      /// <summary>Схема местности и названия географических объектов.</summary>
      map,
      /// <summary>Местность, сфотографированная со спутника.</summary>
      sat,
    }

    /// <summary>Тип карты слоёв.</summary>
    public enum TypeMapLayer
    {
      /// <summary>Слой названия географических объектов.</summary>
      skl,
      /// <summary>Слой пробок.</summary>
      trf,
    }
  }
}
