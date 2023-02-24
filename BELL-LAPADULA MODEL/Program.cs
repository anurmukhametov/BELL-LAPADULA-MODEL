using System;

namespace BELL_LAPADULA_MODEL
{
    enum Operation
    {
        Read,
        Write,
        Change
    }

    internal enum PrivacyLevel : ushort
    {
        NONCONFIDENTIAL = 0,
        CONFIDENTIAL = 1,
        SECRET = 2,
        TOP_SECRET = 3
    }

    internal static class Command
    {
        public const string Read = "read";
        public const string Write = "write";
        public const string Change = "change";
        public const string Exit = "exit";
    }

    internal static class Data
    {
        private static string[] _subjects =
        {
            "Admin",
            "User-1",
            "User-2",
            "User-3",
            "User-4",
            "User-5",
            "User-6",
        };
        private static string[] _objects =
        {
            "File-1",
            "File-2",
            "File-3",
            "File-4",
            "File-5",
            "File-6",
        };
        public static string[] Subjects { get { return _subjects; } }
        public static string[] Objects { get { return _objects; } }
    }

    internal class SubjectPrivacyLevel
    {
        public SubjectPrivacyLevel(int subjectId, PrivacyLevel level)
        {
            SubjectId = subjectId;
            _assignedLevel = level;
            ResetLevel();
        }

        private PrivacyLevel _assignedLevel;
        private PrivacyLevel _currentLevel;

        public int SubjectId { get; }
        public PrivacyLevel Level { get { return _currentLevel; } }

        public bool ChangeLevel(PrivacyLevel level)
        {
            _currentLevel = level <= _currentLevel ? level : _currentLevel;
            return level <= _currentLevel;
        }
        public void ResetLevel()
        {
            _currentLevel = _assignedLevel;
        }
    }

    internal class ObjectPrivacyLevel
    {
        public ObjectPrivacyLevel(int objectId, PrivacyLevel level)
        {
            ObjectId = objectId;
            Level = level;
        }
        public int ObjectId { get; }
        public PrivacyLevel Level { get; }
    }

    internal static class Helper
    {
        public static SubjectPrivacyLevel[] InitializingSubjectsPrivacyLevels()
        {
            var levels = new SubjectPrivacyLevel[Data.Subjects.Length];
            var random = new Random();
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = new SubjectPrivacyLevel(i, i == 0 ? PrivacyLevel.TOP_SECRET : (PrivacyLevel)random.Next(0, 4));
            }
            return levels;
        }

