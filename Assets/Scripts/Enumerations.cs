public enum GameState { PLAY, PAUSE, MAINMENU, PAUSEMENU, ENDMENU, LOADING };

public enum SaveType { LEVELS, OPTIONS };

public enum FrameRateSetting { DEFAULT, RATE_30, RATE_45, RATE_60 };

public enum SceneType { MAINMENU, LEVEL };

namespace Game.AI
{
    public enum NodeStates { EMPTY, FAILURE, RUNNING, SUCCESS };

    public enum CheckType { IS_EQUAL, IS_GREATER, IS_SMALLER };
}

