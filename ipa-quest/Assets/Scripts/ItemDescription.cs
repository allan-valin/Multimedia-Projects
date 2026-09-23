using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    public GameObject description;

    // Start is called before the first frame update
    void Start()
    {
        description.SetActive(false);
    }

    public void Show()
    {
        description.SetActive(true);
    }

    public void Hide()
    {
        description.SetActive(false);
    }
}
