using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenu : MonoBehaviour
{
    [Header("Pantallas del Menú")]
    public GameObject menuInicioPanel; // El contenedor de la pantalla principal
    public GameObject opcionesPanel;     // El contenedor de la pantalla de opciones

    private void Start()
    {
        // Al arrancar, nos aseguramos de que el menú principal esté encendido y las opciones apagadas
        MostrarMenuPrincipal();
    }

    // --- ACCIONES DE LA PANTALLA PRINCIPAL ---
    
    public void IniciarRecorrido()
    {
        // Carga tu escena del campus (asegúrate de poner el nombre exacto de tu escena aquí)
        SceneManager.LoadScene("CampusUES"); 
    }

    public void AbrirOpciones()
    {
        menuInicioPanel.SetActive(false); // Apaga la pantalla principal
        opcionesPanel.SetActive(true);    // Enciende la pantalla de opciones
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo de la simulación...");
        Application.Quit();
    }

    // --- ACCIONES DE LA PANTALLA DE OPCIONES ---

    public void MostrarMenuPrincipal()
    {
        menuInicioPanel.SetActive(true);  // Enciende la pantalla principal
        opcionesPanel.SetActive(false);   // Apaga la pantalla de opciones
    }

    public void AplicarCambios()
    {
        // Aquí guardarás las opciones más adelante. Por ahora regresa al menú.
        Debug.Log("¡Cambios aplicados con éxito!");
        MostrarMenuPrincipal();
    }
}