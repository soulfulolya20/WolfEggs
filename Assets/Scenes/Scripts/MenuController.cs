using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class MenuController : MonoBehaviour {

    public GameObject input;
    public GameObject table;

    public TMP_InputField playerName;
    public TMP_Text status;
    public GameObject tableContent;
    public GameObject tableContentElement;

    private DatabaseReference _databaseReference;

    private void Start() {
        Debug.Log("Я родился!");
        _databaseReference = FirebaseDatabase
            .DefaultInstance
            .RootReference
            .Child("users");

        // На старте загружаем инфу о человечках
        UpdateTable();
    }

    // Метод обновления таблички
    private void UpdateTable() {
        _databaseReference.OrderByValue().GetValueAsync().ContinueWith(task => {
            while (true) {
                if (task.IsCompleted) {
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

                    task.ContinueWithOnMainThread(_ => {
                        int i = 0;
                        foreach (var item in items) {
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

    void LogList<T>(List<T> list) {

        foreach (var item in list) {
            Debug.Log(item);
        }
    }

    // Переключала инпутов
    private void EnableInput() {
        input.SetActive(true);
        table.SetActive(false);
    }

    // Переключала инпутов
    private void EnableTable() {
        input.SetActive(false);
        table.SetActive(true);
    }

    // Конпка старт
    public void OnStartClick() {
        status.text = "";
        string playerNameInput = playerName.text.Trim(); // Убираем пробелы в начале и конце строки

        // Проверка на допустимость символов в имени (допустимы только буквы и цифры)
        if (!Regex.IsMatch(playerNameInput, @"^[a-zA-Z0-9а-яА-Я]+$")) {
            status.text = "Имя игрока может содержать только буквы и цифры.";
            return;
        }

        // Проверка на минимальную длину имени (например, 3 символа)
        if (playerNameInput.Length < 3) {
            status.text = "Имя игрока должно содержать как минимум 3 символа.";
            return;
        }

        // Проверка уникальности имени в базе данных
        DataTransfer.playerName = playerNameInput;
        SceneManager.LoadSceneAsync("GameScene");
    }

    // private async void CheckPlayerNameUniqueness(string playerNameInput)
    // {
    //     // Получаем данные из базы данных
    //     var dataSnapshot = await _databaseReference.OrderByValue().GetValueAsync();
    //
    //     if (dataSnapshot.HasChildren)
    //     {
    //         // Создаем список для хранения имен из базы данных
    //         List<string> databaseNames = new List<string>();
    //
    //         foreach (var child in dataSnapshot.Children)
    //         {
    //             // Получаем имя из базы данных
    //             string databaseName = child.Key;
    //         
    //             // Добавляем имя в список
    //             databaseNames.Add(databaseName);
    //         }
    //
    //         // Проверяем, есть ли введенное имя в списке имен из базы данных
    //         if (databaseNames.Contains(playerNameInput))
    //         {
    //             // Имя уже используется, выводим сообщение об ошибке и выходим из метода
    //             status.text = "Имя игрока уже используется. Пожалуйста, выберите другое имя.";
    //         }
    //         else
    //         {
    //             // Если имя уникально, сохраняем его и переходим к игре
    //             DataTransfer.playerName = playerNameInput;
    //             SceneManager.LoadSceneAsync("GameScene");
    //         }
    //     }
    //     else
    //     {
    //         // Если база данных пуста, можно сразу сохранить имя и перейти к игре
    //         DataTransfer.playerName = playerNameInput;
    //         SceneManager.LoadSceneAsync("GameScene");
    //     }
    // }

    // Кнопка таблицы
    public void OnTableClick() {
        EnableTable();
    }

    // Кнопка назад
    public void OnTableBackClick() {
        EnableInput();
    }
}
