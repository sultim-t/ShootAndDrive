﻿using SD.Game;

namespace SD.UI.Menus
{
    class CutsceneMenu : AAnimatedMenu
    {
        protected override void SignToEvents()
        {
            CutsceneManager.OnCutsceneStart += ShowThisMenu;
        }

        protected override void UnsignFromEvents()
        {
            CutsceneManager.OnCutsceneStart -= ShowThisMenu;
        }
    }
}
