using System;
using TEngine;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    void Awake()
    {
        ModuleSystem.GetModule<IUpdateDriver>();
        ModuleSystem.GetModule<IResourceModule>();
        ModuleSystem.GetModule<IDebuggerModule>();
        ModuleSystem.GetModule<IFsmModule>();
        Settings.ProcedureSetting.StartProcedure().Forget();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 控制日志窗口显隐
            Debugger.Instance.ActiveWindow = !Debugger.Instance.ActiveWindow;
        }
    }
}