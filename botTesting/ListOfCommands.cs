using System;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace botTesting
{
    public class HelloWorld : ModuleBase<SocketCommandContext>
    {
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
                //Embed.WithDescription("");
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
        //make it so empty fields are not shown
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
                    Uri uri = new Uri("https://www.urbandictionary.com/");
                    Embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    Embed.WithTitle("**Information from **" + uri);
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
                await Context.Channel.SendMessageAsync("That word does not exist!");
                return;
            }
        }
        [Command("translate")]
        public async Task Translate(string language1, string language2, [Remainder] string msg)
        {
            //EMBED FOR THIS!!
            Dictionary<string, string> langs = new Dictionary<string, string>();
            langs["Azerbaijan"] = "az";
            langs["Malayalam"] = "ml";
            langs["Albanian"] = "sq";
            langs["Maltese"] = "mt";
            langs["Amharic"] = "am";
            langs["Macedonian"] = "mk";
            langs["English"] = "en";
            langs["Maori"] = "mi";
            langs["Arabic"] = "ar";
            langs["Marathi"] = "mr";
            langs["Armenian"] = "hy";
            langs["Mari"] = "mhr";
            langs["Afrikaans"] = "af";
            langs["Mongolian"] = "mn";
            langs["Basque"] = "eu";
            langs["German"] = "de";
            langs["Bashkir"] = "ba";
            langs["Nepali"] = "ne";
            langs["Belarusian"] = "be";
            langs["Norwegian"] = "no";
            langs["Bengali"] = "bn";
            langs["Punjabi"] = "pa";
            langs["Burmese"] = "my";
            langs["Papiamento"] = "pap";
            langs["Bulgarian"] = "bg";
            langs["Persian"] = "fa";
            langs["Bosnian"] = "bs";
            langs["Polish"] = "pl";
            langs["Welsh"] = "cy";
            langs["Portuguese"] = "pt";
            langs["Hungarian"] = "hu";
            langs["Romanian"] = "ro";
            langs["Vietnamese"] = "vi";
            langs["Russian"] = "ru";
            langs["Haitian(Creole)"] = "ht";
            langs["Cebuano"] = "ceb";
            langs["Galician"] = "gl";
            langs["Serbian"] = "sr";
            langs["Dutch"] = "nl";
            langs["Sinhala"] = "si";
            langs["Hill Mari"] = "mrj";
            langs["Slovakian"] = "sk";
            langs["Greek"] = "el";
            langs["Slovenian"] = "sl";
            langs["Georgian"] = "ka";
            langs["Swahili"] = "sw";
            langs["Gujarati"] = "gu";
            langs["Sundanese"] = "su";
            langs["Danish"] = "da";
            langs["Tajik"] = "tg";
            langs["Hebrew"] = "he";
            langs["Thai"] = "th";
            langs["Yiddish"] = "yi";
            langs["Tagalog"] = "tl";
            langs["Indonesian"] = "id";
            langs["Tamil"] = "ta";
            langs["Irish"] = "ga";
            langs["Tatar"] = "tt";
            langs["Italian"] = "it";
            langs["Telugu"] = "te";
            langs["Icelandic"] = "is";
            langs["Turkish"] = "tr";
            langs["Spanish"] = "es";
            langs["Udmurt"] = "udm";
            langs["Kazakh"] = "kk";
            langs["Uzbek"] = "uz";
            langs["Kannada"] = "kn";
            langs["Ukrainian"] = "uk";
            langs["Catalan"] = "ca";
            langs["Urdu"] = "ur";
            langs["Kyrgyz"] = "ky";
            langs["Finnish"] = "fi";
            langs["Chinese"] = "zh";
            langs["French"] = "fr";
            langs["Korean"] = "ko";
            langs["Hindi"] = "hi";
            langs["Xhosa"] = "xh";
            langs["Croatian"] = "hr";
            langs["Khmer"] = "km";
            langs["Czech"] = "cs";
            langs["Laotian"] = "lo";
            langs["Swedish"] = "sv";
            langs["Latin"] = "la";
            langs["Scottish"] = "gd";
            langs["Latvian"] = "lv";
            langs["Estonian"] = "et";
            langs["Lithuanian"] = "lt";
            langs["Esperanto"] = "eo";
            langs["Luxembourgish"] = "lb";
            langs["Javanese"] = "jv";
            langs["Malagasy"] = "mg";
            langs["Japanese"] = "ja";
            langs["Malay"] = "ms";
            string langFromAndTo = language1 + "-" + language2;
            string key = "trnsl.1.1.20190501T085131Z.60ba4708835f8d1f.10422b658ea3bfcf81a3c05d9da973a83920a8ae";
            var encoded = Uri.EscapeUriString(msg);
            WebClient client = new WebClient();
            string value = client.DownloadString("https://translate.yandex.net/api/v1.5/tr.json/translate?lang=" + langFromAndTo + "&key=" + key + "&text=" + encoded);
            var result = JsonConvert.DeserializeObject<YandexTranslate.RootObject>(value);
            var translatedText = result.text.ElementAt(0);
            foreach (KeyValuePair<string, string> item in langs)
            {
                EmbedBuilder listOfLanguages = new EmbedBuilder();
                listOfLanguages.AddField("Key", item.Key);
                if (item.Value.Contains(language1))
                {
                    language1 = item.Key;
                }
                if (item.Value.Contains(language2))
                {
                    language2 = item.Key;
                    break;
                }
            }
            await Context.Channel.SendMessageAsync("`" + msg + "`" + " translated from `" + language1 + "` is `" + translatedText + "` in `" + language2 + "`");
        }
    }
}
