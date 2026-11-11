

namespace DevSecOps.Services;

public static class PadelRecommendation
{
    // Simple illustrative rules for the exercise, not professional sports advice.
    public static string Create(double temperature, double precipitation, double windSpeed)
    {
        if (precipitation >= 1 || windSpeed >= 35)
        {
            return "NotRecommended";
        }

        if (temperature < 5 || temperature > 35 || windSpeed >= 25)
        {
            return "UseCaution";
        }

        return "Favorable";
    }
}
