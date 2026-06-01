using UnityEngine;

// Herramienta para agregar colliders en masa a un grupo de objetos
// Se ejecuta una vez con un boton, no en tiempo real
public class AgregarColliders : MonoBehaviour
{
    [Header("Tipo de collider a agregar")]
    public bool usarBoxCollider = true;  // true = BoxCollider, false = MeshCollider

    [ContextMenu("Agregar Colliders a Hijos")]
    public void AgregarCollidersAHijos()
    {
        // Recorremos todos los MeshRenderer hijos (objetos visibles)
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        int agregados = 0;

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject obj = renderer.gameObject;

            // Si ya tiene un collider, lo saltamos
            if (obj.GetComponent<Collider>() != null)
                continue;

            // Agregamos el tipo de collider elegido
            if (usarBoxCollider)
            {
                obj.AddComponent<BoxCollider>();
            }
            else
            {
                MeshCollider mc = obj.AddComponent<MeshCollider>();
                mc.convex = false; // false para objetos estaticos
            }

            agregados++;
        }

        Debug.Log("Colliders agregados: " + agregados);
    }
}