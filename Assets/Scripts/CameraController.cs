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
        // Calculamos la diferencia entre la c�mara y el objetivo
        //offset = transform.position - target.position;
        // Asignamos la posici�n inicial
        transform.position = target.position + offset;
        // Inicializamos la posici�n anterior como la actual
        targetLastPosition = target.position;
    }


    void FixedUpdate()
    {
        // Actualizamos la posici�n de la c�mara
        Vector3 nextPosition = target.position + offset;
        // Calculamos la posici�n en base a la diferencia de un frame a otro
        speed = ((nextPosition - transform.position).magnitude / Time.deltaTime) * cameraDelay;
        // Actualizamos la posici�n seg�n la velocidad calculada
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
        // Actualizamos la posici�n actual como la �ltima posci�n
        targetLastPosition = target.position;
    }

    /// <summary>
    /// Configura el layout de la c�mara seg�n el jugador al que pertenezca y el total de jugadores
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
