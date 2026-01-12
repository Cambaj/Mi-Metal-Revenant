using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_Follow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform playerTarget;
    public Vector3 offset = new Vector3(0, 0, -10); // Z debe ser negativo para ver el 2D

    [Header("Input System (Opcional)")]
    [Tooltip("Asigna aquí la acción de MOVER (ej. Player/Move) para activar el 'Look Ahead'")]
    public InputActionReference moveAction;

    [Header("Configuración de Suavizado")]
    public float smoothTime = 0.25f; // Tiempo que tarda en llegar (más alto = más lento)
    public float lookAheadAmount = 2f; // Cuánto se adelanta la cámara según el input

    private Vector3 _currentVelocity; // Variable interna para SmoothDamp

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    private void LateUpdate()
    {
        if (playerTarget == null) return;

        // 1. Obtener la posición base del jugador
        Vector3 targetPosition = playerTarget.position + offset;

        // 2. Lógica "Look Ahead" usando el Input System
        // Si el jugador se mueve, desplazamos el objetivo de la cámara en esa dirección
        if (moveAction != null)
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();

            // Añadimos el desplazamiento basado en el input (X e Y)
            Vector3 lookOffset = new Vector3(input.x, input.y, 0) * lookAheadAmount;
            targetPosition += lookOffset;
        }

        // 3. Mantener el Z de la cámara fijo (muy importante en 2D)
        // Si tu juego es puramente 2D, forzamos Z a la posición actual de la cámara
        targetPosition.z = transform.position.z;

        // 4. Mover la cámara suavemente usando SmoothDamp (mejor que Lerp para cámaras)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            smoothTime
        );
    }
}
