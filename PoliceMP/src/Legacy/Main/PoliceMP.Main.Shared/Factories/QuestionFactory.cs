using PoliceMP.Main.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Main.Shared.Factories
{
    public static class QuestionFactory
    {
        /// <summary>
        ///     Convert a dynamic object to an Question object.
        /// </summary>
        /// <param name="dyn">The dynamic object.</param>
        /// <returns>The Question object.</returns>
        public static Question FromDynamic(dynamic dyn)
        {
            var positiveAnswers = dyn.PositiveAnswers as List<dynamic>;
            var negativeAnswers = dyn.NegativeAnswers as List<dynamic>;

            return new Question
            {
                Text = dyn.Text,
                Attribute = dyn.Attribute,
                PositiveAnswers = positiveAnswers.Cast<string>().ToList(),
                NegativeAnswers = negativeAnswers.Cast<string>().ToList()
            };
        }
    }
}
