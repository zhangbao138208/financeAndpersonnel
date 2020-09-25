namespace DncZeus.Api.ViewModels.System.Dictionary
{
    public class DictionaryJsonModel
    {
        public string Code { get; set; }
        public string TypeCode { get; set; }
        /// <summary>
        /// 展示用
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 使用值
        /// </summary>
        public string Value { get; set; }
        public bool Fixed { get; set; }
    }
}
