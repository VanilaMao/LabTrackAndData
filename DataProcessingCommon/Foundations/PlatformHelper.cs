namespace Lab.Common.Foundations
{
    public static class PlatformHelper
    {
        private static IPlatformHelper _current = new DefaultPlatformHelper();

        /// <summary>
        /// Gets or sets the current <see cref="IPlatformHelper"/>.
        /// </summary>
        public static IPlatformHelper Current
        {
            get { return _current; }
            set { _current = value; }
        }
    }
}
