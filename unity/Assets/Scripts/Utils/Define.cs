using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Join,
        Museum,
        Home,
        Story,
        Game,
        Loading,
        BeforeStory,
        StoryOne,
        StoryTwo,
        StoryThree,
        StoryFour,
        StoryFive,
        StorySix,
        AfterStory,
        GameOne,
        MetaverseWar,
        MotionGame,
        ShipView,
        Quiz
    }
     public enum StoryType
     {
         Before,
         After
     }

    public enum ConfirmType
    {
        Home,
        Exit
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag, 
        PointerEnter,
        PointerExit,
    }

    public enum MouseEvent
    {
        Press,
        Click,
        PointerEnter,
        PointerExit,
    }

    public enum CameraMode
    {
        QuarterView,
    }
}
