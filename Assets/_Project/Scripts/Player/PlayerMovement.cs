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
        Vector2 direction = moveAction.ReadValue<Vector2>();

        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
}
