using TEngine;

namespace GameLogic
{
    public static class EventId
    {
        // -------------------------------- Common --------------------------------
        /// <summary>
        /// 无参。接收到此消息时触发某个动作。
        /// </summary>
        public static readonly int AutoRefresh = RuntimeId.ToRuntimeId("AutoRefresh");
    }
}