using System;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace botTesting
{
    public class ListOfCommands : ModuleBase<SocketCommandContext>
    {
        readonly SortedDictionary<string, string> langs = new SortedDictionary<string, string>
        {
            ["Azerbaijan"] = "az",
            ["Malayalam"] = "ml",   
            ["Albanian"] = "sq",
            ["Maltese"] = "mt",
            ["Amharic"] = "am",
            ["Macedonian"] = "mk",
            ["English"] = "en",
            ["Maori"] = "mi",
            ["Arabic"] = "ar",
            ["Marathi"] = "mr",
            ["Armenian"] = "hy",
            ["Mari"] = "mhr",
            ["Afrikaans"] = "af",
            ["Mongolian"] = "mn",
            ["Basque"] = "eu",
            ["German"] = "de",
            ["Bashkir"] = "ba",
            ["Nepali"] = "ne",
            ["Belarusian"] = "be",
            ["Norwegian"] = "no",
            ["Bengali"] = "bn",
            ["Punjabi"] = "pa",
            ["Burmese"] = "my",
            ["Papiamento"] = "pap",
            ["Bulgarian"] = "bg",
            ["Persian"] = "fa",
            ["Bosnian"] = "bs",
            ["Polish"] = "pl",
            ["Welsh"] = "cy",
            ["Portuguese"] = "pt",
            ["Hungarian"] = "hu",
            ["Romanian"] = "ro",
            ["Vietnamese"] = "vi",
            ["Russian"] = "ru",
            ["Haitian(Creole)"] = "ht",
            ["Cebuano"] = "ceb",
            ["Galician"] = "gl",
            ["Serbian"] = "sr",
            ["Dutch"] = "nl",
            ["Sinhala"] = "si",
            ["Hill Mari"] = "mrj",
            ["Slovakian"] = "sk",
            ["Greek"] = "el",
            ["Slovenian"] = "sl",
            ["Georgian"] = "ka",
            ["Swahili"] = "sw",
            ["Gujarati"] = "gu",
            ["Sundanese"] = "su",
            ["Danish"] = "da",
            ["Tajik"] = "tg",
            ["Hebrew"] = "he",
            ["Thai"] = "th",
            ["Yiddish"] = "yi",
            ["Tagalog"] = "tl",
            ["Indonesian"] = "id",
            ["Tamil"] = "ta",
            ["Irish"] = "ga",
            ["Tatar"] = "tt",
            ["Italian"] = "it",
            ["Telugu"] = "te",
            ["Icelandic"] = "is",
            ["Turkish"] = "tr",
            ["Spanish"] = "es",
            ["Udmurt"] = "udm",
            ["Kazakh"] = "kk",
            ["Uzbek"] = "uz",
            ["Kannada"] = "kn",
            ["Ukrainian"] = "uk",
            ["Catalan"] = "ca",
            ["Urdu"] = "ur",
            ["Kyrgyz"] = "ky",
            ["Finnish"] = "fi",
            ["Chinese"] = "zh",
            ["French"] = "fr",
            ["Korean"] = "ko",
            ["Hindi"] = "hi",
            ["Xhosa"] = "xh",
            ["Croatian"] = "hr",
            ["Khmer"] = "km",
            ["Czech"] = "cs",
            ["Laotian"] = "lo",
            ["Swedish"] = "sv",
            ["Latin"] = "la",
            ["Scottish"] = "gd",
            ["Latvian"] = "lv",
            ["Estonian"] = "et",
            ["Lithuanian"] = "lt",
            ["Esperanto"] = "eo",
            ["Luxembourgish"] = "lb",
            ["Javanese"] = "jv",
            ["Malagasy"] = "mg",
            ["Japanese"] = "ja",
            ["Malay"] = "ms"
        };

        [Command("hello")]
        public async Task Hello()
        {
            Random Rand = new Random();
            string[] HelloTexts = {"What do you want?", "Who are you",
                                  "I don't care about you", "Bye",
                                  "Don't bother me"};
            int RandNum = Rand.Next(HelloTexts.Length);
            await Context.Channel.SendMessageAsync(HelloTexts[RandNum]);
        }
        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("```Here are a list of available commands:\n!help\n!hello\n!embed <phrase>\n!fuck <user>\n!loop <phrase> <numoftimes> (surround phrases longer than one word in quotes)\n!money\n!money <user>\n!money give <user> <amount>\n!money take <user> <amount>\n!store\n!money work\n!money reset <user>\n!money inventory\n!avatar\n!avatar <user>```");
        }
        [Command("fuck")]
        public async Task Fuck([Remainder] IGuildUser OtherUser)
        {
            if (Context.User.Equals(OtherUser))
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " has fucked themselves");
            }
            else if (OtherUser.Username.Equals("myBot"))
            {
                await Context.Channel.SendMessageAsync("No, fuck **_you_**");
            }
            else
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " says fuck you to " + OtherUser.Mention);
            }
        }

        [Command("embed")]
        public async Task Embed([Remainder] string Input = "")
        {
            if (!Input.Equals(""))
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Embed", Context.User.GetAvatarUrl());
                Embed.WithColor(40, 200, 150);
                Embed.WithFooter(Context.User.Username);
                Embed.WithCurrentTimestamp();
                Embed.AddField("User input:", Input);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("Please include a phrase!");
            }
        }
        [Command("F")]
        public async Task F()
        {
            Random Ran = new Random();
            string[] DiffTexts = {"Press F for Respects :cry:", "Drop an F :(",
                                 "You better press F before dying", "You get one life, F away",
                                 "F for your future", "F means Failure, which you already are"};
            int Rand = Ran.Next(DiffTexts.Length);
            await Context.Channel.SendMessageAsync(DiffTexts[Rand]);
        }
        [Command("store")]//more shop items!!
        public async Task Store()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Shop");
            Embed.WithColor(40, 200, 150);
            Embed.AddField("Item 1", "1. Dog $100");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
        [Command("caps")]
        public async Task Caps([Remainder] string Phrase = "")
        {
            if (!Phrase.Equals(""))
            {
                await Context.Channel.SendMessageAsync(Phrase.ToUpper());
            }
            else
            {
                await Context.Channel.SendMessageAsync("Include a message to be capitalized");
            }
        }
        [Command("avatar")]
        public async Task Avatar(IUser User = null)
        {
            Console.WriteLine(Context.User.GetAvatarUrl());
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(255, 0, 238);
            if (User != null)
            {
                if (User.GetAvatarUrl() == null)
                {
                    await Context.Channel.SendMessageAsync("No PFP for this user");
                    return;
                }
                Embed.WithImageUrl(User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                Embed.WithImageUrl(Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
        }
        //todo: send top defintion by default (count thumbs up), if user wants rand then allow random
        //also, maybe split "[" or "("?
        //make it so empty fields (example, author, def, etc.) are not shown
        //scrape urban dictionary for word of the day??
        [Command("define")]
        public async Task Define([Remainder] string Word = "")
        {
            if (!Word.Equals(""))
            {
                var encoded = Uri.EscapeUriString(Word);
                WebClient client = new WebClient();
                string value = client.DownloadString("http://api.urbandictionary.com/v0/define?term=" + encoded);
                var result = JsonConvert.DeserializeObject<UrbanDictionary.RootObject>(value);
                if (result.list.Count > 0)
                {
                    Random rand = new Random();
                    int nextRand = rand.Next(result.list.Count);
                    String date = result.list.ElementAt(nextRand).written_on.ToString();
                    DateTime dateValues = (Convert.ToDateTime(date.ToString()));
                    String day = dateValues.Day.ToString();
                    String month = dateValues.Month.ToString();
                    String year = dateValues.Year.ToString();
                    String fixedDate = month + "/" + day + "/" + year;
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    Embed.WithTitle("**Information from**" + result.list.ElementAt(nextRand).permalink);
                    Embed.WithColor(40, 200, 150);
                    Embed.AddField("Author: ", result.list.ElementAt(nextRand).author);
                    Embed.AddField("Word: ", result.list.ElementAt(nextRand).word);
                    Embed.AddField("Example: ", result.list.ElementAt(nextRand).example);
                    Embed.AddField("Likes: ", result.list.ElementAt(nextRand).thumbs_up);
                    Embed.AddField("Dislikes: ", result.list.ElementAt(nextRand).thumbs_down);
                    Embed.AddField("Written on: ", fixedDate);
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Context.Channel.SendMessageAsync("**Imprecise translation:** " + result.list.ElementAt(nextRand).definition);
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync("That word does not exist!");
                    return;
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Enter a word!");
                return;
            }
        }
        [Command("translate")]
        public async Task Translate(string language1 = "", string language2 = "", [Remainder] string msg = "")
        {
            if (language1.Equals("") || language2.Equals("") || msg.Equals(""))
            {
                await Context.Channel.SendMessageAsync("Provide all parameters!");
                return;
            }
            string valLanguage1 = language1;
            string valLanguage2 = language2;
            for (int i = 0; i < langs.Count; i++)
            {
                if (language1.Length > 3)
                {
                    if (langs.ElementAt(i).Key.Equals(language1))
                    {
                        valLanguage1 = langs.ElementAt(i).Value;
                    }
                }
                if (language2.Length > 3)
                {
                    if (langs.ElementAt(i).Key.Equals(language2))
                    {
                        valLanguage2 = langs.ElementAt(i).Value;
                    }
                }
            }
            string langFromAndTo = valLanguage1 + "-" + valLanguage2;
            string key = "trnsl.1.1.20190501T085131Z.60ba4708835f8d1f.10422b658ea3bfcf81a3c05d9da973a83920a8ae";
            var encoded = Uri.EscapeUriString(msg);
            WebClient client = new WebClient();
            string value = client.DownloadString("https://translate.yandex.net/api/v1.5/tr.json/translate?lang=" + langFromAndTo + "&key=" + key + "&text=" + encoded);
            var result = JsonConvert.DeserializeObject<YandexTranslate.RootObject>(value);
            var workedOrNot = result.code;
            Console.WriteLine(workedOrNot.ToString());
            var translatedText = result.text.ElementAt(0);
            for (int i = 0; i < langs.Count; i++)
            {
                if (langs.ElementAt(i).Value.Equals(language1))
                {
                    language1 = langs.ElementAt(i).Key;
                }
                if (langs.ElementAt(i).Value.Equals(language2))
                {
                    language2 = langs.ElementAt(i).Key;
                }

            }
            await Context.Channel.SendMessageAsync("`" + msg + "`" + " translated from `" + language1 + "` is `" + translatedText + "` in `" + language2 + "`");
        }
        [Command("languages")]
        public async Task Languages(int page)
        {
            EmbedBuilder languages = new EmbedBuilder();
            languages.WithTitle("**Languages**");
            languages.WithColor(40, 200, 150);
            if (page > 12 || page < 0)
            {
                await Context.Channel.SendMessageAsync("Not a valid index!");
            }
            switch (page)
            {
                case 1:
                    for (int i = 0; i < 8; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 1/12");
                    }
                    break;
                case 2:
                    for (int i = 8; i < 16; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 2/12");
                    }
                    break;
                case 3:
                    for (int i = 16; i < 24; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 3/12");
                    }
                    break;
                case 4:
                    for (int i = 24; i < 32; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 4/12");
                    }
                    break;
                case 5:
                    for (int i = 32; i < 40; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 5/12");
                    }
                    break;
                case 6:
                    for (int i = 40; i < 48; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 6/12");
                    }
                    break;
                case 7:
                    for (int i = 48; i < 56; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 7/12");
                    }
                    break;
                case 8:
                    for (int i = 56; i < 64; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 8/12");
                    }
                    break;
                case 9:
                    for (int i = 64; i < 72; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 9/12");
                    }
                    break;
                case 10:
                    for (int i = 72; i < 80; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 10/12");
                    }
                    break;
                case 11:
                    for (int i = 80; i < 88; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 11/12");
                    }
                    break;
                case 12:
                    for (int i = 88; i < 93; i++)
                    {
                        languages.AddField("Full Form: ", langs.ElementAt(i).Key, true);
                        languages.AddField("Shortened Form: ", langs.ElementAt(i).Value, true);
                        languages.AddField("\u200b", "\u200b");
                        languages.WithFooter("Page 12/12");
                    }
                    break;
                default:
                    await Context.Channel.SendMessageAsync("Not a valid page!");
                    return;
            }
            await Context.Channel.SendMessageAsync("", false, languages.Build());
        }
        [Command("wordoftheday")]
        public async Task WordOfTheDay(int index = 0)
        {
            Random Rand = new Random();
            int Value = Rand.Next(int.MaxValue);
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load("https://www.urbandictionary.com/?random=" + Value);
            string meaning = WebUtility.HtmlDecode(document.DocumentNode.SelectNodes("//div[@class='meaning']").ElementAt(index).InnerText);
            string word = WebUtility.HtmlDecode(document.DocumentNode.SelectNodes("//div[@class='def-header']").ElementAt(index).SelectNodes("//a[@class='word']").ElementAt(index).InnerText);          
            string date = WebUtility.HtmlDecode(document.DocumentNode.SelectNodes("//div[@class='contributor']").ElementAt(index).InnerText.Substring(3));
            string example = WebUtility.HtmlDecode(document.DocumentNode.SelectNodes("//div[@class= 'example']").ElementAt(index).InnerText);
            string dateOfWordOfTheDay = WebUtility.HtmlDecode(document.DocumentNode.SelectNodes("//div[@class='ribbon']").ElementAt(index).InnerText);
            string[] splitDateOfWordOfTheDay = dateOfWordOfTheDay.Split(" ");
            string monthOfWOD = splitDateOfWordOfTheDay[0];
            int dayOfWOD = Int32.Parse(splitDateOfWordOfTheDay[1]);
            DateTime dateNow = DateTime.Now;
            string monthNow = dateNow.ToString("MMMM");
            int dayNow = dateNow.Day;
            if (!(index >= 7))
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithColor(40, 200, 150);
                if ((monthOfWOD.Trim().Equals(monthNow.Trim())) && (dayOfWOD == dayNow))
                {
                    embed.WithTitle("**Urban Dictionary's Word Of The Day!**");
                    
                }
                else
                {
                    //if (!(monthOfWOD.Trim().Equals(monthNow.Trim())) && (dayOfWOD == dayNow - 1))
                    //{
                    //    embed.WithTitle("**No new word today (so far), here is the last word of the day posted! On " + monthOfWOD + " " + dayOfWOD + "!**");
                    //}

                    embed.WithTitle("**Urban Dictionary's Word Of The Day On " + monthOfWOD + " " + dayOfWOD + "!**");
                }
                embed.AddField("Word:", word);
                embed.AddField("Example:", example);
                embed.AddField("Author and Date Written:", date);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                await Context.Channel.SendMessageAsync("**Imprecise definition:** " + meaning);
                return;
            }
            else
            {
                await Context.Channel.SendMessageAsync("Send a valid index!");
            }
        }
        [Command("randomword")]
        public async Task RandomWord()
        {
            Random Rand = new Random();
            int randPage = Rand.Next(1000) + 1;
            HtmlWeb web = new HtmlWeb();
            if(randPage == 0)
            {
                web.Load("https://www.urbandictionary.com/random.php");
            }
            else
            {
                web.Load("https://www.urbandictionary.com/random.php?page=" + randPage);
            }
            //get a random word from each random page
        }
        //Make this so it returns a specific breed as well!
        [Command("dogimage")]
        public async Task DogImage()
        {
            WebClient client = new WebClient();
            string value = client.DownloadString("https://dog.ceo/api/breeds/image/random");
            var result = JsonConvert.DeserializeObject<RandomDogPics.RootObject>(value);
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(255, 0, 238);
            Embed.WithImageUrl(result.message);
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
        //https://www.metaweather.com/api/#location use this website for weather!
    }
}
