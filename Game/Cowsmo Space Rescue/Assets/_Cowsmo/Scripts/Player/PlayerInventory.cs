using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    // Usamos un diccionario para almacenar los ítems recolectados (ID del ítem y cantidad)
    // Recuerda que ya manejas serialización, por lo que esto es compatible con tus estructuras.
    [System.NonSerialized]
    public Dictionary<string, int> levelInventory = new Dictionary<string, int>();

    public void AddCollectible(string itemID, int amount)
    {
        if (levelInventory.ContainsKey(itemID))
        {
            levelInventory[itemID] += amount;
        }
        else
        {
            levelInventory[itemID] = amount;
        }

        Debug.Log($"[Inventario] Recogido: {itemID} (+{amount}). Total actual: {levelInventory[itemID]}");
    }

    // Método para obtener el inventario en formato de datos listo para enviar al API
    public Dictionary<string, int> GetInventoryData()
    {
        return levelInventory;
    }
}