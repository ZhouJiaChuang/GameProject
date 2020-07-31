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

/// <summary>
/// 自动状态枚举
/// </summary>
public enum EAutoState
{
    /// <summary>
    /// 当前不进行任何自动状态
    /// 手动操作中
    /// </summary>
    None,
    /// <summary>
    /// 自动攻击状态
    /// 该状态下,会自动攻击视野内所有单位
    /// </summary>
    AutoAttack,
    /// <summary>
    /// 自动任务状态,
    /// 该状态下,自动进行任务所需要的操作
    /// </summary>
    AutoMission,
    /// <summary>
    /// 自动采集状态
    /// 该状态下,会自动采集是视野内所有的可采集目标
    /// </summary>
    AutoGather,
}

/// <summary>
/// 移动完成之后的行为
/// </summary>
public enum EAfterMoveAction
{
    /// <summary>
    /// 仅仅移动到目标点
    /// </summary>
    None,
    /// <summary>
    /// 移动到目标位置进行攻击  
    /// </summary>
    Attack,
}

/// <summary>
/// 单位类型
/// </summary>
public enum EAvaterType
{
    MainPlayer,
    Player,
    Monster,
    Npc,
}

/// <summary>
/// 对象池移除方法
/// </summary>
public enum EPoolItemRemoveMethod
{
    None,
    OnDisEnable,
    RemoveVision,
}

/// <summary>
/// 资源类型
/// </summary>
public enum EResourceType
{
    None = 0,
    /// <summary>
    /// 身体模型
    /// </summary>
    Body,
    /// <summary>
    /// 武器模型
    /// </summary>
    Weapone,
    /// <summary>
    /// 图集资源
    /// </summary>
    Atlas,
    /// <summary>
    /// UI上特效资源
    /// </summary>
    UIEffect,
    /// <summary>
    /// 场景特效资源
    /// </summary>
    SceneEffect,
    /// <summary>
    /// 音效
    /// </summary>
    Audio,
    /// <summary>
    /// 表格
    /// </summary>
    Table,
    /// <summary>
    /// 小地图
    /// </summary>
    MiniMap,
    /// <summary>
    /// 场景文件
    /// </summary>
    Scene,
    /// <summary>
    /// 贴图
    /// </summary>
    Picture,
}

/// <summary>
/// 资源加载优先级
/// 值越大越有限
/// </summary>
public enum EResourceAssistType
{
    None,
    Other,
    Player,
    MainPlayer,
    UI,
    Terrain,
    ForceLoad,
}

/// <summary>
/// 游戏的运行平台
/// </summary>
public enum ERunPlatform
{
    Editor,
    Android,
    IOS,
    AndroidEditor,
}

/// <summary>
/// 编辑器用路径
/// </summary>
public enum EEditorPath
{
    /// <summary>
    /// 空
    /// </summary>
    None,
    /// <summary>
    /// 本地资源加载路径
    /// </summary>
    LocalResourcesLoadPath,
}

public enum EUILayerType
{
    /// <summary>
    /// 预设无设置,走代码
    /// </summary>
    None = -50,
    Back = -30,
    /// <summary>
    /// 1.该层所摆放的游戏UI内容，包括按钮，信息，图片，活动图标等（除TIPS）以及相关的界面特效，都必须是玩家进入游戏中，不做任何操作，就可以看到UI信息
    /// </summary>
    BasicPlane = 0,
    /// <summary>
    /// 2.该层所摆放的游戏UI内容，系统公告以及自动寻路，自动战斗，小飞鞋按钮等，玩家将角色托管给系统操作时，给予一些辅助性UI文字提示或者按钮以及各类气泡提示框
    /// </summary>
    AisstPlane = 50,
    /// <summary>
    /// 3.该层摆放游戏内界面内容，主要是各个功能界面以及对应的二级，三级界面。该层内TIPS信息，界面特效出现较多，统一按照补充说明内第2条处理
    /// </summary>
    WindowsPlane = 200,
    /// <summary>
    /// 4.该层摆放的是需要遮住其余Windows层界面,且需要显示Tips层的界面
    /// </summary>
    TopWindowsPlane = 700,
    /// <summary>
    /// 5.点击无法操作时的警示条
    /// </summary>
    TipsPlane = 800,
    /// <summary>
    /// 6.提示的顶层,该层的提示需要在其他提示上方,但低于新手引导层
    /// </summary>
    TopTipsPlane = 950,
    /// <summary>
    /// 7.该层所摆放的游戏UI为新手引导的UI，包括UI遮罩，指向标，提示UI呼吸框等等，只能是和新手引导有关的UI
    /// </summary>
    GuidePlane = 1000,
    /// <summary>
    /// 8.该层所摆放的是，游戏中转换场景时所需要的UI美术资源和图片资源
    /// </summary>
    ChgsenPlane = 1800,
    /// <summary>
    /// 9.该层只有在玩家客户端无法通信，断线时才会出现，弹窗信息/动画/UI提示
    /// </summary>
    ConnectPlane = 2000,
    /// <summary>
    /// 10.登录界面所有UI信息
    /// </summary>
    TopPlane = 2100,
    /// <summary>
    /// 11.点击反馈特效
    /// </summary>
    Hint = 2200,
}

public enum EResourceLoadType
{
    /// <summary>
    /// 工程内读取,使用Resource.Load()
    /// PS:有时候使用AB不能查看一些东西,例如Animator的状态
    /// </summary>
    Project,
    /// <summary>
    /// StreamAssest资源读取
    /// </summary>
    StreamAssest,
    /// <summary>
    /// 本地文件路径,自己存在ab的文件夹路径
    /// </summary>
    Local,
    /// <summary>
    /// 工程自身的资源路径
    /// 在window上就是file:///C:/Users/admin/AppData/LocalLow/公司/项目名字/
    /// 在Android上就是Android/data/data/包名/files/
    /// </summary>
    Normal,
}