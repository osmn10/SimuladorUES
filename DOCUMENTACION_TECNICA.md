# SimuladorUES — Documentación Técnica

**Proyecto:** Técnicas de Simulación (TDS115) — Avance 1
**Tema:** Simulación de un ecosistema (Paseo virtual por la FIA UES)
**Repositorio:** https://github.com/osmn10/SimuladorUES

---

## 1. Setup del proyecto

### 1.1 Software requerido

| Software | Versión | Uso |
|---|---|---|
| Unity | 6.4 (6000.4.3f1) | Motor del simulador |
| Unity Hub | Última | Gestión de versiones de Unity |
| GitHub Desktop | Última | Control de versiones (alternativa a línea de comandos) |
| Git LFS | 3.4.0+ | Manejo de archivos grandes (modelos 3D, texturas) |
| Visual Studio Code o Visual Studio | Última | Edición de scripts C# |

### 1.2 Estructura del repositorio

El proyecto vive en `C:\ProyectoTDS\SimuladorUES\` (Windows) o `/Users/[usuario]/ProyectoTDS/SimuladorUES/` (Mac). **No debe estar dentro de OneDrive, iCloud o cualquier carpeta sincronizada**, porque corrompe el proyecto Unity.

**Archivos versionados en Git:**
- `Assets/` — todo lo que crea o importa Unity
- `Packages/` — dependencias del proyecto
- `ProjectSettings/` — configuración del proyecto
- `.gitignore` — define qué NO subir
- `.gitattributes` — define qué archivos usan LFS

**Carpetas que Git ignora (Unity las regenera):**
- `Library/`
- `Temp/`
- `Logs/`
- `obj/`
- `Build/`

### 1.3 Git LFS configurado

Git LFS permite manejar archivos binarios grandes (modelos 3D, texturas, audio). Sin LFS, GitHub rechazaría el push del proyecto con assets.

Tipos de archivo en LFS (ver `.gitattributes`):
`.psd`, `.fbx`, `.obj`, `.blend`, `.png`, `.jpg`, `.tga`, `.exr`, `.wav`, `.mp3`, `.mp4`, `.unity`, `.prefab`, `.asset`, `.anim`, `.controller`, `.bytes`

### 1.4 Cómo clonar el proyecto (para nuevos miembros del equipo)

1. Instalar GitHub Desktop: https://desktop.github.com
2. Instalar Git LFS: https://git-lfs.com
3. En GitHub Desktop: **File → Clone Repository → osmn10/SimuladorUES**
4. Local path: `C:\ProyectoTDS\` (o ruta equivalente fuera de OneDrive)
5. Clone (descarga ~300 MB)
6. Instalar Unity 6.4 desde Unity Hub
7. Unity Hub → **Open Project** → seleccionar la carpeta clonada
8. Esperar que Unity importe assets (5-10 min la primera vez)

---

## 2. Assets importados

### 2.1 Starter Assets — Character Controllers (URP)

**Origen:** Unity Asset Store (gratis)
**Ubicación en proyecto:** `Assets/Starter Assets/`
**Función:** Provee el personaje 3D (PlayerArmature), su sistema de movimiento (WASD, mouse, salto, correr) y la cámara de tercera persona.

**Componentes clave:**
- `PlayerArmature` (prefab) — el personaje con esqueleto y animaciones
- `PlayerFollowCamera` (prefab) — Cinemachine Virtual Camera que sigue al jugador
- `MainCamera` (prefab) — la cámara que renderiza
- Scripts internos: `ThirdPersonController.cs`, `BasicRigidBodyPush.cs`, `StarterAssetsInputs.cs`

### 2.2 POLYGON Starter Pack (Synty Studios)

**Origen:** Synty Store (gratis con cuenta)
**Ubicación:** `Assets/Synty/`
**Función:** Conjunto de assets low-poly para construir el campus (edificios, vehículos, props urbanos).

### 2.3 City People FREE Samples (DenysAlmaral)

**Origen:** Unity Asset Store (gratis)
**Ubicación:** `Assets/DenysAlmaral/CityPeople/`
**Función:** Modelos 3D de personajes urbanos (hombres, mujeres, ancianos) para usar como NPCs estudiantes/catedráticos.

### 2.4 Creative Characters FREE — Animated Pack (ithappy)

**Origen:** Unity Asset Store (gratis)
**Ubicación:** `Assets/ithappy/`
**Función:** Personajes con animaciones ya integradas. Útiles para NPCs sin necesidad de Mixamo.

---

## 3. Configuración crítica del proyecto

### 3.1 Render Pipeline: URP

El proyecto usa **Universal Render Pipeline** (no Built-in). Los Starter Assets requieren URP. Cualquier asset que se importe debe ser compatible con URP.

### 3.2 Active Input Handling: Both

Ubicación: **Edit → Project Settings → Player → Other Settings → Active Input Handling**

Valor: **Both** (importante)

Esto permite usar tanto el sistema de Input legacy (`Input.GetKeyDown`) como el nuevo Input System en paralelo. Si solo se deja "Input System Package", el script `CameraSwitcher.cs` falla porque usa la API legacy.

---

## 4. Escena principal: CampusUES

**Ubicación:** `Assets/Scenes/CampusUES.unity`

### 4.1 Estructura de la jerarquía

```
CampusUES
├── Directional Light          ← El "sol" del simulador
├── PlayerArmature             ← Personaje del jugador
│   ├── PlayerCameraRoot       ← Punto al que apunta la cámara
│   │   └── FirstPersonCamera  ← Cámara de primera persona (desactivada por defecto)
│   ├── Geometry               ← Malla visual del personaje
│   └── Skeleton               ← Esqueleto para animaciones
├── PlayerFollowCamera         ← Cinemachine que sigue al PlayerCameraRoot
├── MainCamera                 ← Cámara que renderiza al monitor
├── Plane                      ← Piso con textura del mapa satelital de FIA
├── CameraManager              ← GameObject con script CameraSwitcher
├── Canvas                     ← Contenedor de UI
│   ├── Slider                 ← Control del tiempo (0-24 horas)
│   └── Text (TMP)             ← Texto que muestra "HH:MM"
├── EventSystem                ← Necesario para que la UI responda a clicks
└── DayNightManager            ← GameObject con script DayNightCycle
```

### 4.2 Mapa satelital como piso

Captura satelital de la FIA UES tomada de Google Maps, importada como `Assets/mapa_fia.png` y aplicada como textura del Plane.

**Configuración del Plane:**
- Position: 0, 0, 0
- Scale: 30, 1, 30 (300×300 metros)
- Material: `mapa_fia (Material)` con shader URP/Lit

---

## 5. Funciones implementadas

### 5.1 F1 — Movimiento WASD (sin código propio)

Implementado por Starter Assets. El componente `ThirdPersonController.cs` lee la entrada del teclado y mueve al `PlayerArmature` con un `CharacterController` físico.

**Mapeo:**
- W → adelante
- A → izquierda
- S → atrás
- D → derecha

### 5.2 F2 — Cámara con mouse (sin código propio)

Implementado por Cinemachine + Starter Assets. El `PlayerFollowCamera` tiene asignado `PlayerCameraRoot` como **Follow target**. El movimiento del mouse rota la cámara alrededor del punto de seguimiento.

**Configuración:**
- En `PlayerFollowCamera` → Cinemachine Virtual Camera → **Follow** = PlayerCameraRoot

### 5.3 F3 — Correr con Shift (sin código propio)

Implementado por Starter Assets. El `ThirdPersonController` aumenta la velocidad cuando se mantiene Shift.

### 5.4 F4 — Saltar con Espacio (sin código propio)

Implementado por Starter Assets. El `ThirdPersonController` aplica fuerza vertical al personaje cuando detecta que está en el suelo y se presiona Space.

### 5.5 F5 — Cambio cámara primera/tercera persona (script propio)

**Tecla:** V

**Cómo funciona:**

Hay dos cámaras en la escena:
1. `MainCamera` (tercera persona, activa por defecto)
2. `FirstPersonCamera` (primera persona, hijo de PlayerCameraRoot, desactivada por defecto)

El script `CameraSwitcher.cs` (componente del `CameraManager`) detecta la tecla V y activa una cámara mientras desactiva la otra.

**Script: `Assets/Scripts/CameraSwitcher.cs`**

```csharp
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    [Header("Settings")]
    public KeyCode switchKey = KeyCode.V;

    private bool isFirstPerson = false;

    void Start()
    {
        // Iniciar en tercera persona
        SetCameraMode(false);
    }

    void Update()
    {
        // Detectar tecla V
        if (Input.GetKeyDown(switchKey))
        {
            isFirstPerson = !isFirstPerson;
            SetCameraMode(isFirstPerson);
        }
    }

    void SetCameraMode(bool firstPerson)
    {
        if (firstPersonCamera != null)
            firstPersonCamera.gameObject.SetActive(firstPerson);

        if (thirdPersonCamera != null)
            thirdPersonCamera.gameObject.SetActive(!firstPerson);
    }
}
```

**Inspector del CameraManager:**
- First Person Camera → arrastrar `FirstPersonCamera` desde Hierarchy
- Third Person Camera → arrastrar `MainCamera` desde Hierarchy
- Switch Key → V

**Configuración de FirstPersonCamera:**
- Es un GameObject hijo de `PlayerCameraRoot`
- Tiene un componente Camera
- Position: X=0, Y=0.5, Z=0.2 (ajustada a la altura de los ojos)
- Por defecto: desactivada (checkbox del Inspector desmarcado)

### 5.6 F10 — Ciclo día/noche con slider (script propio)

**Cómo funciona:**

El script `DayNightCycle.cs` (componente del `DayNightManager`) controla 4 elementos de la escena según una variable `currentHour` (0 a 24):

1. **Rotación del sol** (Directional Light) — calculada como `(hora/24) * 360 - 90`
2. **Intensidad y color del sol** — varía con curva senoidal: tenue al amanecer/atardecer, fuerte al mediodía, casi apagada de noche
3. **Color del cielo** (background de MainCamera) — interpola entre 3 colores según fases del día
4. **Texto de hora en pantalla** — formato "HH:MM"

El usuario controla el tiempo con un Slider de 0 a 24, o activa `autoAdvance` para que avance solo.

**Configuración crítica de la cámara:**

MainCamera → Environment → **Background Type: Solid Color** (no Skybox). Esto permite que el script cambie el color del fondo dinámicamente.

**Script completo: `Assets/Scripts/DayNightCycle.cs`**

```csharp
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

    void Start()
    {
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

        mainCamera.backgroundColor = skyColor;
        RenderSettings.ambientLight = skyColor * 0.5f;
    }
}
```

**Inspector del DayNightManager:**
- Sun → arrastrar `Directional Light`
- Time Slider → arrastrar `Slider` (hijo de Canvas)
- Time Text → arrastrar `Text (TMP)` (hijo de Canvas)
- Main Camera → arrastrar `MainCamera`
- Current Hour: 12 (mediodía)
- Auto Advance: ☑ (avance automático)
- Time Speed: 0.2 (cada segundo real = 12 minutos simulados)

**Lógica de fases del cielo:**

| Hora | Fase | Color resultante |
|---|---|---|
| 0-5 | Noche profunda | Azul oscuro |
| 5-7 | Amanecer | Transición noche → naranja |
| 7-9 | Mañana | Transición naranja → azul cielo |
| 9-16 | Día pleno | Azul cielo |
| 16-18 | Tarde | Transición azul → naranja |
| 18-20 | Atardecer | Transición naranja → noche |
| 20-24 | Noche profunda | Azul oscuro |

---

## 6. Conceptos clave de Unity (para el equipo)

### 6.1 GameObject y Componentes

Un **GameObject** es cualquier elemento de la escena (luces, cámaras, modelos, controles vacíos). Por sí solo no hace nada — su comportamiento viene de los **Componentes** que le agregas.

Ejemplo: el `PlayerArmature` es un GameObject que tiene componentes:
- Transform (posición/rotación/escala)
- CharacterController (colisiones)
- Animator (animaciones)
- ThirdPersonController (script de movimiento)

### 6.2 Prefabs

Un **Prefab** es una plantilla de GameObject que se puede instanciar múltiples veces. Si modificas el Prefab original, todos los instanciados se actualizan automáticamente. Útil para edificios, NPCs, props.

### 6.3 Scripts (MonoBehaviour)

Toda clase que herede de `MonoBehaviour` puede pegarse como componente a un GameObject. Métodos especiales:

- `Start()` — se ejecuta una vez al inicio
- `Update()` — se ejecuta cada frame (~60 veces/seg)
- `Awake()` — se ejecuta antes que Start, útil para inicializaciones
- `OnTriggerEnter(Collider other)` — se ejecuta cuando otro objeto entra al collider de este

### 6.4 Variables públicas vs privadas

- `public` → aparece en el Inspector, se puede asignar desde el editor (arrastrando objetos)
- `private` → solo accesible internamente desde el script

Usa `[SerializeField] private` cuando quieras que aparezca en el Inspector pero no sea accesible desde otros scripts.

### 6.5 Time.deltaTime

Es el tiempo en segundos desde el último frame. Multiplicar valores por `Time.deltaTime` los hace independientes del frame rate. Sin esto, en un PC más rápido las cosas se moverían más rápido.

```csharp
// Mal (depende del FPS):
position += 5f;

