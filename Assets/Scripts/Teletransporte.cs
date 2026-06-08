using UnityEngine;

// F14 - Teletransporte rapido del jugador a puntos del campus (edificios).
// Con las teclas numericas 1, 2, 3... el jugador salta al destino correspondiente.
public class Teletransporte : MonoBehaviour
{
    [Header("Referencias")]
    // El objeto del jugador que se mueve (el PlayerArmature de Starter Assets).
    public Transform jugador;

    // Lista de destinos a los que se puede teletransportar (uno por edificio).
    // Se asignan desde el Inspector: pueden ser los WP_ o puntos propios junto a cada edificio.
    public Transform[] destinos;

    void Update()
    {
        // Recorremos los destinos (maximo 9, teclas 1 a 9).
        for (int i = 0; i < destinos.Length && i < 9; i++)
        {
            // KeyCode.Alpha1 es la tecla "1"; sumando i obtenemos 2, 3, etc.
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                Teletransportar(i);
            }
        }
    }

    void Teletransportar(int indice)
    {
        // Validaciones para no romper si falta algo.
        if (jugador == null || destinos == null) return;
        if (indice < 0 || indice >= destinos.Length) return;
        if (destinos[indice] == null) return;

        // MUY IMPORTANTE: el jugador usa un CharacterController, que controla su posicion.
        // Si cambiamos la posicion sin apagarlo, "pelea" el cambio y el teletransporte falla.
        // Por eso lo desactivamos, movemos al jugador, y lo volvemos a activar.
        CharacterController cc = jugador.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        jugador.position = destinos[indice].position;

        if (cc != null) cc.enabled = true;
    }
}
