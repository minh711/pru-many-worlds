using System.Collections.Generic;
public static class RandomHelper
{
    public static List<int> GetUniqueRandomNumbers(int n, int min, int max)
    {
        if (n > (max - min + 1))
        {
            throw new System.ArgumentException("N is larger than the range of unique numbers available.");
        }

        List<int> numbers = new List<int>();
        for (int i = min; i <= max; i++)
        {
            numbers.Add(i);
        }

        List<int> result = new List<int>();
        System.Random random = new System.Random();
        for (int i = 0; i < n; i++)
        {
            int index = random.Next(numbers.Count);
            result.Add(numbers[index]);
            numbers.RemoveAt(index);
        }

        return result;
    }
}
