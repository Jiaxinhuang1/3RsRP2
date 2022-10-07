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
    public int maxRange;
    public int maxExperience;
    public float minSpawnSpeed;
    public int itemLevel;
    public int difficultyLevel;
    public GameManager.Difficulty difficultyState;
    public GameManager.ItemType itemType;
    public static GameManager instance;
    public CameraShake cameraScript;

    [Header("Track Slider Experience")]
    public ProgressBar trashSlider;
    public ProgressBar compostSlider;
    public ProgressBar recycleSlider;
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
    public GameObject typeLabel;

    public GameObject spawner;
    public LevelSO levelSo;


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

        difficultyLevel = PlayerPrefs.GetInt("DifficultyLevel", 1);
        ChangeDifficulty();

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
        
        if(PlayerPrefs.GetString("Lang") == "Chinese")
        {
            //typeLabel.GetComponentInChildren<TextMeshProUGUI>().text = itemType.ToString();
            typeLabel.GetComponentInChildren<Image>().sprite = levelSo.langSprite[1].levels[itemLevel - 1];
        }
        else
        {
            typeLabel.GetComponentInChildren<Image>().sprite = levelSo.langSprite[0].levels[itemLevel - 1];
        }
        var seq = LeanTween.sequence();
        seq.append(LeanTween.alpha(titlePanel.GetComponent<RectTransform>(), 0.75f, 0.5f));
        seq.append(LeanTween.moveX(typeLabel.GetComponent<RectTransform>(), 0, 1f));
        seq.append(LeanTween.alpha(titlePanel.GetComponent<RectTransform>(), 0f, 1f).setDestroyOnComplete(true));
        seq.append(() =>
        {
            LeanTween.moveY(typeLabel.GetComponent<RectTransform>(), -125, 0.5f);
            LeanTween.scale(typeLabel, new Vector3(0.5f, 0.5f, 0.5f), 0.5f).setOnComplete(() => {
                spawner.SetActive(true);
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        trashSlider.current = trashCount;
        compostSlider.current = compostCount;
        recycleSlider.current = recycleCount;
        trashLevelText.text = trashLevel.ToString();
        compostLevelText.text = compostLevel.ToString();
        recycleLevelText.text = recycleLevel.ToString();
    }

    public void ChangeDifficulty()
    {
        if (difficultyLevel == 1)
        {
            difficultyState = Difficulty.Easy;
        }
        else if (difficultyLevel == 2)
        {
            difficultyState = Difficulty.Medium;
        }
        else if(difficultyLevel == 3)
        {
            difficultyState = Difficulty.Hard;
        }
        switch (difficultyState)
        {
            case Difficulty.Easy:
                minSpawnSpeed = 1f;
                maxRange = 5;
                maxExperience = 3;
                Debug.Log("Easy Level");
                Debug.Log("Spawn Speed: " + minSpawnSpeed);
                Debug.Log("Item Range: " + maxRange);
                Debug.Log("Max Experience: " + maxExperience);
                break;
            case Difficulty.Medium:
                minSpawnSpeed = 0.5f;
                maxRange = 10;
                maxExperience = 5;
                Debug.Log("Medium Level");
                Debug.Log("Spawn Speed: " + minSpawnSpeed);
                Debug.Log("Item Range: " + maxRange);
                Debug.Log("Max Experience: " + maxExperience); 
                break;
            case Difficulty.Hard:
                minSpawnSpeed = 0.2f;
                maxRange = 15;
                maxExperience = 10;
                Debug.Log("Hard Level");
                Debug.Log("Spawn Speed: " + minSpawnSpeed);
                Debug.Log("Item Range: " + maxRange);
                Debug.Log("Max Experience: " + maxExperience); 
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
        if (trashCount >= trashSlider.maximum)
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
        if (compostCount >= compostSlider.maximum)
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
        if (recycleCount >= compostSlider.maximum)
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
        NextDifficulty();
    }

    public void NextDifficulty()
    {
        if (trashLevel > 10)
        {
            difficultyLevel = 3;
            ChangeDifficulty();
        }
        else if (trashLevel > 4)
        {
            difficultyLevel = 2;
            ChangeDifficulty();
        }
        PlayerPrefs.SetInt("DifficultyLevel", difficultyLevel);
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
