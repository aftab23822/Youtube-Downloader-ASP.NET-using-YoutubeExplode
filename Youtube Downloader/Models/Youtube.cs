using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;

namespace Youtube_Downloader.Models
{
    public class Youtube
    {

        public string url { get; set;}
        public string title { get; set; }
        public string author { get; set; }
        public TimeSpan? duration { get; set; }
        public StreamManifest availableStreams { get; set; }
        public IStreamInfo audioStreams { get; set; }
        public IVideoStreamInfo videoStreams { get; set; }

    }
}
