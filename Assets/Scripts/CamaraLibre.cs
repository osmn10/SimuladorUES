using UnityEngine;

// Modo camara libre / cinematico
// Tecla F activa/desactiva el vuelo libre por el campus
public class CamaraLibre : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camaraNormal;      // La MainCamera del jugador
    public Camera camaraLibre;       // La camara que vuela libre
    public GameObject jugador;       // El PlayerArmature (para desactivar su control)

    [Header("Configuracion de vuelo")]
    public float velocidadMovimiento = 20f;  // Velocidad horizontal
    public float velocidadVertical = 15f;     // Velocidad subir/bajar
    public float sensibilidadMouse = 2f;      // Sensibilidad de la mirada

    [Header("Estado")]
    public bool modoLibreActivo = false;

    private float rotacionX = 0f;  // Rotacion vertical acumulada
    private float rotacionY = 0f;  // Rotacion horizontal acumulada

    void Update()
    {
        // Tecla F para activar/desactivar el modo libre
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleModoLibre();
        }

        // Si el modo libre esta activo, controlamos la camara
        if (modoLibreActivo)
        {
            ControlarCamaraLibre();
        }
    }

    void ToggleModoLibre()
    {
        modoLibreActivo = !modoLibreActivo;

        if (modoLibreActivo)
        {
            // ENTRAR al modo libre

            // Colocamos la camara libre donde esta la camara normal
            if (camaraNormal != null && camaraLibre != null)
            {
                camaraLibre.transform.position = camaraNormal.transform.position;
                camaraLibre.transform.rotation = camaraNormal.transform.rotation;

                // Inicializamos las rotaciones con la orientacion actual
                Vector3 anguloActual = camaraLibre.transform.eulerAngles;
                rotacionY = anguloActual.y;
                rotacionX = anguloActual.x;
            }

            // Activamos la camara libre, desactivamos la normal
            if (camaraLibre != null) camaraLibre.gameObject.SetActive(true);
            if (camaraNormal != null) camaraNormal.gameObject.SetActive(false);

            // Desactivamos el control del jugador para que no se mueva
            if (jugador != null) jugador.SetActive(false);
        }
        else
        {
            // SALIR del modo libre

            // Activamos la camara normal, desactivamos la libre
            if (camaraLibre != null) camaraLibre.gameObject.SetActive(false);
            if (camaraNormal != null) camaraNormal.gameObject.SetActive(true);

            // Reactivamos el jugador
            if (jugador != null) jugador.SetActive(true);
        }
    }

    void ControlarCamaraLibre()
    {
        if (camaraLibre == null) return;

        // ROTACION CON EL MOUSE
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        rotacionY += mouseX;          // Girar horizontal
        rotacionX -= mouseY;          // Girar vertical (invertido)
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f); // Limitar para no dar volteretas

        camaraLibre.transform.rotation = Quaternion.Euler(rotacionX, rotacionY, 0f);

        // MOVIMIENTO CON WASD
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        // Movemos en la direccion hacia donde mira la camara
        Vector3 movimiento = camaraLibre.transform.forward * vertical +
                             camaraLibre.transform.right * horizontal;

        camaraLibre.transform.position += movimiento * velocidadMovimiento * Time.deltaTime;

        // SUBIR / BAJAR con Espacio y Ctrl
        if (Input.GetKey(KeyCode.Space))
            camaraLibre.transform.position += Vector3.up * velocidadVertical * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftControl))
            camaraLibre.transform.position += Vector3.down * velocidadVertical * Time.deltaTime;

        // ===== LIMITES DE LA CAMARA LIBRE =====
        // Evita que la camara salga del area del mapa o se hunda en el suelo

        Vector3 pos = camaraLibre.transform.position;

        // Limite del suelo (altura minima) - el suelo esta en Y = -0.47
        if (pos.y < 1f)
            pos.y = 1f;

        // Limite de altura maxima (techo, para no irse al infinito hacia arriba)
        if (pos.y > 100f)
            pos.y = 100f;

        // Limites horizontales (mismos bordes que las paredes del mapa)
        // Bordes en X: Oeste -133, Este 116
        if (pos.x < -133f) pos.x = -133f;
        if (pos.x > 116f) pos.x = 116f;

        // Bordes en Z: Sur -171, Norte 79
        if (pos.z < -171f) pos.z = -171f;
        if (pos.z > 79f) pos.z = 79f;

        // Aplicamos la posicion corregida
        camaraLibre.transform.position = pos;
    }
   
}