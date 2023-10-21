using System.Collections.Generic;

//update in inspector with scriptable object
[System.Serializable]

public class SceneRoute
{
    public SceneName fromSceneName;
    public SceneName toSceneName;
    public List<ScenePath> scenePathList;
}