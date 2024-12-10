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

        // Olayý tetikle
        OnTick?.Invoke();

        // Olayýn null olmadýðýný kontrol et ve tetikle
        if (OnTick != null)
        {
            OnTick.Invoke();
        }
    }

    // Olay tanýmlamasý
    public event System.Action OnTick;
}