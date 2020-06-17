# A discord bot that can help in servers (manage servers and also provide as a source of entertinment)
### This bot has tons of commands and can be setup fairly easily. I did not choose to host this particular bot. You can replace the database to whatever DB you wish to use, just remember to update the files. 
### The first step is to make a discord account at https://discord.com/. Once you do this, follow the next steps:
  1. Once you are logged into discord, go to the dev portal: https://discord.com/developers/applications/
  2. Click the new application button and give it a name
  3. On the left hand side, you will see a panel and one of the options is 'Bot', click this
  4. Click on 'Add Bot' and follow through with the prompt that appears
  5. Here, the most important thing to keep track of is the 'Token'. Click reveal token and copy the token, keep this somewhere safe!
### If you want to follow through with the next step, make sure you have SQLite DB Browser installed. You can find the details here: https://sqlitebrowser.org/ 
  1. Make a txt file (you can name it whatever you want). Remember the token you copied? Paste it on the first line
  2. Open up DB Browser and click New Database at the top left
  3. Give the DB a name and save it somewhere you remember
  4. Get the location of the DB (copy path) and paste it as the 2nd line in the txt file you have
### If you choose to use the Yandex translate API, you must get your own key as well. More info here: https://tech.yandex.com/translate/ Once you have the key, you can add it as the 3rd and final line of the txt file. If you choose not to use the API that's okay! Just remember to delete the yandex command in ListOfCommands.cs and also the SortedDictionary. 
### Next, you will need to save this txt file and save it, remembering the location. Copy the path and replace it with any instances of `@"M:\token.txt"` in the files (not a lot). 
### And you should now be good to go! One of the most important things to realize is that the DB is locally setup. This can easily be fixed by either changing the DB to PostgreSQL or something else. Or, look into SQLite hosting services. The only reason I say this is because your bot will not work if you try to host the bot somewhere since the DB is local. 

#### Side note: I added in a chromedriver for expirement purposes but feel free to delete this if you choose to do so. It may not be the best implemenation but it was the first thing I thought of when creating this bot. Or, maybe you want to implement it yourself. Either way, it is something I wanted to do but ended up not doing for various reasons. 
