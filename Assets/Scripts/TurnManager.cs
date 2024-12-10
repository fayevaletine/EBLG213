using UnityEngine;

public class TurnManager
{
    private int m_TurnCount;

    public TurnManager()
    {
        m_TurnCount = 1;
    }

    public void Tick()
    {
        m_TurnCount += 1;
        Debug.Log("Current turn count : " + m_TurnCount);

        // Olay� tetikle
        OnTick?.Invoke();

        // Olay�n null olmad���n� kontrol et ve tetikle
        if (OnTick != null)
        {
            OnTick.Invoke();
        }
    }

    // Olay tan�mlamas�
    public event System.Action OnTick;
}