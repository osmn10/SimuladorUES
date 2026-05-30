using UnityEngine;

// Rota el disco de la brujula segun hacia donde mira el jugador
// Las letras N/E/S/O giran como una rosa de los vientos real
public class Brujula : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;          // El PlayerCameraRoot (lo que rota con el mouse)
    public RectTransform discoBrujula; // El disco con las letras que va a rotar

    void Update()
    {
        if (jugador == null || discoBrujula == null) return;

        // Obtenemos hacia donde mira el jugador (rotacion en Y)
        float anguloJugador = jugador.eulerAngles.y;

        // Rotamos el disco en sentido contrario al jugador
        // Asi cuando el jugador mira al norte, la N queda arriba bajo la marca
        // La rotacion es en el eje Z porque es un elemento 2D de UI
        discoBrujula.localRotation = Quaternion.Euler(0f, 0f, -anguloJugador + 180f);
    }
}