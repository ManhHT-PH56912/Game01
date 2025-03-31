using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public GameObject prefab; // Assign the prefab in the Inspector
    public float spawnInterval = 2f; // Time between spawns
    public Vector2 spawnPositionRange = new Vector2(-5f, 5f); // Random X position range
    public Vector2 scaleRange = new Vector2(0.5f, 2f); // Random scale range
    public float moveSpeed = 2f; // Speed of movement
    public float destroyXPosition = -10f; // X position to destroy the object

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObject()
    {
        // Instantiate the prefab
        GameObject obj = Instantiate(prefab);

        // Set random position
        float randomY = Random.Range(spawnPositionRange.x, spawnPositionRange.y);
        obj.transform.position = new Vector3(10f, randomY, 0f);

        // Set random scale
        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        // Set random color
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(Random.value, Random.value, Random.value);
        }

        // Add movement script
        obj.AddComponent<MoveLeft>().Initialize(moveSpeed, destroyXPosition);
    }
}

public class MoveLeft : MonoBehaviour
{
    private float moveSpeed;
    private float destroyXPosition;
    public void Initialize(float speed, float destroyX)
    {
        moveSpeed = speed;
        moveSpeed = Random.Range(2f, 10f); // Random speed between 2 and 10
        destroyXPosition = destroyX;
    }

    private void Update()
    {
        // Move the object to the left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Destroy the object if it goes out of bounds
        if (transform.position.x < destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}
