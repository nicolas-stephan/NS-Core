using NS.Core.Utils;
using UnityEngine;

public class Example : MonoBehaviour
{
    public SceneField mainScene;

    public void Start() {
        mainScene.LoadScene();
    }
}
