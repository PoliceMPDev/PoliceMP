using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Extensions;

namespace PoliceMP.Shared.Models
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

        public string GetRandomPositiveAnswer()
        {
            return PositiveAnswers.GetRandom();
        }

        public string GetRandomNegativeAnswer()
        {
            return NegativeAnswers.GetRandom();
        }
    }
}