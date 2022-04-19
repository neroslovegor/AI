using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace AIImageAnalysis
{
    public class ImageAnalysis
    {
        private readonly string[] Classes = new string[] { "Animals", "Car", "Cat", "City", "Dog", "Food & Drink", "House", "Interiors", "Nature", "People", "Sport" };
        private ImageClassifier.ModelOutput predictionResult;
        public string filePath { get; } 
        public string fileName { get; }

        public ImageAnalysis(string filePath, string fileName)
        {
            this.filePath = filePath;
            this.fileName = fileName;
        }

        public void ImageClassification()
        {
            var imageBytes = File.ReadAllBytes(filePath);

            ImageClassifier.ModelInput sampleData = new ImageClassifier.ModelInput()
            {
                ImageSource = imageBytes,
            };

            predictionResult = ImageClassifier.Predict(sampleData);
        }

        public Image GetImage()
        {
            return Image.FromFile(filePath);
        }
        public string GetPredictedLabel()
        {
            return predictionResult.PredictedLabel;
        }
        public float[] GetScore()
        {
            return predictionResult.Score;
        }
        public float GetLabel()
        {
            return predictionResult.Score.Max();
        }
        public ValueTuple<string[], float[]> GetResultsClassification()
        {
            ValueTuple<string[], float[]> resultsClassification = new ValueTuple<string[], float[]>();
            resultsClassification.Item1 = Classes;
            resultsClassification.Item2 = predictionResult.Score;
            return resultsClassification;
        }
    }
}
