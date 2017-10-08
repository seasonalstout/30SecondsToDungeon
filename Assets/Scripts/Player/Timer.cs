using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Timer : MonoBehaviour
{
    float timeLeft = 30.0f;

    public Text timer;
    public Text endGame;
    bool gameOver = false;
    bool begun = false;
    bool hasWon = false;
    private bool isMenuShowing = false;

    public Button restartButton, exitButton;

    private void Start()
    {
        restartButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

    public void begin()
    {
        begun = true; 
    }
    void Update()
    {
        if (Input.GetButtonUp("Cancel"))
            TogglePauseMenu();
        if (begun)
        {
            Math.Abs(timeLeft -= Time.deltaTime);
            if (!gameOver)
            {
                timer.text = string.Format("{0:00}:{1:00}",
                    Mathf.Floor(timeLeft) % 60,//seconds
                    Mathf.Floor((timeLeft * 100) % 100));//miliseconds
            }
            else
                timer.text = "00:00";
            if (timeLeft <= 0)
            {
                EndGame(false);
            }
        }
    }

    public void TogglePauseMenu()
    {
        restartButton.gameObject.SetActive(!isMenuShowing);
        exitButton.gameObject.SetActive(!isMenuShowing);

        GameObject.FindWithTag("Player").GetComponent<Controls>().alive = isMenuShowing;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach( GameObject enemy in enemies)
        {
            if (enemy.GetComponent<IEnemy>() != null)
                enemy.GetComponent<IEnemy>().ChasePlayer = isMenuShowing;
        }
        
        begun = isMenuShowing;
        isMenuShowing = !isMenuShowing;
    }

    public void StartGame()
    {
        GameObject.FindWithTag("Player").GetComponent<Controls>().Ready();
    }

    public void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AddTime(float time, GameObject parent)
    {
        string path = "Enemies/plus3";
        StartCoroutine(FlashTextColor(Color.green, parent, path));
        timeLeft += time;
        if (timeLeft > 30.0f)
            timeLeft = 30;
    }

    public IEnumerator FlashTextColor(Color color, GameObject parent, string path)
    {
        timer.color = color;

        Vector3 pos = parent.transform.position;
        pos.y += 2;
        GameObject three = (GameObject)Instantiate(Resources.Load<GameObject>(path),
        pos, FindObjectOfType<Camera>().transform.rotation);
        if (parent.CompareTag("Player"))
            three.transform.SetParent(FindObjectOfType<Camera>().transform);
        else
            three.transform.SetParent(parent.transform);
        three.transform.localScale *= 10;
        yield return new WaitForSeconds(2.0f);
        Destroy(three);
        timer.color = Color.white;
    }

    public void RemoveTime(float time, GameObject parent)
    {
        string path = "Enemies/minus3";
        StartCoroutine(FlashTextColor(Color.red, parent, path));
        timeLeft -= time;
    }

    public void EndGame(bool winner)
    {
        gameOver = true;
        if (winner)
        {
            begun = false;
            endGame.text = "You Won!";
        }
        else
        {
            endGame.text = "You died...";
            GameObject.FindWithTag("Player").GetComponent<Controls>().alive = false;
        }
    }

}