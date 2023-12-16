namespace DeviceSimulator.Wpf.Models
{
    public class MyVersion
    {
        /// <summary>
        /// 头描述
        /// </summary>
        public static string Header { get; set; } = "版本";

        /// <summary>
        /// 最新版本
        /// </summary>
        public static string Last { get; set; } = "v0.0.1";

        /// <summary>
        /// 更新日期
        /// </summary>
        public static string UpdateTime { get; set; } = null!;

        /// <summary>
        /// 更新描述
        /// </summary>
        public static string Decription { get; set; } = null!;

        /// <summary>
        /// 全版本描述
        /// </summary>
        public static string Full { get => $"{Header} {Last}@{UpdateTime}：{Decription}"; }
    }
}