using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    [Range(0, 1)]
    public float cameraDelay = 0.9f;
    public Vector3 offset;

    private Vector3 targetLastPosition;
    private float speed;

    private void OnValidate() {
        transform.position = target.position + offset;
    }

    void Start()
    {
        // Calculamos la diferencia entre la cámara y el objetivo
        //offset = transform.position - target.position;
        // Asignamos la posición inicial
        transform.position = target.position + offset;
        // Inicializamos la posición anterior como la actual
        targetLastPosition = target.position;
    }


    void FixedUpdate()
    {
        // Actualizamos la posición de la cámara
        Vector3 nextPosition = target.position + offset;
        // Calculamos la posición en base a la diferencia de un frame a otro
        speed = ((nextPosition - transform.position).magnitude / Time.deltaTime) * cameraDelay;
        // Actualizamos la posición según la velocidad calculada
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
        // Actualizamos la posición actual como la última posción
        targetLastPosition = target.position;
    }

    /// <summary>
    /// Configura el layout de la cámara según el jugador al que pertenezca y el total de jugadores
    /// </summary>
    /// <param name="player"></param>
    /// <param name="totalPlayer"></param>
    public void SetCameraPlayer(int player, int totalPlayer) {
        float width = totalPlayer > 1 ? 0.5f : 1f;
        float height = totalPlayer > 2 ? 0.5f : 1f;
        float x = 0;
        float y = 0;

        switch (player) {
            case 1:
                y = 0f;
                y = totalPlayer > 2 ? 0.5f : 0f;
                break;
            case 2:
                x = 0.5f;
                y = totalPlayer > 2 ? 0.5f : 0f;
                break;
            case 3:
                x = 0;
                y = 0;
                break;
            case 4:
                x = 0.5f;
                y = 0;
                break;
        }
        cam.rect = new Rect(x, y, width, height);
    }
}
