namespace YouTubeLiveChatHooks
{
    public partial class Form1 : Form
    {
        private YouTubeApi? _youtubeApi = null;
        private string _pageToken = "";
        private string _searchString = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void appendChat(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(this.appendChat), message);
                return;
            }
            textBox4.AppendText(message);
        }

        private void appendSearchedChat(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(this.appendSearchedChat), message);
                return;
            }
            textBox6.AppendText(message);
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            _youtubeApi = new YouTubeApi(textBox1.Text);
            var liveStreamUrl = new LiveStreamUrl(textBox2.Text);
            var videoId = liveStreamUrl.getVideoId();
            if (videoId is null)
            {
                textBox3.AppendText("Video IDの取得に失敗しました。\r\n");
                return;
            }
            var chatId = await _youtubeApi.getChatIdByVideoId(videoId);
            textBox3.AppendText($"Chat ID => {chatId}\r\n");

            launchChatTaller(chatId);
        }

        private void launchChatTaller(string chatId)
        {
            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += async (sender, e) =>
            {
                if (_youtubeApi is null)
                {
                    return;
                }
                var response = await _youtubeApi.getChat(chatId, _pageToken);
                if (response is null)
                {
                    return;
                }
                _pageToken = response.nextPageToken;
                foreach (var item in response.liveChatMessages)
                {
                    // 通常チャットのみ対象とする
                    if (item.Snippet.Type != "textMessageEvent")
                    {
                        continue;
                    }
                    var message = item.Snippet.TextMessageDetails.MessageText;
                    appendChat($"{message}\r\n");
                    if (_searchString != "" && message.Contains(_searchString))
                    {
                        appendSearchedChat($"{message}\r\n");
                    }
                }
            };
            timer.Start();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            _searchString = textBox5.Text;
        }
    }
}