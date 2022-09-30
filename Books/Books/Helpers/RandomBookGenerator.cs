using BooksApiClient.Dto;
using System;
using System.Collections.Generic;

namespace Books.Helpers
{
    public class RandomBookGenerator
    {
        static readonly List<string> authors=new() { "Иванов", "Петров", "Сидоров"};
        static readonly List<string> titles = new() { "Заголовок 1", "Заголовок 2", "Заголовок 3" };
        static string GetRandom(List<string> source)
        {
            return source[Random.Shared.Next(source.Count)];
        }
        public static BookInformationDto Next()
        {
            return new()
            {
                Author=GetRandom(authors),
                Title=GetRandom(titles),
                Description="",
                ISBN=Guid.NewGuid().ToString("N"),
                Year=2022,
                Image=ImageHelper.GetRandomBookImage()
            };
        }
    }
}
