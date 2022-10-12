using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] SpawnedItems;
    private float minX, maxX, minY, maxY;

    // Start is called before the first frame update
    void Start()
    {
        SetSpawnBoundaries();
        StartCoroutine(SpawnObjects());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetSpawnBoundaries()
    {
        Vector2 boundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        minX = -boundaries.x + 4;
        maxX = boundaries.x - 1;
        minY = -boundaries.y + 1;
        maxY = boundaries.y - 2;
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            float xPos = Random.Range(minX, maxX);
            float yPos = Random.Range(minY, maxY);
            Vector3 position = new Vector3(xPos, yPos, 0);
            GameObject item = Instantiate(SpawnedItems[Random.Range(0, GameManager.instance.maxRange)], position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(GameManager.instance.minSpawnSpeed, 1f));
            LeanTween.delayedCall(item, Random.Range (5, 10), () =>
            {
                LeanTween.scale(item, new Vector3(0, 0, 0), Mathf.RoundToInt(Random.Range(3, 8))).setDestroyOnComplete(true);
            });
        }
    }
}
