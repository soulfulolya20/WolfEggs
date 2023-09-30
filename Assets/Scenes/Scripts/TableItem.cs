// Элемент в таблице. Нужен для сортировки
class TableItem {
        
    public string name;
    public int score;

    public TableItem(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

    public override string ToString()
    {
        return $"{name} - {score}";
    }
}