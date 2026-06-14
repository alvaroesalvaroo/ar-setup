using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class MultiprefabSpawner : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager trackedImageManager;

    [System.Serializable]
    public struct MarkerPrefab
    {
        public string markerName; // debe coincidir exactamente con el nombre en la library
        public GameObject prefab;
    }

    [SerializeField] MarkerPrefab[] markerPrefabs;

    // Objetos spawneados, indexados por trackingId
    Dictionary<TrackableId, GameObject> spawnedObjects = new();

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTracksChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTracksChanged;
    }

    void OnTracksChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.added)
            HandleAdded(image);

        foreach (var image in args.updated)
            HandleUpdated(image);

        foreach (var image in args.removed)
            HandleRemoved(image);
    }

    void HandleAdded(ARTrackedImage image)
    {
        GameObject prefab = FindPrefabForMarker(image.referenceImage.name);
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, image.transform.position, image.transform.rotation);
        spawnedObjects[image.trackableId] = obj;
    }

    void HandleUpdated(ARTrackedImage image)
    {
        if (!spawnedObjects.TryGetValue(image.trackableId, out GameObject obj)) return;

        bool isTracking = image.trackingState == TrackingState.Tracking;
        obj.SetActive(isTracking);

        if (isTracking)
        {
            obj.transform.position = image.transform.position;
            obj.transform.rotation = image.transform.rotation;
        }
    }

    void HandleRemoved(ARTrackedImage image)
    {
        if (spawnedObjects.TryGetValue(image.trackableId, out GameObject obj))
        {
            Destroy(obj);
            spawnedObjects.Remove(image.trackableId);
        }
    }

    GameObject FindPrefabForMarker(string markerName)
    {
        foreach (var marker in markerPrefabs)
            if (marker.markerName == markerName)
                return marker.prefab;

        Debug.LogWarning($"No hay prefab para el marker: {markerName}");
        return null;
    }
}
