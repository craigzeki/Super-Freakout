using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _versionText;

    private void Awake()
    {
        _versionText.text = "v" + Application.version;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
