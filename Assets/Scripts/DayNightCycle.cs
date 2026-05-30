using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    public Light sun;
    public Slider timeSlider;
    public TextMeshProUGUI timeText;
    public Camera mainCamera;

    [Header("Settings")]
    [Tooltip("Hora inicial del dia (0-24)")]
    public float currentHour = 12f;

    [Tooltip("Si esta activo, el tiempo avanza solo")]
    public bool autoAdvance = false;

    [Tooltip("Velocidad del tiempo automatico (horas por segundo)")]
    public float timeSpeed = 0.5f;

    [Header("Sky Colors")]
    public Color nightColor = new Color(0.05f, 0.05f, 0.15f);
    public Color sunsetColor = new Color(0.9f, 0.5f, 0.3f);
    public Color dayColor = new Color(0.5f, 0.7f, 1f);

    public Color colorCieloActual { get; private set; }

    private WeatherSystem weatherSystem;

    void Start()
    {
        weatherSystem = FindFirstObjectByType<WeatherSystem>();

        if (timeSlider != null)
        {
            timeSlider.value = currentHour;
            timeSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        UpdateSun();
        UpdateTimeText();
        UpdateSky();
    }

    void Update()
    {
        if (autoAdvance)
        {
            currentHour += timeSpeed * Time.deltaTime;
            if (currentHour >= 24f) currentHour = 0f;

            if (timeSlider != null)
                timeSlider.value = currentHour;

            UpdateSun();
            UpdateTimeText();
            UpdateSky();
        }
    }

    void OnSliderChanged(float newValue)
    {
        currentHour = newValue;
        UpdateSun();
        UpdateTimeText();
        UpdateSky();
    }

    void UpdateSun()
    {
        if (sun == null) return;

        float sunAngle = (currentHour / 24f) * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        if (currentHour >= 6f && currentHour <= 18f)
        {
            float dayProgress = Mathf.Sin((currentHour - 6f) / 12f * Mathf.PI);
            sun.intensity = Mathf.Lerp(0.3f, 1.5f, dayProgress);

            if (currentHour < 8f || currentHour > 17f)
                sun.color = new Color(1f, 0.7f, 0.4f);
            else
                sun.color = Color.white;
        }
        else
        {
            sun.intensity = 0.1f;
            sun.color = new Color(0.4f, 0.5f, 0.8f);
        }
    }

    void UpdateTimeText()
    {
        if (timeText == null) return;

        int hours = Mathf.FloorToInt(currentHour);
        int minutes = Mathf.FloorToInt((currentHour - hours) * 60f);

        timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    void UpdateSky()
    {
        if (mainCamera == null) return;

        Color skyColor;

        if (currentHour < 5f || currentHour >= 20f)
        {
            skyColor = nightColor;
        }
        else if (currentHour < 7f)
        {
            float t = Mathf.InverseLerp(5f, 7f, currentHour);
            skyColor = Color.Lerp(nightColor, sunsetColor, t);
        }
        else if (currentHour < 9f)
        {
            float t = Mathf.InverseLerp(7f, 9f, currentHour);
            skyColor = Color.Lerp(sunsetColor, dayColor, t);
        }
        else if (currentHour < 16f)
        {
            skyColor = dayColor;
        }
        else if (currentHour < 18f)
        {
            float t = Mathf.InverseLerp(16f, 18f, currentHour);
            skyColor = Color.Lerp(dayColor, sunsetColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(18f, 20f, currentHour);
            skyColor = Color.Lerp(sunsetColor, nightColor, t);
        }

        colorCieloActual = skyColor;

        if (weatherSystem == null)
        {
            mainCamera.backgroundColor = skyColor;
            RenderSettings.ambientLight = skyColor * 0.5f;
        }
    }
}