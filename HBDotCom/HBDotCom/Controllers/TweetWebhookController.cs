using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace HBDotCom.Controllers
{
    [Route("webhooks/twitter")]
    public class TweetWebhookController : Controller
    {
        private readonly ITwitterCredentials _credentials;

        private TweetWebhookController()
        {
            _credentials = Auth.ApplicationCredentials;
        }

        [HttpPost]
        public ActionResult Index(string tweet, IFormFile file)
        {
            var fileBytes = GetByteArrayFromFile(file);

            var publishedTweet = Auth.ExecuteOperationWithCredentials(_credentials, () =>
            {
                var publishOptions = new PublishTweetOptionalParameters();
                if (fileBytes != null)
                {
                    publishOptions.MediaBinaries.Add(fileBytes);
                }

                return Tweet.PublishTweet(tweet, publishOptions);
            });

            var routeValueParameters = new Dictionary<string, object>
            {
                { "id", publishedTweet == null ? (Nullable<long>)null : publishedTweet.Id },
                { "actionPerformed", "Publish" },
                { "success", publishedTweet != null }
            };
            return RedirectToAction("TweetPublished", routeValueParameters);
        }

        public ActionResult TweetPublished(Nullable<long> id, string actionPerformed, bool success = true)
        {
            ViewBag.TweetId = id;
            ViewBag.ActionType = actionPerformed;
            ViewBag.Success = success;
            return View();
        }

        public ActionResult DeleteTweet(long id)
        {
            var success = Auth.ExecuteOperationWithCredentials(_credentials, () =>
            {
                var tweet = Tweet.GetTweet(id);
                if (tweet != null)
                {
                    return tweet.Destroy();
                }

                return false;
            });

            var routeValueParameters = new Dictionary<string, object>
            {
                { "id", id },
                { "actionPerformed", "Delete" },
                { "success", success }
            };
            return RedirectToAction("TweetPublished", routeValueParameters);
        }

        private byte[] GetByteArrayFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            file.OpenReadStream().CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
