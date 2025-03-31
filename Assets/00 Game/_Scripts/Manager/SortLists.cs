using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortLists : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    void Start()
    {
        Debug.Log("Original List: " + string.Join(", ", gameObjects.ConvertAll(go => go.name)));

        // Sort the list of GameObjects by name
        SortByName(gameObjects);

        Debug.Log("Sorted List: " + string.Join(", ", gameObjects.ConvertAll(go => go.name)));
    }

    // Method to sort a list of GameObjects by their name
    private void SortByName(List<GameObject> list)
    {
        list.Sort((a, b) => string.Compare(a.name, b.name));
    }
}