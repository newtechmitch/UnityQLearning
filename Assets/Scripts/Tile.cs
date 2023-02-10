using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Text rewardText;
    [SerializeField] private Text qValueText;
    
    public void SetReward(int reward)
    {
        rewardText.text = reward.ToString();
    }
    
    public void SetQValue(float qValue)
    {
        qValueText.text = qValue.ToString("F4");
    }
}
