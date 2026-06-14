using System;
using UnityEngine;
using System.Collections;

public class LocationService : MonoBehaviour
{
    public static double Latitude  { get; private set; }
    public static double Longitude { get; private set; }
    public static bool   Ready     { get; private set; }

    public static event Action LocationChanged;
    void Start()
    {
        // DEFAULT VALUES
        Latitude = 39.462430;
        Longitude = -0.331942;
        StartCoroutine(InitLocation());
    }

    IEnumerator InitLocation()
    {
        // Pedir permiso (necesario en Android/iOS)
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Localización desactivada por el usuario");
            yield break;
        }

        Input.location.Start(
            desiredAccuracyInMeters: 10f,
            updateDistanceInMeters:  10f
        );

        // Esperar hasta 20s a que inicialice
        int timeout = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && timeout > 0)
        {
            yield return new WaitForSeconds(1f);
            timeout--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("No se pudo obtener localización");
            Ready = false;
            yield break;
        }

        Latitude  = Input.location.lastData.latitude;
        Longitude = Input.location.lastData.longitude;
        Ready     = true;
        LocationChanged?.Invoke();
        Debug.Log($"Ubicación: {Latitude}, {Longitude}");
        
        yield return new WaitForSeconds(1f);
        // REINICIAR EL PROCESO EN BUCLE
        StartCoroutine(InitLocation());
    }

    

    void OnDestroy() => Input.location.Stop();
}
