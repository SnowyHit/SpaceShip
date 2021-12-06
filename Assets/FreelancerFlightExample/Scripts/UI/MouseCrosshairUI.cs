using UnityEngine;
using UnityEngine.UI;
public class MouseCrosshairUI : MonoBehaviour
{
    private Image crosshair;

    private void Awake()
    {
        crosshair = GetComponent<Image>();
        Cursor.visible = false;
    }

    private void Update()
    {
        if (crosshair != null)
        {
            crosshair.transform.position = Input.mousePosition;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
