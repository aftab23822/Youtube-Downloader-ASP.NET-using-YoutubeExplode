using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration;
using Youtube_Downloader.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Youtube_Downloader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private  Youtube yt=new Youtube();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

       

        public IActionResult Index()
        {
            return View();
        }


        private async Task<Models.Youtube> FetchVideoData(string url)
        {
            var yt = new Models.Youtube();
            var youtube = new YoutubeClient();

            // You can specify both video ID or URL
            var video = await youtube.Videos.GetAsync(url);
            yt.url = url;
            yt.title = video.Title; // "Collections - Blender 2.80 Fundamentals"
            yt.author = video.Author.Title; // "Blender"
            yt.duration = video.Duration; // 00:07:20

            yt.availableStreams = await youtube.Videos.Streams.GetManifestAsync(url);


            // Get highest quality muxed stream
            yt.videoStreams = yt.availableStreams.GetMuxedStreams().GetWithHighestVideoQuality();

            // ...or highest bitrate audio-only stream
            yt.audioStreams = yt.availableStreams.GetAudioOnlyStreams().GetWithHighestBitrate();

            // ...or highest quality MP4 video-only stream
            //var streamInfo = streamManifest
            //    .GetVideoOnlyStreams()
            //    .Where(s => s.Container == Container.Mp4)
            //    .GetWithHighestVideoQuality();


            // Get the actual stream
            //var stream = await youtube.Videos.Streams.GetAsync(yt.videoStreams);
            return yt;

        }
        [HttpPost]
        public IActionResult Index(Models.Youtube yt)
        {
            ViewBag.videoFound = "true";

            yt =  FetchVideoData(yt.url).Result;
            this.yt=yt;
            return View(this.yt);

        }

        [HttpPost]
        public async Task<IActionResult> DownloadVideo(Models.Youtube yt)
        {
            yt = FetchVideoData(yt.url).Result;
            // Download the stream to a file
            string fileName = $"{yt.title}.{yt.videoStreams.Container}";
            var youtube = new YoutubeClient();
            // Check the file name is valid and doesn't include bad characters
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                // Create a list of invalid characters
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                // If it does, then remove them character by character
                foreach (char c in invalid) { fileName = fileName.Replace(c.ToString(), ""); }
            }
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            await youtube.Videos.Streams.DownloadAsync(yt.videoStreams, filePath);
            ViewBag.DownloadSuccessfull = "true";
            return View("Index",yt);


        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
