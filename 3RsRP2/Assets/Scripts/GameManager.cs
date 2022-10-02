using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard }
    public enum ItemType { Trash, Compost, Recycle }
    public int itemLevel;
    public GameManager.Difficulty difficultyState;
    public GameManager.ItemType itemType;
    public static GameManager instance;
    public CameraShake cameraScript;

    [Header("Track Slider Experience")]
    public Slider trashSlider;
    public Slider compostSlider;
    public Slider recycleSlider;
    public int trashCount;
    public int compostCount;
    public int recycleCount;

    [Header("Track Levels")]
    public TextMeshProUGUI trashLevelText;
    public TextMeshProUGUI compostLevelText;
    public TextMeshProUGUI recycleLevelText;
    public int trashLevel;
    public int compostLevel;
    public int recycleLevel;

    [Header("Lives")]
    public int errorCount;
    public GameObject[] dead;

    [Header("Panels")]
    public GameObject levelCompletePanel;
    public GameObject losePanel;
    public GameObject titlePanel;
    public GameObject typeText;

    public GameObject spawner;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        trashLevel = PlayerPrefs.GetInt("TrashLevel", 0);
        compostLevel = PlayerPrefs.GetInt("CompostLevel", 0);
        recycleLevel = PlayerPrefs.GetInt("RecycleLevel", 0);
        itemLevel = PlayerPrefs.GetInt("ItemLevel", 1);
        Debug.Log(itemLevel);

        if (itemLevel == 1)
        {
            itemType = ItemType.Trash;
        }
        else if (itemLevel == 2)
        {
            itemType = ItemType.Compost;
        }
        else if (itemLevel == 3)
        {
            itemType = ItemType.Recycle;
        }

        typeText.GetComponentInChildren<TextMeshProUGUI>().text = itemType.ToString();
        var seq = LeanTween.sequence();
        seq.append(LeanTween.alpha(titlePanel.GetComponent<RectTransform>(), 0.75f, 0.5f));
        seq.append(LeanTween.moveX(typeText, 1000, 1f));
        seq.append(LeanTween.alpha(titlePanel.GetComponent<RectTransform>(), 0f, 1f).setDestroyOnComplete(true));
        seq.append(() =>
        {
            LeanTween.moveY(typeText, 1000, 1f);
            LeanTween.scale(typeText, new Vector3(0.5f, 0.5f, 0.5f), 1f).setOnComplete(() => {
                spawner.SetActive(true);
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        trashSlider.value = trashCount;
        compostSlider.value = compostCount;
        recycleSlider.value = recycleCount;
        trashLevelText.text = trashLevel.ToString();
        compostLevelText.text = compostLevel.ToString();
        recycleLevelText.text = recycleLevel.ToString();
    }

    public void ChangeDifficulty()
    {
        switch (difficultyState)
        {
            case Difficulty.Easy:
                Debug.Log("Easy Level");
                break;
            case Difficulty.Medium:
                Debug.Log("Medium Level");
                break;
            case Difficulty.Hard:
                Debug.Log("Hard Level");
                break;
        }
    }

    public void ChangeType()
    {
        switch (itemType)
        {
            case ItemType.Trash:
                Debug.Log("Trash Time");
                break;
            case ItemType.Compost:
                Debug.Log("Compost Time");
                break;
            case ItemType.Recycle:
                Debug.Log("Recycle Time");
                break;
        }
    }

    public void CollectItem()
    {
        if (itemType == ItemType.Trash)
        {
            CollectTrash();
        }
        else if (itemType == ItemType.Compost)
        {
            CollectCompost();
        }
        else if (itemType == ItemType.Recycle)
        {
            CollectRecycle();
        }
    }
    public void CollectTrash()
    {
        trashCount++;
        if (trashCount >= trashSlider.maxValue)
        {
            trashLevel++;
            PlayerPrefs.SetInt("TrashLevel", trashLevel);
            NextLevel();
            trashCount = 0;
        }
    }

    public void CollectCompost()
    {
        compostCount++;
        if (compostCount >= compostSlider.maxValue)
        {
            compostLevel++;
            PlayerPrefs.SetInt("CompostLevel", compostLevel);
            NextLevel();
            compostCount = 0;
        }
    }

    public void CollectRecycle()
    {
        recycleCount++;
        if (recycleCount >= compostSlider.maxValue)
        {
            recycleLevel++;
            PlayerPrefs.SetInt("RecycleLevel", recycleLevel);
            NextLevel();
            recycleCount = 0;
        }
    }

    public void NextLevel()
    {
        StopAllCoroutines();
        levelCompletePanel.SetActive(true);
        spawner.SetActive(false);
    }

    public void IncreaseErrorCount()
    {
        errorCount++;
        for (int i = 0; i < errorCount; i++)
        {
            dead[i].SetActive(true);
        }
        if (errorCount == dead.Length)
        {
            losePanel.SetActive(true);
            spawner.SetActive(false);
            Debug.Log("You Lost");
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void NextCategory()
    {
        itemLevel++;
        if (itemLevel == 4)
        {
            itemLevel = 1;
        }
        PlayerPrefs.SetInt("ItemLevel", itemLevel);
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(cameraScript.Shake(duration, magnitude));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    [ContextMenu("Reset PlayerPrefs")]

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
