namespace YouTubeLiveChatHooks
{
    internal class LiveStreamUrl
    {
        private string _url = "";
        public LiveStreamUrl(string url)
        {
            _url = url;
        }

        public string? getVideoId()
        {
            var uri = new Uri(_url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"];
        }
    }
}
