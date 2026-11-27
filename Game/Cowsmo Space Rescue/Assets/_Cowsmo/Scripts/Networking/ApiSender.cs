using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ScrapData
{
    public string player_id;
    public int numero_acto;
    [NonSerialized]
    public Dictionary<string, int> scrap_collected;
    public int time_seconds;
    public int suit_integrity;
}

public class ApiSender : MonoBehaviour
{
    private string baseUrl = "http://127.0.0.1:8000";

    // Método público que puedes llamar desde tu gestor de juego al terminar un nivel
    public void SendGameProgress(string playerId, int acto, Dictionary<string, int> scrap, int time, int integrity)
    {
        StartCoroutine(PostProgressCoroutine(playerId, acto, scrap, time, integrity));
    }
    

    IEnumerator PostProgressCoroutine(string playerId, int acto, Dictionary<string, int> scrap, int time, int integrity)
    {
        // 1. Crear el objeto de datos
        ScrapData progress = new ScrapData
        {
            player_id = playerId,
            numero_acto = acto,
            scrap_collected = scrap,
            time_seconds = time,
            suit_integrity = integrity
        };

        // 2. Convertir el objeto a formato JSON usando JsonUtility
        // Nota: JsonUtility no serializa diccionarios nativamente, 
        // por lo que usamos una solución limpia o enviamos un JSON construido manualmente.
        string jsonBody = JsonUtility.ToJson(progress);
        
        // Como alternativa robusta para Diccionarios en C# plano, armamos el JSON del body a mano:
        jsonBody = $@"{{
            ""player_id"": ""{playerId}"",
            ""numero_acto"": {acto},
            ""scrap_collected"": {{""metal"": {scrap.GetValueOrDefault("metal", 0)}, ""polimero"": {scrap.GetValueOrDefault("polimero", 0)}}},
            ""time_seconds"": {time},
            ""suit_integrity"": {integrity}
        }}";

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        // 3. Configurar la petición POST
        UnityWebRequest request = new UnityWebRequest(baseUrl + "/api/v1/progress", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 4. Enviar la petición
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("¡Progreso guardado con éxito en MongoDB! Respuesta: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al enviar el progreso: " + request.error + " | " + request.downloadHandler.text);
        }
    }

    // Ejemplo de prueba automática al iniciar la escena (puedes borrarlo luego)
    void Start()
    {
        // Simulamos que el jugador "Moo-ry_001" terminó el Acto 5 con algo de chatarra
        Dictionary<string, int> sampleScrap = new Dictionary<string, int>
        {
            { "metal", 15 },
            { "polimero", 8 }
        };

        // Descomenta la siguiente línea si quieres probar el envío automático al dar Play:
        SendGameProgress("Moo-ry_001", 5, sampleScrap, 120, 85);
    }
}