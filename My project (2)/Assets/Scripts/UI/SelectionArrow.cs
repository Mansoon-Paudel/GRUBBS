using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip interactSound;
    private RectTransform arrow;
    private int currentPosition;

    private InputAction _upAction;
    private InputAction _downAction;
    private InputAction _interactAction;

    private void Awake()
    {
        arrow = GetComponent<RectTransform>();

        // Setup input actions
        _upAction = new InputAction(type: InputActionType.Button);
        _upAction.AddBinding("<Keyboard>/upArrow");
        _upAction.AddBinding("<Keyboard>/w");

        _downAction = new InputAction(type: InputActionType.Button);
        _downAction.AddBinding("<Keyboard>/downArrow");
        _downAction.AddBinding("<Keyboard>/s");

        _interactAction = new InputAction(type: InputActionType.Button);
        _interactAction.AddBinding("<Keyboard>/enter");
        _interactAction.AddBinding("<Keyboard>/e");
    }

    private void OnEnable()
    {
        _upAction.Enable();
        _downAction.Enable();
        _interactAction.Enable();
        
        currentPosition = 0;
        ChangePosition(0);
    }

    private void OnDisable()
    {
        _upAction.Disable();
        _downAction.Disable();
        _interactAction.Disable();
    }

    private void Update()
    {
        // Change the position of the selection arrow
        if (_upAction.WasPressedThisFrame())
            ChangePosition(-1);
        if (_downAction.WasPressedThisFrame())
            ChangePosition(1);

        // Interact with current option
        if (_interactAction.WasPressedThisFrame())
            Interact();
    }

    private void ChangePosition(int _change)
    {
        currentPosition += _change;

        if (_change != 0)
            SoundManager.instance.PlaySound(changeSound);

        if (currentPosition < 0)
            currentPosition = buttons.Length - 1;
        else if (currentPosition > buttons.Length - 1)
            currentPosition = 0;

        AssignPosition();
    }
    private void AssignPosition()
    {
        //Assign the Y position of the current option to the arrow (basically moving it up and down)
        arrow.position = new Vector3(arrow.position.x, buttons[currentPosition].position.y);
    }
    private void Interact()
    {
        SoundManager.instance.PlaySound(interactSound);

        //Access the button component on each option and call its function
        buttons[currentPosition].GetComponent<Button>().onClick.Invoke();
    }
}