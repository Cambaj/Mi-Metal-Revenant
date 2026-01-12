using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{

[ExecuteAlways] // ¡Truco Pro! Permite ver el efecto en el Editor sin dar Play

    [Header("Configuración de Capa")]
    [Tooltip("0 = Fondo estático (Lejano), 1 = Se mueve con la cámara (Primer plano)")]
    [Range(0f, 1f)]
    public float parallaxEffect;

    [Tooltip("Activar si quieres que la imagen se repita infinitamente")]
    public bool infiniteLoop = true;

    [Header("Opciones Avanzadas")]
    [Tooltip("Activar si quieres parallax también en vertical (saltos altos)")]
    public bool applyVerticalParallax = false;

    // Referencias internas
    private Transform _cameraTransform;
    private Vector3 _startPosition;
    private float _spriteWidth;

    void Start()
    {
        // En Unity 6, Camera.main está optimizado, pero cachear es buena práctica
        _cameraTransform = Camera.main.transform;

        // Guardamos la posición inicial del objeto
        _startPosition = transform.position;

        // Si es infinito, medimos el ancho del Sprite automáticamente
        if (infiniteLoop)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                _spriteWidth = sprite.bounds.size.x;
            }
            else
            {
                // Fallback si no hay sprite (ej. usas Tilemap)
                Debug.LogWarning($"El objeto {name} no tiene SpriteRenderer para calcular el Loop infinito.");
                infiniteLoop = false;
            }
        }
    }

    void LateUpdate()
    {
        // Validación de seguridad para el modo editor
        if (_cameraTransform == null)
        {
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
            else return;
        }

        // --- LÓGICA X (Horizontal) ---

        // 1. Distancia: Cuánto se ha movido la cámara RELATIVO al mundo
        float temp = (_cameraTransform.position.x * (1 - parallaxEffect));

        // 2. Distancia Parallax: Cuánto debemos mover este objeto
        float dist = (_cameraTransform.position.x * parallaxEffect);

        // 3. Nueva posición X
        float newX = _startPosition.x + dist;

        // 4. Nueva posición Y (Opcional)
        float newY = transform.position.y;
        if (applyVerticalParallax)
        {
            float distY = (_cameraTransform.position.y * parallaxEffect);
            newY = _startPosition.y + distY;
        }

        // 5. Aplicar movimiento (Respetando Z para el orden de capas)
        transform.position = new Vector3(newX, newY, transform.position.z);

        // --- LÓGICA DE REPETICIÓN INFINITA ---
        if (infiniteLoop)
        {
            // Si la cámara se aleja más de lo que mide la imagen ("temp"), reposicionamos el punto de inicio
            if (temp > _startPosition.x + _spriteWidth)
            {
                _startPosition.x += _spriteWidth;
            }
            else if (temp < _startPosition.x - _spriteWidth)
            {
                _startPosition.x -= _spriteWidth;
            }
        }
    }

}
