using UnityEngine;
using UnityEngine.AI;

// Solo anima las piernas segun la velocidad del agente.
// El NavMeshAgent se encarga de mover y de girar. Nada mas. (Version simple original.)
public class NPCAnimacion : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agente;
    public string parametroVert = "Vert";
    public string parametroHor  = "Hor";
    public float suavizado = 0.12f;

    private float velocidadBase;

    void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (agente == null) agente = GetComponentInParent<NavMeshAgent>();
        if (animator != null) animator.applyRootMotion = false;
        if (agente != null) velocidadBase = agente.speed;
    }

    void Update()
    {
        if (animator == null || agente == null) return;

        float velNorm = 0f;
        if (velocidadBase > 0.01f)
            velNorm = Mathf.Clamp01(agente.velocity.magnitude / velocidadBase);

        animator.SetFloat(parametroVert, velNorm, suavizado, Time.deltaTime);
        animator.SetFloat(parametroHor, 0f, suavizado, Time.deltaTime);
    }
}
