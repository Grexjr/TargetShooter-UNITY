using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Events that fire off when certain things are true (i.e. buttons pressed, mouse moved)
    public System.Action onAttack;
    public System.Action onReload;

    // Input maps
    private InputAction attackAction;
    private InputAction lookAction;
    private InputAction reloadAction;

    // Values exposed publicly for controls
    public Vector2 LookInput => lookAction.ReadValue<Vector2>();

    private void Awake()
    {
        attackAction = InputSystem.actions.FindActionMap("Player").FindAction("Attack");
        lookAction = InputSystem.actions.FindActionMap("Player").FindAction("Look");
        reloadAction = InputSystem.actions.FindActionMap("Player").FindAction("Reload"); 
    }

    private void OnEnable()
    {
        attackAction.Enable();
        lookAction.Enable();
        reloadAction.Enable();

        // Says hey, whenever the input system's corresponding action to this variable is pressed, invoke this event
        attackAction.performed += ctx => onAttack?.Invoke();
        reloadAction.performed += ctx => onReload?.Invoke();
    }

    private void OnDisable()
    {
        attackAction.Disable();
        lookAction.Disable();
        reloadAction.Disable();
    }

}
