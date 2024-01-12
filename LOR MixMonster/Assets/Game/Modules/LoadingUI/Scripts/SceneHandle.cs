public class SceneHandle
{
    public static string GetScene(int sceneId)
    {
        switch (sceneId)
        {
            case 1:
                return "LoadingScene";
                break;
            case 2:
                return "MainScene";
                break;
            case 3:
                return "GameScene";
                break;
            case 4:
                return "DefenceScene";
                break;
            case 10:
                return "IntroScene";
                break;
        }
        return null;
    }
}