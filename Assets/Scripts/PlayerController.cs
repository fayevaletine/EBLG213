using UnityEngine;
using UnityEngine.InputSystem; // Input System kütüphanesi

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;

    // Oyuncuyu başlatma metodu
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell); // Oyuncuyu başlangıç hücresine taşır
    }

    // Oyuncuyu yeni bir hücreye taşıma metodu
    public void MoveTo(Vector2Int newCellTarget)
    {
        m_CellPosition = newCellTarget; // Hücre pozisyonunu güncelle
        transform.position = m_Board.CellToWorld(m_CellPosition); // Oyuncunun dünya pozisyonunu güncelle
    }

    private void Update()
    {
        Vector2Int newCellTarget = m_CellPosition; // Yeni hedef hücre
        bool hasMoved = false;

        // Klavye girdilerini kontrol et
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1; // Yukarı hareket
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1; // Aşağı hareket
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1; // Sağa hareket
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1; // Sola hareket
            hasMoved = true;
        }

        if (hasMoved)
        {
            // Yeni hücrenin geçilebilir olup olmadığını kontrol et
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            if (cellData != null && cellData.Passable)
            {
                // Increment the turn count in the GameManager
                GameManager.Instance.TurnManager.Tick();
                MoveTo(newCellTarget); // Oyuncuyu yeni pozisyona taşır

                // Eğer hücrede bir nesne varsa ve nesne `PlayerEntered` fonksiyonuna sahipse:
                if (cellData.ContainedObject is WallObject wallObj)
                {
                    wallObj.PlayerEntered();
                }
                else if (cellData.ContainedObject is FoodObject foodObj)
                {
                    foodObj.PlayerEntered();
                }
            }
        }
    }
}

