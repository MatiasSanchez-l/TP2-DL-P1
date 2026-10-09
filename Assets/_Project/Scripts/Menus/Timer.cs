using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI timerText;

   public float timeRemaining = 60f;

   void Update()
   {
      if (timeRemaining > 0)
      {
        timeRemaining -= Time.deltaTime;
      }
      else if (timeRemaining <= 0)
      {
        timeRemaining = 0;
      }

      int minutes = Mathf.FloorToInt(timeRemaining / 60);
      int seconds = Mathf.FloorToInt(timeRemaining % 60);

      timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
   }
}
