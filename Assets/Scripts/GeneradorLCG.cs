using System;  // Para Math.Log (logaritmo natural), usado en la transformacion inversa

// ============================================================================
//  GeneradorLCG  —  Unidad 3 (numeros pseudoaleatorios) + Unidad 4 (distribuciones)
// ----------------------------------------------------------------------------
//  Generador Congruencial Lineal (LCG, por sus siglas en ingles).
//  Es uno de los metodos clasicos para generar numeros pseudoaleatorios.
//
//  La formula de recurrencia es:
//
//        X(n+1) = ( a * X(n) + c )  mod  m
//
//  donde:
//    X(n) = numero actual (la "semilla" en el primer paso)
//    a    = multiplicador
//    c    = incremento
//    m    = modulo (define el rango y el periodo del generador)
//
//  Dividiendo X entre m obtenemos un numero "uniforme" U en el intervalo [0, 1).
//
//  NO es un MonoBehaviour a proposito: es una clase normal de C# que podemos
//  crear con "new" desde cualquier script (el spawner, el wander, etc.) y asi
//  cada sistema tiene su propia secuencia reproducible a partir de una semilla.
// ============================================================================
public class GeneradorLCG
{
    // ---- Constantes del generador (valores clasicos tipo "glibc") ----
    // m = 2^31. El periodo maximo del generador es m (mas de 2 mil millones).
    private const long m = 2147483648; // 2^31
    private const long a = 1103515245; // multiplicador
    private const long c = 12345;      // incremento

    // Estado interno: el ultimo numero X(n) generado.
    private long estado;

    // Constructor: recibe la semilla inicial X(0).
    // Misma semilla -> misma secuencia (reproducible, util para documentar/validar).
    public GeneradorLCG(long semilla)
    {
        estado = semilla;
    }

    // Avanza el generador un paso y devuelve el entero X(n+1).
    // Usamos 'long' para que la multiplicacion a*estado no se desborde antes del mod.
    public long Siguiente()
    {
        estado = (a * estado + c) % m;
        return estado;
    }

    // Devuelve un numero uniforme U en el intervalo [0, 1).
    // Es simplemente X(n+1) / m.
    public double SiguienteUniforme()
    {
        return (double)Siguiente() / m;
    }

    // Devuelve un entero aleatorio en [min, max) — util para elegir indices
    // (por ejemplo, cual punto de aparicion usar).
    public int Rango(int min, int max)
    {
        if (max <= min) return min;
        return min + (int)(SiguienteUniforme() * (max - min));
    }

    // ------------------------------------------------------------------
    //  Exponencial  —  Unidad 4: metodo de la TRANSFORMACION INVERSA
    // ------------------------------------------------------------------
    //  Una variable exponencial sirve para modelar "tiempos entre eventos"
    //  (tiempo entre llegadas de estudiantes, tiempo de permanencia, etc.).
    //
    //  Si U es uniforme en (0,1), entonces:
    //
    //        T = -media * ln(1 - U)
    //
    //  es una variable EXPONENCIAL con valor promedio = 'media'.
    //  (Esto se obtiene despejando T de la funcion de distribucion acumulada
    //   F(t) = 1 - e^(-t/media), es decir, invirtiendola: de ahi el nombre.)
    public double Exponencial(double media)
    {
        double u = SiguienteUniforme();
        // Evitamos ln(0): si u sale 0, lo empujamos a un valor minimo.
        if (u <= 0.0) u = 1e-9;
        return -media * Math.Log(1.0 - u);
    }
}
