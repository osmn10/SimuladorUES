using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============================================================================
//  NPCSpawner (version simple)  —  Unidad 2 (eventos) + Unidad 4 (Poisson)
// ----------------------------------------------------------------------------
//  Genera estudiantes uno a uno. El tiempo ENTRE llegadas es una variable
//  EXPONENCIAL (proceso de Poisson, Unidad 4), usando nuestro generador LCG
//  (Unidad 3). Cuando se alcanza la poblacion maxima, deja de generar.
//  Los NPCs NO desaparecen: se quedan caminando por el campus.
// ============================================================================
public class NPCSpawner : MonoBehaviour
{
    [Header("Que se genera y donde")]
    public GameObject[] prefabsNPC;       // Prefabs de NPC (ej. hombre y mujer); elige uno al azar
    public Transform[] puntosAparicion;   // Puntos donde aparecen (sobre el NavMesh)
    public Transform[] waypoints;         // Destinos (edificios) que se pasan a cada NPC
    public Transform contenedor;          // _NPCs: donde se cuelgan los NPCs creados

    [Header("Parametros de la simulacion")]
    public int poblacionMax = 18;         // Cuantos estudiantes generar en total
    public float mediaEntreLlegadas = 3f; // Promedio de segundos entre llegadas

    [Header("Variedad de ropa (opcional)")]
    // Materiales de ropa (ej. PolygonStarter_01..04). Cada NPC toma uno al azar,
    // asi no se ven todos iguales. Si lo dejas vacio, no cambia nada.
    public Material[] materialesRopa;

    [Header("Reproducibilidad")]
    public long semilla = 12345;          // Semilla del LCG (misma semilla = misma corrida)

    private GeneradorLCG rng;
    private float tiempoProximaLlegada;
    private int generados = 0;            // Cuantos llevamos creados

    // Para que otros scripts (registro de datos, Unidad 5) sepan cuantos hay.
    public int PoblacionActual { get { return generados; } }

    void Start()
    {
        rng = new GeneradorLCG(semilla);
        // Planificamos la primera llegada (tiempo exponencial).
        tiempoProximaLlegada = Time.time + (float)rng.Exponencial(mediaEntreLlegadas);
    }

    void Update()
    {
        // Si todavia falta gente por generar y ya llego el momento de la proxima llegada...
        if (generados < poblacionMax && Time.time >= tiempoProximaLlegada)
        {
            GenerarNPC();
            // Planificamos la siguiente llegada (otro tiempo exponencial).
            tiempoProximaLlegada = Time.time + (float)rng.Exponencial(mediaEntreLlegadas);
        }
    }

    void GenerarNPC()
    {
        if (prefabsNPC == null || prefabsNPC.Length == 0 ||
            puntosAparicion == null || puntosAparicion.Length == 0)
            return;

        // Elegimos al azar cual modelo de NPC generar (hombre, mujer, etc.).
        GameObject prefabNPC = prefabsNPC[rng.Rango(0, prefabsNPC.Length)];
        if (prefabNPC == null) return;

        // Elegimos un punto de aparicion al azar.
        int idx = rng.Rango(0, puntosAparicion.Length);
        Transform punto = puntosAparicion[idx];

        // Dispersamos un poco la aparicion para que no salgan amontonados,
        // y ajustamos la posicion al NavMesh mas cercano.
        Vector3 desplazamiento = new Vector3(
            (float)(rng.SiguienteUniforme() * 2 - 1), 0f,
            (float)(rng.SiguienteUniforme() * 2 - 1)) * 4f;
        Vector3 posicion = punto.position + desplazamiento;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(posicion, out hit, 12f, NavMesh.AllAreas))
            posicion = hit.position;

        // Calculamos hacia donde mira al aparecer: hacia el centro de los waypoints
        // (el interior del campus). Asi su primer destino queda casi de frente y
        // NO da una gran vuelta al aparecer.
        Quaternion rotacionInicial = punto.rotation;
        Vector3 centro = CentroWaypoints();
        Vector3 haciaCentro = centro - posicion;
        haciaCentro.y = 0f;
        if (haciaCentro.sqrMagnitude > 0.01f)
            rotacionInicial = Quaternion.LookRotation(haciaCentro);

        // Creamos el NPC.
        GameObject go = Instantiate(prefabNPC, posicion, rotacionInicial);
        if (contenedor != null) go.transform.SetParent(contenedor);

        // Le pasamos los waypoints (destinos) a su script de deambular.
        NPCWander wander = go.GetComponent<NPCWander>();
        if (wander != null) wander.waypoints = waypoints;

        // Prioridad de evasion variada para que no se traben entre si.
        NavMeshAgent agente = go.GetComponent<NavMeshAgent>();
        if (agente != null) agente.avoidancePriority = rng.Rango(20, 80);

        // Variedad de ropa: le ponemos un material al azar a todas las mallas del NPC.
        if (materialesRopa != null && materialesRopa.Length > 0)
        {
            Material mat = materialesRopa[rng.Rango(0, materialesRopa.Length)];
            if (mat != null)
            {
                foreach (SkinnedMeshRenderer smr in go.GetComponentsInChildren<SkinnedMeshRenderer>())
                    smr.sharedMaterial = mat;
            }
        }

        generados++;
    }

    // Devuelve el punto promedio (centro) de todos los waypoints.
    Vector3 CentroWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0) return Vector3.zero;
        Vector3 suma = Vector3.zero;
        int n = 0;
        foreach (Transform w in waypoints)
        {
            if (w == null) continue;
            suma += w.position;
            n++;
        }
        return n > 0 ? suma / n : Vector3.zero;
    }
}

