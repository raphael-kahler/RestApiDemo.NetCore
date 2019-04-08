using System;

namespace RestApiDemo.Domain.Values
{
    public class ServingSize
    {
        public int FeedsNumPeople { get; }

        public ServingSize(int feedsNumPeople)
        {
            if (feedsNumPeople < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(feedsNumPeople), feedsNumPeople, "Serving size has to feed more than 0 people.");
            }

            FeedsNumPeople = feedsNumPeople;
        }

        /// <summary>
        /// Get the ratio of this serving size vs the other sice in terms of how many people can be fed.
        /// </summary>
        /// <param name="other">The other serving size.</param>
        /// <returns>The ratio of this serving size vs the other serving size.</returns>
        public double SizeRatioVersus(ServingSize other)
        {
            // notice no check for division by 0 needed, because constructor guarantees each serving size feeds at least one person.
            return 1.0 * FeedsNumPeople / other.FeedsNumPeople;
        }
    }
}
