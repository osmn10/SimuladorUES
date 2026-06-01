using UnityEngine;
using UnityEngine.AI; // Para el NavMeshAgent

// Hace que el NPC camine entre waypoints (puntos de interes) de forma estocastica
// Elige un waypoint al azar, camina hacia el, espera, y elige otro
public class NPCWander : MonoBehaviour
{
    [Header("Waypoints")]
    // Lista de puntos de interes a los que el NPC puede ir
    // Se asignan desde el Inspector arrastrando los WP_1, WP_2, etc.
    public Transform[] waypoints;

    [Header("Configuracion")]
    public float tiempoEsperaMin = 2f;   // Espera minima en cada punto
    public float tiempoEsperaMax = 6f;   // Espera maxima en cada punto
    public float distanciaLlegada = 2f;  // Que tan cerca debe estar para considerar que llego

    private NavMeshAgent agente;
    private int waypointActual = -1;     // Indice del waypoint al que va
    private float temporizador;
    private bool esperando = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        // Elegimos el primer destino al azar
        ElegirWaypointAleatorio();
    }

    void Update()
    {
        // Si no hay waypoints asignados, no hacemos nada
        if (waypoints == null || waypoints.Length == 0) return;

        // Verificamos si el NPC ya llego a su destino
        if (!esperando && !agente.pathPending && agente.remainingDistance <= distanciaLlegada)
        {
            // Llego: empezamos a esperar
            esperando = true;
            // Tiempo de espera aleatorio (estocastico)
            temporizador = Random.Range(tiempoEsperaMin, tiempoEsperaMax);
        }

        // Si esta esperando, contamos el tiempo
        if (esperando)
        {
            temporizador -= Time.deltaTime;

            // Cuando se acaba la espera, elegimos un nuevo destino
            if (temporizador <= 0f)
            {
                esperando = false;
                ElegirWaypointAleatorio();
            }
        }
    }

    void ElegirWaypointAleatorio()
    {
        if (waypoints.Length == 0) return;

        // Elegimos un waypoint al azar (estocastico)
        // Evitamos repetir el mismo waypoint dos veces seguidas
        int nuevoWaypoint;
        do
        {
            nuevoWaypoint = Random.Range(0, waypoints.Length);
        }
        while (nuevoWaypoint == waypointActual && waypoints.Length > 1);

        waypointActual = nuevoWaypoint;

        // Le decimos al agente que camine hacia ese waypoint
        if (waypoints[waypointActual] != null)
            agente.SetDestination(waypoints[waypointActual].position);
    }
}