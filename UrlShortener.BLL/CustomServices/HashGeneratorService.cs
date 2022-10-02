namespace UrlShortener.BLL.CustomServices;

public class HashGeneratorService
{
    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_+";
    public const int HashLength = 7;
    public const ulong PrimeNumber = 518973461827;

    public static ulong CombinationsAmount
    {
        get
        {
            if (_combinationAmount == 0)
            {
                int alphabetLenght = Alphabet.Length;
                _combinationAmount = (ulong)alphabetLenght;
                for (int i = 0; i < (HashLength - 1); i++) _combinationAmount *= (ulong)alphabetLenght;
            }

            return _combinationAmount;
        }
    }

    public HashGeneratorService(RangeProviderService rangeProvider)
    {
        _rangeProvider = rangeProvider;
    }

    /// <summary>
    /// Створити новий хеш (коротке посилання).
    /// </summary>
    /// <returns>Новий хеш.</returns>
    public async Task<string> NextAsync()
    {
        // Ідея як розробити URL shortener була взята тут:
        // https://www.enjoyalgorithms.com/blog/design-a-url-shortening-service-like-tiny-url

        // Для багатьох одночасних запитів потрібен одночасний доступ до лічільника
        // Тому розроблено сервіс RangeProviderService який буде видавати вільні проміжки лічільника
        using var rangeBorrow = await _rangeProvider.GetFreeRange();

        var nextCounterValue = ++rangeBorrow.Range.LastUsedValue;

        // Перетворення лічільника на унікальну строку
        // Помноження двох ulong може призвести до OverflowException
        var mod = ModularMultiplication(nextCounterValue, PrimeNumber, CombinationsAmount);

        // Конвертація числа до символів з Alphabet
        var hash = string.Empty;
        while (mod > 0)
        {
            hash += Alphabet[(int)(mod % (ulong)Alphabet.Length)];
            mod /= (ulong)Alphabet.Length;
        }

        // Доповнити до потрібної довжини
        while (hash.Length < HashLength)
        {
            hash += '0';
        }

        return hash;

        static ulong ModularMultiplication(ulong a, ulong b, ulong mod)
        {
            // Взято тут: https://www.geeksforgeeks.org/how-to-avoid-overflow-in-modular-multiplication/

            ulong res = 0; // Initialize result

            a %= mod;

            while (b > 0)
            {
                // If b is odd, add 'a' to result
                if (b % 2 == 1)
                {
                    res = (res + a) % mod;
                }

                // Multiply 'a' with 2
                a = a * 2 % mod;

                // Divide b by 2
                b /= 2;
            }

            // Return result
            return res % mod;
        }
    }

    public string Next()
    {
        return NextAsync().Result;
    }

    private static ulong _combinationAmount;
    private readonly RangeProviderService _rangeProvider;
}