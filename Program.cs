using System;
using System.Collections.Generic;
using System.Linq;
namespace Mastermind
{
    // Class with Encapsulation
    public class GameSettings
    {
        public string? SecretCode { get; set; }
        public int MaxAttempts { get; set; } = 10; // Default 10 attempts
        public GameSettings(string? secretCode = null, int maxAttempts = 10)
        {
            SecretCode = secretCode;
            MaxAttempts = maxAttempts;
        }
    }
    // Separate class for game logic
    public class CodeChecker
    {
        public (int wellPlaced, int misplaced) CheckCode(string secret, string guess)
        {
            int wellPlaced = 0;
            int misplaced = 0;
            for (int i = 0; i < secret.Length; i++)
            {
                if (secret[i] == guess[i])
                {
                    wellPlaced++;
                }
            }
            var secretCounts = new Dictionary<char, int>();
            var guessCounts = new Dictionary<char, int>();
            for (int i = 0; i < secret.Length; i++)
            {
                if (secret[i] != guess[i])
                {
                    secretCounts[secret[i]] = secretCounts.GetValueOrDefault(secret[i], 0) + 1;
                    guessCounts[guess[i]] = guessCounts.GetValueOrDefault(guess[i], 0) + 1;
                }
            }
            foreach (var kvp in guessCounts)
            {
                char c = kvp.Key;
                int guessCount = kvp.Value;
                int secretCount = secretCounts.GetValueOrDefault(c, 0);
                misplaced += Math.Min(guessCount, secretCount);
            }
            return (wellPlaced, misplaced);
        }
    }
    // Main game class with Composition
    public class MastermindGame
    {
        private readonly GameSettings settings;
        private readonly CodeChecker checker;
        private readonly Random random;
        private string secretCode;
        private int currentRound;
        public MastermindGame(GameSettings settings)
        {
            this.settings = settings;
            this.checker = new CodeChecker();
            this.random = new Random();
            this.currentRound = 0;
            if (string.IsNullOrEmpty(settings.SecretCode))
            {
                this.secretCode = GenerateRandomCode();
            }
            else
            {
                this.secretCode = settings.SecretCode;
            }
        }
        // Generate secret code
        private string GenerateRandomCode()
        {
            var availableDigits = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8' }; // 9 pieces
            var code = new char[4]; // 4 distinct pieces
            for (int i = 0; i < 4; i++)
            {
                int index = random.Next(availableDigits.Count);
                code[i] = availableDigits[index];
                availableDigits.RemoveAt(index);
            }
            return new string(code);
        }
        public void Start()
        {
            Console.WriteLine("Can you break the code? Enter a valid guess."); // start message
            while (currentRound < settings.MaxAttempts)
            {
                Console.WriteLine("---");
                Console.WriteLine($"Round {currentRound}");
                Console.Write(">");
                string? input = Console.ReadLine(); // Standard input
                if (input == null) // (Ctrl+D)
                {
                    break;
                }
                if (!IsValidGuess(input))
                {
                    Console.WriteLine("Wrong input!"); // invalid message
                    continue;
                }
                if (input == secretCode)
                {
                    Console.WriteLine("Congratz! You did it!"); // win message
                    return;
                }
                var (wellPlaced, misplaced) = checker.CheckCode(secretCode, input);
                Console.WriteLine($"Well placed pieces: {wellPlaced}"); // format
                Console.WriteLine($"Misplaced pieces: {misplaced}"); // format
                currentRound++;
            }
            Console.WriteLine($"Game over! The secret code was: {secretCode}");
        }
        // Validate 4 distinct digits from 0-8
        private bool IsValidGuess(string? guess)
        {
            if (string.IsNullOrEmpty(guess) || guess.Length != 4)
                return false;
            foreach (char c in guess)
            {
                if (c < '0' || c > '8')
                    return false;
            }
            var uniqueDigits = new HashSet<char>(guess);
            return uniqueDigits.Count == 4;
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = ParseArguments(args);
            var game = new MastermindGame(settings);
            game.Start();
        }
        // parameters (-c and -t)
        private static GameSettings ParseArguments(string[] args)
        {
            string? secretCode = null;
            int maxAttempts = 10;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-c": // [CODE] parameter
                        if (i + 1 < args.Length)
                        {
                            secretCode = args[i + 1];
                            i++;
                        }
                        break;
                    case "-t": // [ATTEMPTS] parameter
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int attempts))
                        {
                            maxAttempts = attempts;
                            i++;
                        }
                        break;
                }
            }
            return new GameSettings(secretCode, maxAttempts);
        }
    }
}