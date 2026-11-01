using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace QLNS.Core.Common
{
    public static class FaceRecognitionHelper
    {
        // Ngưỡng khoảng cách Euclidean tối đa cho phép xác thực cùng 1 khuôn mặt (chuẩn 128-dim ResNet)
        public const double MaxEuclideanDistanceThreshold = 0.48;

        public static bool IsValidDescriptor(List<double>? descriptor)
        {
            if (descriptor == null || descriptor.Count < 64)
                return false;

            // Kiểm tra vector không phải toàn số 0 hoặc NaN
            bool hasNonZero = false;
            foreach (var val in descriptor)
            {
                if (double.IsNaN(val) || double.IsInfinity(val))
                    return false;
                if (Math.Abs(val) > 1e-6)
                    hasNonZero = true;
            }

            return hasNonZero;
        }

        public static string SerializeDescriptor(List<double> descriptor)
        {
            return JsonSerializer.Serialize(descriptor);
        }

        public static List<double>? DeserializeDescriptor(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<List<double>>(json);
            }
            catch
            {
                return null;
            }
        }

        public static (bool IsMatch, double Distance, decimal SimilarityPercent) VerifyFaceMatch(
            List<double> storedDescriptor,
            List<double> capturedDescriptor)
        {
            if (storedDescriptor.Count != capturedDescriptor.Count || storedDescriptor.Count == 0)
                return (false, 999.0, 0m);

            double sumSquares = 0.0;
            for (int i = 0; i < storedDescriptor.Count; i++)
            {
                double diff = storedDescriptor[i] - capturedDescriptor[i];
                sumSquares += diff * diff;
            }

            double distance = Math.Sqrt(sumSquares);
            bool isMatch = distance <= MaxEuclideanDistanceThreshold;

            // Quy đổi khoảng cách sang thang điểm phần trăm độ khớp trực quan (0% - 100%)
            double rawSimilarity = Math.Max(0.0, (1.0 - (distance / 1.0)) * 100.0);
            decimal similarityPercent = Math.Round((decimal)Math.Min(99.9, rawSimilarity), 2);

            return (isMatch, Math.Round(distance, 4), similarityPercent);
        }

        public static async Task<string?> SaveBase64ImageAsync(string? base64Data, string subFolder, string filePrefix)
        {
            if (string.IsNullOrWhiteSpace(base64Data))
                return null;

            try
            {
                var cleanBase64 = base64Data;
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex >= 0)
                {
                    cleanBase64 = base64Data.Substring(commaIndex + 1);
                }

                byte[] imageBytes = Convert.FromBase64String(cleanBase64);
                if (imageBytes.Length == 0)
                    return null;

                var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subFolder);
                if (!Directory.Exists(rootFolder))
                {
                    Directory.CreateDirectory(rootFolder);
                }

                var fileName = $"{filePrefix}_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
                var fullPath = Path.Combine(rootFolder, fileName);
                await File.WriteAllBytesAsync(fullPath, imageBytes);

                return $"/uploads/{subFolder}/{fileName}";
            }
            catch
            {
                return null;
            }
        }
    }
}
