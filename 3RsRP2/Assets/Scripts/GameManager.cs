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
    public GameObject typeLabel;
    public GameObject pausePanel;

    public GameObject spawner;
    public GameObject LevelItems;
    public LevelSO levelSo;

    [Header("Audio")]
    public AudioSource levelCompleteSound;
    public AudioSource levelFailedSound;
    public AudioSource menuOpenSound;
    public AudioSource explosionSound;
    public AudioSource correctSound;

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
            typeLabel.transform.GetChild(0).GetComponent<Image>().sprite = levelSo.langSprite[1].levels[itemLevel - 1];
        }
        else
        {
            typeLabel.transform.GetChild(0).GetComponent<Image>().sprite = levelSo.langSprite[0].levels[itemLevel - 1];
        }
        var seq = LeanTween.sequence();

        seq.append(LeanTween.moveX(typeLabel.GetComponent<RectTransform>(), 0, 1f));
        seq.append(() =>
        {
            LeanTween.moveY(typeLabel.GetComponent<RectTransform>(), -100, 0.5f).setDelay(1f);
            LeanTween.moveX(typeLabel.GetComponent<RectTransform>(), 150, 0.5f).setDelay(1f);
            LeanTween.scale(typeLabel, new Vector3(0.75f, 0.75f, 0.75f), 0.5f).setDelay(1f).setOnComplete(() => {
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeInHierarchy)
            {
                UnPauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (spawner.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void UnPauseGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
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
        correctSound.Play();
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
        levelCompleteSound.Play();
        StopAllCoroutines();
        //levelCompletePanel.SetActive(true);
        for (int i = 0; i < LevelItems.transform.childCount; i++)
        {
            LevelItems.transform.GetChild(i).GetComponent<Image>().sprite = levelSo.levelSprites[itemLevel - 1].types[i];
        }
        OpenPanel(levelCompletePanel);
        spawner.SetActive(false);
    }

    public void IncreaseErrorCount()
    {
        errorCount++;
        for (int i = 0; i < errorCount; i++)
        {
            explosionSound.Play();
            dead[i].SetActive(true);
        }
        if (errorCount == dead.Length)
        {
            levelFailedSound.Play();
            //losePanel.SetActive(true);
            OpenPanel(losePanel);
            spawner.SetActive(false);
            Debug.Log("You Lost");
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
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

    public void OpenPanel(GameObject go)
    {
        LeanTween.scale(go, new Vector3(1, 1, 1), 0.2f);
    }

    public void ClosePanel(GameObject go)
    {
        LeanTween.scale(go, new Vector3(1, 0, 1), 0.2f);
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