        public static ObjectPrivacyLevel[] InitializingObjectsPrivacyLevels()
        {
            var levels = new ObjectPrivacyLevel[Data.Objects.Length];
            var random = new Random();
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = new ObjectPrivacyLevel(i, (PrivacyLevel)random.Next(0, 4));
            }
            return levels;
        }

        public static void PrintPrivacyLevels(SubjectPrivacyLevel[] subjectPrivacyLevels, ObjectPrivacyLevel[] objectPrivacyLevels)
        {
            Console.WriteLine("Subject:");
            foreach (var level in subjectPrivacyLevels)
            {
                Console.WriteLine($"{Data.Subjects[level.SubjectId]}: {level.Level}");
            }
            Console.WriteLine();

            Console.WriteLine("Object:");
            foreach (var level in objectPrivacyLevels)
            {
                Console.WriteLine($"{Data.Objects[level.ObjectId]}: {level.Level}");
            }
            Console.WriteLine();
        }

        private static int FindSubjectByName(string name)
        {
            int subjectId = -1;
            for (int i = 0; i < Data.Subjects.Length; i++)
            {
                if (name == Data.Subjects[i])
                {
                    subjectId = i;
                    break;
                }
            }
            return subjectId;
        }

        public static int Identification()
        {
            int subjectId = -1;
            Console.Write("Login: ");
            while (subjectId == -1)
            {
                string input = Console.ReadLine();
                if (input != Command.Exit)
                {
                    subjectId = FindSubjectByName(input);
                    if (subjectId != -1)
                    {
                        Console.WriteLine("Identification was successful.\n");
                    }
                    else
                    {
                        Console.Write("Invalid username, please try again: ");
                    }
                }
                else
                {
                    break;
                }
            }
            return subjectId;
        }

        public static int ReadObjectNumber()
        {
            Console.Write($"Enter object number (1-{Data.Objects.Length}): ");
            while (true)
            {
                var input = Convert.ToInt32(Console.ReadLine());
                if (input >= 1 && input <= Data.Objects.Length)
                {
                    return input;
                }
                else
                {
                    Console.Write("The object with this number does not exist, try again: ");
                }
            }
        }

        private static bool AccessVerification(SubjectPrivacyLevel subjectPrivacyLevel, ObjectPrivacyLevel objectPrivacyLevel, Operation operation)
        {
            switch (operation)
            {
                case Operation.Read:
                    return subjectPrivacyLevel.Level >= objectPrivacyLevel.Level;
                case Operation.Write:
                    return subjectPrivacyLevel.Level <= objectPrivacyLevel.Level;
                default:
                    return false;
            }
        }

        public static void Read(SubjectPrivacyLevel subjectPrivacyLevel, ObjectPrivacyLevel objectPrivacyLevel)
        {
            if (AccessVerification(subjectPrivacyLevel, objectPrivacyLevel, Operation.Read))
            {
                Console.WriteLine($"{Data.Objects[objectPrivacyLevel.ObjectId]} successfully read.\n");
            }
            else
            {
                Console.WriteLine("Read denied.\n");
            }
        }

        public static void Write(SubjectPrivacyLevel subjectPrivacyLevel, ObjectPrivacyLevel objectPrivacyLevel)
        {
            if (AccessVerification(subjectPrivacyLevel, objectPrivacyLevel, Operation.Write))
            {
                Console.WriteLine($"{Data.Objects[objectPrivacyLevel.ObjectId]} successfully write.\n");
            }
            else
            {
                Console.WriteLine("Write denied.\n");
            }
        }

        public static int ReadLevelNumber()
        {
            Console.Write("Enter level number (1-4): ");
            while (true)
            {
                var input = Convert.ToInt32(Console.ReadLine());
                if (input >= 1 && input <= 4)
                {
                    return input;
                }
                else
                {
                    Console.Write("Level with this number does not exist, try again: ");
                }
            }
        }

        public static void Change(SubjectPrivacyLevel subjectPrivacyLevel)
        {
            var levelNumber = ReadLevelNumber() - 1;
            if (subjectPrivacyLevel.ChangeLevel((PrivacyLevel)levelNumber))
            {
                Console.WriteLine($"{Data.Subjects[subjectPrivacyLevel.SubjectId]} changed the privacy level to {subjectPrivacyLevel.Level}\n");
            }
            else
            {
                Console.WriteLine("It is forbidden to increase the level of privacy.\n");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var input = string.Empty;
            var subjectPrivacyLevels = Helper.InitializingSubjectsPrivacyLevels();
            var objectPrivacyLevels = Helper.InitializingObjectsPrivacyLevels();
            Helper.PrintPrivacyLevels(subjectPrivacyLevels, objectPrivacyLevels);

            while (input != Command.Exit)
            {
                int subjectId = Helper.Identification();
                if (subjectId != -1)
                {
                    while (input != Command.Exit)
                    {
                        Console.Write("Command: ");
                        input = Console.ReadLine();
                        switch (input)
                        {
                            case Command.Read:
                                Helper.Read(subjectPrivacyLevels[subjectId], objectPrivacyLevels[Helper.ReadObjectNumber() - 1]);
                                break;
                            case Command.Write:
                                Helper.Write(subjectPrivacyLevels[subjectId], objectPrivacyLevels[Helper.ReadObjectNumber() - 1]);
                                break;
                            case Command.Change:
                                Helper.Change(subjectPrivacyLevels[subjectId]);
                                break;
                            case Command.Exit:
                                subjectPrivacyLevels[subjectId].ResetLevel();
                                Console.WriteLine();
                                break;
                            default:
                                Console.WriteLine("Unknown command.\n");
                                break;
                        }
                    }
                    input = string.Empty;
                }
                else
                {
                    input = Command.Exit;
                }
            }
        }
    }
}
