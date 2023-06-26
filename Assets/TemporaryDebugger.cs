using UnityEngine;

public class TemporaryDebugger : MonoBehaviour
{
    public static TemporaryDebugger Instance { get; private set; }

    [SerializeField] private TMPro.TMP_Text _label;

    private void Start()
    {
        Instance = this;
	}

    public void Print(object text)
    {
        _label.text += text.ToString();
	}

}
