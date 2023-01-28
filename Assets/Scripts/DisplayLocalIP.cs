using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayLocalIP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ipLabel;
    // Start is called before the first frame update
    void Start()
    {
        _ipLabel.text = "Your IP: " + LocalIP.Instance.MyAddressLocal;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
