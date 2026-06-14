using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    private Image mapImage;
    void Start()
    {
        mapImage = GetComponent<Image>();
        LocationService.LocationChanged += OnLocationChange;
        StartCoroutine(UpdateMap());
    }

    void OnDestroy()
    {
        LocationService.LocationChanged -= OnLocationChange;
    }
    
    public void OnLocationChange()
    {
        StopCoroutine(nameof(UpdateMap));
        StartCoroutine(UpdateMap());
    }

    IEnumerator UpdateMap()
    {
        /*while (!LocationService.Ready)
        {
            yield return null;
        }*/
        int zoom = 15;
        int x = Lon2Tile(LocationService.Longitude, zoom);
        int y = Lat2Tile(LocationService.Latitude, zoom);

        string url = $"https://tile.openstreetmap.org/{zoom}/{x}/{y}.png";

        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        // OSM requiere un User-Agent o puede rechazar la petición
        request.SetRequestHeader("User-Agent", "UnityARApp/1.0");
    
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f) // pivot centrado
            );
            mapImage.sprite = sprite;
            mapImage.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
        }
        else
        {
            Debug.LogError($"Error descargando tile de openmaps: {request.error}");
        }
        
    }
    
    int Lon2Tile(double lon, int zoom) =>
        (int)Math.Floor((lon + 180.0) / 360.0 * Math.Pow(2, zoom));

    int Lat2Tile(double lat, int zoom) =>
        (int)Math.Floor((1 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                                      1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2, zoom));
}
