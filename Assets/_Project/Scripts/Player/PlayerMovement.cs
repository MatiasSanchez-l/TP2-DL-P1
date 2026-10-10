using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private InputAction moveAction;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector2 direction = input.y * new Vector2(1, 0.45f) + input.x * new Vector2(1, -0.45f);

        transform.position += (Vector3)direction.normalized * speed * Time.deltaTime;
    }
}
