/*
 * Created on Wed Oct 07 2026
 *
 * Copyright (c) 2026 Your Company
 *
 * Descripcion: Este script controla el menú principal del juego. Permite iniciar el juego, abrir el panel de créditos, instrucciones y salir del juego.
 */

using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public GameObject credits;

    public GameObject instruccions;
    public GameObject mainMenu;


    public void OpenCreditsPanel()
    {
        mainMenu.SetActive(false);
        credits.SetActive(true);

    }

    public void OpenInstruccionsPanel()
    {
        mainMenu.SetActive(false);
        instruccions.SetActive(true);

    }

    public void OpenMenuPanel()
    {
        mainMenu.SetActive(true);
        credits.SetActive(false);
        instruccions.SetActive(false);

    }

    public void QuitGame()
    {
        Application.Quit();

    }

    public void PlayGame()
    {
        SceneManager.LoadScene("City");    

    }
}
