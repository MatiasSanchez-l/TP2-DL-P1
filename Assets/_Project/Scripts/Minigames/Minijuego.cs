/*
 * Descripcion: Poner este componente en cada escena de minijuego.
 * Cuando el minijuego termina, llamar a Ganar() o Perder() (desde codigo o desde un boton).
 * Se puede heredar de esta clase para hacer la logica de cada minijuego.
 */

using UnityEngine;

public class Minijuego : MonoBehaviour
{
    public void Ganar()
    {
        Terminar(true);
    }

    public void Perder()
    {
        Terminar(false);
    }

    private void Terminar(bool gano)
    {
        if (VentanaMinijuego.Instancia == null)
        {
            Debug.Log(gano ? "Minijuego ganado" : "Minijuego perdido");
            return;
        }

        VentanaMinijuego.Instancia.Cerrar(gano);
    }
}
