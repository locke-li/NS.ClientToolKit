namespace CenturyGame.Framework
{
    public interface IGameEntry
    {
        void StartGame();

        void Update();

        void LateUpdate();

        void FixedUpdate();

        void QuitGame();

        void OnApplicationFocus(bool focus);

        void OnApplicationPause(bool pause);
    }
}