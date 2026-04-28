// "using" sirve para importar bibliotecas (codigo ya escrito que vamos a usar)
using UnityEngine;       // Biblioteca principal de Unity (Light, Transform, Camera, etc.)
using UnityEngine.UI;    // Biblioteca para componentes de UI clasicos (Slider, Button, etc.)
using TMPro;             // Biblioteca para TextMeshPro (texto de mejor calidad)


// Esta clase es un componente que se puede agregar a un GameObject en Unity
// "MonoBehaviour" es la clase base de todos los scripts de Unity
public class DayNightCycle : MonoBehaviour
{
    // ========== VARIABLES PUBLICAS (visibles en el Inspector de Unity) ==========

    // [Header("texto")] crea un titulo en el Inspector para organizar visualmente
    [Header("References")]

    // "public" hace que la variable aparezca en el Inspector de Unity
    // "Light" es el tipo: una luz de Unity (en este caso sera el sol)
    public Light sun;

    // "Slider" es el control deslizante que ya creamos en la UI
    public Slider timeSlider;

    // "TextMeshProUGUI" es el tipo del texto en pantalla (TextMeshPro version UI)
    // Aqui guardaremos la referencia al texto que muestra la hora "12:00"
    public TextMeshProUGUI timeText;

    // Referencia a la camara principal para cambiar el color del cielo
    // Se asigna manualmente en el Inspector arrastrando MainCamera
    public Camera mainCamera;


    [Header("Settings")]

    // [Tooltip("texto")] muestra una ayuda cuando pasas el mouse sobre la variable en el Inspector
    [Tooltip("Hora inicial del dia (0-24)")]

    // "float" es un tipo de numero con decimales
    // currentHour empieza en 12f (mediodia). La "f" indica que es float
    public float currentHour = 12f;


    [Tooltip("Si esta activo, el tiempo avanza solo")]

    // "bool" es un tipo de variable que solo puede ser true (verdadero) o false (falso)
    // Si autoAdvance es true, el tiempo pasara automaticamente
    public bool autoAdvance = false;


    [Tooltip("Velocidad del tiempo automatico (horas por segundo)")]
    // 0.5f significa que cada segundo real avanza media hora simulada
    public float timeSpeed = 0.5f;


    // Colores del cielo segun la hora del dia
    // Cada [Header] organiza visualmente las variables en el Inspector
    [Header("Sky Colors")]

    // Color cuando es de noche profunda (medianoche)
    // "Color" es un tipo de Unity con 4 valores: Rojo, Verde, Azul, Alpha (transparencia)
    // Los valores van de 0 a 1 (0 = nada, 1 = al maximo)
    public Color nightColor = new Color(0.05f, 0.05f, 0.15f);     // Azul muy oscuro

    // Color cuando es amanecer o atardecer
    public Color sunsetColor = new Color(0.9f, 0.5f, 0.3f);       // Naranja calido

    // Color cuando es mediodia
    public Color dayColor = new Color(0.5f, 0.7f, 1f);            // Azul cielo


    // ========== METODOS DE UNITY ==========

