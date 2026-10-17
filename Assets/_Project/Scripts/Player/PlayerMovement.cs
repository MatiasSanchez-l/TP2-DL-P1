using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private AudioClip[] stepsClips;
    [SerializeField] private float stepsVolume = 1f;
    [SerializeField] private float stepsInterval = 0f;
    [SerializeField] private bool stepsRandom = false;

    private InputAction moveAction;
    private float stepsTimer;
    private int stepsIndex;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");
    }

    private void Update()
    {
        if (VentanaMinijuego.EstaAbierta)
        {
            stepsTimer = 0f;
            return;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector2 direction = input.y * new Vector2(1, 0.45f) + input.x * new Vector2(1, -0.45f);

        transform.position += (Vector3)direction.normalized * speed * Time.deltaTime;

        ActualizarPasos(input != Vector2.zero);
    }

    private void ActualizarPasos(bool caminando)
    {
        if (!caminando)
        {
            stepsTimer = 0f;
            return;
        }

        stepsTimer -= Time.deltaTime;

        if (stepsTimer <= 0f)
        {
            if (stepsRandom)
            {
                SFXManager.instance.PlayRandomSFXClip(stepsClips, transform, stepsVolume);
            }
            else
            {
                SFXManager.instance.PlaySFXClip(stepsClips[stepsIndex], transform, stepsVolume);
                stepsIndex = (stepsIndex + 1) % stepsClips.Length;
            }

            stepsTimer = stepsInterval > 0f ? stepsInterval : stepsClips[0].length;
        }
    }
}
