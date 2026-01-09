using JKFrame;
using System.Collections.Generic;

public class Player_BuffController : BuffController
{
    private Dictionary<Buff, UI_BuffSlot> slotDic = new Dictionary<Buff, UI_BuffSlot>();

    protected override void OnBuffStart(Buff buff)
    {
        base.OnBuffStart(buff);
        UI_GameSceneMainWindow mainWindow = UISystem.GetWindow<UI_GameSceneMainWindow>();
        UI_BuffSlot slot = mainWindow.AddBuff(buff.config);
        slot.UpdateLayer(buff.layer);
        slot.UpdateMask(buff.destroyTimer / buff.config.duration);
        slotDic.Add(buff, slot);
    }

    protected override void OnBuffUpdate(Buff buff)
    {
        base.OnBuffUpdate(buff);
        UI_BuffSlot slot = slotDic[buff];
        slot.UpdateMask(buff.destroyTimer / buff.config.duration);
        slot.UpdateLayer(buff.layer); ;
    }

    protected override void OnBuffEnd(Buff buff)
    {
        base.OnBuffEnd(buff);
        if (slotDic.Remove(buff, out UI_BuffSlot slot))
        {
            UI_GameSceneMainWindow mainWindow = UISystem.GetWindow<UI_GameSceneMainWindow>();
            mainWindow.RemoveBuff(slot);
        }
    }
}