    // "Start()" es un metodo especial de Unity que se ejecuta UNA SOLA VEZ
    // cuando el juego empieza (cuando le das Play o cuando se crea el objeto)
    void Start()
    {
        // Verificamos que el slider este asignado (no sea null/vacio)
        // Esto evita errores si olvidaste vincularlo en el Inspector
        if (timeSlider != null)
        {
            // Le decimos al slider que su valor inicial sea la hora actual
            timeSlider.value = currentHour;

            // Le decimos al slider: "cuando el usuario te mueva,
            // llama a la funcion OnSliderChanged"
            // Es como suscribirse a un evento
            timeSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        // Llamamos a las funciones de actualizacion para que todo este sincronizado al inicio
        UpdateSun();          // Posicionar el sol
        UpdateTimeText();     // Mostrar la hora en pantalla
        UpdateSky();          // Pintar el cielo del color correcto
    }


    // "Update()" es un metodo especial de Unity que se ejecuta CADA FRAME
    // (60 veces por segundo aproximadamente, depende del rendimiento)
    void Update()
    {
        // Si autoAdvance esta activado, hacer que el tiempo pase solo
        if (autoAdvance)
        {
            // Time.deltaTime es el tiempo en segundos desde el ultimo frame
            // Lo multiplicamos por timeSpeed para avanzar la hora suavemente
            currentHour += timeSpeed * Time.deltaTime;

            // Si paso de las 24 horas, volvemos a 0 (medianoche del nuevo dia)
            if (currentHour >= 24f) currentHour = 0f;

            // Actualizar el slider visualmente para que coincida con la hora
            if (timeSlider != null)
                timeSlider.value = currentHour;

            // Actualizar todo lo que depende de la hora
            UpdateSun();          // Sol
            UpdateTimeText();     // Texto en pantalla
            UpdateSky();          // Color del cielo
        }
    }


    // ========== METODOS PROPIOS ==========

    // Esta funcion se ejecuta cada vez que el usuario mueve el slider
    // El parametro "newValue" es el nuevo valor del slider (entre 0 y 24)
    void OnSliderChanged(float newValue)
    {
        currentHour = newValue;  // Actualizar la hora con el valor del slider
        UpdateSun();             // Recalcular la posicion y color del sol
        UpdateTimeText();        // Actualizar el texto en pantalla
        UpdateSky();             // Actualizar el color del cielo
    }


    // Esta funcion mueve el sol y cambia su color/intensidad segun la hora
    void UpdateSun()
    {
        // Si no hay sol asignado, salir de la funcion para evitar errores
        if (sun == null) return;


        // ========== ROTACION DEL SOL ==========

        // Calculamos el angulo del sol segun la hora del dia:
        // Hora 6 (6am)  -> angulo 0   -> sol en el horizonte este (amanecer)
        // Hora 12 (12pm) -> angulo 90  -> sol arriba (mediodia)
        // Hora 18 (6pm) -> angulo 180 -> sol en el horizonte oeste (atardecer)
        // Hora 0  (12am) -> angulo 270 -> sol abajo (medianoche)
        float sunAngle = (currentHour / 24f) * 360f - 90f;

        // Aplicamos la rotacion al transform del sol
        // Quaternion.Euler convierte 3 angulos (X, Y, Z) en una rotacion
        // Y = 170 le da un angulo lateral para que el sol no salga exactamente perpendicular
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);


        // ========== INTENSIDAD Y COLOR DEL SOL ==========

        // Es de DIA si la hora esta entre 6am (6) y 6pm (18)
        if (currentHour >= 6f && currentHour <= 18f)
        {
            // Calculamos la "progresion" del dia:
            // - 6am y 6pm = 0 (sol en el horizonte, luz debil)
            // - 12pm = 1 (sol arriba, luz al maximo)
            // Usamos seno (Sin) para que la transicion sea suave (curva)
            float dayProgress = Mathf.Sin((currentHour - 6f) / 12f * Mathf.PI);

            // Mathf.Lerp interpola entre dos valores segun el progreso (0 a 1)
            // 0.3 = luz minima, 1.5 = luz maxima
            sun.intensity = Mathf.Lerp(0.3f, 1.5f, dayProgress);


            // Color del sol segun la hora:
            // Amanecer (6am-8am) y atardecer (5pm-6pm) = naranja calido
            // Mediodia (8am-5pm) = blanco brillante
            if (currentHour < 8f || currentHour > 17f)
            {
                // new Color(rojo, verde, azul) en escala 0-1
                // 1, 0.7, 0.4 = naranja calido
                sun.color = new Color(1f, 0.7f, 0.4f);
            }
            else
            {
                // Color.white es un atajo para new Color(1, 1, 1) = blanco puro
                sun.color = Color.white;
            }
        }
        else
        {
            // Es de NOCHE (entre 6pm y 6am)
            // Luz muy tenue para simular noche
            sun.intensity = 0.1f;

            // Color azulado para simular luz de luna
            // 0.4, 0.5, 0.8 = azul oscuro
            sun.color = new Color(0.4f, 0.5f, 0.8f);
        }
    }


