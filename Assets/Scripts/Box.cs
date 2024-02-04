using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] int experienceToGive = 5;

    public void Open()
    {
        FindAnyObjectByType<Player>().AddExperience(experienceToGive);
        Destroy(gameObject);
    }
}
