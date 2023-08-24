using TMPro;
using UnityEngine;

public class CompleteGameBehaviour : MonoBehaviour
{

    public TMP_Text CompleteTimeText;

    public void SetGameCompleteTime(long completeTime)
    {
        long hour = (int)(completeTime / 3600);

        int minuteOrSecond = (int)(completeTime % 3600);

        int minute = minuteOrSecond / 60;

        int second = minuteOrSecond % 60;

        CompleteTimeText.SetText(string.Format("Complete Time : {0}:{1}:{2}", hour, minute, second));
    }

}