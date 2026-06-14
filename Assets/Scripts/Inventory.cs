using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private Dictionary<string, int> quantities = new();
    private Dictionary<string, Item> itemData = new();

    static string SavePath => Path.Combine(Application.persistentDataPath, "inventory.json");

    // Wrapper serializable porque JsonUtility no soporta Dictionary directamente
    [System.Serializable]
    private class InventoryEntry
    {
        public string itemName;
        public int quantity;
        public int value;
        public string description;
    }

    [System.Serializable]
    private class InventorySaveData
    {
        public List<InventoryEntry> entries = new();
    }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void AddItem(Item item)
    {
        if (Instance.quantities.ContainsKey(item.itemName))
        {
            Instance.quantities[item.itemName]++;
        }
        else
        {
            Instance.quantities[item.itemName] = 1;
            Instance.itemData[item.itemName] = item;
        }

        Debug.Log($"[Inventory] {item.itemName} x{Instance.quantities[item.itemName]}");
    }

    public static void SaveInventory()
    {
        try
        {
            InventorySaveData saveData = new();

            foreach (var key in Instance.quantities.Keys)
            {
                Item item = Instance.itemData[key];
                saveData.entries.Add(new InventoryEntry
                {
                    itemName    = item.itemName,
                    quantity    = Instance.quantities[key],
                    value       = item.value,
                    description = item.description
                });
            }

            string json = JsonUtility.ToJson(saveData, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[Inventory] Guardado en {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Inventory] Error al guardar: {e.Message}");
        }
    }

    public static void LoadInventory()
    {
        Instance.quantities.Clear();
        Instance.itemData.Clear();

        if (!File.Exists(SavePath))
        {
            Debug.Log("[Inventory] No hay guardado previo");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

            foreach (var entry in saveData.entries)
            {
                Item item = new Item
                {
                    itemName    = entry.itemName,
                    value       = entry.value,
                    description = entry.description
                };

                Instance.quantities[entry.itemName] = entry.quantity;
                Instance.itemData[entry.itemName]   = item;
            }

            Debug.Log($"[Inventory] Cargados {Instance.quantities.Count} items");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Inventory] Error al cargar: {e.Message}");
        }
    }

    // Util para consultar
    public static int GetQuantity(string itemName) =>
        Instance.quantities.TryGetValue(itemName, out int q) ? q : 0;

    public static IReadOnlyDictionary<string, int> GetAll() => Instance.quantities;
}