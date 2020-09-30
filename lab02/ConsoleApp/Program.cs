using DatabaseLibrary.Data;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DatabaseLibrary.Operations;

namespace ConsoleApp
{
    class Program
    {
        static bool flag;

        static void Main(string[] args)
        {
            using (TvChannelContext db = new TvChannelContext())
            {
                flag = true;
                while (flag)
                {
                    System.Console.Clear();
                    ShowMenu();
                    
                    int n;
                    if(int.TryParse(Console.ReadLine(), out n))
                    {
                        SelectTask(n, db);
                    }
                }
            }
            ShowMessage();
        }
        
        static void ShowMessage(string message = "")
        {
            Console.WriteLine(message);
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить.\n");
            Console.ReadKey();
        }

        static void ShowMenu()
        {
            Console.WriteLine("Task01: выборка всех данных из таблицы, стоящей в схеме базы данных на стороне отношения «один».");
            Console.WriteLine("Task02: выборка данных из таблицы, стоящей в схеме базы данных нас стороне отношения «один», " +
                    "отфильтрованные по определенному условию, налагающему ограничения на одно или несколько полей."); ;
            Console.WriteLine("Task03: выборка данных, сгруппированных по любому из полей данных с выводом какого - либо итогового результата" +
                    " (min, max, avg, сount или др.) по выбранному полю из таблицы, стоящей в схеме базы данных нас стороне отношения «многие».");
            Console.WriteLine("Task04: выборка данных из двух полей двух таблиц, связанных между собой отношением «один-ко-многим».");
            Console.WriteLine("Task05: выборка данных из двух таблиц, связанных между собой отношением «один-ко-многим» и отфильтрованным по некоторому " +
                    "условию, налагающему ограничения на значения одного или нескольких полей.");
            Console.WriteLine("Task06: вставка данных в таблицы, стоящей на стороне отношения «Один».");
            Console.WriteLine("Task07: вставка данных в таблицы, стоящей на стороне отношения «Многие».");
            Console.WriteLine("Task08: удаление данных из таблицы, стоящей на стороне отношения «Один».");
            Console.WriteLine("Task09: удаление данных из таблицы, стоящей на стороне отношения «Многие».");
            Console.WriteLine("Task10: обновление удовлетворяющих определенному условию записей в любой из таблиц базы данных.");

            Console.WriteLine("\n0: выход.\n");
            Console.WriteLine("Перейти к заданию: ");
        }

        static void SelectTask(int n, TvChannelContext db)
        {
            Console.Clear();
            switch (n)
            {
                case 1:
                    ShowMessage(LinqOperations.SelectGenres(db));
                    break;
                case 2:
                    ShowMessage(LinqOperations.SelectGenresByFilter(db));
                    break;
                case 3:
                    ShowMessage(LinqOperations.SelectTimetablesByGroup(db));
                    break;
                case 4:
                    ShowMessage(LinqOperations.SelectShowsAndGenres(db));
                    break;
                case 5:
                    ShowMessage(LinqOperations.SelectShowsAndGenresByFilter(db));
                    break;
                case 6:
                    Console.Write("Введите название жанра: ");
                    string genreName = Console.ReadLine();

                    Console.Write("Введите описание жанра: ");
                    string genreDescription = Console.ReadLine();

                    ShowMessage(LinqOperations.InsertGenre(genreName, genreDescription, db));
                    break;
                case 7:
                    Console.Write("Введите название передачи: ");
                    string name = Console.ReadLine();

                    Console.Write("Введите дату выхода передачи: ");
                    DateTime releaseDate = DateTime.Parse(Console.ReadLine());

                    Console.Write("Введите продолжительность передачи: ");
                    TimeSpan duration = TimeSpan.Parse(Console.ReadLine());

                    Console.Write("Введите рейтинг передачи: ");
                    int mark = int.Parse(Console.ReadLine());

                    Console.Write("Введите месяц рейтинга: ");
                    int markMonth = int.Parse(Console.ReadLine());

                    Console.Write("Введите название жанра: ");
                    int markYear = int.Parse(Console.ReadLine());

                    Console.Write("Введите id жанра: ");
                    int genreId = int.Parse(Console.ReadLine());

                    Console.Write("Введите описание передачи: ");
                    string descripion = Console.ReadLine();

                    ShowMessage(LinqOperations.InsertShows(name, releaseDate, duration, mark,
                        markMonth, markYear, genreId, descripion, db));
                    break;
                case 8:
                    Console.Write("Введите id жанра, который хотите удалить: ");
                    int deleteGenreId = int.Parse(Console.ReadLine());

                    ShowMessage(LinqOperations.DeleteGenre(deleteGenreId, db));

                    break;
                case 9:
                    Console.Write("Введите id передачи, которую хотите удалить: ");
                    int deleteShowId = int.Parse(Console.ReadLine());

                    ShowMessage(LinqOperations.DeleteShow(deleteShowId, db));

                    break;
                case 10:
                    Console.Write("Введите id жанра, который хотите обновить: ");
                    int updGenreId = int.Parse(Console.ReadLine());

                    Console.Write("Введите название жанра: ");
                    string updGenreName = Console.ReadLine();

                    Console.Write("Введите описание жанра: ");
                    string updGenreDescription = Console.ReadLine();

                    ShowMessage(LinqOperations.UpdateGenre(updGenreId, updGenreName, updGenreDescription, db));

                    break;
                case 0:
                    flag = false;
                    break;
            }
        }
    }
}
