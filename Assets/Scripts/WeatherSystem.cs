using UnityEngine;
using TMPro;

public class WeatherSystem : MonoBehaviour
{
    public enum WeatherState { Despejado, Nublado, Lluvia }

    [Header("Estado actual")]
    public WeatherState estadoActual = WeatherState.Despejado;

    [Header("Particulas de lluvia")]
    public ParticleSystem sistemaLluvia;

    [Header("Luz del sol")]
    public Light luzDireccional;

    [Header("Multiplicadores de iluminacion")]
    [Range(0f, 1f)] public float multiplicadorDespejado = 1.0f;
    [Range(0f, 1f)] public float multiplicadorNublado = 0.6f;
    [Range(0f, 1f)] public float multiplicadorLluvia = 0.4f;

    [Header("Colores del cielo (se multiplican con DayNightCycle)")]
    public Color cieloDespejado = new Color(1f, 1f, 1f);
    public Color cieloNublado = new Color(0.55f, 0.6f, 0.65f);
    public Color cieloLluvia = new Color(0.3f, 0.35f, 0.4f);

    [Header("UI opcional")]
    public TextMeshProUGUI textoClima;

    [Header("Audio opcional")]
    public AudioSource audioLluvia;

    private float intensidadOriginalSol;
    private DayNightCycle dayNight;
    private Camera camaraPrincipal;

    void Start()
    {
        if (luzDireccional != null)
            intensidadOriginalSol = luzDireccional.intensity;

        dayNight = FindFirstObjectByType<DayNightCycle>();
        camaraPrincipal = Camera.main;

        AplicarEstadoClima();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            CambiarClima();

        AplicarColorCielo();
    }

    public void CambiarClima()
    {
        int siguiente = ((int)estadoActual + 1) % 3;
        estadoActual = (WeatherState)siguiente;
        AplicarEstadoClima();
    }

    void AplicarEstadoClima()
    {
        float multiplicadorLuz = 1f;
        bool activarLluvia = false;

        switch (estadoActual)
        {
            case WeatherState.Despejado:
                multiplicadorLuz = multiplicadorDespejado;
                activarLluvia = false;
                break;
            case WeatherState.Nublado:
                multiplicadorLuz = multiplicadorNublado;
                activarLluvia = false;
                break;
            case WeatherState.Lluvia:
                multiplicadorLuz = multiplicadorLluvia;
                activarLluvia = true;
                break;
        }

        if (luzDireccional != null)
            luzDireccional.intensity = intensidadOriginalSol * multiplicadorLuz;

        if (sistemaLluvia != null)
        {
            if (activarLluvia && !sistemaLluvia.isPlaying)
                sistemaLluvia.Play();
            else if (!activarLluvia && sistemaLluvia.isPlaying)
                sistemaLluvia.Stop();
        }

        if (audioLluvia != null)
        {
            if (activarLluvia && !audioLluvia.isPlaying)
                audioLluvia.Play();
            else if (!activarLluvia && audioLluvia.isPlaying)
                audioLluvia.Stop();
        }

        if (textoClima != null)
            textoClima.text = "Clima: " + estadoActual.ToString();
    }

    void AplicarColorCielo()
    {
        if (camaraPrincipal == null) return;

        Color colorClima = cieloDespejado;
        switch (estadoActual)
        {
            case WeatherState.Despejado: colorClima = cieloDespejado; break;
            case WeatherState.Nublado: colorClima = cieloNublado; break;
            case WeatherState.Lluvia: colorClima = cieloLluvia; break;
        }

        if (dayNight != null)
        {
            Color colorCieloDia = dayNight.colorCieloActual;
            Color colorFinal = colorCieloDia * colorClima;

            camaraPrincipal.backgroundColor = colorFinal;
            RenderSettings.ambientLight = colorFinal * 0.5f;
        }
        else
        {
            camaraPrincipal.backgroundColor = colorClima;
            RenderSettings.ambientLight = colorClima * 0.5f;
        }
    }
}