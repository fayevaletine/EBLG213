using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // Singleton instance of GameManager
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;

    // TurnManager property
    public TurnManager TurnManager { get; private set; }

    public UIDocument UIDoc; // Reference to UIDocument in the scene
    private Label m_FoodLabel; // UI label for displaying food amount

    private int m_FoodAmount = 100;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // Initialize TurnManager
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        // Initialize BoardManager and spawn Player
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

        // Link UI label
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = "Food : " + m_FoodAmount;
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        Debug.Log("Current amount of food : " + m_FoodAmount);
    }
}
