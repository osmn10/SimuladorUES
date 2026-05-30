using UnityEngine;
using UnityEngine.UI;
using StarterAssets; // Para acceder al script de inputs de los Starter Assets

// Sistema de stamina: se consume al correr y se recupera al descansar
// Cuando se agota, bloquea el sprint forzando al jugador a caminar
public class StaminaSystem : MonoBehaviour
{
    [Header("Referencias")]
    public Slider barraStamina;              // La barra visual del HUD
    public StarterAssetsInputs inputJugador; // El script de inputs del jugador

    [Header("Configuracion de stamina")]
    public float staminaMaxima = 100f;
    public float velocidadConsumo = 25f;
    public float velocidadRecuperacion = 15f;
    public float staminaMinimaParaCorrer = 10f;

    [Header("Estado actual (solo lectura)")]
    public float staminaActual;
    public bool puedeCorrer = true;

    void Start()
    {
        staminaActual = staminaMaxima;

        if (barraStamina != null)
        {
            barraStamina.maxValue = staminaMaxima;
            barraStamina.value = staminaActual;
        }
    }

    void Update()
    {
        if (inputJugador == null) return;

        // Leemos si el jugador esta intentando correr (sprint del Starter Assets)
        bool intentandoCorrer = inputJugador.sprint;

        // Verificamos si se esta moviendo (el input Move no es cero)
        bool moviendose = inputJugador.move.magnitude > 0.1f;

        // Si esta corriendo, moviendose, y tiene stamina: consumimos
        if (intentandoCorrer && moviendose && staminaActual > 0f && puedeCorrer)
        {
            staminaActual -= velocidadConsumo * Time.deltaTime;

            if (staminaActual <= 0f)
            {
                staminaActual = 0f;
                puedeCorrer = false;
            }
        }
        else
        {
            // Recuperamos stamina cuando no corre
            staminaActual += velocidadRecuperacion * Time.deltaTime;

            if (staminaActual > staminaMaxima)
                staminaActual = staminaMaxima;

            if (staminaActual >= staminaMinimaParaCorrer)
                puedeCorrer = true;
        }

        // CLAVE: si no puede correr (sin stamina), forzamos sprint = false
        // Esto hace que el jugador vuelva a caminar automaticamente
        if (!puedeCorrer)
        {
            inputJugador.sprint = false;
        }

        // Actualizamos la barra visual
        if (barraStamina != null)
            barraStamina.value = staminaActual;
    }
}
