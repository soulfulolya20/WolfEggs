using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject zero;

    public float time = 1.5f;

    public int hp;
    public int score;

    public TextMesh counter;

    public GameObject hp1;
    public GameObject hp2;
    public GameObject hp3;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    public int playerPose;

    public GameObject pressStart;

    public bool isPlaying = false;

    public AudioSource soundStep;
    public AudioSource soundCrash;
    public AudioSource soundCount;


    private DatabaseReference _database;

    [HideInInspector] public Transform egg;
    [HideInInspector] public GameObject[] egg1;
    [HideInInspector] public GameObject[] egg2;
    [HideInInspector] public GameObject[] egg3;
    [HideInInspector] public GameObject[] egg4;

    void Start()
    {
        _database = FirebaseDatabase
            .DefaultInstance
            .RootReference
            .Child("users");

        egg = GameObject.Find("Spawn1").transform;
        egg1 = new GameObject[10];
        for (int i = 0; i < egg.childCount; i++)
        {
            egg1[i] = egg.Find(i.ToString()).gameObject;
        }

        egg = GameObject.Find("Spawn2").transform;
        egg2 = new GameObject[10];
        for (int i = 0; i < egg.childCount; i++)
        {
            egg2[i] = egg.Find(i.ToString()).gameObject;
        }

        egg = GameObject.Find("Spawn3").transform;
        egg3 = new GameObject[10];
        for (int i = 0; i < egg.childCount; i++)
        {
            egg3[i] = egg.Find(i.ToString()).gameObject;
        }

        egg = GameObject.Find("Spawn4").transform;
        egg4 = new GameObject[10];
        for (int i = 0; i < egg.childCount; i++)
        {
            egg4[i] = egg.Find(i.ToString()).gameObject;
        }

        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);

        foreach (GameObject eg in egg1)
        {
            eg.SetActive(false);
        }

        foreach (GameObject eg in egg2)
        {
            eg.SetActive(false);
        }

        foreach (GameObject eg in egg3)
        {
            eg.SetActive(false);
        }

        foreach (GameObject eg in egg4)
        {
            eg.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerPose = 1;
            player1.SetActive(true);
            player2.SetActive(false);
            player3.SetActive(false);
            player4.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            playerPose = 2;
            player1.SetActive(false);
            player2.SetActive(true);
            player3.SetActive(false);
            player4.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            playerPose = 3;
            player1.SetActive(false);
            player2.SetActive(false);
            player3.SetActive(true);
            player4.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            playerPose = 4;
            player1.SetActive(false);
            player2.SetActive(false);
            player3.SetActive(false);
            player4.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPlaying) StopGame();
            else StartGame();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator Timer()
    {
        GameObject egg = Instantiate(zero);
        EggsContoller comp = egg.AddComponent<EggsContoller>();
        comp.game = this.GetComponent<Game>();
        comp.spawn = UnityEngine.Random.Range(1, 5);
        if (comp.spawn == 1) comp.egg = egg1;
        if (comp.spawn == 2) comp.egg = egg2;
        if (comp.spawn == 3) comp.egg = egg3;
        if (comp.spawn == 4) comp.egg = egg4;


        yield return new WaitForSeconds(time);
        StartCoroutine(Timer());
    }

    public void Step()
    {
        soundStep.Play();
    }

    public void Count()
    {
        score++;
        counter.text = score.ToString();
        soundCount.Play();
    }

    public void Crash()
    {
        hp++;
        soundCrash.Play();
        if (hp >= 1) hp1.SetActive(true);
        if (hp >= 2) hp2.SetActive(true);
        if (hp >= 3) hp3.SetActive(true);
        if (hp > 3) StopGame();
    }

    private void StartGame()
    {
        isPlaying = true;
        pressStart.SetActive(false);

        player1.SetActive(true);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
        playerPose = 1;
        score = 0;


        hp1.SetActive(false);
        hp2.SetActive(false);
        hp3.SetActive(false);

        StartCoroutine(Timer());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void StopGame()
    {
        isPlaying = false;
        StopAllCoroutines();
        foreach (GameObject eg in egg1)
        {
            eg.SetActive(false);
        }

        foreach (GameObject eg in egg2)
        {
            eg.SetActive(false);
        }

        foreach (GameObject eg in egg3)
        {
            eg.SetActive(false);
        }

        foreach (GameObject eg in egg4)
        {
            eg.SetActive(false);
        }

        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);

        pressStart.SetActive(true);

        GameObject[] eggsArray = GameObject.FindGameObjectsWithTag("zero");
        foreach (GameObject eg in eggsArray)
            if (eg.name == "Egg(Clone)")
                Destroy(eg);

        var namee = DataTransfer.playerName;
        _database.Child(namee).SetValueAsync(score).ContinueWith(task =>
        {
            while (true)
            {
                if (task.IsCompleted)
                {
                    task.ContinueWithOnMainThread(_ => { SceneManager.LoadSceneAsync("TableScene"); });
                    break;
                }
            }
        });
        
    }
}