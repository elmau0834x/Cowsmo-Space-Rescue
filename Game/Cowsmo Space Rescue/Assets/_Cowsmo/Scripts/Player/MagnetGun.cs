using UnityEngine;
using UnityEngine.InputSystem;

public class MagnetGun : MonoBehaviour
{
    [Header("Configuración de Succión 2D")]
    public float suctionRange = 6f;          // Radio de alcance para aspirar o atrapar
    public LayerMask interactableLayer;     // Capa de coleccionables y objetos de puzle
    public Transform holdPoint;             // Punto donde flota el objeto atrapado

    [Header("Impulso al Soltar")]
    public float throwHorizontalForce = 6f; // Fuerza hacia los lados del riel
    public float throwUpwardForce = 2f;     // Pequeño arco hacia arriba

    private bool isHoldingObject = false;
    private Rigidbody heldObjectRb;
    private Collider heldObjectCollider;
    private Collider playerCollider;
    private float originalDrag;

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Si el objeto sostenido fue destruido o desactivado externamente, limpiamos el estado
        if (isHoldingObject && (heldObjectRb == null || !heldObjectRb.gameObject.activeInHierarchy))
        {
            ClearHeldState();
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isHoldingObject)
            {
                ReleaseObject();
            }
            else
            {
                TryInteract2D();
            }
        }
    }

    void TryInteract2D()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, suctionRange, interactableLayer);

        Collider closestItem = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.gameObject == gameObject) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestItem = hit;
            }
        }

        if (closestItem == null) return;

        // 1. Coleccionable (chatarra, energía, etc.): se absorbe al inventario y se elimina de la escena
        Collectible item = closestItem.GetComponent<Collectible>();
        if (item != null)
        {
            PlayerInventory inventory = GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddCollectible(item.itemID, item.quantity);
            }

            Destroy(closestItem.gameObject);
            return;
        }

        // 2. Objeto de puzle con Rigidbody: se vuelve cinemático para no empujar a Moo-ry
        Rigidbody rb = closestItem.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            heldObjectRb = rb;
            heldObjectCollider = closestItem;
            isHoldingObject = true;

            originalDrag = heldObjectRb.linearDamping;
            heldObjectRb.isKinematic = true;
            heldObjectRb.useGravity = false;
            heldObjectRb.linearVelocity = Vector3.zero;
            heldObjectRb.angularVelocity = Vector3.zero;

            if (playerCollider != null && heldObjectCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, heldObjectCollider, true);
            }

            if (holdPoint != null)
            {
                heldObjectRb.position = holdPoint.position;
                heldObjectRb.rotation = holdPoint.rotation;
            }
        }
    }

    void ReleaseObject()
    {
        if (heldObjectRb != null)
        {
            heldObjectRb.isKinematic = false;
            heldObjectRb.useGravity = true;
            heldObjectRb.linearDamping = originalDrag;

            if (playerCollider != null && heldObjectCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, heldObjectCollider, false);
            }

            // Dirección hacia izquierda o derecha en el riel según la escala del sprite
            float directionX = transform.localScale.x >= 0f ? 1f : -1f;
            Vector3 ejectForce = new Vector3(directionX * throwHorizontalForce, throwUpwardForce, 0f);
            heldObjectRb.AddForce(ejectForce, ForceMode.Impulse);
        }

        ClearHeldState();
    }

    void ClearHeldState()
    {
        heldObjectRb = null;
        heldObjectCollider = null;
        isHoldingObject = false;
    }

    void FixedUpdate()
    {
        if (isHoldingObject && heldObjectRb != null && holdPoint != null)
        {
            heldObjectRb.position = holdPoint.position;
            heldObjectRb.rotation = holdPoint.rotation;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, suctionRange);
    }
}