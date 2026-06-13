using UnityEngine;

public class DatosEdificio : MonoBehaviour
{
    [Header("Información Principal")]
    public string nombreEdificio;
    public Sprite fotoFachada;

    [Header("Detalles del Edificio")]
    public string ubicacion;
    [TextArea(2, 4)]
    public string descripcion;
    public string uso;

    [Header("Información Extra")]
    [TextArea(2, 4)]
    public string datoCurioso;
}
