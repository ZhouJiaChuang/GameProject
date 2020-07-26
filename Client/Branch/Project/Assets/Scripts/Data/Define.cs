/***
 * Author --- ZJC
 * Description --- 杂项定义
 * Function:
 * 
 */

 /// <summary>
 /// 场景枚类
 /// 通过这个枚举名字进行场景加载
 /// </summary>
public enum ESceneType
{
    /// <summary>
    /// 初始场景,用来进行游戏配置的加载
    /// </summary>
    FirstScene,
    /// <summary>
    /// 空白场景,用来进行场景切换的场景过度
    /// </summary>
    EmptyScene,
    /// <summary>
    /// 登入场景,用来展示登入创角
    /// </summary>
    LoginScene,
    /// <summary>
    /// 主要的游戏场景
    /// </summary>
    MainScene
}

