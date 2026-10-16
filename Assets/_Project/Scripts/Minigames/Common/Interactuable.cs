/*
 * Descripcion: Objeto interactuable. Cuando el Player esta cerca muestra la tecla "E".
 * Al apretar E abre la ventana de minijuego con la escena que se le haya arrastrado.
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Interactuable : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Minijuego")]
    [Tooltip("Arrastrar aca la escena del minijuego que abre este objeto")]
    [SerializeField] private SceneAsset escenaMinijuego;
#endif
    [SerializeField, HideInInspector] private string nombreEscena;

    [SerializeField] private string tituloVentana = "Minijuego";
    [SerializeField] private VentanaMinijuego prefabVentana;

    [Header("Interaccion")]
    [SerializeField] private float radio = 1.5f;
    [SerializeField] private GameObject indicadorTecla;
    [Tooltip("Si esta activo, una vez ganado el minijuego no se puede volver a jugar")]
    [SerializeField] private bool unSoloUso = true;

    [Header("Resultado")]
    public UnityEvent alGanar;
    public UnityEvent alPerder;

    private Transform player;
    private InputAction interactAction;
    private bool completado;

    public bool Completado => completado;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        interactAction = InputSystem.actions.FindAction("Player/Interact");

        if (indicadorTecla != null) indicadorTecla.SetActive(false);
    }

    private void Update()
    {
        bool puedeInteractuar = PlayerCerca() && !VentanaMinijuego.EstaAbierta && !completado;

        if (indicadorTecla != null && indicadorTecla.activeSelf != puedeInteractuar)
            indicadorTecla.SetActive(puedeInteractuar);

        if (puedeInteractuar && interactAction != null && interactAction.WasPressedThisFrame())
            AbrirMinijuego();
    }

    private bool PlayerCerca()
    {
        if (player == null) return false;
        return Vector2.Distance(player.position, transform.position) <= radio;
    }

    private void AbrirMinijuego()
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning($"{name}: no tiene una escena de minijuego asignada.", this);
            return;
        }

        VentanaMinijuego ventana = VentanaMinijuego.Instancia;
        if (ventana == null) ventana = Instantiate(prefabVentana);

        ventana.Abrir(nombreEscena, tituloVentana, AlTerminar);
    }

    private void AlTerminar(bool gano)
    {
        if (gano)
        {
            if (unSoloUso) completado = true;
            alGanar.Invoke();
        }
        else
        {
            alPerder.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radio);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        nombreEscena = escenaMinijuego != null ? escenaMinijuego.name : "";
        if (escenaMinijuego == null) return;

        string path = AssetDatabase.GetAssetPath(escenaMinijuego);
        var escenas = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (escenas.Exists(e => e.path == path)) return;

        escenas.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = escenas.ToArray();
        Debug.Log($"Escena '{escenaMinijuego.name}' agregada a la lista de escenas del build.");
    }
#endif
}
