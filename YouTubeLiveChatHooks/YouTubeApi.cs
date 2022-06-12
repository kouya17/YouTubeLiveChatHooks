using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
namespace YouTubeLiveChatHooks
{
    internal class ChatResponse
    {
        public IList<LiveChatMessage> liveChatMessages { get; set; }
        public string nextPageToken { get; set; }

        public ChatResponse(IList<LiveChatMessage> liveChatMessages, string nextPageToken)
        {
            this.liveChatMessages = liveChatMessages;
            this.nextPageToken = nextPageToken;
        }
    }

    internal class YouTubeApi
    {
        private string _apiKey = "";
        private YouTubeService? _youtubeService = null;
        public YouTubeApi(string apiKey) {
            _apiKey = apiKey;
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey
            });
        }

        public async Task<string> getChatIdByVideoId(string videoId)
        {
            if (_youtubeService is null)
            {
                return "";
            }
            var request = _youtubeService.Videos.List("liveStreamingDetails");
            request.Id = videoId;

            var response = await request.ExecuteAsync();
            return response.Items[0].LiveStreamingDetails.ActiveLiveChatId;
        }

        public async Task<ChatResponse?> getChat(string chatId, string pageToken = "")
        {
            if(_youtubeService is null)
            {
                return null;
            }
            var request = _youtubeService.LiveChatMessages.List(chatId, "snippet");
            if (pageToken != "")
            {
                request.PageToken = pageToken;
            }
            var response = await request.ExecuteAsync();
            return new ChatResponse(response.Items, response.NextPageToken);
        }
    }
}
