using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetectorEdificios : MonoBehaviour
{
    [Header("Referencias de la UI")]
    public GameObject panelGlobo;
    public Image imagenFachada;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoUbicacion;
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoUso;
    public TextMeshProUGUI textoDatoCurioso;

    private void Start()
    {
        // Diagnóstico 1: Saber si el script del jugador está cargado en memoria
        Debug.Log("🤖 [SISTEMA] DetectorEdificios inicializado correctamente en: " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Diagnóstico 2: Imprimir ABSOLUTAMENTE TODO lo que el jugador toque
        Debug.Log("💥 [COLISIÓN DETECTADA] Acabo de entrar en contacto con el objeto: " + other.gameObject.name);

        // Buscamos el script de datos en el objeto que tocamos, o en sus padres/hijos por si acaso
        DatosEdificio datos = other.GetComponent<DatosEdificio>();
        if (datos == null) datos = other.GetComponentInParent<DatosEdificio>();
        if (datos == null) datos = other.GetComponentInChildren<DatosEdificio>();

        if (datos != null)
        {
            Debug.Log("📝 [DATOS ENCONTRADOS] Cargando información de: " + datos.nombreEdificio);

            // Inyectar datos en la UI
            if (imagenFachada != null && datos.fotoFachada != null) imagenFachada.sprite = datos.fotoFachada;
            if (textoNombre != null) textoNombre.text = datos.nombreEdificio;
            if (textoUbicacion != null) textoUbicacion.text = "<b>📍 Ubicación</b>\n" + datos.ubicacion;
            if (textoDescripcion != null) textoDescripcion.text = "<b>📝 Descripción</b>\n" + datos.descripcion;
            if (textoUso != null) textoUso.text = "<b>🎓 Uso</b>\n" + datos.uso;
            if (textoDatoCurioso != null) textoDatoCurioso.text = "<b>• Dato curioso</b>\n" + datos.datoCurioso;

            // Mostrar el globo
            if (panelGlobo != null) panelGlobo.SetActive(true);
        }
        else
        {
            // Diagnóstico 3: El trigger funciona, pero el objeto no tiene el script de datos
            Debug.LogWarning("⚠️ [ADVERTENCIA] Choqué con " + other.gameObject.name + " pero no tiene el script 'DatosEdificio' asignado.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DatosEdificio datos = other.GetComponent<DatosEdificio>();
        if (datos == null) datos = other.GetComponentInParent<DatosEdificio>();
        if (datos == null) datos = other.GetComponentInChildren<DatosEdificio>();

        if (datos != null && panelGlobo != null)
        {
            panelGlobo.SetActive(false);
        }
    }

    public void CerrarTarjetaManualmente()
    {
        if (panelGlobo != null) panelGlobo.SetActive(false);
    }
}