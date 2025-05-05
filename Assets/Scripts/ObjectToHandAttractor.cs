using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectToHandAttractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private InputActionReference inputAction;
    [SerializeField]
    private Transform attractedObject;
    [SerializeField]
    private Transform targetHand;
    [SerializeField]
    private float maxMoveSpeed = 5;
    [SerializeField]
    private Behaviour[] componentsDisabledWhileAttracting;

    [Header("States")]
    [SerializeField]
    private bool isAttracting;

    private Vector3 previousPosition;
    private bool isReturning;

    private void OnEnable()
    {
        inputAction.action.performed += Action_performed;
        inputAction.action.canceled += Action_canceled;
    }

    private void Action_performed(InputAction.CallbackContext ctx)
    {
        isAttracting = true;
        isReturning = false;
        previousPosition = attractedObject.position;
        foreach (var component in componentsDisabledWhileAttracting)
            component.enabled = false;
    }

    private void Action_canceled(InputAction.CallbackContext ctx)
    {
        isAttracting = false;
        isReturning = true;
    }

    private void Update()
    {
        if (isAttracting)
        {
            float maxDistance = maxMoveSpeed * Time.deltaTime;
            attractedObject.position = Vector3.MoveTowards(attractedObject.position, targetHand.position, maxDistance);
        }
        else if (isReturning)
        {
            float maxDistance = maxMoveSpeed * Time.deltaTime;
            attractedObject.position = Vector3.MoveTowards(attractedObject.position, previousPosition, maxDistance);
            if (attractedObject.position == previousPosition)
            {
                isReturning = false;
                foreach (var component in componentsDisabledWhileAttracting)
                    component.enabled = true;
            }
        }
    }

    private void OnDisable()
    {
        inputAction.action.performed -= Action_performed;
        inputAction.action.canceled -= Action_canceled;
	}
}
