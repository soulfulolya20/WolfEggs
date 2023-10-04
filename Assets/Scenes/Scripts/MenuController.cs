using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour{

    public GameObject input;
    public GameObject table;

    public TMP_InputField playerName;
    public TMP_Text status;
    public GameObject tableContent;
    public GameObject tableContentElement;

    private DatabaseReference _databaseReference;
    
    private void Start()
    {
        Debug.Log("Я родился!");
        _databaseReference = FirebaseDatabase
            .DefaultInstance
            .RootReference
            .Child("users");
        
        // На старте загружаем инфу о человечках
        UpdateTable();
    }
    
    // Метод обновления таблички
    private void UpdateTable()
    {
        _databaseReference.OrderByValue().GetValueAsync().ContinueWith(task =>
        {
            while (true)
            {
                if (task.IsCompleted)
                {
                    // Получаем человечков и складываем их в листик
                    var dataSnapshot = task.Result;
                    List<TableItem> items = new List<TableItem>();
                    foreach (var child in dataSnapshot.Children)
                        items.Add(new TableItem(child.Key, int.Parse(child.Value.ToString())));

                    //LogList(items);
                    // Сортируем человечков
                    items = items.OrderByDescending(obj => obj.score).ToList();
                    LogList(items);

                    // Выводим человечков
                    
                    task.ContinueWithOnMainThread(_ =>
                    {   
                        int i = 0;
                        foreach (var item in items)
                        {
                            tableContentElement.GetComponent<TMP_Text>().text = $"{i + 1}. {item}";
                            Instantiate(tableContentElement, tableContent.transform);
                            i++;
                        }
                    });
                        
                    break;
                }
            }
        });
    }
    
    void LogList<T>(List<T> list)
    {

        foreach (var item in list)
        {
            Debug.Log(item);
        }
    }
    
    // Переключала инпутов
    private void EnableInput()
    {
        input.SetActive(true);
        table.SetActive(false);
    }
    
    // Переключала инпутов
    private void EnableTable()
    {
        input.SetActive(false);
        table.SetActive(true);
    }

    
    // Конпка старт
    public void OnStartClick()
    {
        status.text = "";
        if (playerName.text == "")
        {
            status.text = "Нужно ввести имя игрока!";
            return;
        }
        // Сохраняем имя игрока
        DataTransfer.playerName = playerName.text;
        
        // Переход на новую сцену с игрой
        SceneManager.LoadSceneAsync("GameScene");
    }
    
    // Кнопка таблицы
    public void OnTableClick()
    {
        EnableTable();
    }
    
    // Кнопка назад
    public void OnTableBackClick()
    {
        EnableInput();
    }
    

}
