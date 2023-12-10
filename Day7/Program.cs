using System.Diagnostics;
using System.Runtime.Serialization.Formatters;

internal class Program {
    class Scores {
        public int score { get; set; }
        public string hand { get; set; }
    }

    static void Main(string[] args) {
        string path;
        string[] data;

        //path = @"..\..\..\InputTest.txt";
        path = @"..\..\..\InputFull.txt";
        data = File.ReadAllLines(path);

        List<string> cardList = new List<string>();
        Dictionary<string, int> bids = new Dictionary<string, int>();

        for (int i = 0; i < data.Length; i++) {
            string hand = data[i].Split(' ')[0];
            cardList.Add(hand);

            // Save bids in separate table
            bids[hand] = int.Parse(data[i].Split(' ')[1]);
        }

        List<Scores> tab = sortCards(cardList);
        long score = 0;
        for (int i = 0; i < tab.Count; i++) score += (i + 1) * bids[tab[i].hand];

        Console.WriteLine("RESULT: " + score);
        // RESULT 1: 253603890
        // RESULT 2: 253630098
    }

    static List<Scores> sortCards(List<string> cardList) {
        // Replace chars for sort
        for (int i = 0; i < cardList.Count; i++) {
            cardList[i] = cardList[i].Replace('T', 'a');
            //cardList[i] = cardList[i].Replace('J', 'b');  // Part 1
            cardList[i] = cardList[i].Replace('J', '1');    // Part 2 -> J is the weakest card
            cardList[i] = cardList[i].Replace('Q', 'c');
            cardList[i] = cardList[i].Replace('K', 'd');
            cardList[i] = cardList[i].Replace('A', 'e');
        }

        // Sort alphabetically
        cardList.Sort();

        // Restore original chars
        for (int i = 0; i < cardList.Count; i++) {
            cardList[i] = cardList[i].Replace('a', 'T');
            //cardList[i] = cardList[i].Replace('b', 'J');  // Part 1
            cardList[i] = cardList[i].Replace('1', 'J');    // Part 2
            cardList[i] = cardList[i].Replace('c', 'Q');
            cardList[i] = cardList[i].Replace('d', 'K');
            cardList[i] = cardList[i].Replace('e', 'A');
        }

        // Define list <score, hand>
        List<Scores> list = new List<Scores>();
        for (int i = 0; i < cardList.Count; i++) {
            list.Add(new Scores { score = 0, hand = cardList[i] });
        }

        // Detect hand type and update score
        for (int i = 0; i < list.Count; i++) {
            //list[i].score = getHandScore(list[i].hand);   // Part 1
            list[i].score = getBestScore(list[i].hand);     // Part 2
        }

        // Sort by score, preserving the previous order
        list = list.OrderBy(item => item.score).ToList();

        return list;
    }

    static int getHandScore(string hand) {
        // Count cards in hand
        List<char> c = new List<char>();
        List<int> s = new List<int>();
        for (int i = 0; i < hand.Length; i++) {
            if (!c.Contains(hand[i])) {
                c.Add(hand[i]);
                s.Add(hand.Count(c => c == hand[i])); // Count the char in string
            }
        }

        // Determine hand type
        int score = -1;
        s.Sort((a, b) => b.CompareTo(a));   // Sort descending

        if (s.Count == 5) score = 1;                // 1 - Hign card
        if (s.Count == 4) score = 2;                // 2 - One pair
        if (s.Count == 3 && s[0] == 2) score = 3;   // 3 - Two pair
        if (s.Count == 3 && s[0] == 3) score = 4;   // 4 - Three of a kind
        if (s.Count == 2 && s[0] == 3) score = 5;   // 5 - Full house
        if (s.Count == 2 && s[0] == 4) score = 6;   // 6 - Four of a kind
        if (s.Count == 1) score = 7;                // 7 - Five of a kind

        return score;
    }

    static int getBestScore(string hand) {
        if (!hand.Contains('J')) return getHandScore(hand);

        char[] cards = { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
        int bestScore = -1;

        // Find best Joker - assuming that all Jokers in hand become the same card
        for (int i = 0; i < cards.Length; i++) {
            int score = getHandScore(hand.Replace('J', cards[i]));
            if (score > bestScore) bestScore = score;
        }

        return bestScore;
    }
}