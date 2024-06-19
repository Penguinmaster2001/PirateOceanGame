
# I need to completely rethink the wfc implementation

1. What is a hex edge? There are issues from just requiring the edges to match. Instead should I require the edge to border a type? Instead of edge1 - edge2 match it's edge1 - type2 and edge2 - type1. I don't know if this would work.

2. The wfc algorithm needs to work with and within MultiHexes. Do MultiHexes make sense? There will need to be so many different patterns. I may just want to stick with individual hex wfc, and then maybe MultiHexes can be predefined structures instead.

3. I'm completely changing the hex type system, I need to rewrite pretty much everything because of that.

4. Everything is a WfcHex, the base class for Hex doesn't really have a use. But I want the base hex to be what's actually used everywhere. I need to figure out where to use the WfcHexes so they don't replace base hexes.
