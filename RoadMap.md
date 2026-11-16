# Roadmap

This is the plan for building the game. I'm making this game on my own, so I want to keep things manageable and build it one piece at a time.

The main idea is a cozy 2D farming game where you grow crops, discover new plants, and experiment with different combinations. I want the farming to feel satisfying, but I also want discovering and collecting plants to be a big part of the game. Kinda like that Dragon island or smt

# 1. Get the basics working DONE

Before adding any actual farming, I need to get the world and grid working properly.

- Set up the Unity project and main scene. DONE
- Set up the camera. DONE
- Create the Grid and Tilemaps. DONE
- Set up the ground Tilemap. DONE
- Set up the Tilemap used for displaying the mouse highlight. DONE
- Get the mouse position in the world. DONE
- Convert the mouse position into a grid cell. DONE
- Make the correct tile highlight when I move the mouse. DONE
- Make sure the highlight disappears when the mouse leaves the playable area. DONE
- Set up the farm boundaries. DONE
- Make sure the grid still works when the camera moves or zooms. DONE

# 2. Build the farming area DONE

Now that the grid works, I can start making the actual farm.

- Make a clear distinction between normal ground and farmland. DONE
- Add the ability to till soil. DONE
- Add the ability to water soil. DONE
- Make sure each tile remembers its own state. DONE
- Display the correct appearance for each soil state. DONE
- Prevent interactions with tiles that shouldn't be usable. DONE

# 3. Get crops working ONGOING

- Add a way to plant seeds. DONE
- Make sure seeds can only be planted in suitable soil. DONE
- Make crops grow over real time. DONE
- Add different growth stages. DONE
- Display the correct sprite for each growth stage. DONE
- Make watering speed up growth. DONE- Only grow with water
- Make crops grow even when I'm not actively interacting with them. DONE
- Add harvesting. DONE
- Give the player the correct amount of produce after harvesting. NEXT
- Make sure harvested crops leave the soil in the correct state. DONE

The first crop will be the turnip:

- Growth time: 24 seconds.
- Growth stages: 6.
- Seed cost: 1.
- Harvest yield: 2 turnips.

# 4. Add an inventory

- Create an inventory system. Kinda
- Add and remove items. DONE
- Keep track of item quantities. DONE
- Display the inventory on screen. NEXT
- Add seeds to the inventory. Kinda
- Ability to sell Seeds DONE
- Make planting consume Money. DONE
- Make harvesting add crops. DONE
- Handle items that the player doesn't have enough of. DONE

# 5. Make adding new crops easier

- Its name and description.
- Its seed item.
- Its growth time.
- Its growth stages.
- Its harvest yield.
- Its sprites.
- Any special properties.

Tasks:

- Create a CropDefinition. DOPNE
- Create a way to keep track of all available crops.
- Move the turnip's information into a CropDefinition. DONE
- Make sure the farming system works with different crop definitions. NEXT
- Add a few more crops to test it.
- Make sure adding a crop doesn't require changing the farming system itself.

# 6. Add the botanical journal

- Create the journal interface.
- Display discovered crops.
- Show information about each crop.
- Keep track of which crops the player has discovered.
- Show undiscovered entries without revealing everything.
- Add descriptions and other information to crop entries.
- Make discovering a new crop feel rewarding.

# 7. Add crop experimentation and alchemy

- Decide how crop combinations should work.
- Create a system for defining combinations.
- Add a way to combine or experiment with crops.
- Make successful experiments unlock new plants.
- Make unsuccessful experiments give useful feedback.
- Add discovered results to the botanical journal.
- Make sure combinations aren't completely random or impossible to figure out.

# 8. Expand the world

- Create the world layout.
- Add more areas to explore.
- Add decorations and environmental details.
- Add different types of ground and terrain.
- Add more places where crops can be grown.
- Add buildings or other useful locations if needed.
- Add optional characters or other interactions if they fit the game.

# 9. Improve the visuals and sound

- Settle on a consistent art style.
- Make the ground and soil tiles look good.
- Add sprites for the crops and their growth stages.
- Add animations where they make sense.
- Improve the inventory and journal interfaces.
- Add sound effects for planting, watering, and harvesting.
- Add background music.
- Add small visual details that make the world feel alive.

# 10. Saving and finishing the game

- Save the inventory.
- Save the state of every farm tile.
- Save crop growth progress.
- Save discovered plants.
- Save journal progress.
- Load everything correctly when starting the game.
- Test saving and loading in different situations.
- Fix bugs and awkward interactions.
- Balance crop growth times and harvest amounts.
- Make sure the game doesn't lose progress unexpectedly.

# What I want the first playable version to include

- A working grid and farm.
- Mouse-based tile selection.
- Tilling and watering.
- Planting and harvesting.
- A few different crops.
- Crops that grow over real time.
- A working inventory.
- Basic crop definitions.
- A simple botanical journal.
- Saving and loading.

# A few things to keep in mind

- I'm building this myself, so I should avoid making things unnecessarily complicated.
- I should get individual features working before combining them.
- I shouldn't add systems just because they might be useful someday.
- I should test things as I go instead of leaving all testing until the end.
- I should commit working changes to Git regularly.
- If something becomes too complicated, I should stop and figure out whether there's a simpler way to do it.
- I should focus on making the game enjoyable, not just getting tasks checked off.

**Most importantly: I want to finish a small, playable game before I start trying to make the huge version in my head.**
