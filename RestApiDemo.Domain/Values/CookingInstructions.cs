using System;

namespace RestApiDemo.Domain.Values
{
    public class CookingInstructions
    {
        // Could be fancy and have markdown support or html tag support. Or support text as separate paragraphs.
        public string Text { get; }

        public CookingInstructions(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}
