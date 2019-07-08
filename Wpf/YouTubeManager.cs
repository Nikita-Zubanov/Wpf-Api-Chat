using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YoutubeSearch;

namespace Wpf
{
    class YouTubeManager
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }

        public const string apiKey = "AIzaSyBo2-CpX0CLfuvIr0DYDBq1OISZebsxSfI";

        public async Task SetIdAndUrlVideo(string videoName, string channelName)
        {
            VideoSearch items = new VideoSearch();
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = GetType().ToString()
            });

            SearchResource.ListRequest searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = channelName + " " + videoName;
            searchListRequest.MaxResults = 30;
            SearchListResponse searchListResponse = await searchListRequest.ExecuteAsync();
            SearchResult searchResponse = searchListResponse.Items.FirstOrDefault(c => c.Snippet.ChannelTitle.ToLower().Replace(" ", "") == channelName);

            Id = searchResponse.Id.VideoId;
            Url = "https://www.youtube.com/watch?v=" + Id;
        }

        public void SetVideoInfo()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = this.GetType().ToString()
            });

            var videoRequest = youtubeService.Videos.List("snippet, contentDetails, statistics");
            videoRequest.Id = Id;
            var response = videoRequest.Execute();
            
            Title = response.Items[0].Snippet.Title;
            Description = response.Items[0].Snippet.Description;
            PublishDate = response.Items[0].Snippet.PublishedAt.Value;
            ViewCount = (int)response.Items[0].Statistics.ViewCount;
            LikeCount = (int)response.Items[0].Statistics.LikeCount;
            DislikeCount = (int)response.Items[0].Statistics.DislikeCount;
        }
    }
}
