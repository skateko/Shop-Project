using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace L11.Controllers
{
    public class GameController : Controller
    {
        public void SetSession()
        {
            Random random = new Random();
            int Range = 30;
            HttpContext.Session.SetString("random", JsonConvert.SerializeObject(random));
            HttpContext.Session.SetInt32("Range", 30);                                  //Range = userRange;
            HttpContext.Session.SetInt32("NumberToGuess", random.Next(0, Range));       //NumberToGuess = random.Next(0, Range);
            HttpContext.Session.SetInt32("Counter", 0);                                 //Counter = 0;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Set(int userRange)
        {
            ViewBag.OperationType = nameof(Set);
            
            byte[] arr;
            HttpContext.Session.TryGetValue("random", out arr);
            if (arr != null)
            {
                Random random = JsonConvert.DeserializeObject<Random>(HttpContext.Session.GetString("random"));
                if (userRange < 0)
                {
                    ViewBag.Message = "Invalid argument passed to range";
                }
                else
                {
                    HttpContext.Session.SetInt32("Range", userRange);                           //Range = userRange;
                    HttpContext.Session.SetInt32("NumberToGuess", random.Next(0, userRange));   //NumberToGuess = random.Next(0, Range);
                    HttpContext.Session.SetInt32("Counter", 0);                                 //Counter = 0;
                    ViewBag.Message = $"Range changed, Attempts counter was set to 0";
                    ViewBag.Range = userRange;          //Range;
                    ViewBag.Counter = 0;                //Counter;
                }
                ViewBag.Range = HttpContext.Session.GetInt32("Range");          //Range;
                ViewBag.Counter = HttpContext.Session.GetInt32("Counter");      //Counter;
            }
            else
            {
                SetSession();
                ViewBag.Message = $"Session was not established. It was done automatically, you can now play the game";
            }
            return View("Index");
        }
        public IActionResult Draw()
        {
            ViewBag.OperationType = nameof(Draw);

            byte[] arr;
            HttpContext.Session.TryGetValue("random", out arr);
            if (arr != null)
            {
                Random random = JsonConvert.DeserializeObject<Random>(HttpContext.Session.GetString("random"));
                var cookieRange = HttpContext.Session.GetInt32("Range");
                var cookieCounter = HttpContext.Session.GetInt32("Counter");
                var cookieToGuess = HttpContext.Session.GetInt32("NumberToGuess");
                int drawNumber = random.Next(0, (int)cookieRange);

                HttpContext.Session.SetInt32("Counter", (int)++cookieCounter);                          //Counter++;
                ViewBag.Range = cookieRange;                                                            //ViewBag.Range = Range;
                ViewBag.Counter = cookieCounter;                                                        //ViewBag.Counter = Counter;
                ViewBag.UserNumber = drawNumber;
                if (drawNumber == cookieToGuess)
                {
                    ViewBag.Message = $"You are lucky winner! Guessed number was: {cookieToGuess}";
                    ViewBag.GameState = "game-won";
                    HttpContext.Session.SetInt32("NumberToGuess", random.Next(0, (int)cookieRange));    //NumberToGuess = random.Next(0, Range);
                    HttpContext.Session.SetInt32("Counter", 0);                                         //Counter = 0;
                }
                else if (drawNumber < cookieToGuess)
                {
                    ViewBag.Message = $"Ooh little unlucky! Your drawn number was smaller than it should be.";
                    ViewBag.GameState = "game-not-won";
                }
                else
                {
                    ViewBag.Message = $"Ooh little unlucky! Your number was higher than it should be.";
                    ViewBag.GameState = "game-not-won";
                }
            }
            else
            {
                SetSession();
                ViewBag.Message = $"Session was not established. It was done automatically, you can now play the game";
            }
            return View("Index");
        }
        public IActionResult Guess(int userNumber)
        {
            ViewBag.OperationType = nameof(Guess);

            byte[] arr;
            HttpContext.Session.TryGetValue("random", out arr);
            if (arr != null)
            {
                Random random = JsonConvert.DeserializeObject<Random>(HttpContext.Session.GetString("random"));
                var cookieRange = HttpContext.Session.GetInt32("Range");
                var cookieCounter = HttpContext.Session.GetInt32("Counter");
                var cookieToGuess = HttpContext.Session.GetInt32("NumberToGuess");

                ViewBag.Range = cookieRange;         
                ViewBag.UserNumber = userNumber;
            
                if (userNumber > cookieRange || userNumber < 0)
                {
                    ViewBag.Message = $"Number is not in range, nothing changed";
                    ViewBag.GameState = "game-not-won";
                    ViewBag.Counter = cookieCounter;                                                        //ViewBag.Counter = Counter;
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetInt32("Counter", (int)++cookieCounter);                          //Counter++;
                    ViewBag.Counter = cookieCounter;
                    if (userNumber == cookieToGuess)
                    {
                        ViewBag.Message = $"You have guessed the number! Guessed number was: {cookieToGuess}";
                        ViewBag.GameState = "game-won";
                        HttpContext.Session.SetInt32("NumberToGuess", random.Next(0, (int)cookieRange));    //NumberToGuess = random.Next(0, Range);
                        HttpContext.Session.SetInt32("Counter", 0);                                         //Counter = 0;
                    }
                    else if (userNumber < cookieToGuess)
                    {
                        ViewBag.Message = $"Ooh little unlucky! Your number was smaller than it should be.";
                        ViewBag.GameState = "game-not-won";
                    }
                    else
                    {
                        ViewBag.Message = $"Ooh little unlucky! Your number was higher than it should be.";
                        ViewBag.GameState = "game-not-won";
                    }
                }
            }
            else
            {
                SetSession();
                ViewBag.Message = $"Session was not established. It was done automatically, you can now play the game";
            }
            return View("Index");
        }
    }
}
