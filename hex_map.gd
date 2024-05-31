
extends Node



var size := 0
var hexes := { }
var not_collapsed := [ ]
var collapse_shortlist := [ ]



# The lists of things each terrain type can border
var wfc_rules := {
	# Hex.UNDEF:	[Hex.UNDEF, Hex.WATER, Hex.SHORE, Hex.FOREST, Hex.MOUNT, Hex.PORT],
	# Hex.WATER:	[Hex.UNDEF, Hex.WATER, Hex.SHORE, Hex.PORT],
	# Hex.SHORE:	[Hex.UNDEF, Hex.WATER, Hex.SHORE, Hex.PORT, Hex.FOREST],
	# Hex.FOREST:	[Hex.UNDEF, Hex.SHORE, Hex.FOREST, Hex.MOUNT],
	# Hex.MOUNT:	[Hex.UNDEF, Hex.FOREST, Hex.MOUNT],
	# Hex.PORT:	[Hex.UNDEF, Hex.WATER, Hex.SHORE]

	#			 U, W, S, F, M, P
	Hex.UNDEF:	[6, 6, 6, 6, 6, 6],
	Hex.WATER:	[6, 6, 4, 0, 0, 1],
	Hex.SHORE:	[6, 4, 3, 3, 0, 1],
	Hex.FOREST:	[6, 0, 6, 6, 6, 0],
	Hex.MOUNT:	[6, 0, 0, 6, 5, 0],
	Hex.PORT:	[6, 5, 1, 0, 0, 0]
	# Hex.UNDEF:	[[0, 6], [0, 6], [0, 6], [0, 6], [0, 6], [0, 6]],
	# Hex.WATER:	[[0, 6], [0, 6], [0, 2], [0, 0], [0, 0], [0, 0]],
	# Hex.SHORE:	[[0, 6], [1, 5], [1, 5], [0, 3], [0, 0], [0, 1]],
	# Hex.FOREST:	[[0, 6], [0, 0], [0, 6], [0, 6], [0, 4], [0, 0]],
	# Hex.MOUNT:	[[0, 6], [0, 0], [0, 0], [0, 4], [0, 6], [0, 0]],
	# Hex.PORT:	[[0, 6], [4, 5], [1, 2], [0, 0], [0, 0], [0, 0]]
}
const NUM_TYPES = 6



# Add a hex to the map at coords
func _add_hex(q: int, r: int) -> void:
	var new_hex := WfcHex.new(q, r)

	hexes[[q, r]] = new_hex



# Create a map in a parallelogram shape
func _generate_parallelogram(l: int, w: int) -> void:
	for q in range(l):
		for r in range(w):
			_add_hex(q, r)



# Generate a map in a triangle shape
# (0, 0) is the bottom point, (side_length, 0), (0, side_length) are the other corners
func generate_triangle(side_length: int) -> void:
	size = side_length
	for q in range(side_length):
		for r in range(side_length - q):
			_add_hex(q, r)



# Generate a hexagon shaped map
func _generate_hexagon(radius: int) -> void:
	for q in range(-radius, radius + 1):
		var r1 : int = max(-radius, -q - radius)
		var r2 : int = min( radius, -q + radius)

		for r in range(r1, r2 + 1):
			_add_hex(q, r)



# Get the hex in the map at the coordinates. Null if there is no hex there
func get_hex(q: int, r: int) -> Hex:
	return hexes.get([q, r], null)



# Return a list of all the hexes in the map
func get_hexes() -> Array:
	return hexes.values()



# Return a list of the six neighbors to the hex, null if a neighbor doesn't exist
func get_hex_neighbors(hex: Hex) -> Array:
	var neighbor_dirs := [
		[ 1,  0], # Right
		[ 1, -1], # Up Right
		[ 0, -1], # Up Left
		[-1,  0], # Left
		[-1,  1], # Down Left
		[ 0,  1]  # Down Right
	]

	var neigbors := [ ]

	for dir: Array in neighbor_dirs:
		neigbors.append(get_hex(hex.q + dir[0], hex.r + dir[1]))

	return neigbors



