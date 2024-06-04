
extends Node


# This is so incredibly badly organized
# So many of these methods belong in the Hex or WfcHex class



var _size := 0
var _hexes := { }
var _collapsed := [ ]
var _not_collapsed := [ ]
var _shortlist : HexShortlist = HexShortlist.new()



# List of the allowed bordering edges for each hex
var type_edges := [ ]
var all_types := [ ]
var type_names := [ ]
var type_weights := [ ]



# Add a hex to the map at coords
func _add_hex(q: int, r: int) -> void:
	var new_hex := WfcHex.new(q, r)

	_hexes[[q, r]] = new_hex

	new_hex.set_allowed_types(all_types)
	_not_collapsed.append(new_hex)



# Generate a map in a triangle shape
# (0, 0) is the bottom point, (side_length, 0), (0, side_length) are the other corners
func generate_triangle(side_length: int) -> void:
	_size = side_length
	for q in range(side_length):
		for r in range(side_length - q):
			_add_hex(q, r)
	
	seed_type(10, type_names.find("super_deep_water"))
	seed_type(5, type_names.find("mountain"))



func seed_type(num: int, type: int) -> void:
	for i in range(num):
		collapse_hex(_not_collapsed.pick_random(), type)



# Get the hex in the map at the coordinates. Null if there is no hex there
func get_hex(q: int, r: int) -> Hex:
	return _hexes.get([q, r], null)



# Return a list of all the _hexes in the map
func get_hexes() -> Array:
	return _hexes.values()



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

	var neighbors := [ ]

	for dir: Array in neighbor_dirs:
		neighbors.append(get_hex(hex._q + dir[0], hex._r + dir[1]))

	return neighbors



# Return a list of the types of the adjacent edge of each neighbor in order
# 0 for uncollapsed neighbors
func get_adjacent_edges(hex: Hex) -> Array:
	var adjacent_edges := [ ]

	var neighbors := get_hex_neighbors(hex)

	for i in range(6):
		if not neighbors[i].is_collapsed():
			adjacent_edges.append(0)
			continue

		adjacent_edges.append(neighbors[i].get_edge(i + 3))
	
	return adjacent_edges



func get_collapsed_hexes() -> Array:
	return _collapsed



func collapse_next_hex() -> Hex:
	var next_to_collapse := _get_most_constrained()

	if next_to_collapse != null:
		collapse_hex(next_to_collapse, next_to_collapse.get_random_allowed_type())

		# next_to_collapse = _get_most_constrained()
		return next_to_collapse
	
	return null



func collapsed_all_hexes() -> bool:
	return _not_collapsed.size() == 0



func display_data() -> void:
	print("%d of %d (%f) done, num left: %d, shortlist length: %d" % \
		[_collapsed.size(), _hexes.size(), float(_collapsed.size()) / float(_hexes.size()), \
		_not_collapsed.size(), _shortlist.count()])



func collapse_coords(q: int, r: int, type: int) -> void:
	var hex := get_hex(q, r)

	if hex == null:
		return

	collapse_hex(hex, type)



