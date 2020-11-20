using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestLevelSelect : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button exitButton;
    
    void Start()
    {
        level1Button.onClick.AddListener(() => SceneManager.LoadScene(1));
        level2Button.onClick.AddListener(() => SceneManager.LoadScene(2));
        level3Button.onClick.AddListener(() => SceneManager.LoadScene(3));
        exitButton.onClick.AddListener(Application.Quit);
    }
}
