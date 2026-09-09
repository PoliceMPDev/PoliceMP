using System.Collections.Generic;

namespace PoliceMP.Main.Shared.Models
{
    /// <summary>
    /// Represents a question.
    /// </summary>
    public class Question
    {
        /// <summary>
        /// The actual question.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The attribute that effects the answer. For example, drunk or search.
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// The list of all positive answers.
        /// </summary>
        public List<string> PositiveAnswers { get; set; }

        /// <summary>
        /// The list of all negative answers.
        /// </summary>
        public List<string> NegativeAnswers { get; set; }
    }
}
