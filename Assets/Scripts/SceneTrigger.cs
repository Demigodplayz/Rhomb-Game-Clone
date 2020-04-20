using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{


    //Fade animasyonu bitince animasyon tarafından level yüklemek için çağrılır.
    public void OnFadeComplete()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameManager.Instance.levelIndex);
    }
}
