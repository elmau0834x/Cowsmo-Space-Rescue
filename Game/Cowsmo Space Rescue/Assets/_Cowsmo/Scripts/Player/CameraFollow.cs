using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target; // Player_Base_6

    [Header("Configuración de Seguimiento")]
    public float smoothTime = 0.15f;
    public Vector3 offset = new Vector3(0f, 1.3f, -6f);
    public bool centrarAlIniciar = true;

    [Header("Límites del Nivel (Opcional)")]
    public bool usarLimites = false;
    public Vector2 limiteMinimo = new Vector2(-45f, 0f);
    public Vector2 limiteMaximo = new Vector2(100f, 25f);

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        if (target != null && centrarAlIniciar)
        {
            transform.position = CalcularPosicionObjetivo();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = CalcularPosicionObjetivo();
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private Vector3 CalcularPosicionObjetivo()
    {
        float posX = target.position.x + offset.x;
        float posY = target.position.y + offset.y;
        float posZ = target.position.z + offset.z;

        if (usarLimites)
        {
            posX = Mathf.Clamp(posX, limiteMinimo.x, limiteMaximo.x);
            posY = Mathf.Clamp(posY, limiteMinimo.y, limiteMaximo.y);
        }

        return new Vector3(posX, posY, posZ);
    }

    private void OnDrawGizmosSelected()
    {
        if (!usarLimites) return;

        Gizmos.color = Color.cyan;
        Vector3 centro = new Vector3((limiteMinimo.x + limiteMaximo.x) * 0.5f, (limiteMinimo.y + limiteMaximo.y) * 0.5f, 0f);
        Vector3 tamano = new Vector3(limiteMaximo.x - limiteMinimo.x, limiteMaximo.y - limiteMinimo.y, 0.1f);
        Gizmos.DrawWireCube(centro, tamano);
    }
}