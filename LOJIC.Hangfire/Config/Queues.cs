namespace LOJIC.Hangfire.Config
{
    public class Queue
    {
        /// <summary>
        /// Default queue
        /// </summary>
        public const string Default = "default";
        /// <summary>
        /// Should be used to manually schedule events
        /// </summary>
        public const string AdHoc = "adhoc";
        /// <summary>
        /// Used for jobs that target IIS Web Servers
        /// </summary>
        public const string IISWebServer = "IISWebServer";

        public const string AgeGis = "age_gis";
    }
}
