using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YandexMaps;
using YandexMaps.Cntrls;

public class MapLoader : MonoBehaviour
{
    [SerializeField] internal Location _Location;
    [Header("Координаты")] public string Coordinates;
    [Range (-0.01f, 0.01f)]
    public float OffsetLat;
    [Range (-2f, 2f)]
    public float OffsetLon;

    [Header("Отрисовка карты")] public int Gridsize;
    public GameObject PlanePrefab;
    public Transform PlanesContainer;
    public List<GameObject> Planes;


    void Start()
    {
        MapCntrl.MLoader = this;
        _Location = MapCntrl.InitMap(Coordinates);
    }


    public void InitMap()
    {
        string[] ArLocate = Coordinates.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
        MapCntrl.Latitude = double.Parse(ArLocate[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
        MapCntrl.Longitude = double.Parse(ArLocate[1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
        Planes = new List<GameObject>();
        int nbChildren = PlanesContainer.childCount;
        for (int i = nbChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(PlanesContainer.GetChild(i).gameObject);
        }

        for (int row = 0; row < Gridsize; row++)
        {
            for (int col = 0; col < Gridsize; col++)
            {
                GameObject plane = Instantiate(PlanePrefab, new Vector3(col, 0, row), PlanePrefab.transform.rotation);
                MapCntrl.LatitudePlane = MapCntrl.Latitude + OffsetLat * col;
                MapCntrl.LongitudePlane = MapCntrl.Longitude +  OffsetLon * row;
                StartCoroutine(MapCntrl.DownloadMap(plane.GetComponent<MeshRenderer>()));
                Planes.Add(plane);
                plane.transform.parent = PlanesContainer;
            }
        }
    }
}