func generate_terrain_types() -> void:
	not_collapsed = get_hexes()


	# collapse_coords(0,  0,  Hex.MOUNT)
	# collapse_coords(0,  1,  Hex.FOREST)
	# collapse_coords(1,  0,  Hex.FOREST)
	# collapse_coords(1,  1,  Hex.PORT)
	# collapse_coords(2,  1,  Hex.WATER)
	# collapse_coords(1,  2,  Hex.WATER)
	# collapse_coords(8,  20, Hex.MOUNT)
	# collapse_coords(25, 5,  Hex.MOUNT)

	# for q in range(4, size / 4):
	# 	for r in range(4, size / 4):
	# 		collapse_coords(q, r, Hex.WATER)

	var next_to_collapse := _get_most_constrained()

	while next_to_collapse != null:
		collapse_hex(next_to_collapse, _pick_random_weighted(next_to_collapse.valids))

		if not_collapsed.size() % 50 == 0:
			print("%f done, num left: %d, shortlist length: %d" %  [1.0 - (float(not_collapsed.size()) / float(hexes.size())), not_collapsed.size(), collapse_shortlist.size()])

		next_to_collapse = _get_most_constrained()



func collapse_coords(q: int, r: int, type: int) -> void:
	var hex := get_hex(q, r)

	if hex == null or not hex is WfcHex:
		return

	collapse_hex(hex, type)


func collapse_hex(hex: WfcHex, type: int) -> void:
	if hex.is_collapsed():
		return
	
	not_collapsed.erase(hex)
	collapse_shortlist.erase(hex)

	hex.collapse(type)

	# Set the neighbor type counts
	hex.valids = wfc_rules[type].duplicate()

	for neighbor: WfcHex in get_hex_neighbors(hex):
		if neighbor == null:
			continue

		if neighbor.is_collapsed():
			neighbor.valids[type] -= 1

			hex.valids[neighbor.get_terrain_type()] -= 1
		
		elif not collapse_shortlist.has(neighbor):
			collapse_shortlist.append(neighbor)



func _pick_random_weighted(valids: Array) -> int:
	var possible := [Hex.WATER, Hex.SHORE, Hex.FOREST, Hex.MOUNT, Hex.PORT]

	for i: int in range(NUM_TYPES):
		if valids[i] <= 0:
			possible.erase(i)
	

	if possible.is_empty():
		return Hex.UNDEF;

	var weights_map := {
		Hex.UNDEF:	0,
		Hex.WATER:	20,
		Hex.SHORE:	4,
		Hex.FOREST:	10,
		Hex.MOUNT:	8,
		Hex.PORT:	1
	}

	var cum_weight := 0
	var cum_weights := [ ]

	for i: int in range(valids.size()):
		if valids[i] <= 0:
			possible.erase(i)

	for i in range(possible.size()):
		cum_weight += weights_map[possible[i]]
		cum_weights.append(cum_weight)
	
	var selection := randi_range(0, cum_weight - 1)

	var selection_ind := 0

	while cum_weights[selection_ind] < selection:
		selection_ind += 1

	return possible[selection_ind]



func _get_most_constrained() -> WfcHex:
	var lowest_constraint := 10
	var lowest_constrained := [ ]

	if not_collapsed.is_empty():
		return null

	if collapse_shortlist.is_empty():
		return not_collapsed.pick_random()

	for hex: WfcHex in collapse_shortlist:
		hex.set_valids(_valid_types(hex))

		if hex.get_valid_total() < lowest_constraint:
			lowest_constrained.clear()
			lowest_constraint = hex.get_valid_total()

			lowest_constrained.append(hex)

		elif hex.get_valid_total() == lowest_constraint:
			lowest_constrained.append(hex)

	return lowest_constrained.pick_random()



# Return a list of the valid types for the hex at coords
func _valid_types(hex: WfcHex) -> Array:
	var valids : Array = [1, 1, 1, 1, 1, 1]

	for neighbor: WfcHex in get_hex_neighbors(hex):
		if neighbor == null or not neighbor.is_collapsed():
			continue
		
		for i in range(NUM_TYPES):
			if neighbor.valids[i] <= 0:
				valids[i] = 0

	return valids
