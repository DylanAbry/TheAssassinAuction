# TheAssassinAuction
- PLEASE NOTE: This game is best played on a screen with 1920 * 1080 resolution! UI elements (an important part of this game!!) may be off scale if played on a different screen size...

# Game Outline: 
- What is The Assassin Auction:
“The Assassin Auction” is a turn-based strategy game where players blindly bid for weapons of different tiers (1 to 5) against a clay figure CPU, Fleece deFraud. The goal of the game is to have a collection of weapons with a higher overall tier score sum than Fleece. However, multiple game elements are out of the player’s control: randomized weapons for bid, risking purchased weapons in an unknown bonus game, Fleece’s highest bid, trading in a weapon in hopes to win a better one, and even when the auction will end, which is random every game! It is the ultimate non-monetary (but still intense) assassin gambling experience!
- Technologies Used:
The Assassin Auction was developed in the Unity (2021) game engine with a built-in render pipeline for 3D assets. The C# programming was completed using the Visual Studios 2019 IDE and features Unity’s System, Generic, Engine, Random, UI and Scene Management libraries. 3D assets were imported from the Unity Asset Store or Sketchfab, and audio was sampled from freesound.org and mixed in Audacity. Some images, such as the Classic winning screen, were taken by me and edited with a Laplacian filter using Google Colab AI.
- System Architecture:
In both the “Classic” and “deFraud’s Deathtrap” modes, multiple C# dictionaries and lists are used to track the weapons (and their UI icons) not yet bid, in both competitors’ collections, and traded in. A random number from 0 to the weapon dictionary’s length is generated prior to each round to determine which weapon index will be up for bid, hence why the weapon for bid is never known until purchased. The probability that Fleece will raise its bid will gradually decrease as the price rises and its maximum threshold is $300. He will also trade when he has under $150 so he never runs out of money. Fleece also has multiple phrases he can randomly choose to say when the player wins or loses a bid and bonus game (he also has 12 possible phrases he can say during the game’s loading screen). The UI was built with Unity’s Canvas system and displays all the game features, including funds, CPU reactions, inventories and bid prompts to ensure the left mouse is the only required controller.
- Roadblocks and Solutions:
One primary roadblock that appeared during the development process was having the correct weapons appear on the win screen. For a while, Unity was acting buggy because I would calculate each player’s weapons and overall score at the game’s end. Some weapons wouldn’t show up even if the player won them. So, I instead created a list that updated the player’s score throughout the game so the code wouldn’t be as taxing on the Unity Engine and matched the game object weapon variables with their strings in the dictionary. When the game ends, the engine then matches the weapon icon in the ending scene with its string in the game’s dictionary to determine which should be active or inactive, depending on what was present in the player’s collection. This same issue happened when exchanging weapons from the collection, so instead of executing the changes at once, I had to consistently update the data structures during gameplay so Unity would not act buggy when adjusting a player’s collection after adding or trading in a weapon.

# Download Instructions:
- Click the link in the repository description to be brought to the game's page on Itchio!
- Download the ZIP file, right click on it and extract all the files
- Run the application file in the folder (if Windows Protection pops up, just select More Info and then Run Anyway)
  
# Controls: 
- Only the left mouse click is required to play the game!
