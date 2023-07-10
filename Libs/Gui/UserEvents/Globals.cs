global using IWin = UserEvents.IWinUserEventsSupport;
global using IMainWin = UserEvents.IMainWinUserEventsSupport;
global using INode = UserEvents.INodeStateUserEventsSupport;
global using ICtrl = UserEvents.ICtrlUserEventsSupport;
global using Obs = System.Reactive.Linq.Observable;
global using static LoggingConfig.Singletons;
global using static LoggingConfig.Logging_.LogUtils;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("1_UserEvents.Tests")]