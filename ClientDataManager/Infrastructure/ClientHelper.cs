namespace ClientDataManager.Infrastructure
{
    public static class ClientHelper
    {
        public static string GetStreet(string? text)
        {
            var candidate = text.Trim();
            if (!candidate.Any(Char.IsWhiteSpace))
                return text;

            return candidate.Split(' ').FirstOrDefault();
        }

        public static string GetNumber(string? input)
        {
            int startIndex = input.IndexOf('.') + 1;
            int endIndex = input.IndexOf(',');
            if (startIndex >= 0 && endIndex > startIndex && endIndex <= input.Length)
            {
                return input.Substring(startIndex, endIndex - startIndex).Trim();
            }
            else { return input; }
        }
        public static string GetCity(string? input)
        {
            int startIndex = IndexOfNth(input, ",", 0) + 1;
            int endIndex = IndexOfNth(input,",", 1);
            if (startIndex >= 0 && endIndex > startIndex && endIndex <= input.Length)
            {
                    return input.Substring(startIndex, endIndex - startIndex).Trim();
            }
            else { return input.Substring(startIndex).Trim(); }
        }

        public static int IndexOfNth(this string str, string value, int nth = 0)
        {
            if (nth < 0)
                throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");

            int offset = str.IndexOf(value);
            for (int i = 0; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1);
            }

            return offset;
        }
    }
}