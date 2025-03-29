
using System.Collections.Concurrent;

namespace Ayanotan.Sage;

// 存放战斗中的缓存数据 战斗重置后也会跟着清除
// 举例： 诗人需要记录上一次的双dot什么时候上的/吃了多少强化资源，来决定是否在恰当的时候立即刷新双dot
public class SageBattleData
{
    public static ConcurrentDictionary<string, long> Healing_CD = new ConcurrentDictionary<string, long>();

    public static void Reset()
    {
        Healing_CD = new ConcurrentDictionary<string, long>();
    }
}
