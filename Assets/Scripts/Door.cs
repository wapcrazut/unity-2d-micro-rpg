using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform destination;

    public void Open()
    {
        Debug.Log("Opened!");
        Player player = FindAnyObjectByType<Player>();
        player.gameObject.transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y, player.transform.position.z);
    }
}
