using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography;

//Sadece Editörde Kullanılır 
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Cube : MonoBehaviour
{

    #region public variables
    public float speed = 5;
    public GameObject fadedImage;
    public GameObject blockPrefab;
    [Range(0, 2)]
    public int groupNumber = 0;
    #endregion

    #region private variables
    GameManager _gm;
    Transform[] _target;
    LineRenderer _lineRenderer;
    Rigidbody _rb;
    bool _clicked = false;
    int _wavePointIndex = 0;
    #endregion


    //Waypointleri görür,rotayı oluşturur ve çizer
    private void Awake()
    {

        _rb = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _target = new Transform[transform.childCount];
        _lineRenderer.positionCount = _target.Length;


        for (int i = 0; i < _target.Length; i++)
        {
            _target[i] = transform.GetChild(i);
            _lineRenderer.SetPosition(i, _target[i].position);
        }

        Instantiate(fadedImage, _target[_target.Length - 1].position, transform.rotation);
    }

    private void Start()
    {
        _gm = GameManager.Instance;
        if (groupNumber != 0)
        {
            _gm.cubes.Add(this);
        }

        GroupBasedColor();

    }

    //Gruba sahip olan küpleri belirginleştirir.
    void GroupBasedColor()
    {
        if (groupNumber == 1)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        else if (groupNumber == 2)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    //Grup 0'sa harekete geç
    private void OnMouseUp()
    {
        if (groupNumber == 0)
        {
            PreMove();
        }
        else
        {
            _gm.GroupCheck(groupNumber);
        }
    }

    public void PreMove()
    {
        if (!(_wavePointIndex >= _target.Length - 1))
        {
            transform.DetachChildren();
            _clicked = true;
            //Engel sıra kontrol
            _gm.BlockControl();
        }
    }


    //Hareket
    private void FixedUpdate()
    {
        if (_clicked)
        {
            Vector3 dir = _target[_wavePointIndex].position - transform.position;
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

            //lineRenderer.SetPosition(_wavePointIndex, transform.position);

            if (Vector3.Distance(transform.position, _target[_wavePointIndex].position) <= 0.2f)
            {
                GetNextWaypoint();
            }
        }

    }

    //GameOver!
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube")
        {
            speed = 0;
            _gm.GameOver();
        }
        if (collision.gameObject.tag == "Block")
        {
            speed = 0;
            _gm.GameOver();
        }

    }

    //Engel listesinden kendi engelini kaldırır ve objeyi yok eder
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Block")
        {
            _gm.blocks.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    //Rotada üzerinde bir sonraki waypointi bul
    void GetNextWaypoint()
    {
        //Eğer rota tamamlandıysa
        if (_wavePointIndex >= _target.Length - 1)
        {
            _lineRenderer.positionCount = 0;
            _clicked = false;
            _rb.mass = 10000;
            _gm.LevelCompleteCheck();
            return;
        }

        _wavePointIndex++;
    }

    //Sadece Editörde Kullanılır
    //Sahnede rotayı daha rahat görebilmemiz için belirteçler oluşturur
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.GetChild(i).localPosition, transform.localScale);
        }
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.GetChild(i).localPosition, transform.GetChild(i + 1).localPosition);
        }
    }
#endif
}

//Sadece Editörde Kullanılır
//Inspector'da button oluşturarak waypoint yaratmamızı sağlar
#if UNITY_EDITOR
[CustomEditor(typeof(Cube))]
//[System.Serializable]
class cubeEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        serializedObject.Update();

        Cube cubeScript = (Cube)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Waypoint Türet", GUILayout.Height(25), GUILayout.Width(100)))
        {
            GameObject wayPoint = new GameObject();
            wayPoint.tag = "Point";
            wayPoint.name = "WayPoint " + cubeScript.transform.childCount;
            wayPoint.transform.parent = cubeScript.transform;
            wayPoint.transform.position = cubeScript.transform.position;
        }

        if (GUILayout.Button("Engel Türet", GUILayout.Height(25), GUILayout.Width(100)))
        {
            Transform parent = cubeScript.transform.GetChild(cubeScript.transform.childCount - 1).transform;
            Transform parentsSibling = cubeScript.transform.GetChild(cubeScript.transform.childCount - 2).transform;
            Vector3 pos = Vector3.Lerp(parentsSibling.position, parent.position, 0.5f);
            GameObject block = Instantiate(cubeScript.blockPrefab, pos, Quaternion.LookRotation(pos - parent.position, Vector3.forward), parent);
            block.tag = "Block";
            block.name = "Block";
        }
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
