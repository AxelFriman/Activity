using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Activity
{
    class TaskProperty
    {
        public int ID;
        public string name;
        public int cost;
    }
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
        public static List<TaskProperty> Properties = new List<TaskProperty>
        {
            new TaskProperty { ID = 1, name = "НАРИСОВАТЬ", cost = 4 },
            new TaskProperty { ID = 2, name = "ОБЪЯСНИТЬ", cost = 3 },
            new TaskProperty { ID = 3, name = "ПОКАЗАТЬ", cost = 5 }
        };

    }

    class Player
    {
        public string Name { get; set; }
        public Boolean Ready { get; set; }

        public Player(string name)
        {
            Name = name;
            Ready = true;
        }
    }

    class Team
    {
        public string Name { get; set; }
        public int Score { get; private set; }
        public List<Player> Players { get; set; }

        public Team()
        {
            Players = new List<Player>();
            Score = 0;
        }

        public void resetReady()
        {
            foreach (Player plr in Players)
            {
                plr.Ready = true;
            }
        }

        public void addPoints(int points)
        {
            this.Score += points;
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

        public List<Team> findWinners()
        {
            List<Team> winners = new List<Team>();
            foreach (Team tm in Teams)
            {
                if (tm.Score >= ScoreLimit)
                {
                    winners.Add(tm);
                }
            }
            return winners;
        }
    }

    static class Engine
    {
        public static Game createGame()
        {
            string playersInput;
            Game newGame = new Game();
            do
            {
                Console.Write("Введите количество команд: ");
                playersInput = Console.ReadLine();
            }
            while (!isNumber(playersInput));
            int teamsNumber = Convert.ToInt32(playersInput);
            for (int i = 0; i < teamsNumber; i++)
            {
                Team newTeam = new Team();
                do
                {
                    Console.Write("Введите название команды {0}: ", i+1);
                }
                while (String.IsNullOrEmpty(playersInput = Console.ReadLine())) ;
                newTeam.Name = playersInput;
                do
                {
                    Console.Write("Количество участников в ней: ");
                    playersInput = Console.ReadLine();
                }
                while (!isNumber(playersInput));
                int playersNumber = Convert.ToInt32(playersInput);
                for (int j = 0; j < playersNumber; j++)
                {
                    do
                    {
                        Console.Write("Имя участника {0}/{1}: ", j + 1, playersNumber);
                    }
                    while (String.IsNullOrEmpty(playersInput = Console.ReadLine()));
                    Player newPlayer = new Player(playersInput);
                    newTeam.Players.Add(newPlayer);
                }   
                newGame.Teams.Add(newTeam);
                Console.Clear();
            }
            do
            {
                Console.Write("Очков для победы: ");
                playersInput = Console.ReadLine();
            }
            while (!isNumber(playersInput));
            newGame.ScoreLimit = Convert.ToInt32(playersInput);

            string dictionary = @"dictionary.txt";
            newGame.Tasks = Task.loadDictionary(dictionary);
            return newGame;
        }
        public static void startGame(Game game)
        {
            Boolean gameEnded = false;
            do
            {
                foreach (Team curTeam in game.Teams)
                {
                    if ((from plr in curTeam.Players
                        where curTeam.Players.Any(x => x.Ready)
                        select plr).Count() == 0)
                    {
                        curTeam.resetReady();
                    }
                    Player curPlayer = (from Player plr in curTeam.Players
                                        where plr.Ready
                                        select plr).First<Player>();
                    Console.Clear();
                    Console.WriteLine("Ход игрока {0} из команды {1}", curPlayer.Name, curTeam.Name);
                    Console.WriteLine("Эникей для продолжения.");
                    Console.ReadKey();
                    curPlayer.Ready = false;
                    int unixTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds % 2147483647);
                    var r = new Random(unixTime);
                    List<Task> selectedTasks = new List<Task>();
                    for (int i = 1; i <= 3; i++)
                    {
                        var selected = (from Task tsk in game.Tasks
                                         where tsk.Type == i
                                         select tsk);
                        Task randomTask = selected.ElementAt(r.Next(0, selected.Count()));
                        selectedTasks.Add(randomTask);
                        Console.WriteLine(i + " - " + Task.Properties.Find(x => x.ID == i).name + " " + randomTask.Word);
                    }
                    string playersInput;
                    do
                    {
                        Console.Write("Цифру 1-3: ");
                        playersInput = Console.ReadLine();
                    }
                    while (!isValidChoice(playersInput));
                    int playersChoice = Convert.ToInt32(playersInput);
                    int activeTaskCost = Task.Properties.Find(x => x.ID == playersChoice).cost;
                    try
                    {
                        Console.WriteLine("МИНУТА ПОШЛА!\nEnter - есть верный ответ");
                        string name = Reader.ReadLine(60000);
                        curTeam.addPoints(activeTaskCost);
                        Console.WriteLine("Команде {0} начислено баллов: {1}.\n Эникей для продолжения.", curTeam.Name, activeTaskCost);
                        Console.ReadLine();
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Время вышло.\nЭникей для продолжения.");
                        Console.ReadLine();
                    }
                }                

                List<Team> winners = game.findWinners();
                gameEnded = winners.Count > 0;
                if (gameEnded)
                {
                    Console.WriteLine("Игра окончена. Победители:");
                    foreach (Team winner in winners)
                    {
                        Console.WriteLine(winner.Name);
                    }
                }
                else
                {
                    printResults(game);
                    Console.WriteLine("Эникей для продолжения.");
                    Console.ReadLine();
                }
            }
            while (!gameEnded);
        }
        public static void printResults(Game game)
        {
            Console.WriteLine("Текущие результаты: ");
            foreach (Team tm in game.Teams)
            {
                Console.WriteLine("{0} - {1};",tm.Name, tm.Score);
            }
        }
        public static Boolean isValidChoice(string userInput)
        {
            Boolean isValid = false;
            try
            {
                if (String.IsNullOrEmpty(userInput))
                {
                    throw new NullReferenceException();
                }
                if (Enumerable.Range(1,3).Contains(Convert.ToInt32(userInput)))
                {
                    isValid = true;
                }
            }
            catch(FormatException)
            {
                Console.WriteLine("Неверный ввод.");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Пустой ввод.");
            }
            return isValid;
        }
        public static Boolean isNumber(string userInput)
        {
            Boolean isValid = false;
            try
            {

                if (String.IsNullOrEmpty(userInput))
                {
                    throw new NullReferenceException();
                }
                Convert.ToInt32(userInput);
                isValid = true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный ввод.");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Пустой ввод.");
            }
            return isValid;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Game g = Engine.createGame();
            Engine.startGame(g);
            Console.ReadLine();
        }
    }

    class Reader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        static Reader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        // omit the parameter to read a line without a timeout
        public static string ReadLine(int timeOutMillisecs = Timeout.Infinite)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            else
                throw new TimeoutException("User did not provide input within the timelimit.");
        }
    }
}
