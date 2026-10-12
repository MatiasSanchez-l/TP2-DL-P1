/*
 * Descripcion: Ventana generica de minijuegos. Carga la escena del minijuego de forma aditiva
 * y hace que su camara dibuje solo en un cuadrado en el centro de la pantalla,
 * asi el juego de fondo se sigue viendo.
 */

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class VentanaMinijuego : MonoBehaviour
{
    public static VentanaMinijuego Instancia { get; private set; }
    public static bool EstaAbierta { get; private set; }

    [Tooltip("Tamaño del cuadrado en proporcion al alto de la pantalla")]
    [SerializeField, Range(0.3f, 0.95f)] private float tamano = 0.7f;
    [SerializeField] private RectTransform marco;
    [SerializeField] private TMP_Text titulo;
    [SerializeField] private Button botonCerrar;

    private static readonly Vector3 Desplazamiento = new Vector3(10000f, 10000f, 0f);

    private string escenaActual;
    private Camera camaraMinijuego;
    private Action<bool> alTerminar;
    private InputAction cancelAction;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        if (botonCerrar != null) botonCerrar.onClick.AddListener(() => Cerrar(false));
        cancelAction = InputSystem.actions.FindAction("UI/Cancel");

        marco.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instancia != this) return;
        Instancia = null;
        EstaAbierta = false;
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    public void Abrir(string nombreEscena, string tituloVentana, Action<bool> callback)
    {
        if (EstaAbierta) return;

        EstaAbierta = true;
        escenaActual = nombreEscena;
        alTerminar = callback;

        if (titulo != null) titulo.text = tituloVentana;
        marco.gameObject.SetActive(true);
        ActualizarRect();

        SceneManager.sceneLoaded += AlCargarEscena;
        SceneManager.LoadSceneAsync(nombreEscena, LoadSceneMode.Additive);
    }

    public void Cerrar(bool gano)
    {
        if (!EstaAbierta) return;

        EstaAbierta = false;
        SceneManager.sceneLoaded -= AlCargarEscena;
        marco.gameObject.SetActive(false);

        Scene escena = SceneManager.GetSceneByName(escenaActual);
        if (escena.isLoaded) SceneManager.UnloadSceneAsync(escena);
        camaraMinijuego = null;

        Action<bool> callback = alTerminar;
        alTerminar = null;
        callback?.Invoke(gano);
    }

    private void Update()
    {
        if (!EstaAbierta) return;

        if (cancelAction != null && cancelAction.WasPressedThisFrame())
            Cerrar(false);
    }

    private void LateUpdate()
    {
        if (EstaAbierta) ActualizarRect();
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name != escenaActual) return;
        SceneManager.sceneLoaded -= AlCargarEscena;

        GameObject[] raices = escena.GetRootGameObjects();
        foreach (GameObject raiz in raices)
            raiz.transform.position += Desplazamiento;

        camaraMinijuego = BuscarEnEscena<Camera>(raices);
        if (camaraMinijuego == null) camaraMinijuego = CrearCamara(escena);
        camaraMinijuego.depth = 100;

        foreach (GameObject raiz in raices)
        {
            foreach (AudioListener listener in raiz.GetComponentsInChildren<AudioListener>(true))
                listener.enabled = false;
            foreach (EventSystem sistema in raiz.GetComponentsInChildren<EventSystem>(true))
                sistema.gameObject.SetActive(false);

            foreach (Canvas canvas in raiz.GetComponentsInChildren<Canvas>(true))
            {
                if (!canvas.isRootCanvas || canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camaraMinijuego;
                canvas.planeDistance = 1f;
            }
        }

        ActualizarRect();
    }

    private void ActualizarRect()
    {
        float lado = Screen.height * tamano;
        float ancho = lado / Screen.width;
        float alto = lado / Screen.height;
        Rect rect = new Rect(0.5f - ancho / 2f, 0.5f - alto / 2f, ancho, alto);

        marco.anchorMin = rect.min;
        marco.anchorMax = rect.max;
        marco.offsetMin = Vector2.zero;
        marco.offsetMax = Vector2.zero;

        if (camaraMinijuego != null) camaraMinijuego.rect = rect;
    }

    private Camera CrearCamara(Scene escena)
    {
        GameObject go = new GameObject("CamaraMinijuego");
        SceneManager.MoveGameObjectToScene(go, escena);
        go.transform.position = Desplazamiento + new Vector3(0f, 0f, -10f);

        Camera cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.15f);
        return cam;
    }

    private static T BuscarEnEscena<T>(GameObject[] raices) where T : Component
    {
        foreach (GameObject raiz in raices)
        {
            T componente = raiz.GetComponentInChildren<T>(true);
            if (componente != null) return componente;
        }
        return null;
    }
}
