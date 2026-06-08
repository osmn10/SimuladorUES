using UnityEngine;
using UnityEngine.AI;

// Hace que el NPC camine entre waypoints (puntos de interes) de forma estocastica
// Elige un waypoint al azar, camina hacia el, espera, y elige otro
public class NPCWander : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Configuracion")]
    public float tiempoEsperaMin = 2f;
    public float tiempoEsperaMax = 6f;
    public float distanciaLlegada = 2f;

    private NavMeshAgent agente;
    private int waypointActual = -1;
    private float temporizador;
    private bool esperando = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        ElegirWaypointAleatorio();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Proteccion: si el agente todavia no esta sobre el NavMesh, no hacemos nada.
        // Evita el error "GetRemainingDistance... placed on a NavMesh".
        if (agente == null || !agente.isOnNavMesh) return;

        if (!esperando && !agente.pathPending && agente.remainingDistance <= distanciaLlegada)
        {
            esperando = true;
            temporizador = Random.Range(tiempoEsperaMin, tiempoEsperaMax);
        }

        if (esperando)
        {
            temporizador -= Time.deltaTime;
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

        int nuevoWaypoint;
        do
        {
            nuevoWaypoint = Random.Range(0, waypoints.Length);
        }
        while (nuevoWaypoint == waypointActual && waypoints.Length > 1);

        waypointActual = nuevoWaypoint;

        if (waypoints[waypointActual] != null)
            agente.SetDestination(waypoints[waypointActual].position);
    }
}
