using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [Header("Configuración del Ítem")]
    public string itemID = "scrap_metal"; // Identificador para el inventario y base de datos
    public int quantity = 1;

    void Awake()
    {
        // Asegurarnos de que el collider 2D funcione como un sensor (Trigger)
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos que el objeto que choca sea el jugador
        if (other.CompareTag("Player"))
        {
            // Buscamos el componente de inventario en el jugador
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddCollectible(itemID, quantity);
            }

            // Efecto visual o sonido de recolección (opcional aquí)
            
            // Destruimos el coleccionable de la escena
            Destroy(gameObject);
        }
    }
}
