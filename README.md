# MoonTiler

A polyomino tile placement game about building a base on the moon.

## Overview

Three shapes are drawn from a deck of cards. Each turn the player selects a shape and places adjecent to an existing base tile. The first card is always used up even though the player chooses a different card.

* Water must be placed adjacent to solar power tiles to produce water
* Astronauts require water (oxygen) to survive
* Astronauts work in adjacent research tiles which produces points
* The game ends when none of the three shapes can be placed

The goal of the game is to produce to most amount of research. Good luck!

## Design and inspiration

My goal has been to create a game with a "board game" feeling, in particular a "roll and write" or in this case a "flip and write". I have been inspired by:

* Cartographers - https://boardgamegeek.com/boardgame/263918/cartographers
* Second chance - https://boardgamegeek.com/boardgame/265683/second-chance

Having only 2 shapes to choose from felt very limiting and random. By showing 3 shapes this gives the player a chance to plan the next move, but by always loosing the left most shape this gives additional incentive to place this shape before the other. Knowing when to discard this shape also becomes an interesting strategy.

## Future ideas

* Change set of shapes
* Having a "daily challenge" with a certain order of the shapes
* Adding obstactles on the map that must be removed
* Add additional scoring rules, for example negative score if an astronaut dies

## Credits

### Design and development

Jan Kronquist

https://twitter.com/jankronquist
https://www.gravityforce20.com/

### Moon image

NASA and https://space-facts.com/transparent-planet-pictures/

### UI Icons, tiles, sounds

Tiles & paper background
https://www.kenney.nl/assets/scribble-platformer

Icons
https://www.kenney.nl/assets/game-icons

Sounds
https://www.kenney.nl/assets/casino-audio

### Icons from the Noun Project

Water by Academic Technologies, FoMD from the Noun Project
https://thenounproject.com/search/?q=246906&i=246906

Flask by Vectors Market from the Noun Project
https://thenounproject.com/search/?q=1065464&i=1065464

Skull by Vectors Market from the Noun Project
https://thenounproject.com/search/?q=1065689&i=1065689

sun by mikicon from the Noun Project
https://thenounproject.com/search/?q=729233&i=729233

moonbase by Akriti Bhusal from the Noun Project
https://thenounproject.com/search/?q=2801962&i=2801962

Astronaut by Vectors Point from the Noun Project
https://thenounproject.com/search/?q=3261845&i=3261845

### Scriptable Object Architecture

https://github.com/DanielEverland/ScriptableObject-Architecture
