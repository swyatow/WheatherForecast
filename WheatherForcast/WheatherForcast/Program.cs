using Newtonsoft.Json;
using System.Text;
using System.Web;
using WheatherForcast;

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
        int cursorX = Console.GetCursorPosition().Left;
        int cursorY = Console.GetCursorPosition().Top;
        int nextDayIter = 8;
        string infoString;
        DateTime iterDate;
        List currentDateWeather;
        for (int i = 0; i < 4; i++)
        {
            currentDateWeather = info.List[nextDayIter];
            // Дата
            iterDate = DateTime.Parse(currentDateWeather.Dt_txt);
            Console.Write($"{iterDate.ToShortDateString(),25} | ");
            Console.SetCursorPosition(cursorX, cursorY + 1);
            // День недели
            infoString = iterDate.ToString("dddd");
            infoString = infoString[0].ToString().ToUpper() + infoString.Substring(1);
            Console.Write($"{infoString,25} | ");
            Console.SetCursorPosition(cursorX, cursorY + 2);
            // Температура
            //Temp_min == Temp_max ?
            infoString = $"{Math.Round(currentDateWeather.Main.Temp_min, 2)}...{Math.Round(currentDateWeather.Main.Temp_max, 2)}";
            Console.Write($"{infoString,25} | ");
            Console.SetCursorPosition(cursorX, cursorY + 3);
            // Описание погоды
            infoString = currentDateWeather.Weather[0].Description;
            infoString = infoString[0].ToString().ToUpper() + infoString.Substring(1);
            Console.Write($"{infoString,25} | ");
            cursorX = Console.GetCursorPosition().Left;
            Console.SetCursorPosition(cursorX, cursorY);

            nextDayIter += 8;
        }
        Console.SetCursorPosition(0, cursorY + 6);
    }
    else
    {
        Console.WriteLine("Не верно указан город!");
    }
    Console.WriteLine("Нажмите ENTER для повторного запроса прогноза погоды");
    Console.ReadLine();
    Console.Clear();
}

//----------------------------------------------------------------------------

string GetWind(int deg)
{
    switch (deg)
    {
        case int when deg > 345 && deg <= 360 || deg >= 0 && deg < 15:
            return "северный";
        case int when deg >= 15 && deg <= 75:
            return "северо-восточный";
        case int when deg > 75 && deg < 105:
            return "восточный";
        case int when deg >= 105 && deg <= 165:
            return "юго-восточный";
        case int when deg > 165 && deg < 195:
            return "южный";
        case int when deg >= 195 && deg <= 255:
            return "юго-западный";
        case int when deg > 255 && deg < 285:
            return "западный";
        case int when deg >= 285 && deg <= 345:
            return "северо-западный";
        default:
            return "???";
    }
}