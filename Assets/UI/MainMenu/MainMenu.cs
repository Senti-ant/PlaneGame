using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject OptionsMenu;

    public void ToggleOptionsMenu() => OptionsMenu.SetActive(!OptionsMenu.activeInHierarchy);
    public void Play(int level /*0 is the tutorial*/ ) => SceneManager.LoadScene(level+1);
    public void Quit() => Application.Quit();
}