# Remove the hex from the shortlist and uncollapsed list
# Tell the hex to collapse
# Constrain the neighbors
func collapse_hex(hex: WfcHex, type: int) -> void:
	if hex.is_collapsed():
		return
		
	var neighbor_dirs := [
		[ 1,  0], # Right
		[ 1, -1], # Up Right
		[ 0, -1], # Up Left
		[-1,  0], # Left
		[-1,  1], # Down Left
		[ 0,  1]  # Down Right
	]
		
	# var diagonal_dirs := [
	# 	[ 2, -1],
	# 	[ 1, -2],
	# 	[-1, -1],
	# 	[-2,  1],
	# 	[-1,  2],
	# 	[ 1,  1]
	# ]
	
	_collapsed.append(hex)
	_not_collapsed.erase(hex)
	_shortlist.remove(hex)

	hex.collapse(type)

	# Constrain each neighbor
	var neighbors := get_hex_neighbors(hex)

	for i in range(6):
		if neighbors[i] == null or neighbors[i].is_collapsed():
			continue

		# Keep track of this for update_or_insert()
		var previous_num : int = neighbors[i].get_constraint()

		# Constrain the adjacent edge on the neighbor, which is offset by 3
		neighbors[i].constrain_edge(i + 3, [hex.get_edge(i)])

		_shortlist.update_or_insert(neighbors[i], previous_num)

		# Hex past neighbor
		var second_neighbor := get_hex(neighbors[i]._q + neighbor_dirs[i][0], neighbors[i]._r + neighbor_dirs[i][1])

		if second_neighbor == null or second_neighbor.is_collapsed():
			continue

		previous_num = second_neighbor.get_constraint()

		second_neighbor.constrain_edge(i + 3, neighbors[i].get_allowed_edge_types(i))

		_shortlist.update_or_insert(second_neighbor, previous_num)


		var diag_left_edge := (i + 2) % 6
		var diag_left := get_hex(neighbors[i]._q + neighbor_dirs[diag_left_edge][0], neighbors[i]._r + neighbor_dirs[diag_left_edge][1])

		if diag_left == null or diag_left.is_collapsed():
			continue

		previous_num = diag_left.get_constraint()

		diag_left.constrain_edge(diag_left_edge + 3, neighbors[i].get_allowed_edge_types(diag_left_edge))

		_shortlist.update_or_insert(diag_left, previous_num)


		var diag_right_edge := (i - 2) % 6
		var diag_right := get_hex(neighbors[i]._q + neighbor_dirs[diag_right_edge][0], neighbors[i]._r + neighbor_dirs[diag_right_edge][1])

		if diag_right == null or diag_right.is_collapsed():
			continue

		previous_num = diag_right.get_constraint()

		diag_right.constrain_edge(diag_right_edge + 3, neighbors[i].get_allowed_edge_types(diag_right_edge))

		_shortlist.update_or_insert(diag_right, previous_num)



# Return the most constrained tile, or a random uncollapsed one if the shortlist is empty
func _get_most_constrained() -> WfcHex:
	if _not_collapsed.is_empty():
		return null

	if _shortlist.is_empty():
		return _not_collapsed.pick_random()
	
	return _shortlist.get_most_constrained_random()



# Read the json file, fill the type_edges with edge permutations,
# and return a list of materials
func get_hex_types(path: String) -> Array:
	var file := FileAccess.open(path, FileAccess.READ)
	var data := file.get_as_text()

	var json := JSON.new()
	json.parse(data)

	var json_parse : Dictionary = json.data

	var names_from_json := json_parse.keys()
	var edges_from_json := [ ]
	var weights_from_json := [ ]
	var materials_from_json := [ ]

	for tile_type: Dictionary in json_parse.values():
		edges_from_json.append(tile_type["edges"])
		weights_from_json.append(tile_type["weight"])
		materials_from_json.append(tile_type["material"])
	

	var type_materials := [ ]

	for i in range(edges_from_json.size()):
		var symmetric_edge_arrays := generate_symmetries(edges_from_json[i])
		type_edges.append_array(symmetric_edge_arrays)

		# All the symmetries get the same material
		for j in range(symmetric_edge_arrays.size()):
			type_names.append(names_from_json[i])
			type_weights.append(weights_from_json[i] / symmetric_edge_arrays.size())
			type_materials.append(materials_from_json[i])
	
	for i in range(type_edges.size()):
		all_types.append(i)

	return type_materials



# This permutes the edges according to the Dihedral Group of Order 6,
# the symmetries of a hexagon
# It ignores duplicate entries
# It will result in a maximum of 12 unique edge arrays
func generate_symmetries(edges: Array) -> Array:
	var symmetries := [edges]

	# All rotations
	for i in range(1, 6):
		var rotated := rotate_edges(edges, i)
		if not symmetries.has(rotated):
			symmetries.append(rotated)

	# All rotations followed by a reflection
	for i in range(6):
		var reflected := reflect_edges(rotate_edges(edges, i))
		if not symmetries.has(reflected):
			symmetries.append(reflected)

	return symmetries



# Rotate the edges amount times, the array must have length 6
func rotate_edges(edges: Array, amount: int) -> Array:
	var rotated_edges := []

	for i in range(6):
		rotated_edges.append(edges[(i + amount) % 6])

	return rotated_edges



# Reflect the edges across the (0, 3) axis, the array must have length 6
func reflect_edges(edges: Array) -> Array:
	var reflected_edges := edges

	reflected_edges[1] = edges[5]
	reflected_edges[5] = edges[1]

	reflected_edges[2] = edges[4]
	reflected_edges[4] = edges[2]

	return reflected_edges
