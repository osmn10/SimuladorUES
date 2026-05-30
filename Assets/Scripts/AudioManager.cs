using UnityEngine;

// Maneja el audio ambiental del simulador
// El murmullo de gente baja de noche (campus vacio)
// La lluvia se controla desde WeatherSystem
public class AudioManager : MonoBehaviour
{
    [Header("Fuentes de audio")]
    public AudioSource audioAmbiente;   // Murmullo de gente/campus
    public AudioSource audioLluvia;     // Sonido de lluvia

    [Header("Referencias")]
    public DayNightCycle dayNight;      // Para saber la hora del dia

    [Header("Configuracion del murmullo")]
    public float volumenDia = 0.5f;     // Volumen del murmullo de dia
    public float volumenNoche = 0.05f;  // Volumen del murmullo de noche
    [Range(0f, 24f)] public float horaAmanecer = 7f;
    [Range(0f, 24f)] public float horaAnochecer = 19f;

    [Header("Mute global")]
    public bool silenciado = false;     // Estado de mute (tecla M)

    private float volumenLluviaOriginal;

    void Start()
    {
        if (dayNight == null)
            dayNight = FindFirstObjectByType<DayNightCycle>();

        if (audioLluvia != null)
            volumenLluviaOriginal = audioLluvia.volume;
    }

    void Update()
    {
        // Tecla M para silenciar/activar todo
        if (Input.GetKeyDown(KeyCode.M))
            silenciado = !silenciado;

        ActualizarVolumenAmbiente();
    }

    void ActualizarVolumenAmbiente()
    {
        if (audioAmbiente == null) return;

        // Si esta en mute, todo a volumen 0
        if (silenciado)
        {
            audioAmbiente.volume = 0f;
            if (audioLluvia != null) audioLluvia.volume = 0f;
            return;
        }

        // Restauramos volumen de lluvia
        if (audioLluvia != null)
            audioLluvia.volume = volumenLluviaOriginal;

        // Murmullo segun la hora
        if (dayNight != null)
        {
            float hora = dayNight.currentHour;

            if (hora >= horaAmanecer && hora < horaAnochecer)
                audioAmbiente.volume = volumenDia;   // Dia: campus con gente
            else
                audioAmbiente.volume = volumenNoche; // Noche: casi vacio
        }
        else
        {
            audioAmbiente.volume = volumenDia;
        }
    }
}