using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private AudioClip stepsClip;
    [SerializeField] private float stepsVolume = 1f;

    private InputAction moveAction;
    private AudioSource stepsSource;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");

        stepsSource = gameObject.AddComponent<AudioSource>();
        stepsSource.clip = stepsClip;
        stepsSource.volume = stepsVolume;
        stepsSource.loop = true;
        stepsSource.playOnAwake = false;
    }

    private void Update()
    {
        if (VentanaMinijuego.EstaAbierta)
        {
            ActualizarPasos(false);
            return;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector2 direction = input.y * new Vector2(1, 0.45f) + input.x * new Vector2(1, -0.45f);

        transform.position += (Vector3)direction.normalized * speed * Time.deltaTime;

        ActualizarPasos(input != Vector2.zero);
    }

    private void ActualizarPasos(bool caminando)
    {
        if (caminando && !stepsSource.isPlaying)
        {
            stepsSource.Play();
        }
        else if (!caminando && stepsSource.isPlaying)
        {
            stepsSource.Stop();
        }
    }
}