// Bien (5 unidades por segundo, sin importar FPS):
position += 5f * Time.deltaTime;
```

### 6.6 Mathf.Lerp y Color.Lerp

Interpolación lineal. Mezcla dos valores según un factor t (entre 0 y 1):
- t=0 → 100% del primer valor
- t=1 → 100% del segundo valor
- t=0.5 → mezcla 50/50

Útil para transiciones suaves.

---

## 7. Estado del Avance 1 (al cierre de la sesión actual)

### 7.1 Funciones implementadas

✅ F1 — Movimiento WASD
✅ F2 — Cámara con mouse
✅ F3 — Correr con Shift
✅ F4 — Saltar con Espacio
✅ F5 — Cambio cámara primera/tercera persona (V)
✅ F10 — Ciclo día/noche con slider, texto de hora y cielo dinámico

**6 de 15 funciones implementadas.** La rúbrica pide mínimo 5 para el Avance 1.

### 7.2 Pendiente para el Avance 1 (entrega 3 de mayo)

- [ ] Modelar el campus FIA con assets de Synty (5 edificios mínimo)
- [ ] F6 — Panel de información al acercarse a edificio
- [ ] Mejoras estéticas a la UI (slider y texto)
- [ ] Documento Word con: portada, índice, objetivos, 15 funciones documentadas, 15 referencias APA, capturas
- [ ] Video de máximo 6 minutos explicando el funcionamiento

### 7.3 Pendiente para entrega final (después del Avance 1)

- [ ] F7 — Mini-mapa en esquina
- [ ] F8 — Marcadores flotantes sobre edificios
- [ ] F9 — Brújula en HUD
- [ ] F11 — NPCs estudiantes caminando con NavMesh
- [ ] F12 — Sistema de clima (lluvia, nublado, despejado)
- [ ] F13 — Sonido ambiental
- [ ] F14 — Teletransporte rápido entre edificios
- [ ] F15 — Modo cámara libre

---

## 8. Edificios objetivo de la FIA

Los 5 edificios prioritarios para el simulador (identificables en el mapa satelital):

1. **Biblioteca FIA-UES** — esquina suroeste
2. **Edificio A** — sur del campus
3. **Edificio B (Aulas)** — centro del campus
4. **Edificio D** — centro-este del campus
5. **Auditorio Miguel Mármol** — sur-suroeste, junto a Edificio A

Edificios adicionales si hay tiempo (también visibles en el mapa):
- Escuela de Ingeniería Civil
- Escuela de Ingeniería Industrial (EII)
- Escuela de Ingeniería Eléctrica (EIE)
- Edificio de Potencia
- Centro de Investigación de Metrología (CIM)
- Administración Académica FIA
- Escuela de Posgrado
- Unidad de Ciencias Básicas
- Laboratorio F-2

---

## 9. Workflow de Git para el equipo

### 9.1 Antes de empezar a trabajar (cada vez)

1. Abrir GitHub Desktop
2. Click en **Fetch origin** → si dice **Pull origin**, click ahí también
3. Esto baja los cambios que otros hicieron
4. Abrir Unity y trabajar normal

### 9.2 Después de trabajar (cada vez)

1. Guardar la escena en Unity (Ctrl+S)
2. Cerrar Unity (recomendado para evitar locks)
3. Abrir GitHub Desktop
4. Verás los archivos modificados en la pestaña **Changes**
5. Escribir un Summary descriptivo (ejemplo: "Agregar Edificio B al campus")
6. Click **Commit to main**
7. Click **Push origin**

### 9.3 Regla de oro

**Nunca trabajar simultáneamente en la misma escena con otra persona.** Los archivos `.unity` y `.prefab` son binarios y no se pueden mergear automáticamente. Si dos personas modifican la escena al mismo tiempo, uno pierde su trabajo.

**Coordinarse así:**
- Solo una persona modifica `CampusUES.unity` a la vez
- Los demás trabajan en scripts (carpeta `Scripts/`) o en assets nuevos (que no modifican la escena)
- Antes de empezar, avisar al equipo "voy a trabajar en la escena"
- Al terminar, push inmediato y avisar "ya pueden continuar"

---

## 10. Recursos útiles

### Documentación Unity
- Unity Manual: https://docs.unity3d.com/Manual/index.html
- Scripting API: https://docs.unity3d.com/ScriptReference/

### Tutoriales recomendados
- Starter Assets — Third Person Controller: https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526
- Cinemachine: https://docs.unity3d.com/Packages/com.unity.cinemachine@2.10/manual/index.html

### Para el documento del Avance 1
- Rúbrica oficial del proyecto (PDF en aula virtual)
- Norma APA 7ma edición para citas y referencias

---

**Última actualización:** 28 de abril de 2026
**Sesión documentada:** Setup completo + F1-F5 + F10 con cielo dinámico
