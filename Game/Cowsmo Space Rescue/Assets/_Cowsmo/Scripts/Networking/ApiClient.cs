using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    // URL base de tu API en FastAPI ejecutándose localmente
    private string baseUrl = "http://127.0.0.1:8000";

    void Start()
    {
        // Al iniciar el juego, probamos la conexión y descargamos los niveles
        StartCoroutine(CheckHealthAndGetLevels());
    }

    IEnumerator CheckHealthAndGetLevels()
    {
        // 1. Primero hacemos un health-check rápido
        UnityWebRequest healthRequest = UnityWebRequest.Get(baseUrl + "/health-check");
        yield return healthRequest.SendWebRequest();

        if (healthRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al conectar con el backend de Cowsmo: " + healthRequest.error);
            yield break;
        }

        Debug.Log("¡Backend conectado exitosamente! Respuesta: " + healthRequest.downloadHandler.text);

        // 2. Si el servidor responde, pedimos los niveles desde MySQL
        UnityWebRequest levelsRequest = UnityWebRequest.Get(baseUrl + "/api/v1/levels");
        yield return levelsRequest.SendWebRequest();

        if (levelsRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = levelsRequest.downloadHandler.text;
            Debug.Log("Niveles recibidos desde MySQL vía FastAPI:\n" + jsonResponse);
            
            // Aquí puedes procesar el JSON con JsonUtility para poblar tu menú de selección de actos
        }
        else
        {
            Debug.LogWarning("No se pudieron obtener los niveles: " + levelsRequest.error);
        }
    }
}