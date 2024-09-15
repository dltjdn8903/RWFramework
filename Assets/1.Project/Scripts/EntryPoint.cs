using UnityEngine;

public class EntryPoint
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AppInit()
    {
        Application.targetFrameRate = 60;

        Time.timeScale = 1f;

        Application.runInBackground = false;

        //TableData.Load();

        Game.gameData.Init();

        //MDSoundManager.Init();

        //InitializeScreenInput();
    }

    ///// <summary>
    ///// 네트워크 관련 초기화
    ///// </summary>
    //private static void InitializeNetwork()
    //{
    //    if(MDHttpNetwork.IsInitialized)
    //        MDHttpNetwork.Reset();

    //    MDHttpNetwork.Initialize();

    //    MDHttpNetwork.SetUrlBaseMethod(() => GameConfig.ServerUrlBase);

    //    MDHttpNetwork.OnBeginRequest = () => TanukiUIWait.OpenUI();
    //    MDHttpNetwork.OnEndRequest = () => TanukiUIWait.CloseWait();
    //}
    
    ///// <summary>
    ///// 가상 입력장치 관련 초기화
    ///// </summary>
    //private static void InitializeScreenInput()
    //{
    //    OnScreenMouse.CreateInstance();
    //    OnScreenMouse.SetEnable(true);
    //    OnScreenMouse.SetActive(false);
    //    OnScreenMouse.SetSensitivityMethod(() => Mathf.Min(Game.gameData.option.JoypadXSensitivity, Game.gameData.option.JoypadYSensitivity));
    //    OnScreenMouse.SetDataMethod(() => GameConfig.Config.screenMouse);

    //    TanukiUIScreenKeyboard.CreateInstance();
    //    TanukiUIScreenKeyboard.SetActive(false);
    //}
}
