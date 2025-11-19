using UnityEngine;


public class HADebug
{
    /// <summary>
    /// 是否控制台输出：
    /// true 输出，false 不输出
    /// </summary>
    public static bool DebugMode {  get; set; }

    #region 普通
    /// <summary>
    /// 日志：普通
    /// </summary>
    /// <param name="msg">日志消息</param>
    public static void Log(string msg)
    {
        if (DebugMode)
        {
            Debug.Log(msg);
        }
    }

    /// <summary>
    /// 格式化日志：普通
    /// </summary>
    /// <param name="format">使用占位符的日志消息</param>
    /// <param name="args">对应占位符的参数</param>
    public static void LogFormat(string format, params object[] args)
    {
        if (DebugMode)
        {
            Debug.LogFormat(format, args);
        }
    }
    #endregion

    #region 警告
    /// <summary>
    /// 日志：警告
    /// </summary>
    /// <param name="msg">日志消息</param>
    public static void LogWarning(string msg)
    {
        if (DebugMode)
        {
            Debug.LogWarning(msg);
        }
    }

    /// <summary>
    /// 格式化日志：警告
    /// </summary>
    /// <param name="format">使用占位符的日志消息</param>
    /// <param name="args">对应占位符的参数</param>
    public static void LogWarning(string format, params object[] args)
    {
        if (DebugMode)
        {
            Debug.LogWarningFormat(format, args);
        }
    }
    #endregion

    #region 错误
    /// <summary>
    /// 日志：错误
    /// </summary>
    /// <param name="msg">日志消息</param>
    public static void LogError(string msg)
    {
        if (DebugMode)
        {
            Debug.LogError(msg);
        }
    }

    /// <summary>
    /// 格式化日志：错误
    /// </summary>
    /// <param name="format">使用占位符的日志消息</param>
    /// <param name="args">对应占位符的参数</param>
    public static void LogErrorFormat(string format, params object[] args)
    {
        if (DebugMode)
        {
            Debug.LogErrorFormat(format, args);
        }
    }
    #endregion
}
