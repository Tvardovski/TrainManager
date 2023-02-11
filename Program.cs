using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleKey ExitKey = ConsoleKey.E;

            WagonDatabase database = new WagonDatabase();
            TrainManager trainManager = new TrainManager(database);
            DispatchedTrainsStorage dispatchedTrains = new DispatchedTrainsStorage();

            bool isWorking = true;

            while (isWorking)
            {
                dispatchedTrains.ShowInfo();

                trainManager.CreateRoute();

                Console.Clear();

                TicketGenerator ticketGenerator = new TicketGenerator(database.GetTypes());
                ticketGenerator.ShowInfo();

                Train train = trainManager.CreateTrain(ticketGenerator);
                dispatchedTrains.AddTrain(train);

                Console.WriteLine($"\nНажмите {ExitKey} чтобы выйти.\n" +
                    $"Нажмите любую другую клавишу чтобы отправить состав и перейти к следующему поезду.");

                if (Console.ReadKey().Key == ExitKey)
                    isWorking = false;

                Console.Clear();
            }
        }
    }

    class TrainManager
    {
        private WagonDatabase _database;

        private Route _route = null;

        public TrainManager(WagonDatabase database)
        {
            _database = database;
        }

        public Train CreateTrain(TicketGenerator ticketsGenerator)
        {
            Console.WriteLine("На основе проданных билетов был сформирован состав");

            return new Train(_route, CreateWagons(ticketsGenerator));
        }

        public void CreateRoute()
        {
            bool isCorrect = false;

            while (isCorrect == false)
            {
                Console.WriteLine("Введите название пункта отправления");
                string departure = Console.ReadLine();

                Console.WriteLine("Введите название пунта прибытия");
                string arrival = Console.ReadLine();

                if (departure.ToUpper() == arrival.ToUpper())
                {
                    Console.WriteLine("Пункт назначения и пункт прибытия не могут быть одинаковыми! " +
                        "Попробуйте ещё раз!");
                }
                else
                {
                    isCorrect = true;

                    _route = new Route(departure, arrival);
                }
            }
        }

        private List<Wagon> CreateWagons(TicketGenerator ticketsGenerator)
        {
            List<Wagon> wagons = new List<Wagon>();

            foreach (string type in ticketsGenerator.GetTypes())
            {
                int amount = 0;

                while (amount < ticketsGenerator.GetNumber(type))
                {
                    Wagon wagon = new Wagon(type, _database.GetCapacity(type));
                    wagons.Add(wagon);

                    amount += wagon.Capacity;
                }
            }

            return wagons;
        }
    }

    class TicketGenerator
    {
        private static Random _random = new Random();

        private int _lowerLimit = 30;
        private int _upperLimit = 100;
        private Dictionary<string, int> _ticketsNumbers;

        public TicketGenerator(string[] types)
        {
            _ticketsNumbers = Create(types);
        }

        public int GetNumber(string type) => _ticketsNumbers[type];

        public string[] GetTypes() => _ticketsNumbers.Keys.ToArray();

        public void ShowInfo()
        {
            Console.WriteLine("Билеты, проданные на данный маршрут:");

            foreach (string type in _ticketsNumbers.Keys)
            {
                Console.WriteLine($"{type} - {_ticketsNumbers[type]}");
            }
        }

        private Dictionary<string, int> Create(string[] types)
        {
            Dictionary<string, int> ticketsNumbers = new Dictionary<string, int>();

            foreach (string type in types)
            {
                ticketsNumbers.Add(type, _random.Next(_lowerLimit, _upperLimit));
            }

            return ticketsNumbers;
        }
    }

    class Route
    {
        private string _departure;
        private string _arrival;

        public Route(string departure, string arrival)
        {
            _departure = departure;
            _arrival = arrival;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Маршрут: {_departure} - {_arrival}");
        }
    }

    class Wagon
    {
        private readonly string _type;

        public Wagon(string type, int capacity)
        {
            _type = type;
            Capacity = capacity;
        }

        public int Capacity { get; }

        public void ShowInfo()
        {
            Console.Write($"{_type} ");
        }
    }

    class Train
    {
        private readonly Route _route;
        private readonly List<Wagon> _wagons;

        public Train(Route route, List<Wagon> Cars)
        {
            _route = route;
            _wagons = Cars;
        }

        public void ShowInfo()
        {
            int number = 1;

            _route.ShowInfo();

            Console.Write("Вагоны в поезде: ");

            foreach (Wagon wagon in _wagons)
            {
                Console.Write($"{number}.");
                wagon.ShowInfo();

                number++;
            }

            Console.WriteLine();
        }
    }

    class WagonDatabase
    {
        private readonly Dictionary<string, int> _capacity = new Dictionary<string, int>();

        public WagonDatabase()
        {
            _capacity = Fill();
        }

        public string[] GetTypes() => _capacity.Keys.ToArray();

        public int GetCapacity(string type) => _capacity[type];

        private Dictionary<string, int> Fill()
        {
            return new Dictionary<string, int>()
            {
                ["Плацкарт"] = 54,
                ["Купе"] = 32,
                ["СВ"] = 10
            };
        }
    }

    class DispatchedTrainsStorage
    {
        private List<Train> _trains;

        public DispatchedTrainsStorage()
        {
            _trains = new List<Train>();
        }

        public void AddTrain(Train train) => _trains.Add(train);

        public void ShowInfo()
        {
            if (_trains.Count == 0)
                Console.WriteLine("Нет сформированных поездов\n");
            else
            {
                Console.WriteLine("Сформированные поезда:");
                _trains.ForEach(train => train.ShowInfo());
                Console.WriteLine();
            }
        }
    }
}