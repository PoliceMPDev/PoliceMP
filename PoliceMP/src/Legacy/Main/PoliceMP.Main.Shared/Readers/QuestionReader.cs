using Newtonsoft.Json;
using PoliceMP.Main.Shared.Models;
using System.Collections.Generic;
using System.IO;

namespace PoliceMP.Main.Shared.Readers
{
    public class QuestionReader
    {
        /// <summary>
        ///     The location of the JSON file.
        /// </summary>
        private const string FILE_LOCATION = "Questions.json";

        /// <summary>
        ///     The list of questions.
        /// </summary>
        private static List<Question> _questions;

        /// <summary>
        ///     Get all the questions.
        /// </summary>
        /// <returns>A list of all the questions.</returns>
        public static List<Question> All()
        {
            if (_questions != null && _questions.Count != 0) return _questions;

            using (var file = File.OpenText(FILE_LOCATION))
            {
                var serializer = new JsonSerializer();
                _questions = (List<Question>)serializer.Deserialize(file, typeof(List<Question>));
            }

            return _questions;
        }
    }
}
