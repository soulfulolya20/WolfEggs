using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;


// Здесь вся логика игры
public class GameController : MonoBehaviour {

    private DatabaseReference _database;
    
    // Это твой счет игры
    private int score;
    
    void Start()
    {
        _database = FirebaseDatabase
            .DefaultInstance
            .RootReference
            .Child("users");
    }
    
    
    // Этот метод должен вызываться кодга игра окончена
    public void OnFinish()
    {
        // Рандом чисто для примера имитации набора очков
        System.Random random = new System.Random();
        score = random.Next(1, 101);
        // Вырежи его и поставь очки которые получены в игре
        
        // Достаем имя игрока
        var name = DataTransfer.playerName;
        
        // Посылаем результат на Firebase
        _database.Child(name).SetValueAsync(score);
        
        // Загружаемся в игру
        SceneManager.LoadSceneAsync("TableScene");
    }
    
}
