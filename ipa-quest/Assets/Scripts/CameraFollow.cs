/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private float followSpeed = 0.1f;

    [SerializeField] private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    private bool canZoomIn = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleOffsetChange();
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);
    }

    void HandleOffsetChange()
    {
        float offsetChange = 0.1f; // Change this value to control the speed of offset change
        float zoomChange = 0.1f; // Change this value to control the speed of zoom change
        float returnSpeed = 0.05f; // Change this value to control the speed of returning to the original position

        bool keyIsPressed = false;

        if (Input.GetKey(KeyCode.W) && offset.y < 5)
        {
            offset.y += offsetChange;
            keyIsPressed = true;
        }
        if (Input.GetKey(KeyCode.S) && offset.y > -5)
        {
            offset.y -= offsetChange;
            keyIsPressed = true;
        }
        /*if (Input.GetKey(KeyCode.A) && offset.x > -10)
        {
            offset.x -= offsetChange;
            keyIsPressed = true;
        }
        if (Input.GetKey(KeyCode.D) && offset.x < 10)
        {
            offset.x += offsetChange;
            keyIsPressed = true;
        }*/
        if ((Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) && offset.z > -25)
        {
            offset.z -= zoomChange;
            canZoomIn = true;
        }
        if ((Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus)) && canZoomIn && offset.z < -15)
        {
            offset.z += zoomChange;
        }

        if (!keyIsPressed)
        {
            offset.x = Mathf.Lerp(offset.x, 0, returnSpeed);
            offset.y = Mathf.Lerp(offset.y, 0, returnSpeed);
        }
    }
}