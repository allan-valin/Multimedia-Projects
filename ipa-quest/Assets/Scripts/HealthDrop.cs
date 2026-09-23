using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    public GameObject HeartModel;
    public Transform transform;
	
    public void DropHeart()
    {
        Vector2 position = transform.position;
        GameObject heart = Instantiate(HeartModel, position, Quaternion.identity);
        heart.SetActive(true);
        Destroy(heart, 10f);
        
    }
    

}