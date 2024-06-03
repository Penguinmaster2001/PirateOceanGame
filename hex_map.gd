
extends Node



var size := 0
var hexes := { }
var not_collapsed := [ ]
var collapsed := [ ]
var shortlist : HexShortlist = HexShortlist.new()



# List of the allowed bordering edges for each hex
var wfc_rules := [ ]



# Add a hex to the map at coords
func _add_hex(q: int, r: int) -> void:
	var new_hex := WfcHex.new(q, r)

	hexes[[q, r]] = new_hex



# Generate a map in a triangle shape
# (0, 0) is the bottom point, (side_length, 0), (0, side_length) are the other corners
func generate_triangle(side_length: int) -> void:
	size = side_length
	for q in range(side_length):
		for r in range(side_length - q):
			_add_hex(q, r)

	# not_collapsed = get_hexes()
	# collapse_coords(0,  0,  Hex.MOUNT)
	# collapse_coords(0,  1,  Hex.FOREST)
	# collapse_coords(1,  0,  Hex.FOREST)
	# collapse_coords(1,  1,  Hex.PORT)
	# collapse_coords(2,  1,  Hex.WATER)
	# collapse_coords(1,  2,  Hex.WATER)

	# for i in range(4, size / 4):
	# 	collapse_coords(i, i, Hex.WATER)



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



func generate_terrain_types() -> Hex:
	var next_to_collapse := _get_most_constrained()

	if next_to_collapse != null:
		collapse_hex(next_to_collapse, _pick_random_weighted(next_to_collapse.valids))

		# next_to_collapse = _get_most_constrained()
		return next_to_collapse
	
	return null



func display_data() -> void:
	print("%f done, num left: %d, shortlist length: %d" % \
		[1.0 - (float(not_collapsed.size()) / float(hexes.size())), not_collapsed.size(), shortlist.count()])



func collapse_coords(q: int, r: int, type: int) -> void:
	var hex := get_hex(q, r)

	if hex == null:
		return

	collapse_hex(hex, type)



func collapse_hex(hex: WfcHex, type: int) -> void:
	if hex.is_collapsed():
		return
	
	not_collapsed.erase(hex)
	shortlist.remove(hex)
	collapsed.append(hex)

	hex.collapse(type)

	# Set the neighbor type counts
	hex.valids = wfc_rules[type].duplicate()

	for neighbor: WfcHex in get_hex_neighbors(hex):
		if neighbor == null:
			continue

		if neighbor.is_collapsed():
			neighbor.valids[type] -= 1

			hex.valids[neighbor.get_terrain_type()] -= 1

			continue

		var previous_num := neighbor.get_num_options()

		neighbor.set_valids(_valid_types(neighbor))

		shortlist.update_or_insert(neighbor, previous_num)



func _pick_random_weighted(valids: Array) -> int:
	var possible := [Hex.WATER, Hex.SHORE, Hex.FOREST, Hex.MOUNT, Hex.PORT]

	for i: int in range(6):
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



# Return the most constrained tile, or a random uncollapsed one if the shortlist is empty
func _get_most_constrained() -> WfcHex:
	if not_collapsed.is_empty():
		return null

	if shortlist.is_empty():
		return not_collapsed.pick_random()
	
	return shortlist.get_most_constrained_random()



# Return a list of the valid types for the hex at coords
func _valid_types(hex: WfcHex) -> Array:
	var valids : Array = [1, 1, 1, 1, 1, 1]

	for neighbor: WfcHex in get_hex_neighbors(hex):
		if neighbor == null or not neighbor.is_collapsed():
			continue
		
		for i in range(6):
			if neighbor.valids[i] <= 0:
				valids[i] = 0

	return valids



func get_hex_types(path: String) -> Array:
	var file := FileAccess.open(path, FileAccess.READ)
	var data := file.get_as_text()

	var json := JSON.new()
	json.parse(data)

	var json_parse : Dictionary = json.data
	
	var hex_type_materials := [ ]

	for tile_type: Dictionary in json_parse.values():
		wfc_rules.append(tile_type["edges"])
		hex_type_materials.append(tile_type["material"])

	return hex_type_materials
