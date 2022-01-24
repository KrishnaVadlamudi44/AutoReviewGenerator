namespace AutoReviewGenerator
{
    public class Review
    {
        public double Overall { get; set; }
        public string Reviewtext { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Rating: {0}\n" +
                "Review: {1}",
                Overall, Reviewtext
            );
        }
    }
}
