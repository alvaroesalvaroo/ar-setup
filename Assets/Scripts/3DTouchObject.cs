using UnityEngine;
using UnityEngine.InputSystem;

public class TouchObject : MonoBehaviour
{
    [SerializeField] Camera arCamera;
    
    void Update()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        if (!touch.press.wasPressedThisFrame) return;

        Vector2 screenPos = touch.position.ReadValue();
        TryHitObject(screenPos);
    }

    void TryHitObject(Vector2 screenPos)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // hit.collider.gameObject es el objeto tocado
            Debug.Log($"Tocado: {hit.collider.gameObject.name}");
            hit.collider.gameObject.SendMessage("OnTouched", SendMessageOptions.DontRequireReceiver);
        }
    }
}
