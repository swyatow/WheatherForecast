using Newtonsoft.Json;
using System.Text;
using System.Web;
using WheatherForcast;

Dictionary<DayOfWeek, string> days = new Dictionary<DayOfWeek, string>()
{
    [DayOfWeek.Monday] = "Понедельник",
    [DayOfWeek.Tuesday] = "Вторник",
    [DayOfWeek.Wednesday] = "Среда",
    [DayOfWeek.Thursday] = "Четверг",
    [DayOfWeek.Friday] = "Пятница",
    [DayOfWeek.Saturday] = "Суббота",
    [DayOfWeek.Sunday] = "Воскресенье"
};

//-------------------------------------------------------------------------------------
Console.OutputEncoding = Encoding.UTF8;
var apiKey = "c87d180368ccb05488b4db3805db2762";
var client = new HttpClient();

while (true)
{
    Console.Write("Прогноз погоды! Введите город: ");
    var city = Console.ReadLine();
    Console.WriteLine("Выясняем, подождите пожалуйста");
    var response = await client.GetAsync(@$"https://api.openweathermap.org/data/2.5/forecast?q={HttpUtility.UrlEncode(city)}&appid={apiKey}&units=metric&lang=ru");

    if (response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadAsStringAsync();
        var info = JsonConvert.DeserializeObject<WheatherInfo>(result);
        Console.Clear();
        Console.WriteLine(
            $"Погода в городе {info.City.Name}, {info.City.Country} на {DateTime.Now} - {info.List[0].Weather[0].Description}\n" +
            $"Температура воздуха - {Math.Round(info.List[0].Main.Temp, 1)}°С\n" +
            $"А по ощущениям - {Math.Round(info.List[0].Main.Feels_like, 1)}°С\n" +
            $"Ветер - {info.List[0].Wind.Speed}м/с, {GetWind(info.List[0].Wind.Deg)}\n" +
            $"Влажность - {info.List[0].Main.Humidity}%\n" +
            $"Давление - {Math.Round(info.List[0].Main.Grnd_level / 1.33322, 2)} мм рт. ст. (нормальное давление - 760 мм рт. ст.)\n\n" +
            $"Прогноз погоды на 4 дня:\n");
        int nextDayIter = 8;
        DateTime iterDate;
        for (int i = 0; i < 4; i++)
        {
            iterDate = DateTime.Parse(info.List[nextDayIter].Dt_txt);
            Console.Write($"{iterDate.ToShortDateString(), 25} | ");
            nextDayIter += 8;
        }
        Console.WriteLine();
        nextDayIter = 8;
        for (int i = 0; i < 4; i++)
        {
            iterDate = DateTime.Parse(info.List[nextDayIter].Dt_txt);
            Console.Write($"{days[iterDate.DayOfWeek], 25} | ");
            nextDayIter += 8;
        }
        Console.WriteLine();
        nextDayIter = 8;
        for (int i = 0; i < 4; i++)
        {
            //Temp_min == Temp_max ?
            string temp = $"{Math.Round(info.List[nextDayIter].Main.Temp_min, 2)}...{Math.Round(info.List[nextDayIter].Main.Temp_max, 2)}";
            Console.Write($"{temp, 25} | ");
            nextDayIter += 8;
        }
        Console.WriteLine();
        nextDayIter = 8;
        for (int i = 0; i < 4; i++)
        {
            string wheatherDesc = info.List[nextDayIter].Weather[0].Description
                .Substring(0, 1)
                .ToUpper()
                + info.List[nextDayIter].Weather[0].Description
                .Substring(1, info.List[nextDayIter].Weather[0].Description.Length - 1);
            Console.Write($"{wheatherDesc, 25} | ");
            nextDayIter += 8;
        }
    }
    else
    {
        Console.WriteLine("Не верно указан город!");
    }

    Console.WriteLine("\n\n\nНажмите ENTER для повторного запроса прогноза погоды");
    Console.ReadLine();
    Console.Clear();
}



//----------------------------------------------------------------------------


string GetWind(int deg)
{
    if (deg > 345 && deg <= 360 || deg >= 0 && deg < 15)
    {
        return "северный";
    }
    else if (deg >= 15 && deg <= 75)
    {
        return "северо-восточный";
    }
    else if (deg > 75 && deg < 105)
    {
        return "восточный";
    }
    else if (deg >= 105 && deg <= 165)
    {
        return "юго-восточный";
    }
    else if (deg > 165 && deg < 195)
    {
        return "южный";
    }
    else if (deg >= 195 && deg <= 255)
    {
        return "юго-западный";
    }
    else if (deg > 255 && deg < 285)
    {
        return "западный";
    }
    else if (deg >= 285 && deg <= 345)
    {
        return "северо-западный";
    }
    else
    {
        return "???";
    }
}