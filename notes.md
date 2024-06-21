
# I need to completely rethink the wfc implementation

- What is a hex edge? There are issues from just requiring the edges to match. Instead should I require the edge to border a type? Instead of edge1 - edge2 match it's edge1 - type2 and edge2 - type1. I don't know if this would work.

- The wfc algorithm needs to work with and within MultiHexes. Do MultiHexes make sense? There will need to be so many different patterns. I may just want to stick with individual hex wfc, and then maybe MultiHexes can be predefined structures instead.

- Everything is a WfcHex, the base class for Hex doesn't really have a use. But I want the base hex to be what's actually used everywhere. I need to figure out where to use the WfcHexes so they don't replace base hexes.

# It's time to move on from map generation. I still have some other things to get working on

- Boat crew

- Boat resources

- Boat customization

- AI fleets

# Port, boats, fleet controllers, etc. organization

- Abstract class for things with crew and resources

- Seaman base class for Officer and Sailor classes

- CrewManager has lists of Officers and Sailors

- Interface for controllable things which player fleet controller and ai fleet controller will use

- Class for boat modules

- Class for boat

- Registry system for crew, boats, ports, sea creatures? etc.