    // Esta funcion convierte el numero de hora (ej. 14.5) a texto formato "14:30"
    // y lo muestra en el TextMeshPro asignado en el Inspector
    void UpdateTimeText()
    {
        // Si no hay texto asignado, salir para evitar errores
        if (timeText == null) return;

        // Mathf.FloorToInt convierte un float a int redondeando hacia abajo
        // Ejemplo: 14.7 -> 14
        // hours guarda la parte de "horas completas"
        int hours = Mathf.FloorToInt(currentHour);

        // Calculamos los minutos:
        // 1. Restamos las horas completas para quedarnos con la fraccion: 14.5 - 14 = 0.5
        // 2. Multiplicamos por 60 para convertir esa fraccion a minutos: 0.5 * 60 = 30 minutos
        int minutes = Mathf.FloorToInt((currentHour - hours) * 60f);

        // string.Format crea un texto con los valores insertados
        // "{0:00}" significa: insertar el primer valor con minimo 2 digitos (rellena con 0)
        // Por ejemplo: hours=8, minutes=5 -> "08:05"
        // {0:00}:{1:00}
        //   |     |
        //   |     +-- segundo valor (minutes), 2 digitos minimo
        //   +-- primer valor (hours), 2 digitos minimo
        timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }


    // Esta funcion calcula el color del cielo segun la hora actual
    // y lo aplica al "Background" de la camara y a la luz ambiental del mundo
    void UpdateSky()
    {
        // Si no hay camara asignada, salir para evitar errores
        if (mainCamera == null) return;

        // Variable que guardara el color final del cielo (lo definiremos abajo)
        Color skyColor;


        // Decidimos el color segun la hora del dia
        // El cielo tiene varias fases:
        // 1. Madrugada (0-5):    azul oscuro (color de noche)
        // 2. Amanecer (5-7):     transicion noche -> atardecer (naranja)
        // 3. Manana (7-9):       transicion atardecer -> dia (azul cielo)
        // 4. Dia (9-16):         azul cielo (color de dia)
        // 5. Tarde (16-18):      transicion dia -> atardecer (naranja)
        // 6. Atardecer (18-20):  transicion atardecer -> noche (azul oscuro)
        // 7. Noche (20-24):      azul oscuro (color de noche)

        if (currentHour < 5f || currentHour >= 20f)
        {
            // Es de noche: usar color de noche directamente
            skyColor = nightColor;
        }
        else if (currentHour < 7f)
        {
            // Amanecer (5am-7am): transicion de noche a naranja
            // Mathf.InverseLerp calcula que tan avanzada esta la transicion (0-1)
            // Ejemplo: si son las 6am (mitad entre 5 y 7), t = 0.5
            float t = Mathf.InverseLerp(5f, 7f, currentHour);

            // Color.Lerp mezcla dos colores segun el factor t (0 a 1)
            // t=0 -> 100% nightColor, t=1 -> 100% sunsetColor, t=0.5 -> mezcla 50/50
            skyColor = Color.Lerp(nightColor, sunsetColor, t);
        }
        else if (currentHour < 9f)
        {
            // Manana (7am-9am): transicion de naranja a azul cielo
            float t = Mathf.InverseLerp(7f, 9f, currentHour);
            skyColor = Color.Lerp(sunsetColor, dayColor, t);
        }
        else if (currentHour < 16f)
        {
            // Dia pleno (9am-4pm): azul cielo constante
            skyColor = dayColor;
        }
        else if (currentHour < 18f)
        {
            // Tarde (4pm-6pm): transicion de azul a naranja
            float t = Mathf.InverseLerp(16f, 18f, currentHour);
            skyColor = Color.Lerp(dayColor, sunsetColor, t);
        }
        else
        {
            // Atardecer (6pm-8pm): transicion de naranja a azul oscuro
            float t = Mathf.InverseLerp(18f, 20f, currentHour);
            skyColor = Color.Lerp(sunsetColor, nightColor, t);
        }


        // Aplicar el color calculado al fondo de la camara
        // backgroundColor es el color que se ve "atras" de todo cuando la camara
        // esta configurada como "Solid Color" en lugar de "Skybox"
        mainCamera.backgroundColor = skyColor;

        // Tambien cambiamos la "luz ambiental" del mundo para coherencia
        // RenderSettings es una clase global de Unity para configurar el render
        // ambientLight es la luz que recibe TODO desde todas las direcciones
        // (sin esto, los objetos en sombra se verian super oscuros)
        // Multiplicamos por 0.5 para que no sea tan intensa
        RenderSettings.ambientLight = skyColor * 0.5f;
    }
}