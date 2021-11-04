using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform loadingScreen;

    private Transform player;
    private Image playerHealthBar;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        loadingScreen.sizeDelta = new Vector2(Screen.width, Screen.height);
        Invoke("HideLoadingScreen", 1.0f);
    }

    void Update()
    {
        if (player != null)
        {
            float playerHealth = Mathf.Abs(player.position.x);
            playerHealth = Mathf.Clamp(playerHealth, 0.0f, 5.0f);
            playerHealth = 1.0f - (playerHealth / 5.0f);
            playerHealthBar.fillAmount = playerHealth;
            if (playerHealth < 0.5f && playerHealthBar.color != Color.red)
                playerHealthBar.color = Color.red;
            else if (playerHealth >= 0.5f && playerHealthBar.color == Color.red)
                playerHealthBar.color = Color.green;

        }
    }

    private void LateUpdate()
    {
        if (player != null)
            playerHealthBar.transform.parent.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
    }

    public void LoadFirstLevel()
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadSceneAsync(1);
    }


    public void QuitGame()
    {
        Debug.Log("Quit is clicked");
#if UNITY_EDITOR  
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_STANDALONE
        Application.Quit();
#endif
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            GameObject.FindWithTag("QuitButton").GetComponent<Button>().onClick.AddListener(QuitGame);
            player = GameObject.FindWithTag("Player").transform;
            playerHealthBar = GameObject.FindWithTag("PlayerHealthBar").GetComponent<Image>();

            Invoke("HideLoadingScreen", 1.0f);
        }
    }

    private void HideLoadingScreen()
    {
        StartCoroutine(LerpLoadingScreen(new Vector2(0.0f, -Screen.height), 0.2f));
    }

    private void ShowLoadingScreen()
    {
        StartCoroutine(LerpLoadingScreen(new Vector2(0.0f, 0.0f), 0.2f));
    }

    public void StartButtonPressed()
    {
        ShowLoadingScreen();
        Invoke("LoadFirstLevel", 1.0f);
    }

    private IEnumerator LerpLoadingScreen(Vector2 targetPos, float duration)
    {
        Vector2 startingPos = loadingScreen.anchoredPosition;
        float currentTime = 0.0f;
        while (currentTime < duration)
        {
            loadingScreen.anchoredPosition = Vector2.Lerp(startingPos, targetPos, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
        loadingScreen.anchoredPosition = targetPos;
    }
}