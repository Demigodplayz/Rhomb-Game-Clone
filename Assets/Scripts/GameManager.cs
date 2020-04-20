using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public Animator animator;
    public List<GameObject> blocks;
    public Material blockFadeMat, blockedMat;

    [HideInInspector]
    public List<Cube> cubes;

    //LevelIndex editörde görünmez ve yüklenecek sahnenin indexini taşır.
    //Sahne geçişlerinde animasyon tarafından kullanılır.
    [HideInInspector]
    public int levelIndex;

    private int _correctPos = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _cubeCount();
        BlockControl();
    }

    //Sahne açılışında sahnede kaç tane küp olduğunu sayar
    private int _cubeCount()
    {
        return GameObject.FindGameObjectsWithTag("Cube").Count();
    }

    //GameOver fonksiyonu
    public void GameOver()
    {
        animator.SetTrigger("FadeOut");
        levelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    //Levelların tamamlanıp tamamlanmadığını kontrol eder
    public void LevelCompleteCheck()
    {
        _correctPos++;

        if (_correctPos == _cubeCount())
        {
            animator.SetTrigger("FadeOut");
            if (levelIndex < SceneManager.sceneCount-1)
            {
                levelIndex = SceneManager.GetActiveScene().buildIndex + 1;

            }
            else
            {
                levelIndex = 0;
            }
        }
    }


    public void GroupCheck(int index)
    {
        foreach (var item in cubes)
        {
            if (item.groupNumber == index)
            {
                item.PreMove();
            }

        }
    }

    //Engelleri kontrol eder
    public void BlockControl()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (!blocks[i].GetComponent<BoxCollider>().isTrigger)
            {
                blocks[i].GetComponent<BoxCollider>().isTrigger = true;
                blocks[i].GetComponent<MeshRenderer>().material = blockFadeMat;
            }
            else
            {
                blocks[i].GetComponent<BoxCollider>().isTrigger = false;
                blocks[i].GetComponent<MeshRenderer>().material = blockedMat;
            }
        }
    }
}
