using ICSharpCode.SharpZipLib.GZip;
using Markov;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace AutoReviewGenerator
{
    public class HostedCacheService:IHostedService
    {
        private readonly IMemoryCache _memoryCache;
        public HostedCacheService(IMemoryCache cache)
        {
            _memoryCache = cache;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var counter = 0;
            var reviewsDataSet = new Dictionary<int, List<Review>>();


            using (var fs = new FileStream("./SampleData/Books_5.gz", FileMode.Open, FileAccess.Read))
            using (var gzipStream = new GZipInputStream(fs))
            using (var streamReader = new StreamReader(gzipStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    if (counter > 100000) break;
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var review = serializer.Deserialize<Review>(reader);

                        if (review != null && review.Overall > 0 && review.Reviewtext != null)
                        {

                            var rating = (int)Math.Floor(review.Overall);

                            if (!reviewsDataSet.ContainsKey(rating))
                            {
                                reviewsDataSet.Add(rating, new List<Review>());
                            }
                            reviewsDataSet[rating].Add(review);
                            counter++;
                        }

                    }
                }
            }

            foreach (var rating in reviewsDataSet.Keys)
            {
                _memoryCache.Set($"Rating-{rating}", CreateMarkovChain(reviewsDataSet[rating]));
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            for(var i = 1; i<=5; i++)
            {
                _memoryCache.Remove($"Rating-{i}");
            }
        }


        private static MarkovChain<string> CreateMarkovChain(List<Review> dataset)
        {
            var ReviewTextChain = new MarkovChain<string>(3);

            foreach (var review in dataset)
            {
                ReviewTextChain.Add(review.Reviewtext.Split(new char[] { ' ' }));
            }

            return ReviewTextChain;

        }
    }
}
