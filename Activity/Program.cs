using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Activity
{
    class Task
    {
        public string Word { get; }
        public int Type { get; }
        public Task(string word, int type)
        {
            this.Word = word;
            this.Type = type;
        }
        public static List<Task> loadDictionary(string path)
        {
            StreamReader reader = new StreamReader(path);
            List<Task> words = new List<Task>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                words.Add(new Task(line.Substring(0, line.Length - 2), Convert.ToInt32(line.Substring(line.Length - 1, 1))));
            }
            return words;
        }
    }

    public class Player
    {
        public string Name { get; set; }

        public Player(string name)
        {
            Name = name;
        }
    }

    class Team
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public List<Player> Players { get; set; }

        public Team()
        {
            Players = new List<Player>();
        }
    }

    class Game
    {
        public List<Team> Teams { get; set; }
        public List<Task> Tasks { get; set; }
        public int CurRound { get; set; }
        public int ScoreLimit { get; set; }

        public Game()
        {
            Teams = new List<Team>();
            Tasks = new List<Task>();
        }
    }

    static class Engine
    {
        public static Game createGame()
        {
            Game newGame = new Game();
            Console.Write("Введите количество команд: ");
            int teamsNumber = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < teamsNumber; i++)
            {
                Team newTeam = new Team();
                Console.Write("Введите название команды {0}: ", i+1);
                newTeam.Name = Console.ReadLine();
                Console.Write("Количество участников в ней: ");
                int playersNumber = Convert.ToInt32(Console.ReadLine());
                for (int j = 0; j < playersNumber; j++)
                {
                    Console.Write("Имя участника {0}/{1}: ", j+1, playersNumber);
                    Player newPlayer = new Player(Console.ReadLine());
                    newTeam.Players.Add(newPlayer);
                }   
                newGame.Teams.Add(newTeam);
            }
            Console.Write("Очков для победы: ");
            newGame.ScoreLimit = Convert.ToInt32(Console.ReadLine());

            string dictionary = @"dictionary.txt";
            newGame.Tasks = Task.loadDictionary(dictionary);
            return newGame;
        }
        public static void startGame()
        {
             
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Game g = Engine.createGame();
            Engine.startGame();
            Console.ReadLine();
        }
    }
}
